/*Copyright 2022 Guillaume Spalla

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

namespace MATCH
{
    namespace Scenarios
    {
        public class TidyUp : Scenario
        {
            public Inferences.Manager m_inferenceManager;
            public FiniteStateMachine.Display m_graph;

            FiniteStateMachine.Manager m_gradationManager;

            Inferences.ObjectInInteractionSurface m_inferenceObjectInStorage;
            Inferences.ObjectOutInteractionSurface m_inferenceObjectOutStorage;
            Inferences.ObjectOutInteractionSurface m_inferenceObjectOutObject;

            Assistances.InteractionSurface m_storage;
            Assistances.InteractionSurface m_object;

            EventHandler s_inferenceObjectDetectedInStorage;
            EventHandler s_inferenceObjectDetectedOutStorage;
            EventHandler s_inferenceIgnoreObject;
            EventHandler s_inferenceObjectDetectedOutObject;

            Utilities.PhysicalObjectInformation m_objectdetected;

            readonly string mainObject = "cup";

            private void Awake()
            {
                m_gradationManager = new FiniteStateMachine.Manager();
                setId("Rangement");
            }

            // Start is called before the first frame update
            void Start()
            {
                InitializeScenario();
            }

            void InitializeScenario()
            {
                Manager.Instance.addScenario(this);

                //Surfaces
                m_storage = Assistances.Factory.Instance.CreateInteractionSurface("Storage", default, new Vector3(0.4f, 0.4f, 0.4f), "Mouse_Green_Glowing", true, true, Utilities.Utility.GetEventHandlerEmpty(), transform);
                m_storage.SetLocalPosition(new Vector3(0f, 0f, 0.5f));
                m_object = Assistances.Factory.Instance.CreateInteractionSurface("Object", default, new Vector3(0.5f, 0.5f, 0.5f), "Mouse_Yellow_Glowing", true, true, Utilities.Utility.GetEventHandlerEmpty(), transform);
                m_object.GetInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

                Assistances.Basic successController = Assistances.Factory.Instance.CreateCube("Mouse_Congratulation", m_storage.transform);
                Assistances.Basic exclamationMark = Assistances.Factory.Instance.CreateCube("Mouse_Exclamation_Red", true, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0), true, m_object.GetInteractionSurface().transform);


                //Inferences
                m_inferenceObjectOutStorage = new Inferences.ObjectOutInteractionSurface("Out Storage", CallbackDetectedOutStorage, mainObject, m_storage);
                m_inferenceManager.RegisterInference(m_inferenceObjectOutStorage);




                //*States*//

                //Stand by
                FiniteStateMachine.MouseUtilitiesGradationAssistance sStandBy = m_gradationManager.addNewAssistanceGradation("StandBy");
                sStandBy.addFunctionShow(delegate (EventHandler e)
                {
                    m_inferenceObjectOutStorage = new Inferences.ObjectOutInteractionSurface("Out Storage", CallbackDetectedOutStorage, mainObject, m_storage);
                    m_inferenceManager.RegisterInference(m_inferenceObjectOutStorage);
                    m_object.GetInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //need to ignore raycast if the object is in the same area that the last one
                    onChallengeStart();
                }, Utilities.Utility.GetEventHandlerEmpty());
                sStandBy.setFunctionHide(delegate (EventHandler e)
                {
                    e?.Invoke(this, EventArgs.Empty);
                }, Utilities.Utility.GetEventHandlerEmpty());

                //Waiting for taking object
                FiniteStateMachine.MouseUtilitiesGradationAssistance sWaitingToTakeObject = m_gradationManager.addNewAssistanceGradation("Waiting To Take Object");
                sWaitingToTakeObject.addFunctionShow(delegate (EventHandler e)
                {
                    m_object.transform.localPosition = m_objectdetected.GetCenter();
                    m_object.GetInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Spatial Awareness"); //for increase the object's area recognition
                    m_inferenceObjectOutObject = new Inferences.ObjectOutInteractionSurface("Out Object", CallbackDetectedOutObject, mainObject, m_object);
                    m_inferenceManager.RegisterInference(m_inferenceObjectOutObject);

                    Inferences.Factory.Instance.CreateDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "IgnoreObject", delegate (System.Object o, EventArgs e)
                    {
                        s_inferenceIgnoreObject?.Invoke(this, e);
                    }, m_object.GetInteractionSurface().gameObject);
                }, Utilities.Utility.GetEventHandlerEmpty());
                sWaitingToTakeObject.setFunctionHide(delegate (EventHandler e)
                {
                    e?.Invoke(this, EventArgs.Empty);
                }, Utilities.Utility.GetEventHandlerEmpty());

                //Exclamation Mark
                FiniteStateMachine.MouseUtilitiesGradationAssistance sExclamationMark = m_gradationManager.addNewAssistanceGradation("Exclamation mark");
                sExclamationMark.setFunctionHideAndShow(exclamationMark);


                //Transport
                FiniteStateMachine.MouseUtilitiesGradationAssistance sTransport = m_gradationManager.addNewAssistanceGradation("Transport");
                sTransport.addFunctionShow(delegate (EventHandler e)
                {
                    m_inferenceObjectInStorage = new Inferences.ObjectInInteractionSurface("Transport", CallbackDetectedInStorage, mainObject, m_storage);
                    m_inferenceManager.RegisterInference(m_inferenceObjectInStorage);
                }, Utilities.Utility.GetEventHandlerEmpty());
                sTransport.setFunctionHide(delegate (EventHandler e)
                {
                    e?.Invoke(this, EventArgs.Empty);
                }, Utilities.Utility.GetEventHandlerEmpty());

                //Success
                FiniteStateMachine.MouseUtilitiesGradationAssistance sSuccess = m_gradationManager.addNewAssistanceGradation("Success");
                sSuccess.setFunctionHideAndShow(successController);
                sSuccess.addFunctionShow(delegate (EventHandler e)
                {
                    onChallengeSuccess();
                }, Utilities.Utility.GetEventHandlerEmpty());



                //Connexions

                //Success first for priority
                s_inferenceObjectDetectedInStorage += sTransport.goToState(sSuccess);
                s_inferenceObjectDetectedInStorage += sWaitingToTakeObject.goToState(sSuccess);
                s_inferenceObjectDetectedInStorage += sExclamationMark.goToState(sSuccess);

                s_inferenceObjectDetectedOutStorage += sStandBy.goToState(sWaitingToTakeObject);
                s_inferenceIgnoreObject += sWaitingToTakeObject.goToState(sExclamationMark);
                s_inferenceObjectDetectedOutObject += sExclamationMark.goToState(sTransport);
                s_inferenceObjectDetectedOutObject += sWaitingToTakeObject.goToState(sTransport);

                successController.s_touched += sSuccess.goToState(sStandBy);

                m_gradationManager.setGradationInitial("StandBy");

                //Graph
                m_graph.setManager(m_gradationManager);

            }

            void CallbackDetectedInStorage(System.Object o, EventArgs e) //Callback emitted when the object is in storage
            {
                m_inferenceManager.UnregisterInference(m_inferenceObjectInStorage);
                s_inferenceObjectDetectedInStorage?.Invoke(this, EventArgs.Empty);
            }

            void CallbackDetectedOutStorage(System.Object o, EventArgs e) //Callback emitted when the object is out of the storage
            {
                m_inferenceManager.UnregisterInference(m_inferenceObjectOutStorage);
                m_objectdetected = ((Utilities.EventHandlerArgObject)e).ObjectDetected;
                s_inferenceObjectDetectedOutStorage?.Invoke(this, EventArgs.Empty);
            }

            void CallbackDetectedOutObject(System.Object o, EventArgs e) //Callback emitted when the object is out of the object area
            {
                m_inferenceManager.UnregisterInference(m_inferenceObjectOutObject);
                s_inferenceObjectDetectedOutObject?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

