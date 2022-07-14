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

public class MouseChallengeTidy : MouseChallengeAbstract
{
    public MouseUtilitiesContextualInferences m_inferenceManager;
    public MouseUtilitiesDisplayGraph m_graph;

    MouseUtilitiesGradationAssistanceManager m_gradationManager;

    MouseUtilitiesInferenceObjectInInteractionSurface m_inferenceObjectInStorage;
    MouseUtilitiesInferenceObjectOutInteractionSurface m_inferenceObjectOutStorage;
    //MouseUtilitiesInferenceSetInteractionSurfaceToObject m_inferenceObjectArea;
    MouseUtilitiesInferenceObjectOutInteractionSurface m_inferenceObjectOutObject;

    MouseInteractionSurface m_storage;
    MouseInteractionSurface m_object;

    EventHandler s_inferenceObjectDetectedInStorage;
    EventHandler s_inferenceObjectDetectedOutStorage;
    EventHandler s_inferenceObjectAreaSet;
    EventHandler s_inferenceIgnoreObject;
    EventHandler s_inferenceObjectDetectedOutObject;

    MousePhysicalObjectInformation m_objectdetected;

    string mainObject = "cup";

    private void Awake()
    {
        m_gradationManager = new MouseUtilitiesGradationAssistanceManager();
        setId("Rangement");
    }

    // Start is called before the first frame update
    void Start()
    {
        //MouseUtilitiesAdminMenu.Instance.addSwitchButton("Storage ignore raycast", callbackIgnore);
        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initializeScenario()
    {
        MouseScenarioManager.Instance.addScenario(this);

        //Surfaces
        m_storage = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("Storage", default, new Vector3(0.4f, 0.4f, 0.4f), "Mouse_Green_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        m_storage.setLocalPosition(new Vector3(0, 1, 0));
        m_object = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("Object", default, new Vector3(1f, 1f, 1f), "Mouse_Yellow_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        m_object.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        MouseAssistanceBasic successController = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Congratulation",m_storage.transform);
        MouseAssistanceBasic exclamationMark = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Exclamation_Red", true, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0), true, m_object.getInteractionSurface().transform);


        //Inferences
        m_inferenceObjectOutStorage = new MouseUtilitiesInferenceObjectOutInteractionSurface("Out Storage", callbackDetectedOutStorage, mainObject, m_storage);
        m_inferenceManager.registerInference(m_inferenceObjectOutStorage);

        


        //*States*//

        //Stand by
        /*MouseUtilitiesGradationAssistance sStandBy = m_gradationManager.addNewAssistanceGradation("StandBy");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            
            m_inferenceObjectInPlateArea = new MouseUtilitiesInferenceObjectInInteractionSurface("In Plate Area", callbackDetectedInPlateArea, mainObject, m_platearea);
            m_inferenceManager.registerInference(m_inferenceObjectInPlateArea);
            onChallengeStandBy();
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        */
        //Stand by
        MouseUtilitiesGradationAssistance sStandBy = m_gradationManager.addNewAssistanceGradation("StandBy");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceObjectOutStorage = new MouseUtilitiesInferenceObjectOutInteractionSurface("Out Storage", callbackDetectedOutStorage, mainObject, m_storage);
            m_inferenceManager.registerInference(m_inferenceObjectOutStorage);
            m_object.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast"); //need to ignore raycast if the object is in the same area that the last one
            onChallengeStart();
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        //Set Object Area      
        /*MouseUtilitiesGradationAssistance sIdentifyObjectArea = m_gradationManager.addNewAssistanceGradation("Identify Object Area");
        sIdentifyObjectArea.addFunctionShow(delegate (EventHandler e)
        {
            //m_inferenceObjectArea = new MouseUtilitiesInferenceSetInteractionSurfaceToObject("Identify Object Area", callbackDetectedObjectArea, mainObject, m_object);
            //m_inferenceManager.registerInference(m_inferenceObjectArea);
            m_object.transform.localPosition = m_objectdetected.getCenter();
        }, MouseUtilities.getEventHandlerEmpty());
        sIdentifyObjectArea.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        */
        //Waiting for taking object
        MouseUtilitiesGradationAssistance sWaitingToTakeObject = m_gradationManager.addNewAssistanceGradation("Waiting To Take Object");
        sWaitingToTakeObject.addFunctionShow(delegate (EventHandler e)
        {
            m_object.transform.localPosition = m_objectdetected.getCenter();
            m_object.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Default"); //for increase the object's area recognition
            m_inferenceObjectOutObject = new MouseUtilitiesInferenceObjectOutInteractionSurface("Out Object", callbackDetectedOutObject, mainObject, m_object);
            m_inferenceManager.registerInference(m_inferenceObjectOutObject);

            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceComingAndLeavingInferenceOneShot(m_inferenceManager, "IgnoreObject", delegate (System.Object o, EventArgs e)
            {
                s_inferenceIgnoreObject?.Invoke(this, e);
            }, m_object.getInteractionSurface().gameObject);
        }, MouseUtilities.getEventHandlerEmpty());
        sWaitingToTakeObject.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        
        //Exclamation Mark
        MouseUtilitiesGradationAssistance sExclamationMark = m_gradationManager.addNewAssistanceGradation("Exclamation mark");
        sExclamationMark.setFunctionHideAndShow(exclamationMark);


        //Transport
        MouseUtilitiesGradationAssistance sTransport = m_gradationManager.addNewAssistanceGradation("Transport");
        sTransport.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceObjectInStorage = new MouseUtilitiesInferenceObjectInInteractionSurface("Transport", callbackDetectedInStorage, mainObject, m_storage);
            m_inferenceManager.registerInference(m_inferenceObjectInStorage);
        }, MouseUtilities.getEventHandlerEmpty());
        sTransport.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        //Success
        MouseUtilitiesGradationAssistance sSuccess = m_gradationManager.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(successController);
        sSuccess.addFunctionShow(delegate (EventHandler e)
        {
            onChallengeSuccess();
        }, MouseUtilities.getEventHandlerEmpty());



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
    /*void callbackDetectedInPlateArea(System.Object o, EventArgs e) //Callback emitted when the object is in the plate area
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectInPlateArea);
        s_inferenceObjectDetectedInPlateArea?.Invoke(this, EventArgs.Empty);
    }*/

    void callbackDetectedInStorage(System.Object o, EventArgs e) //Callback emitted when the object is in storage
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectInStorage);
        s_inferenceObjectDetectedInStorage?.Invoke(this, EventArgs.Empty);
    }

    void callbackDetectedOutStorage(System.Object o, EventArgs e) //Callback emitted when the object is out of the storage
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectOutStorage);
        m_objectdetected = ((MouseEventHandlerArgObject)e).m_object;
        s_inferenceObjectDetectedOutStorage?.Invoke(this, EventArgs.Empty);
    }

    /*
    void callbackDetectedObjectArea(System.Object o, EventArgs e) 
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectArea);
        s_inferenceObjectAreaSet?.Invoke(this, EventArgs.Empty);
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "CB Detetct Object Area");
    }*/
    void callbackDetectedOutObject(System.Object o, EventArgs e) //Callback emitted when the object is out of the object area
    {
        m_inferenceManager.unregisterInference(m_inferenceObjectOutObject);
        s_inferenceObjectDetectedOutObject?.Invoke(this, EventArgs.Empty);
    }

    /*
    void callbackIgnore() //callback emitted when the button is clicked : necessary for the proper functioning of the scenario
    {
        
        if (m_storage.getInteractionSurface().gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
        {
            m_storage.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Default");
            //m_platearea.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Default");
        }
        else
        {
            m_storage.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            //m_platearea.getInteractionSurface().gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

    }
    */
}
