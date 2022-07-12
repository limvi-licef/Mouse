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
        public class DustingTheTable : Scenario
        {
            public int m_numberOfCubesToAddInRow;
            public int m_numberOfCubesToAddInColumn;

            public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
            public AudioListener m_audioListener;

            public GameObject m_refAssistanceDialog;
            public GameObject m_refInteractionSurface;

            Transform m_reminderView;
            Assistances.ReminderOneClockMoving m_reminderController;

            Transform m_assistanceStimulateLevel2View;
            Assistances.ArchWithTextAndHelp m_assistanceConnectWithArchController;

            Transform m_assistancePicturalView;
            Assistances.CubeWithVisualGradation m_assistancePicturalController;

            Transform m_successView;
            Assistances.Basic m_successController;

            Transform m_assistanceSurfaceTouchedView;
            Assistances.ProcessingSurface m_assistanceSurfaceTouchedController;

            public Transform m_displayGraphView;
            FiniteStateMachine.Display m_displayGraphController;

            //Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

            FiniteStateMachine.Manager m_assistanceGradationManager;

            public MATCH.Inferences.Manager m_inferenceEngine;

            EventHandler s_time20h;
            EventHandler s_ignoreRedSurface;
            //EventHandler s_ignoreExclamationMark;
            EventHandler s_dialogSecondButtonOk;
            EventHandler s_dialogSecondButtonNok;
            EventHandler s_dialogSecondButtonLeave;
            EventHandler s_backToTable;
            EventHandler s_caregiverCall;

            MATCH.Inferences.Time m_inference20h;

            private void Awake()
            {
                // Initialize variables
                setId("Nettoyer la table");
            }

            // Start is called before the first frame update
            void Start()
            {
                // Variables

                // Children
                m_reminderView = gameObject.transform.Find("Reminder");
                m_reminderController = m_reminderView.GetComponent<Assistances.ReminderOneClockMoving>();

                m_assistanceStimulateLevel2View = gameObject.transform.Find("MouseChallengeCleanTableAssistanceStimulateLevel2");
                m_assistanceConnectWithArchController = m_assistanceStimulateLevel2View.GetComponent<Assistances.ArchWithTextAndHelp>();

                m_assistancePicturalView = gameObject.transform.Find("AssistanceStimulateLevel1");
                m_assistancePicturalController = m_assistancePicturalView.GetComponent<Assistances.CubeWithVisualGradation>();

                m_successView = gameObject.transform.Find("Success");
                m_successController = m_successView.GetComponent<Assistances.Basic>();

                m_assistanceSurfaceTouchedView = gameObject.transform.Find("AssistanceSurfaceTouched");
                m_assistanceSurfaceTouchedController = m_assistanceSurfaceTouchedView.GetComponent<Assistances.ProcessingSurface>();

                m_displayGraphController = m_displayGraphView.GetComponent<FiniteStateMachine.Display>();

                m_assistanceGradationManager = new FiniteStateMachine.Manager();

                DateTime tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
                m_inference20h = new MATCH.Inferences.Time("time 20h", tempTime, CallbackInferenceTime20h);

                // Initialization of the scenario
                InitializeScenariov2();

                // Drawing the graph
                //m_displayGraphController.setManager(m_assistanceGradationManager);
            }

            void CallbackInferenceTime20h(System.Object o, EventArgs e)
            {
                m_inferenceEngine.UnregisterInference(m_inference20h);
                s_time20h?.Invoke(o, e);
            }

            public MATCH.Inferences.Time GetInference()
            {
                return m_inference20h;
            }

            // Update is called once per frame
            /*void Update()
            {

            }*/

            void InitializeScenariov1()
            {
                /** Initializing the assistances we will be needing **/

                Manager.Instance.addScenario(this);

                // First interaction surface, i.e. for the table
                GameObject interactionTableView = Instantiate(m_refInteractionSurface, gameObject.transform);
                Assistances.InteractionSurface interactionTableController = interactionTableView.GetComponent<Assistances.InteractionSurface>();
                interactionTableController.SetAdminButtons("table");
                interactionTableController.SetColor("Mouse_Cyan_Glowing");
                interactionTableView.transform.localPosition = new Vector3(0.949f, -0.017f, 1.117f);
                interactionTableController.SetScaling(new Vector3(1.1f, 0.02f, 0.7f));
                interactionTableController.ShowInteractionSurfaceTable(true);
                interactionTableController.SetPreventResizeY(true);


                // Second interaction surface, i.e. for the rag
                GameObject interactionRagView = Instantiate(m_refInteractionSurface, gameObject.transform);
                Assistances.InteractionSurface interactionRagController = interactionRagView.GetComponent<Assistances.InteractionSurface>();
                interactionRagController.SetColor("Mouse_Orange_Glowing");
                interactionRagController.SetAdminButtons("rag");
                interactionRagView.transform.localPosition = new Vector3(0, -0.008f, 3.843f);
                interactionRagController.ShowInteractionSurfaceTable(true);

                m_assistanceSurfaceTouchedView.localScale = new Vector3(interactionTableController.GetInteractionSurface().localScale.x,
                        m_assistanceSurfaceTouchedView.localScale.y, interactionTableController.GetInteractionSurface().localScale.z);
                interactionTableController.EventInteractionSurfaceScaled += delegate
                {
                    m_assistanceSurfaceTouchedView.localScale = new Vector3(interactionTableController.GetInteractionSurface().localScale.x,
                        m_assistanceSurfaceTouchedView.localScale.y, interactionTableController.GetInteractionSurface().localScale.z);
                };


                // First stimulate assistance
                m_assistancePicturalController.SetCubeMaterialVivid("Mouse_Help_Bottom_Vivid", "Mouse_Help_Top-Left_Vivid", "Mouse_Help_Top-Right_Vivid");

                // Cueing for the beginning of the scenario
                GameObject initialCueingView = Instantiate(m_refAssistanceDialog, interactionTableView.transform);
                Assistances.Dialog initialCueingController = initialCueingView.GetComponent<Assistances.Dialog>();
                initialCueingController.setDescription("Que faites-vous typiquement apr�s manger?");
                initialCueingController.addButton("Je ne sais pas", true, 0.2f);
                initialCueingController.enableBillboard(true);

                // Cueing for the solution
                GameObject solutionView = Instantiate(m_refAssistanceDialog, interactionRagView.transform);
                Assistances.Dialog solutionController = solutionView.GetComponent<Assistances.Dialog>();
                solutionController.setDescription("Ne serait-ce pas un bon moment pour nettoyer la table? \n Vous avez pour cela besoin du chiffon ci - dessous.", 0.15f);
                solutionController.enableBillboard(true);

                // Setting the parents, the connections for the objects briding other objects etc. (the idea being to leave that to a software dedicated to configure the scenarios)
                Utilities.Utility.setParentToObject(m_assistancePicturalView, interactionTableView.transform);
                Utilities.Utility.setParentToObject(m_successView, interactionTableView.transform);
                Utilities.Utility.setParentToObject(m_assistanceStimulateLevel2View, interactionRagView.transform);
                Utilities.Utility.setParentToObject(m_assistanceSurfaceTouchedView, interactionTableView.transform);

                m_assistancePicturalController.m_surfaceWithStarsViewTarget = interactionTableController.GetInteractionSurface();

                m_assistanceConnectWithArchController.setArchStartAndEndPoint(initialCueingView.transform, interactionRagView.transform);
                m_reminderController.AddObjectToBeClose(interactionRagView.transform);
                m_reminderController.AddObjectToBeClose(m_assistancePicturalController.m_hologramView);
                m_reminderController.AddObjectToBeClose(m_assistancePicturalController.m_surfaceWithStarsView);
                m_reminderController.AddObjectToBeClose(m_assistancePicturalController.m_help);
                m_reminderController.AddObjectToBeClose(m_assistanceConnectWithArchController.m_hologramHelp);
                m_reminderController.AddObjectToBeClose(m_assistanceConnectWithArchController.m_textView);
                m_reminderController.AddObjectToBeClose(interactionTableController.GetInteractionSurface());
                m_reminderController.AddObjectToBeClose(initialCueingView.transform);
                m_reminderController.AddObjectToBeClose(solutionView.transform);
                m_reminderController.AddObjectToBeClose(m_assistanceSurfaceTouchedView);

                // Settings the states
                FiniteStateMachine.MouseUtilitiesGradationAssistance sStandBy = m_assistanceGradationManager.addNewAssistanceGradation("StandBy");
                SetStandByTransitions(sStandBy);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sCubeRagTable = m_assistanceGradationManager.addNewAssistanceGradation("CubeRagTable");
                SetCubeRagTransitions(sCubeRagTable);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sReminder = m_assistanceGradationManager.addNewAssistanceGradation("Reminder");
                SetReminderTransitions(sReminder);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sMessageCue = m_assistanceGradationManager.addNewAssistanceGradation("MessageCue");
                sMessageCue.setFunctionHideAndShow(initialCueingController);
                sMessageCue.addFunctionShow(m_reminderController);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sArchToRag = m_assistanceGradationManager.addNewAssistanceGradation("ArchToRag");
                SetConnectWithArchTransitions(sArchToRag);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sSolution = m_assistanceGradationManager.addNewAssistanceGradation("Solution");
                sSolution.setFunctionHideAndShow(solutionController);
                sSolution.addFunctionShow(m_reminderController);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sSurfaceToClean = m_assistanceGradationManager.addNewAssistanceGradation("CleaningSurface");
                SetRagInteractionSurfaceTransitions(sSurfaceToClean);
                FiniteStateMachine.MouseUtilitiesGradationAssistance sSuccess = m_assistanceGradationManager.addNewAssistanceGradation("Success");
                SetSuccessTransitions(sSuccess);

                // Setting the initial state (so that the reset object can work)
                m_assistanceGradationManager.setGradationInitial("StandBy");

                // States changing
                interactionTableController.EventInteractionSurfaceTableTouched += sStandBy.goToState(sCubeRagTable);
                m_assistancePicturalController.m_eventHologramStimulateLevel1Gradation1Or2Touched += sCubeRagTable.goToState(sMessageCue);
                initialCueingController.m_buttonsController[0].s_buttonClicked += sMessageCue.goToState(sArchToRag);
                m_assistanceConnectWithArchController.m_eventHologramHelpTouched += sArchToRag.goToState(sSolution);
                interactionRagController.EventInteractionSurfaceTableTouched += sCubeRagTable.goToState(sSurfaceToClean);
                interactionRagController.EventInteractionSurfaceTableTouched += sMessageCue.goToState(sSurfaceToClean);
                interactionRagController.EventInteractionSurfaceTableTouched += sArchToRag.goToState(sSurfaceToClean);
                interactionRagController.EventInteractionSurfaceTableTouched += sSolution.goToState(sSurfaceToClean);
                m_assistanceSurfaceTouchedController.EventSurfaceCleaned += sSurfaceToClean.goToState(sSuccess);
                m_successController.s_touched += sSuccess.goToState(sStandBy);

                m_reminderController.EventHologramClockTouched += sCubeRagTable.goToState(sReminder);
                m_reminderController.EventHologramClockTouched += sMessageCue.goToState(sReminder);
                m_reminderController.EventHologramClockTouched += sArchToRag.goToState(sReminder);
                m_reminderController.EventHologramClockTouched += sSolution.goToState(sReminder);
                m_reminderController.EventHologramClockTouched += sSurfaceToClean.goToState(sReminder);
                m_reminderController.EventHologramWindowButtonBackTouched += sReminder.setGradationPrevious();
                m_reminderController.EventHologramWindowButtonOkTouched += sReminder.goToState(sStandBy);



                // Add button to reset scenario
                AdminMenu.Instance.addButton("Reset clean table challenge", delegate () { m_assistanceGradationManager.goBackToOriginalState(); });
            }

            void InitializeScenariov2()
            {
                Manager.Instance.addScenario(this);

                // Interaction surface table
                Assistances.InteractionSurface interactionSurfaceTable = Assistances.Factory.Instance.CreateInteractionSurface("table v2", AdminMenu.Panels.Default, new Vector3(1.1f, 0.02f, 0.7f), "Mouse_Cyan_Glowing", true, true, Utilities.Utility.getEventHandlerEmpty(), transform);
                //interactionSurfaceTable.setLocalPosition(new Vector3(0.949f, -0.017f, 1.117f));
                interactionSurfaceTable.SetPreventResizeY(true);
                interactionSurfaceTable.transform.position = new Vector3(0.8258258700370789f, 0.4396502375602722f, 2.451075315475464f);

                // Interaction surface rag
                Assistances.InteractionSurface interactionSurfaceRag = Assistances.Factory.Instance.CreateInteractionSurface("rag v2", AdminMenu.Panels.Default, new Vector3(0.2f, 0.01f, 0.2f), "Mouse_Orange_Glowing", true, true, Utilities.Utility.getEventHandlerEmpty(), transform);
                interactionSurfaceRag.transform.localPosition = new Vector3(0, -0.008f, 3.843f);
                //interactionSurfaceRag.getInteractionSurface().transform.localPosition = new Vector3(0, -0.008f, 3.843f);


                // Red surface on table
                Assistances.Basic redSurface = Assistances.Factory.Instance.CreateFlatSurface("Mouse_Red_Glowing", new Vector3(interactionSurfaceTable.GetLocalPosition().x, interactionSurfaceTable.GetLocalPosition().y + 0.02f, interactionSurfaceTable.GetLocalPosition().z), interactionSurfaceTable.transform);
                redSurface.SetScale(interactionSurfaceTable.GetLocalScale().x, redSurface.GetScale().y, interactionSurfaceTable.GetLocalScale().z);
                interactionSurfaceTable.EventInteractionSurfaceScaled += delegate { redSurface.SetScale(new Vector3(interactionSurfaceTable.GetInteractionSurface().localScale.x, redSurface.GetScale().y, interactionSurfaceTable.GetInteractionSurface().localScale.z)); };

                // Exclamation mark
                Assistances.Basic exclamationMark = Assistances.Factory.Instance.CreateCube("Mouse_Exclamation_Red", true, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0), true, interactionSurfaceTable.transform);

                // First message
                string firstMessage = "Que faites-vous normalement apr�s manger?"; // Used in the first two dialogs
                Assistances.Dialog firstDialog = Assistances.Factory.Instance.CreateDialogNoButton("", firstMessage, interactionSurfaceTable.transform);

                // Second dialog
                Assistances.Dialog secondDialog = Assistances.Factory.Instance.CreateDialogThreeButtons("", firstMessage, "Je sais!", delegate (System.Object o, EventArgs e) { s_dialogSecondButtonOk?.Invoke(this, e); }, "Je ne sais pas", delegate (System.Object o, EventArgs e) { s_dialogSecondButtonNok?.Invoke(this, e); }, "Cela ne m'int�resse pas", delegate (System.Object o, EventArgs e) { s_dialogSecondButtonLeave?.Invoke(this, e); }, interactionSurfaceTable.transform);

                // Surface to clean
                Assistances.ProcessingSurface surfaceToProcess = Assistances.Factory.Instance.CreateSurfaceToProcess(interactionSurfaceTable.transform);
                surfaceToProcess.transform.localScale = new Vector3(interactionSurfaceTable.GetInteractionSurface().localScale.x, surfaceToProcess.transform.localScale.y, interactionSurfaceTable.GetInteractionSurface().localScale.z);
                interactionSurfaceTable.EventInteractionSurfaceScaled += delegate (System.Object o, EventArgs e)
                {
                    surfaceToProcess.transform.localScale = new Vector3(interactionSurfaceTable.GetInteractionSurface().localScale.x, surfaceToProcess.transform.localScale.y, interactionSurfaceTable.GetInteractionSurface().localScale.z);
                };

                // Dialog to ask to get the rag
                Assistances.Dialog dialogRag = Assistances.Factory.Instance.CreateDialogNoButton("", "Vous devez nettoyer la table avec un chiffon", interactionSurfaceTable.transform);

                // Dialog to inform where the rag is
                // To do later

                // Calling the caregiver
                Assistances.Dialog dialogCallCaregiver = Assistances.Factory.Instance.CreateDialogTwoButtons("", "Est-ce que j'appelle votre aidant?", "Oui", delegate (System.Object o, EventArgs e) { s_caregiverCall?.Invoke(this, e); }, "Non", delegate (System.Object o, EventArgs e) { s_caregiverCall?.Invoke(this, e); }, interactionSurfaceTable.transform);

                // Success
                Assistances.Basic success = Assistances.Factory.Instance.CreateCube("Mouse_Congratulation", interactionSurfaceTable.transform);

                // Inferences
                //MouseUtilitiesInferenceTime i20h = new MouseUtilitiesInferenceTime("")
                /*MouseUtilitiesContextualInferencesFactory.Instance.createTemporalInferenceOneShot(m_inferenceEngine, "CleanTable20h", delegate (System.Object o, EventArgs e)
                {
                    s_time20h?.Invoke(this, e);
                }, 20);*/
                m_inferenceEngine.RegisterInference(m_inference20h);


                Inferences.Factory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "IgnoreRedSurfaceAndExclamationMark", delegate (System.Object o, EventArgs e)
                {
                    s_ignoreRedSurface?.Invoke(this, e);
                }/*s_ignoreExclamationMark*/, redSurface.gameObject);

                // States
                FiniteStateMachine.MouseUtilitiesGradationAssistance sStandBy = m_assistanceGradationManager.addNewAssistanceGradation("Stand-by");
                sStandBy.addFunctionShow(delegate (EventHandler e)
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Standby show function called");

                    onChallengeStandBy();

                    //MouseUtilitiesInferenceTime i20h = new MouseUtilitiesInferenceTime("")
                    /*MouseUtilitiesContextualInferencesFactory.Instance.createTemporalInferenceOneShot(m_inferenceEngine, "CleanTable20h", delegate (System.Object o, EventArgs ee)
                    {
                        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Callback for 20h called. Signal should be triggered ...");

                        s_time20h?.Invoke(this, ee);
                    }, 20);*/
                    m_inferenceEngine.RegisterInference(m_inference20h);
                }, Utilities.Utility.getEventHandlerEmpty());
                sStandBy.setFunctionHide(delegate (EventHandler e)
                { e?.Invoke(this, EventArgs.Empty); }, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sRedSurface = m_assistanceGradationManager.addNewAssistanceGradation("Red surface");
                sRedSurface.setFunctionHideAndShow(redSurface);
                sRedSurface.addFunctionShow(delegate (EventHandler eh)
                {
                    onChallengeStart();
                    Inferences.Factory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "IgnoreRedSurface", delegate (System.Object o, EventArgs e)
                    {
                        s_ignoreRedSurface.Invoke(this, e);
                    }/*s_ignoreRedSurface*/, redSurface.gameObject);
                }, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sRedSurfaceAndExclamation = m_assistanceGradationManager.addNewAssistanceGradation("Red surface + exclamation mark");
                sRedSurfaceAndExclamation.addFunctionShow(exclamationMark);
                sRedSurfaceAndExclamation.addFunctionShow(redSurface);
                sRedSurfaceAndExclamation.setFunctionHide(delegate (EventHandler e)
                {
                    exclamationMark.Hide(Utilities.Utility.getEventHandlerEmpty());
                    redSurface.Hide(Utilities.Utility.getEventHandlerEmpty());

                    e?.Invoke(this, EventArgs.Empty);
                }, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sFirstDialog = m_assistanceGradationManager.addNewAssistanceGradation("First dialog");
                sFirstDialog.setFunctionHideAndShow(firstDialog);
                sFirstDialog.addFunctionShow(delegate (EventHandler e) {
                    Inferences.Factory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "BackToTable", delegate (System.Object oo, EventArgs ee)
                    {
                        s_backToTable.Invoke(this, ee);
                    }, interactionSurfaceTable.gameObject);
                }, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sProcessSurface = m_assistanceGradationManager.addNewAssistanceGradation("Process surface");
                sProcessSurface.addFunctionShow(surfaceToProcess.ShowInteractionCubesTablePanel, Utilities.Utility.getEventHandlerEmpty());
                sProcessSurface.setFunctionHide(surfaceToProcess.Hide, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sSecondDialog = m_assistanceGradationManager.addNewAssistanceGradation("Second dialog");
                sSecondDialog.setFunctionHideAndShow(secondDialog);

                FiniteStateMachine.MouseUtilitiesGradationAssistance sDialogRag = m_assistanceGradationManager.addNewAssistanceGradation("Dialog rag");
                sDialogRag.setFunctionHideAndShow(dialogRag);
                sDialogRag.addFunctionShow(delegate (EventHandler e) {
                    Inferences.Factory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "BackToTable", delegate (System.Object oo, EventArgs ee)
                    {
                        s_backToTable.Invoke(this, ee);
                    }/*s_ignoreExclamationMark*/, interactionSurfaceTable.gameObject);
                }, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sSuccess = m_assistanceGradationManager.addNewAssistanceGradation("Success");
                sSuccess.setFunctionHideAndShow(success);
                sSuccess.addFunctionShow(delegate (EventHandler e)
                {
                    onChallengeSuccess();
                }, Utilities.Utility.getEventHandlerEmpty());

                FiniteStateMachine.MouseUtilitiesGradationAssistance sCaregiverCall = m_assistanceGradationManager.addNewAssistanceGradation("Caregiver call");
                sCaregiverCall.setFunctionHideAndShow(dialogCallCaregiver);

                // Set original state
                m_assistanceGradationManager.setGradationInitial("Stand-by");

                // Connecting the states
                s_time20h += delegate (System.Object o, EventArgs e)
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Inference 20h for cleaning table called");
                };
                s_time20h += sStandBy.goToState(sRedSurface);
                s_ignoreRedSurface += sRedSurface.goToState(sRedSurfaceAndExclamation);
                exclamationMark.s_touched += sRedSurfaceAndExclamation.goToState(sFirstDialog);

                interactionSurfaceRag.EventInteractionSurfaceTableTouched += delegate (System.Object o, EventArgs e)
                {
                    m_inferenceEngine.UnregisterInference("BackToTable");
                    m_inferenceEngine.UnregisterInference("BackToTableInternal");
                };
                interactionSurfaceRag.EventInteractionSurfaceTableTouched += sRedSurface.goToState(sProcessSurface);
                interactionSurfaceRag.EventInteractionSurfaceTableTouched += sRedSurfaceAndExclamation.goToState(sProcessSurface);
                interactionSurfaceRag.EventInteractionSurfaceTableTouched += sFirstDialog.goToState(sProcessSurface);
                interactionSurfaceRag.EventInteractionSurfaceTableTouched += sDialogRag.goToState(sProcessSurface);

                s_backToTable += sFirstDialog.goToState(sSecondDialog);

                s_dialogSecondButtonOk += sSecondDialog.goToState(sProcessSurface);
                s_dialogSecondButtonOk += delegate (System.Object o, EventArgs e)
                {
                    Inferences.Factory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "BackToTable", delegate (System.Object oo, EventArgs ee)
                    {
                        s_backToTable.Invoke(this, e);
                    }/*s_ignoreExclamationMark*/, interactionSurfaceTable.gameObject);
                };

                s_backToTable += sProcessSurface.goToState(sDialogRag);

                s_dialogSecondButtonNok += sSecondDialog.goToState(sDialogRag);
                s_dialogSecondButtonLeave += sSecondDialog.goToState(sStandBy);

                surfaceToProcess.EventSurfaceCleaned += sProcessSurface.goToState(sSuccess);

                s_backToTable += sDialogRag.goToState(sCaregiverCall);
                s_caregiverCall += sCaregiverCall.goToState(sStandBy);

                success.s_touched += sSuccess.goToState(sStandBy);

                m_displayGraphController.setManager(m_assistanceGradationManager);
            }

            void CallbackInferenceDistanceAssistanceStimulateLevel1(System.Object sender, EventArgs args)
            {
                m_assistancePicturalController.IncreaseGradation();
                m_inferenceEngine.UnregisterInference("inferenceAssistancePicturalDistance");
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
            }

            void SetStandByTransitions(FiniteStateMachine.MouseUtilitiesGradationAssistance state)
            { // This state is the initial state. So no transitions needed here
                state.addFunctionShow(delegate (EventHandler e)
                {
                    m_assistancePicturalController.SetGradationToMinimum();
                }, Utilities.Utility.getEventHandlerEmpty());

                state.setFunctionHide(delegate (EventHandler e)
                {
                    // Play sound to get the user's attention from audio on top of visually
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                    e?.Invoke(this, EventArgs.Empty);
                }, Utilities.Utility.getEventHandlerEmpty());
            }

            void SetCubeRagTransitions(FiniteStateMachine.MouseUtilitiesGradationAssistance state)
            {
                state.addFunctionShow(m_assistancePicturalController);
                state.addFunctionShow(m_reminderController);
                state.addFunctionShow(delegate (EventHandler e)
                {
                    // Set the inference
                    MATCH.Inferences.DistanceLeaving inferenceAssistancePicturalDistance = new MATCH.Inferences.DistanceLeaving("inferenceAssistancePicturalDistance", CallbackInferenceDistanceAssistanceStimulateLevel1, m_assistancePicturalView.gameObject, 3.0f);

                    m_inferenceEngine.RegisterInference(inferenceAssistancePicturalDistance);
                }, Utilities.Utility.getEventHandlerEmpty());

                state.setFunctionHide(delegate (EventHandler e)
                {
                    m_assistancePicturalController.Hide(Utilities.Utility.getEventHandlerEmpty());

                    e?.Invoke(this, EventArgs.Empty);

                    m_inferenceEngine.UnregisterInference("inferenceAssistancePicturalDistance");
                }, Utilities.Utility.getEventHandlerEmpty());
            }

            void SetReminderTransitions(FiniteStateMachine.MouseUtilitiesGradationAssistance state)
            {
                state.setFunctionHide(m_reminderController.Hide, Utilities.Utility.getEventHandlerEmpty());
            }

            void SetConnectWithArchTransitions(FiniteStateMachine.MouseUtilitiesGradationAssistance state)
            {
                state.addFunctionShow(m_assistanceConnectWithArchController.show, Utilities.Utility.getEventHandlerEmpty());
                state.addFunctionShow(m_reminderController.show, Utilities.Utility.getEventHandlerEmpty());

                state.setFunctionHide(m_assistanceConnectWithArchController.hide, Utilities.Utility.getEventHandlerEmpty());
            }

            void SetRagInteractionSurfaceTransitions(FiniteStateMachine.MouseUtilitiesGradationAssistance state)
            {
                state.addFunctionShow(delegate (EventHandler e)
                {
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                }, Utilities.Utility.getEventHandlerEmpty());

                state.addFunctionShow(m_assistanceSurfaceTouchedController.ShowInteractionCubesTablePanel, Utilities.Utility.getEventHandlerEmpty());
                state.addFunctionShow(m_reminderController.show, Utilities.Utility.getEventHandlerEmpty());

                state.setFunctionHide(m_assistanceSurfaceTouchedController.Hide, Utilities.Utility.getEventHandlerEmpty());
            }

            void SetSuccessTransitions(FiniteStateMachine.MouseUtilitiesGradationAssistance state)
            {
                state.addFunctionShow(delegate (EventHandler e)
                {
                    m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                    m_reminderController.Hide(Utilities.Utility.getEventHandlerEmpty());
                }, Utilities.Utility.getEventHandlerEmpty());

                state.addFunctionShow(m_successController.show, Utilities.Utility.getEventHandlerEmpty());

                state.setFunctionHide(m_successController.Hide, Utilities.Utility.getEventHandlerEmpty());
            }
        }

    }

}
