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

    EventHandler s_defaultCalled;

    //public MouseUtilitiesTimer m_timer;

    Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

    MouseUtilitiesGradationAssistanceManager m_assistanceGradationManager;

    public MouseUtilitiesContextualInferences m_inferenceEngine;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        //m_timer.m_timerDuration = 20;

        // Children
        /*m_containerRagView = gameObject.transform.Find("Rag");
        m_containerTableView = gameObject.transform.Find("Table");*/

        m_reminderView = gameObject.transform.Find("Reminder");
        m_reminderController = m_reminderView.GetComponent<MouseChallengeCleanTableReminderOneClockMoving>();

        /*m_assistanceCueingView = gameObject.transform.Find("CueingWindow");
        m_assistanceCueingController = m_assistanceCueingView.GetComponent<MouseCueing>();*/

        m_assistanceStimulateLevel2View = gameObject.transform.Find("MouseChallengeCleanTableAssistanceStimulateLevel2");
        m_assistanceConnectWithArchController = m_assistanceStimulateLevel2View.GetComponent<MouseChallengeCleanTableAssistanceStimulateLevel2>();

        m_assistancePicturalView = gameObject.transform.Find("AssistanceStimulateLevel1");
        m_assistancePicturalController = m_assistancePicturalView.GetComponent<MouseAssistanceStimulateLevel1>();

        /*m_assistanceSolutionView = gameObject.transform.Find("SolutionWindow");
        m_assistanceSolutionController = m_assistanceSolutionView.GetComponent<MouseAssistanceBasic>();*/

        m_successView = gameObject.transform.Find("Success");
        m_successController = m_successView.GetComponent<MouseAssistanceBasic>();

        //m_containerTableController = m_containerTableView.GetComponent<MouseInteractionSurface>();

        //m_containerRagController = m_containerRagView.GetComponent<MouseRag>();

        m_assistanceSurfaceTouchedView = gameObject.transform.Find("AssistanceSurfaceTouched");
        m_assistanceSurfaceTouchedController = m_assistanceSurfaceTouchedView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>();

        m_displayGraphController = m_displayGraphView.GetComponent<MouseUtilitiesDisplayGraph>();

        // Sanity checks

        // Timer for gradation
        /*m_timer.m_eventTimerFinished += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            if (m_assistancePicturalController.increaseGradation() == false)
            {
                m_timer.m_timerDuration = 20;
                m_timer.startTimerOneShot();
            }
            else
            { // Means we have reached the last level
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Last gradation level reached for stimulate assistance level 1 - no timer will be started anymore");

                // Increasing reminder assistance gradation
                m_reminderController.increaseGradation();
            }
        });*/

        m_assistanceGradationManager = new MouseUtilitiesGradationAssistanceManager();

        // Initialization of the scenario
        // initializeScenario1();
        //initializeScenario2();
        initializeScenario1bis();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void resetChallenge()
    {
        MouseDebugMessagesManager.Instance.displayMessage("MouseChallengeCleanTable", "resetChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        //m_timer.stopTimer();

        m_assistanceGradationManager.goBackToOriginalState();
    }

    /*void initializeScenario2()
    {
        // Initializing the assistances we will be needing
        
        GameObject initialCueingView = Instantiate(m_refAssistanceDialog, m_containerTableView);
        MouseAssistanceDialog initialCueingController = initialCueingView.GetComponent<MouseAssistanceDialog>();
        //initialCueingView.SetActive(true);
        initialCueingController.setDescription("Que faites-vous typiquement après manger?", 0.2f);
        initialCueingController.addButton("Je ne sais pas", 0.2f);
        initialCueingController.show(MouseUtilities.getEventHandlerEmpty());

        // Gradations
        MouseUtilitiesGradationAssistance cueing = m_assistanceGradationManager.addNewAssistanceGradation("Cueing");
        MouseUtilitiesGradationAssistance later = m_assistanceGradationManager.addNewAssistanceGradation("Later");

        cueing.setFunctionHide(initialCueingController.hide, MouseUtilities.getEventHandlerEmpty());
        cueing.addFunctionShow(initialCueingController.show, MouseUtilities.getEventHandlerEmpty());

        // Links
        initialCueingController.m_buttonsController[0].s_buttonClicked += cueing.addGradationNext(later);

        m_assistanceGradationManager.setGradationInitial("Cueing");

        s_defaultCalled?.Invoke(this, EventArgs.Empty);
    }*/

    /*void initializeScenario1()
    {
        // Setting the parents, the connections for the objects briding other objects etc. (the idea being to leave that to a software dedicated to configure the scenarios)
        MouseUtilities.setParentToObject(m_assisTancePicturalView, m_containerTableView);
        MouseUtilities.setParentToObject(m_assistanceCueingView, m_containerTableView);
        MouseUtilities.setParentToObject(m_assistanceSolutionView, m_containerRagView);
        MouseUtilities.setParentToObject(m_successView, m_containerTableView);
        MouseUtilities.setParentToObject(m_assistanceStimulateLevel2View, m_containerRagView);

        m_assistancePicturalController.m_surfaceWithStarsViewTarget = m_containerTableController.m_interactionSurfaceTableView;

        m_assistanceConnectWithArchController.setArchStartAndEndPoint(m_assistanceCueingView, m_containerRagView);
        m_reminderController.addObjectToBeClose(m_containerRagView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_hologramView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_surfaceWithStarsView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_help);
        m_reminderController.addObjectToBeClose(m_assistanceCueingController.m_text);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_hologramHelp);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_textView);
        m_reminderController.addObjectToBeClose(m_assistanceSolutionController.m_childView);
        m_reminderController.addObjectToBeClose(m_containerTableController.m_interactionSurfaceTableView);

        // Settings the states
        MouseUtilitiesGradationAssistance sStandBy = m_assistanceGradationManager.addNewAssistanceGradation("StandBy");
        setStandByTransitions(sStandBy);
        MouseUtilitiesGradationAssistance sCubeRagTable = m_assistanceGradationManager.addNewAssistanceGradation("CubeRagTable");
        setCubeRagTransitions(sCubeRagTable);
        MouseUtilitiesGradationAssistance sReminder = m_assistanceGradationManager.addNewAssistanceGradation("ReminderCubeRagTable");
        setReminderTableTransitions(sReminder);
        MouseUtilitiesGradationAssistance sMessageCue = m_assistanceGradationManager.addNewAssistanceGradation("MessageCue");
        setMessageCueTransitions(sMessageCue);
        MouseUtilitiesGradationAssistance sReminderCueTable = m_assistanceGradationManager.addNewAssistanceGradation("ReminderCueTable");
        setReminderTableTransitions(sReminderCueTable);
        MouseUtilitiesGradationAssistance sArchToRag = m_assistanceGradationManager.addNewAssistanceGradation("ArchToRag");
        setConnectWithArchTransitions(sArchToRag);
        MouseUtilitiesGradationAssistance sSolution = m_assistanceGradationManager.addNewAssistanceGradation("Solution");
        setSolutionTransitions(sSolution);
        MouseUtilitiesGradationAssistance sSurfaceToClean = m_assistanceGradationManager.addNewAssistanceGradation("CleaningSurface");
        setRagInteractionSurfaceTransitions(sSurfaceToClean);
        MouseUtilitiesGradationAssistance sSuccess = m_assistanceGradationManager.addNewAssistanceGradation("Success");
        setSuccessTransitions(sSuccess);

        // Setting the initial state (so that the reset object can work)
        m_assistanceGradationManager.setGradationInitial("StandBy");

        // States changing
        m_containerTableController.m_eventInteractionSurfaceTableTouched += sStandBy.addGradationNext(sCubeRagTable);
        m_assistancePicturalController.m_eventHologramStimulateLevel1Gradation1Or2Touched += sCubeRagTable.addGradationNext(sMessageCue);
        m_assistanceCueingController.s_buttonClicked += sMessageCue.addGradationNext(sArchToRag);
        m_assistanceConnectWithArchController.m_eventHologramHelpTouched += sArchToRag.addGradationNext(sSolution);
        m_containerRagController.m_eventHologramInteractionSurfaceTouched += sCubeRagTable.addGradationNext(sSurfaceToClean);
        m_containerRagController.m_eventHologramInteractionSurfaceTouched += sMessageCue.addGradationNext(sSurfaceToClean);
        m_containerRagController.m_eventHologramInteractionSurfaceTouched += sArchToRag.addGradationNext(sSurfaceToClean);
        m_containerRagController.m_eventHologramInteractionSurfaceTouched += sSolution.addGradationNext(sSurfaceToClean);
        m_containerTableController.m_eventInteractionSurfaceCleaned += sSurfaceToClean.addGradationNext(sSuccess);
        m_successController.s_touched += sSuccess.addGradationNext(sStandBy);

        m_reminderController.m_eventHologramClockTouched += sCubeRagTable.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sMessageCue.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sArchToRag.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sSolution.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sSurfaceToClean.addGradationNext(sReminder);
        m_reminderController.m_eventHologramWindowButtonBackTouched += sReminder.setGradationPrevious();
        m_reminderController.m_eventHologramWindowButtonOkTouched += sReminder.addGradationNext(sStandBy);
    }*/

    void initializeScenario1bis()
    {
        /** Initializing the assistances we will be needing **/
        //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Initializing scenario");

        // First interaction surface, i.e. for the table
        GameObject interactionTableView = Instantiate(m_refInteractionSurface, gameObject.transform);
        MouseInteractionSurface interactionTableController = interactionTableView.GetComponent<MouseInteractionSurface>();
        interactionTableController.setAdminButtons("table");
        interactionTableController.setColor("Mouse_Cyan_Glowing");
        interactionTableView.transform.localPosition = new Vector3(0.949f, -0.017f, 1.117f);
        interactionTableController.setScaling(new Vector3(1.1f, 0.02f, 0.7f));
        interactionTableController.showInteractionSurfaceTable(true);

        // Second interaction surface, i.e. for the rag
        GameObject interactionRagView = Instantiate(m_refInteractionSurface, gameObject.transform);
        MouseInteractionSurface interactionRagController = interactionRagView.GetComponent<MouseInteractionSurface>();
        interactionRagController.setColor("Mouse_Orange_Glowing");
        interactionRagController.setAdminButtons("rag");
        interactionRagView.transform.localPosition = new Vector3(0, -0.008f, 3.843f);
        interactionRagController.showInteractionSurfaceTable(true);

        // Cueing for the beginning of the scenario
        GameObject initialCueingView = Instantiate(m_refAssistanceDialog, /*m_containerTableView*/ interactionTableView.transform);
        MouseAssistanceDialog initialCueingController = initialCueingView.GetComponent<MouseAssistanceDialog>();
        initialCueingController.setDescription("Que faites-vous typiquement après manger?", 0.2f);
        initialCueingController.addButton("Je ne sais pas", 0.2f);
        initialCueingController.enableBillboard(true);

        // Cueing for the solution
        GameObject solutionView = Instantiate(m_refAssistanceDialog, /*m_containerRagView*/ interactionRagView.transform);
        MouseAssistanceDialog solutionController = solutionView.GetComponent<MouseAssistanceDialog>();
        solutionController.setDescription("Ne serait-ce pas un bon moment pour nettoyer la table? \n Vous avez pour cela besoin du chiffon ci - dessous.", 0.15f);
        solutionController.enableBillboard(true);

        // Setting the parents, the connections for the objects briding other objects etc. (the idea being to leave that to a software dedicated to configure the scenarios)
        MouseUtilities.setParentToObject(m_assistancePicturalView, /*m_containerTableView*/ interactionTableView.transform);
        /*MouseUtilities.setParentToObject(m_assistanceCueingView, m_containerTableView);
        MouseUtilities.setParentToObject(m_assistanceSolutionView, m_containerRagView);*/
        MouseUtilities.setParentToObject(m_successView, /*m_containerTableView*/ interactionTableView.transform);
        MouseUtilities.setParentToObject(m_assistanceStimulateLevel2View, /*m_containerRagView*/ interactionRagView.transform);
        MouseUtilities.setParentToObject(m_assistanceSurfaceTouchedView, interactionTableView.transform);

        m_assistancePicturalController.m_surfaceWithStarsViewTarget = /*m_containerTableController.m_interactionSurfaceView*/ interactionTableController.getInteractionSurface();

        m_assistanceConnectWithArchController.setArchStartAndEndPoint(/*m_assistanceCueingView*/ initialCueingView.transform, /*m_containerRagView*/ interactionRagView.transform);
        m_reminderController.addObjectToBeClose(/*m_containerRagView*/ interactionRagView.transform);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_hologramView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_surfaceWithStarsView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_help);
        //m_reminderController.addObjectToBeClose(m_assistanceCueingController.m_text);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_hologramHelp);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_textView);
        //m_reminderController.addObjectToBeClose(m_assistanceSolutionController.m_childView);
        m_reminderController.addObjectToBeClose(/*m_containerTableController.m_interactionSurfaceView*/interactionTableController.getInteractionSurface());
        m_reminderController.addObjectToBeClose(initialCueingView.transform);
        m_reminderController.addObjectToBeClose(solutionView.transform);

        // Settings the states
        MouseUtilitiesGradationAssistance sStandBy = m_assistanceGradationManager.addNewAssistanceGradation("StandBy");
        setStandByTransitions(sStandBy);
        MouseUtilitiesGradationAssistance sCubeRagTable = m_assistanceGradationManager.addNewAssistanceGradation("CubeRagTable");
        setCubeRagTransitions(sCubeRagTable);
        MouseUtilitiesGradationAssistance sReminder = m_assistanceGradationManager.addNewAssistanceGradation("Reminder");
        setReminderTransitions(sReminder);
        MouseUtilitiesGradationAssistance sMessageCue = m_assistanceGradationManager.addNewAssistanceGradation("MessageCue");
        //setMessageCueTransitions(sMessageCue);
        sMessageCue.setFunctionHide(initialCueingController.hide, MouseUtilities.getEventHandlerEmpty());
        sMessageCue.addFunctionShow(initialCueingController.show, MouseUtilities.getEventHandlerEmpty());
        sMessageCue.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());
        //MouseUtilitiesGradationAssistance sReminderCueTable = m_assistanceGradationManager.addNewAssistanceGradation("ReminderCueTable");
        //setReminderTableTransitions(sReminderCueTable);
        MouseUtilitiesGradationAssistance sArchToRag = m_assistanceGradationManager.addNewAssistanceGradation("ArchToRag");
        setConnectWithArchTransitions(sArchToRag);
        MouseUtilitiesGradationAssistance sSolution = m_assistanceGradationManager.addNewAssistanceGradation("Solution");
        //setSolutionTransitions(sSolution);
        sSolution.setFunctionHide(solutionController.hide, MouseUtilities.getEventHandlerEmpty());
        sSolution.addFunctionShow(solutionController.show, MouseUtilities.getEventHandlerEmpty());
        sSolution.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());
        MouseUtilitiesGradationAssistance sSurfaceToClean = m_assistanceGradationManager.addNewAssistanceGradation("CleaningSurface");
        setRagInteractionSurfaceTransitions(sSurfaceToClean);
        MouseUtilitiesGradationAssistance sSuccess = m_assistanceGradationManager.addNewAssistanceGradation("Success");
        setSuccessTransitions(sSuccess);

        // Setting the initial state (so that the reset object can work)
        m_assistanceGradationManager.setGradationInitial("StandBy");

        // States changing
        /*m_containerTableController*/interactionTableController.m_eventInteractionSurfaceTableTouched += sStandBy.addGradationNext(sCubeRagTable);
        m_assistancePicturalController.m_eventHologramStimulateLevel1Gradation1Or2Touched += sCubeRagTable.addGradationNext(sMessageCue);
        initialCueingController.m_buttonsController[0].s_buttonClicked += sMessageCue.addGradationNext(sArchToRag);
        m_assistanceConnectWithArchController.m_eventHologramHelpTouched += sArchToRag.addGradationNext(sSolution);
        /*m_containerRagController.m_eventHologramInteractionSurfaceTouched*/
        interactionRagController.m_eventInteractionSurfaceTableTouched += sCubeRagTable.addGradationNext(sSurfaceToClean);
        /*m_containerRagController.m_eventHologramInteractionSurfaceTouched*/
        interactionRagController.m_eventInteractionSurfaceTableTouched += sMessageCue.addGradationNext(sSurfaceToClean);
        /*m_containerRagController.m_eventHologramInteractionSurfaceTouched*/
        interactionRagController.m_eventInteractionSurfaceTableTouched += sArchToRag.addGradationNext(sSurfaceToClean);
        /*m_containerRagController.m_eventHologramInteractionSurfaceTouched*/
        interactionRagController.m_eventInteractionSurfaceTableTouched += sSolution.addGradationNext(sSurfaceToClean);
        /*m_containerTableController*/
        /*interactionTableController.m_eventInteractionSurfaceCleaned*/
        m_assistanceSurfaceTouchedController.m_eventSurfaceCleaned += sSurfaceToClean.addGradationNext(sSuccess);
        m_successController.s_touched += sSuccess.addGradationNext(sStandBy);

        m_reminderController.m_eventHologramClockTouched += sCubeRagTable.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sMessageCue.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sArchToRag.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sSolution.addGradationNext(sReminder);
        m_reminderController.m_eventHologramClockTouched += sSurfaceToClean.addGradationNext(sReminder);
        m_reminderController.m_eventHologramWindowButtonBackTouched += sReminder.setGradationPrevious();
        m_reminderController.m_eventHologramWindowButtonOkTouched += sReminder.addGradationNext(sStandBy);

        // Drawing the graph
        m_displayGraphController.setManager(m_assistanceGradationManager);
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
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Table surface should be touchable again");

            //m_timer.stopTimer();
            m_assistancePicturalController.setGradationToMinimum();
            //m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(delegate (EventHandler e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Hide function called for StandBy state");

            // Play sound to get the user's attention from audio on top of visually
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);

            // Start timer for gradation
            //m_timer.startTimerOneShot();

            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
    }

    void setCubeRagTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistancePicturalController.show, MouseUtilities.getEventHandlerWithDebugMessage("Assistance stimulate level 1 should be displayed now"));
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerWithDebugMessage("Assistance reminder should be displayed now"));
        state.addFunctionShow(delegate (EventHandler e)
        {
            

            // Set the inference
            MouseUtilitiesInferenceDistanceFromObject inferenceAssistancePicturalDistance = new MouseUtilitiesInferenceDistanceFromObject("inferenceAssistancePicturalDistance", callbackInferenceDistanceAssistanceStimulateLevel1, m_assistancePicturalView.gameObject, 3.0f);

            m_inferenceEngine.registerInference(inferenceAssistancePicturalDistance);

            //e?.Invoke(this, EventArgs.Empty);

        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(delegate (EventHandler e)
        {
            m_assistancePicturalController.hide(MouseUtilities.getEventHandlerEmpty());
            //m_timer.stopTimer();

            e?.Invoke(this, EventArgs.Empty);

            m_inferenceEngine.unregisterInference("inferenceAssistancePicturalDistance");
        }, MouseUtilities.getEventHandlerEmpty());
    }

    void setReminderTransitions(MouseUtilitiesGradationAssistance state)
    {
        /*state.addFunctionShow(delegate (EventHandler e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

            //m_timer.stopTimer();
            //m_assistancePicturalController.setGradationToMinimum();
            //m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());*/

        //state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_reminderController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    /*void setMessageCueTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.addFunctionShow(m_assistanceCueingController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerWithDebugMessage("Assistance reminder should be displayed now"));

        state.setFunctionHide(m_assistanceCueingController.hide, MouseUtilities.getEventHandlerEmpty());
    }*/

    void setConnectWithArchTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistanceConnectWithArchController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());
        /*state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            //m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());*/


        state.setFunctionHide(m_assistanceConnectWithArchController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    /*void setSolutionTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistanceSolutionController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_assistanceSolutionController.hide, MouseUtilities.getEventHandlerEmpty());
    }*/

    void setRagInteractionSurfaceTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            //m_reminderController.setGradationToMinimum();
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Showing interaction surface");
        }, MouseUtilities.getEventHandlerEmpty());

        state.addFunctionShow(m_assistanceSurfaceTouchedController.showInteractionCubesTablePanel, MouseUtilities.getEventHandlerEmpty());

        
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());

        //state.setFunctionHide(m_containerTableController.hideInteractionSurfaceTable, MouseUtilities.getEventHandlerEmpty());
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

    // One to hide, and once hidden, can show several
    void hideAndShow(Action<EventHandler> fHide, List<Action<EventHandler>> fShows)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        fHide(new EventHandler(delegate (System.Object o, EventArgs e)
        {
            foreach (Action<EventHandler> fShow in fShows)
            {
                fShow(MouseUtilities.getEventHandlerEmpty());
            }
        }));
    }
}
