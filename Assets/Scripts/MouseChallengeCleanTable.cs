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


public class MouseChallengeCleanTable : MonoBehaviour
{
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;

    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    public GameObject m_refAssistanceDialog;
    public GameObject m_refInteractionSurface;

    Transform m_reminderView;
    MouseChallengeCleanTableReminderOneClockMoving m_reminderController;

    Transform m_assistanceStimulateLevel2View;
    MouseChallengeCleanTableAssistanceStimulateLevel2 m_assistanceConnectWithArchController;

    Transform m_assistancePicturalView;
    MouseAssistanceStimulateLevel1 m_assistancePicturalController;

    Transform m_successView;
    MouseAssistanceBasic m_successController;

    Transform m_assistanceSurfaceTouchedView;
    MouseChallengeCleanTableSurfaceToPopulateWithCubes m_assistanceSurfaceTouchedController;

    public Transform m_displayGraphView;
    MouseUtilitiesDisplayGraph m_displayGraphController;

    Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

    MouseUtilitiesGradationAssistanceManager m_assistanceGradationManager;

    public MouseUtilitiesContextualInferences m_inferenceEngine;

    EventHandler s_time20h;
    EventHandler s_ignoreRedSurface;
    EventHandler s_ignoreExclamationMark;
    EventHandler s_dialogSecondButtonOk;
    EventHandler s_dialogSecondButtonNok;
    EventHandler s_dialogSecondButtonLeave;
    EventHandler s_backToTable;
    EventHandler s_caregiverCall;

    MouseUtilitiesInferenceTime m_inference20h;

    // Start is called before the first frame update
    void Start()
    {
        // Variables

        // Children
        m_reminderView = gameObject.transform.Find("Reminder");
        m_reminderController = m_reminderView.GetComponent<MouseChallengeCleanTableReminderOneClockMoving>();

        m_assistanceStimulateLevel2View = gameObject.transform.Find("MouseChallengeCleanTableAssistanceStimulateLevel2");
        m_assistanceConnectWithArchController = m_assistanceStimulateLevel2View.GetComponent<MouseChallengeCleanTableAssistanceStimulateLevel2>();

        m_assistancePicturalView = gameObject.transform.Find("AssistanceStimulateLevel1");
        m_assistancePicturalController = m_assistancePicturalView.GetComponent<MouseAssistanceStimulateLevel1>();

        m_successView = gameObject.transform.Find("Success");
        m_successController = m_successView.GetComponent<MouseAssistanceBasic>();

        m_assistanceSurfaceTouchedView = gameObject.transform.Find("AssistanceSurfaceTouched");
        m_assistanceSurfaceTouchedController = m_assistanceSurfaceTouchedView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>();

        m_displayGraphController = m_displayGraphView.GetComponent<MouseUtilitiesDisplayGraph>();

        m_assistanceGradationManager = new MouseUtilitiesGradationAssistanceManager();

        DateTime tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
        m_inference20h = new MouseUtilitiesInferenceTime("time 20h", tempTime, callbackInferenceTime20h);

        // Initialization of the scenario
        initializeScenariov2();

        // Drawing the graph
        //m_displayGraphController.setManager(m_assistanceGradationManager);
    }

    void callbackInferenceTime20h(System.Object o, EventArgs e)
    {
        m_inferenceEngine.unregisterInference(m_inference20h);
        s_time20h?.Invoke(o, e);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void initializeScenariov1()
    {
        /** Initializing the assistances we will be needing **/
        // First interaction surface, i.e. for the table
        GameObject interactionTableView = Instantiate(m_refInteractionSurface, gameObject.transform);
        MouseInteractionSurface interactionTableController = interactionTableView.GetComponent<MouseInteractionSurface>();
        interactionTableController.setAdminButtons("table");
        interactionTableController.setColor("Mouse_Cyan_Glowing");
        interactionTableView.transform.localPosition = new Vector3(0.949f, -0.017f, 1.117f);
        interactionTableController.setScaling(new Vector3(1.1f, 0.02f, 0.7f));
        interactionTableController.showInteractionSurfaceTable(true);
        interactionTableController.setPreventResizeY(true);


        // Second interaction surface, i.e. for the rag
        GameObject interactionRagView = Instantiate(m_refInteractionSurface, gameObject.transform);
        MouseInteractionSurface interactionRagController = interactionRagView.GetComponent<MouseInteractionSurface>();
        interactionRagController.setColor("Mouse_Orange_Glowing");
        interactionRagController.setAdminButtons("rag");
        interactionRagView.transform.localPosition = new Vector3(0, -0.008f, 3.843f);
        interactionRagController.showInteractionSurfaceTable(true);

        m_assistanceSurfaceTouchedView.localScale = new Vector3(interactionTableController.getInteractionSurface().localScale.x,
                m_assistanceSurfaceTouchedView.localScale.y, interactionTableController.getInteractionSurface().localScale.z);
        interactionTableController.s_interactionSurfaceScaled += delegate
        {
            m_assistanceSurfaceTouchedView.localScale = new Vector3(interactionTableController.getInteractionSurface().localScale.x,
                m_assistanceSurfaceTouchedView.localScale.y, interactionTableController.getInteractionSurface().localScale.z);
        };
        

        // First stimulate assistance
        m_assistancePicturalController.setCubeMaterialVivid("Mouse_Help_Bottom_Vivid", "Mouse_Help_Top-Left_Vivid", "Mouse_Help_Top-Right_Vivid");

        // Cueing for the beginning of the scenario
        GameObject initialCueingView = Instantiate(m_refAssistanceDialog, interactionTableView.transform);
        MouseAssistanceDialog initialCueingController = initialCueingView.GetComponent<MouseAssistanceDialog>();
        initialCueingController.setDescription("Que faites-vous typiquement après manger?");
        initialCueingController.addButton("Je ne sais pas", true, 0.2f);
        initialCueingController.enableBillboard(true);

        // Cueing for the solution
        GameObject solutionView = Instantiate(m_refAssistanceDialog, interactionRagView.transform);
        MouseAssistanceDialog solutionController = solutionView.GetComponent<MouseAssistanceDialog>();
        solutionController.setDescription("Ne serait-ce pas un bon moment pour nettoyer la table? \n Vous avez pour cela besoin du chiffon ci - dessous.", 0.15f);
        solutionController.enableBillboard(true);

        // Setting the parents, the connections for the objects briding other objects etc. (the idea being to leave that to a software dedicated to configure the scenarios)
        MouseUtilities.setParentToObject(m_assistancePicturalView, interactionTableView.transform);
        MouseUtilities.setParentToObject(m_successView, interactionTableView.transform);
        MouseUtilities.setParentToObject(m_assistanceStimulateLevel2View, interactionRagView.transform);
        MouseUtilities.setParentToObject(m_assistanceSurfaceTouchedView, interactionTableView.transform);

        m_assistancePicturalController.m_surfaceWithStarsViewTarget = interactionTableController.getInteractionSurface();

        m_assistanceConnectWithArchController.setArchStartAndEndPoint(initialCueingView.transform, interactionRagView.transform);
        m_reminderController.addObjectToBeClose(interactionRagView.transform);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_hologramView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_surfaceWithStarsView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_help);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_hologramHelp);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_textView);
        m_reminderController.addObjectToBeClose(interactionTableController.getInteractionSurface());
        m_reminderController.addObjectToBeClose(initialCueingView.transform);
        m_reminderController.addObjectToBeClose(solutionView.transform);
        m_reminderController.addObjectToBeClose(m_assistanceSurfaceTouchedView);

        // Settings the states
        MouseUtilitiesGradationAssistance sStandBy = m_assistanceGradationManager.addNewAssistanceGradation("StandBy");
        setStandByTransitions(sStandBy);
        MouseUtilitiesGradationAssistance sCubeRagTable = m_assistanceGradationManager.addNewAssistanceGradation("CubeRagTable");
        setCubeRagTransitions(sCubeRagTable);
        MouseUtilitiesGradationAssistance sReminder = m_assistanceGradationManager.addNewAssistanceGradation("Reminder");
        setReminderTransitions(sReminder);
        MouseUtilitiesGradationAssistance sMessageCue = m_assistanceGradationManager.addNewAssistanceGradation("MessageCue");
        sMessageCue.setFunctionHideAndShow(initialCueingController);
        sMessageCue.addFunctionShow(m_reminderController);
        MouseUtilitiesGradationAssistance sArchToRag = m_assistanceGradationManager.addNewAssistanceGradation("ArchToRag");
        setConnectWithArchTransitions(sArchToRag);
        MouseUtilitiesGradationAssistance sSolution = m_assistanceGradationManager.addNewAssistanceGradation("Solution");
        sSolution.setFunctionHideAndShow(solutionController);
        sSolution.addFunctionShow(m_reminderController);
        MouseUtilitiesGradationAssistance sSurfaceToClean = m_assistanceGradationManager.addNewAssistanceGradation("CleaningSurface");
        setRagInteractionSurfaceTransitions(sSurfaceToClean);
        MouseUtilitiesGradationAssistance sSuccess = m_assistanceGradationManager.addNewAssistanceGradation("Success");
        setSuccessTransitions(sSuccess);

        // Setting the initial state (so that the reset object can work)
        m_assistanceGradationManager.setGradationInitial("StandBy");

        // States changing
        interactionTableController.m_eventInteractionSurfaceTableTouched += sStandBy.goToState(sCubeRagTable);
        m_assistancePicturalController.m_eventHologramStimulateLevel1Gradation1Or2Touched += sCubeRagTable.goToState(sMessageCue);
        initialCueingController.m_buttonsController[0].s_buttonClicked += sMessageCue.goToState(sArchToRag);
        m_assistanceConnectWithArchController.m_eventHologramHelpTouched += sArchToRag.goToState(sSolution);
        interactionRagController.m_eventInteractionSurfaceTableTouched += sCubeRagTable.goToState(sSurfaceToClean);
        interactionRagController.m_eventInteractionSurfaceTableTouched += sMessageCue.goToState(sSurfaceToClean);
        interactionRagController.m_eventInteractionSurfaceTableTouched += sArchToRag.goToState(sSurfaceToClean);
        interactionRagController.m_eventInteractionSurfaceTableTouched += sSolution.goToState(sSurfaceToClean);
        m_assistanceSurfaceTouchedController.m_eventSurfaceCleaned += sSurfaceToClean.goToState(sSuccess);
        m_successController.s_touched += sSuccess.goToState(sStandBy);

        m_reminderController.m_eventHologramClockTouched += sCubeRagTable.goToState(sReminder);
        m_reminderController.m_eventHologramClockTouched += sMessageCue.goToState(sReminder);
        m_reminderController.m_eventHologramClockTouched += sArchToRag.goToState(sReminder);
        m_reminderController.m_eventHologramClockTouched += sSolution.goToState(sReminder);
        m_reminderController.m_eventHologramClockTouched += sSurfaceToClean.goToState(sReminder);
        m_reminderController.m_eventHologramWindowButtonBackTouched += sReminder.setGradationPrevious();
        m_reminderController.m_eventHologramWindowButtonOkTouched += sReminder.goToState(sStandBy);

        

        // Add button to reset scenario
        MouseUtilitiesAdminMenu.Instance.addButton("Reset clean table challenge", delegate () { m_assistanceGradationManager.goBackToOriginalState(); });
    }

    void initializeScenariov2()
    {
        // Interaction surface table
        MouseInteractionSurface interactionSurfaceTable = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("table v2", MouseUtilitiesAdminMenu.Panels.Default, new Vector3(1.1f, 0.02f, 0.7f), "Mouse_Cyan_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        //interactionSurfaceTable.setLocalPosition(new Vector3(0.949f, -0.017f, 1.117f));
        interactionSurfaceTable.setPreventResizeY(true);
        interactionSurfaceTable.transform.position = new Vector3(0.8258258700370789f, 0.4396502375602722f, 2.451075315475464f);

        // Interaction surface rag
        MouseInteractionSurface interactionSurfaceRag = MouseUtilitiesAssistancesFactory.Instance.createInteractionSurface("rag v2", MouseUtilitiesAdminMenu.Panels.Default, new Vector3(0.2f, 0.01f, 0.2f), "Mouse_Orange_Glowing", true, true, MouseUtilities.getEventHandlerEmpty(), transform);
        interactionSurfaceRag.transform.localPosition = new Vector3(0, -0.008f, 3.843f);
        //interactionSurfaceRag.getInteractionSurface().transform.localPosition = new Vector3(0, -0.008f, 3.843f);


        // Red surface on table
        MouseAssistanceBasic redSurface = MouseUtilitiesAssistancesFactory.Instance.createFlatSurface("Mouse_Red_Glowing", new Vector3(interactionSurfaceTable.getLocalPosition().x, interactionSurfaceTable.getLocalPosition().y+0.02f, interactionSurfaceTable.getLocalPosition().z), interactionSurfaceTable.transform);
        redSurface.setScale(interactionSurfaceTable.getLocalScale().x, redSurface.getScale().y, interactionSurfaceTable.getLocalScale().z);
        interactionSurfaceTable.s_interactionSurfaceScaled += delegate { redSurface.setScale(new Vector3(interactionSurfaceTable.getInteractionSurface().localScale.x, redSurface.getScale().y, interactionSurfaceTable.getInteractionSurface().localScale.z)); };

        // Exclamation mark
        MouseAssistanceBasic exclamationMark = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Exclamation_Red", true, new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 0, 0), true, interactionSurfaceTable.transform);

        // First message
        string firstMessage = "Que faites-vous normalement après manger?"; // Used in the first two dialogs
        MouseAssistanceDialog firstDialog = MouseUtilitiesAssistancesFactory.Instance.createDialogNoButton("", firstMessage, interactionSurfaceTable.transform);

        // Second dialog
        MouseAssistanceDialog secondDialog = MouseUtilitiesAssistancesFactory.Instance.createDialogThreeButtons("", firstMessage, "Je sais!", delegate(System.Object o, EventArgs e) { s_dialogSecondButtonOk?.Invoke(this, e); }, "Je ne sais pas", delegate (System.Object o, EventArgs e) { s_dialogSecondButtonNok?.Invoke(this, e); }, "Cela ne m'intéresse pas", delegate (System.Object o, EventArgs e) { s_dialogSecondButtonLeave?.Invoke(this, e); }, interactionSurfaceTable.transform);

        // Surface to clean
        MouseChallengeCleanTableSurfaceToPopulateWithCubes surfaceToProcess = MouseUtilitiesAssistancesFactory.Instance.createSurfaceToProcess(interactionSurfaceTable.transform);
        surfaceToProcess.transform.localScale = new Vector3(interactionSurfaceTable.getInteractionSurface().localScale.x, surfaceToProcess.transform.localScale.y, interactionSurfaceTable.getInteractionSurface().localScale.z);
        interactionSurfaceTable.s_interactionSurfaceScaled += delegate (System.Object o, EventArgs e)
        {
            surfaceToProcess.transform.localScale = new Vector3(interactionSurfaceTable.getInteractionSurface().localScale.x, surfaceToProcess.transform.localScale.y, interactionSurfaceTable.getInteractionSurface().localScale.z);
        };

        // Dialog to ask to get the rag
        MouseAssistanceDialog dialogRag = MouseUtilitiesAssistancesFactory.Instance.createDialogNoButton("", "Vous devez nettoyer la table avec un chiffon", interactionSurfaceTable.transform);

        // Dialog to inform where the rag is
        // To do later

        // Calling the caregiver
        MouseAssistanceDialog dialogCallCaregiver =  MouseUtilitiesAssistancesFactory.Instance.createDialogTwoButtons("", "Est-ce que j'appelle votre aidant?", "Oui", delegate(System.Object o, EventArgs e) { s_caregiverCall?.Invoke(this, e); }, "Non", delegate (System.Object o, EventArgs e) { s_caregiverCall?.Invoke(this, e); }, interactionSurfaceTable.transform);

        // Success
        MouseAssistanceBasic success = MouseUtilitiesAssistancesFactory.Instance.createCube("Mouse_Congratulation", interactionSurfaceTable.transform);

        // Inferences
        //MouseUtilitiesInferenceTime i20h = new MouseUtilitiesInferenceTime("")
        /*MouseUtilitiesContextualInferencesFactory.Instance.createTemporalInferenceOneShot(m_inferenceEngine, "CleanTable20h", delegate (System.Object o, EventArgs e)
        {
            s_time20h?.Invoke(this, e);
        }, 20);*/
        m_inferenceEngine.registerInference(m_inference20h);
        

        MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "IgnoreRedSurfaceAndExclamationMark", delegate (System.Object o, EventArgs e)
        {
            s_ignoreRedSurface?.Invoke(this, e);
        }/*s_ignoreExclamationMark*/, redSurface.gameObject);
        
        // States
        MouseUtilitiesGradationAssistance sStandBy = m_assistanceGradationManager.addNewAssistanceGradation("Stand-by");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Standby show function called");

            //MouseUtilitiesInferenceTime i20h = new MouseUtilitiesInferenceTime("")
            /*MouseUtilitiesContextualInferencesFactory.Instance.createTemporalInferenceOneShot(m_inferenceEngine, "CleanTable20h", delegate (System.Object o, EventArgs ee)
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Callback for 20h called. Signal should be triggered ...");

                s_time20h?.Invoke(this, ee);
            }, 20);*/
            m_inferenceEngine.registerInference(m_inference20h);
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        { e?.Invoke(this, EventArgs.Empty); }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sRedSurface = m_assistanceGradationManager.addNewAssistanceGradation("Red surface");
        sRedSurface.setFunctionHideAndShow(redSurface);
        sRedSurface.addFunctionShow(delegate (EventHandler eh)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "IgnoreRedSurface", delegate (System.Object o, EventArgs e)
            {
                s_ignoreRedSurface.Invoke(this, e);
            }/*s_ignoreRedSurface*/, redSurface.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sRedSurfaceAndExclamation = m_assistanceGradationManager.addNewAssistanceGradation("Red surface + exclamation mark");
        sRedSurfaceAndExclamation.addFunctionShow(exclamationMark);
        sRedSurfaceAndExclamation.addFunctionShow(redSurface);
        sRedSurfaceAndExclamation.setFunctionHide(delegate (EventHandler e)
        {
            exclamationMark.hide(MouseUtilities.getEventHandlerEmpty());
            redSurface.hide(MouseUtilities.getEventHandlerEmpty());

            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sFirstDialog = m_assistanceGradationManager.addNewAssistanceGradation("First dialog");
        sFirstDialog.setFunctionHideAndShow(firstDialog);
        sFirstDialog.addFunctionShow(delegate (EventHandler e) {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "BackToTable", delegate (System.Object oo, EventArgs ee)
            {
                s_backToTable.Invoke(this, ee);
            }, interactionSurfaceTable.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sProcessSurface = m_assistanceGradationManager.addNewAssistanceGradation("Process surface");
        sProcessSurface.addFunctionShow(surfaceToProcess.showInteractionCubesTablePanel, MouseUtilities.getEventHandlerEmpty());
        sProcessSurface.setFunctionHide(surfaceToProcess.hide, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sSecondDialog = m_assistanceGradationManager.addNewAssistanceGradation("Second dialog");
        sSecondDialog.setFunctionHideAndShow(secondDialog);

        MouseUtilitiesGradationAssistance sDialogRag = m_assistanceGradationManager.addNewAssistanceGradation("Dialog rag");
        sDialogRag.setFunctionHideAndShow(dialogRag);
        sDialogRag.addFunctionShow(delegate (EventHandler e) {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "BackToTable", delegate (System.Object oo, EventArgs ee)
            {
                s_backToTable.Invoke(this, ee);
            }/*s_ignoreExclamationMark*/, interactionSurfaceTable.gameObject);
        }, MouseUtilities.getEventHandlerEmpty());

        MouseUtilitiesGradationAssistance sSuccess = m_assistanceGradationManager.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(success);

        MouseUtilitiesGradationAssistance sCaregiverCall = m_assistanceGradationManager.addNewAssistanceGradation("Caregiver call");
        sCaregiverCall.setFunctionHideAndShow(dialogCallCaregiver);

        // Set original state
        m_assistanceGradationManager.setGradationInitial("Stand-by");

        // Connecting the states
        s_time20h += delegate (System.Object o, EventArgs e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Inference 20h for cleaning table called");
        };
        s_time20h += sStandBy.goToState(sRedSurface);
        s_ignoreRedSurface += sRedSurface.goToState(sRedSurfaceAndExclamation);
        exclamationMark.s_touched += sRedSurfaceAndExclamation.goToState(sFirstDialog);

        interactionSurfaceRag.m_eventInteractionSurfaceTableTouched += delegate (System.Object o, EventArgs e)
        {
            m_inferenceEngine.unregisterInference("BackToTable");
            m_inferenceEngine.unregisterInference("BackToTableInternal");
        };
        interactionSurfaceRag.m_eventInteractionSurfaceTableTouched += sRedSurface.goToState(sProcessSurface);
        interactionSurfaceRag.m_eventInteractionSurfaceTableTouched += sRedSurfaceAndExclamation.goToState(sProcessSurface);
        interactionSurfaceRag.m_eventInteractionSurfaceTableTouched += sFirstDialog.goToState(sProcessSurface);
        interactionSurfaceRag.m_eventInteractionSurfaceTableTouched += sDialogRag.goToState(sProcessSurface);

        s_backToTable += sFirstDialog.goToState(sSecondDialog);

        s_dialogSecondButtonOk += sSecondDialog.goToState(sProcessSurface);
        s_dialogSecondButtonOk += delegate (System.Object o, EventArgs e)
        {
            MouseUtilitiesContextualInferencesFactory.Instance.createDistanceLeavingAndComingInferenceOneShot(m_inferenceEngine, "BackToTable", delegate (System.Object oo, EventArgs ee)
            {
                s_backToTable.Invoke(this, e);
            }/*s_ignoreExclamationMark*/, interactionSurfaceTable.gameObject);
        };
        
        s_backToTable += sProcessSurface.goToState(sDialogRag);
        
        s_dialogSecondButtonNok += sSecondDialog.goToState(sDialogRag);
        s_dialogSecondButtonLeave += sSecondDialog.goToState(sStandBy);

        surfaceToProcess.m_eventSurfaceCleaned += sProcessSurface.goToState(sSuccess);

        s_backToTable += sDialogRag.goToState(sCaregiverCall);
        s_caregiverCall += sCaregiverCall.goToState(sStandBy);

        success.s_touched += sSuccess.goToState(sStandBy);

    }

    void callbackInferenceDistanceAssistanceStimulateLevel1(System.Object sender, EventArgs args)
    {
        m_assistancePicturalController.increaseGradation();
        m_inferenceEngine.unregisterInference("inferenceAssistancePicturalDistance");
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void setStandByTransitions(MouseUtilitiesGradationAssistance state)
    { // This state is the initial state. So no transitions needed here
        state.addFunctionShow(delegate (EventHandler e)
        {
            m_assistancePicturalController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(delegate (EventHandler e)
        {
            // Play sound to get the user's attention from audio on top of visually
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
    }

    void setCubeRagTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistancePicturalController);
        state.addFunctionShow(m_reminderController);
        state.addFunctionShow(delegate (EventHandler e)
        {
            // Set the inference
            MouseUtilitiesInferenceDistanceLeaving inferenceAssistancePicturalDistance = new MouseUtilitiesInferenceDistanceLeaving("inferenceAssistancePicturalDistance", callbackInferenceDistanceAssistanceStimulateLevel1, m_assistancePicturalView.gameObject, 3.0f);

            m_inferenceEngine.registerInference(inferenceAssistancePicturalDistance);
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(delegate (EventHandler e)
        {
            m_assistancePicturalController.hide(MouseUtilities.getEventHandlerEmpty());

            e?.Invoke(this, EventArgs.Empty);

            m_inferenceEngine.unregisterInference("inferenceAssistancePicturalDistance");
        }, MouseUtilities.getEventHandlerEmpty());
    }

    void setReminderTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.setFunctionHide(m_reminderController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setConnectWithArchTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistanceConnectWithArchController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_assistanceConnectWithArchController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setRagInteractionSurfaceTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
        }, MouseUtilities.getEventHandlerEmpty());

        state.addFunctionShow(m_assistanceSurfaceTouchedController.showInteractionCubesTablePanel, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_assistanceSurfaceTouchedController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setSuccessTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
            m_reminderController.hide(MouseUtilities.getEventHandlerEmpty());
        }, MouseUtilities.getEventHandlerEmpty());

        state.addFunctionShow(m_successController.show, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_successController.hide, MouseUtilities.getEventHandlerEmpty());
    }
}
