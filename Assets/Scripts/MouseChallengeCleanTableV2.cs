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


public class MouseChallengeCleanTableV2 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public int m_numberOfCubesToAddInRow;
    public int m_numberOfCubesToAddInColumn;

    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    Transform m_containerTableView;
    MouseTable m_containerTableController;
    Transform m_containerRagView;
    MouseRag m_containerRagController;

    Transform m_reminderView;
    MouseChallengeCleanTableReminderOneClockMoving m_reminderController;

    Transform m_assistanceCueingView;
    MouseCueing m_assistanceCueingController;

    Transform m_assistanceStimulateLevel2View;
    MouseChallengeCleanTableAssistanceStimulateLevel2 m_assistanceConnectWithArchController;

    Transform m_assisTancePicturalView;
    MouseAssistanceStimulateLevel1 m_assistancePicturalController;

    Transform m_assistanceSolutionView;
    MouseAssistanceBasic m_assistanceSolutionController;

    Transform m_successView;
    MouseAssistanceBasic m_successController;

    bool m_surfaceTableTouched; // Bool to detect the touch trigerring the challenge only once.
    bool m_surfaceRagTouched;

    public MouseUtilitiesTimer m_timer;

    enum ChallengeCleanTableStates
    {
        StandBy = 0,
        AssistanceStimulateLevel1 = 1,
        AssistanceStimulateLevel2 = 2,
        AssistanceCueing = 3,
        AssistanceSolution = 4,
        AssistanceReminder = 5,
        Challenge = 6,
        Success = 7
    }

    ChallengeCleanTableStates m_stateCurrent;
    ChallengeCleanTableStates m_statePrevious;

    Vector3 m_positionLocalReferenceForHolograms = new Vector3(0.0f, 0.6f, 0.0f);

    MouseUtilitiesGradationAssistanceManager m_assistanceGradationManager;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_surfaceTableTouched = false;
        m_surfaceRagTouched = true; // True by default, as we do not want the table to be populated if the user grabs the rag without having the challenge starting first. Indeed, in this situation, that means the users does not need any assistance.

        m_stateCurrent = ChallengeCleanTableStates.StandBy;
        m_statePrevious = ChallengeCleanTableStates.StandBy;

        m_timer.m_timerDuration = 20;

        // Children
        m_containerRagView = gameObject.transform.Find("Rag");
        m_containerTableView = gameObject.transform.Find("Table");

        m_reminderView = gameObject.transform.Find("Reminder");
        m_reminderController = m_reminderView.GetComponent<MouseChallengeCleanTableReminderOneClockMoving>();

        m_assistanceCueingView = gameObject.transform.Find("CueingWindow");
        m_assistanceCueingController = m_assistanceCueingView.GetComponent<MouseCueing>();

        m_assistanceStimulateLevel2View = gameObject.transform.Find("MouseChallengeCleanTableAssistanceStimulateLevel2");
        m_assistanceConnectWithArchController = m_assistanceStimulateLevel2View.GetComponent<MouseChallengeCleanTableAssistanceStimulateLevel2>();

        m_assisTancePicturalView = gameObject.transform.Find("AssistanceStimulateLevel1");
        m_assistancePicturalController = m_assisTancePicturalView.GetComponent<MouseAssistanceStimulateLevel1>();

        m_assistanceSolutionView = gameObject.transform.Find("SolutionWindow");
        m_assistanceSolutionController = m_assistanceSolutionView.GetComponent<MouseAssistanceBasic>();

        m_successView = gameObject.transform.Find("Success");
        m_successController = m_successView.GetComponent<MouseAssistanceBasic>();

        m_containerTableController = m_containerTableView.GetComponent<MouseTable>();

        m_containerRagController = m_containerRagView.GetComponent<MouseRag>();

        // Sanity checks

        // Timer for gradation
        m_timer.m_eventTimerFinished += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            if (m_assistancePicturalController.increaseGradation() == false)
            {
                m_timer.m_timerDuration = 20;
                m_timer.startTimerOneShot();
            }
            else
            { // Means we have reached the last level
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Last gradation level reached for stimulate assistance level 1 - no timer will be started anymore");

                // Increasing reminder assistance gradation
                m_reminderController.increaseGradation();
            }
        });

        m_assistanceGradationManager = new MouseUtilitiesGradationAssistanceManager();
        m_assistanceGradationManager.m_debug = m_debug;

        // Initialization of the scenario
        initializeScenario1();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void resetChallenge()
    {
        m_debug.displayMessage("MouseChallengeCleanTable", "resetChallenge", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_surfaceTableTouched = false;
        m_surfaceRagTouched = false;

        /*m_assistancePicturalController.hide(MouseUtilities.getEventHandlerEmpty());
        m_assistancePicturalController.setPositionToOriginalLocation();
        m_assistancePicturalController.setGradationToMinimum();
        m_reminderController.setGradationToMinimum();
        m_reminderController.hide(MouseUtilities.getEventHandlerEmpty());
        m_reminderController.setObjectToOriginalPosition();
        m_containerTableController.hideInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
        m_assistanceCueingController.hide(MouseUtilities.getEventHandlerEmpty());
        m_assistanceSolutionController.hide(MouseUtilities.getEventHandlerEmpty());
        m_assistanceConnectWithArchController.hide(MouseUtilities.getEventHandlerEmpty());
        m_successController.hide(MouseUtilities.getEventHandlerEmpty());*/

        m_timer.stopTimer();

        m_assistanceGradationManager.goBackToOriginalState();

        //m_assistanceGradationManager.
    }

    void initializeScenario1()
    {
        // Setting the parents, the connections for the objects briding other objects etc. (the idea being to leave that to a software dedicated to configure the scenarios)
        MouseUtilities.setParentToObject(m_assisTancePicturalView, m_containerTableView);
        MouseUtilities.setParentToObject(m_assistanceCueingView, m_containerTableView);
        MouseUtilities.setParentToObject(m_assistanceSolutionView, m_containerRagView);
        MouseUtilities.setParentToObject(m_successView, m_containerTableView);

        m_assistancePicturalController.m_surfaceWithStarsViewTarget = m_containerTableController.m_interactionSurfaceTableView;

        m_assistanceConnectWithArchController.setArchStartAndEndPoint(m_assistanceCueingView, m_containerRagView);
        m_reminderController.addObjectToBeClose(m_containerRagView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_hologramView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_surfaceWithStarsView);
        m_reminderController.addObjectToBeClose(m_assistancePicturalController.m_help);
        m_reminderController.addObjectToBeClose(m_assistanceCueingController.m_text);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_hologramHelp);
        m_reminderController.addObjectToBeClose(m_assistanceConnectWithArchController.m_hologramText);
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
        m_assistanceCueingController.m_eventHelpButtonClicked += sMessageCue.addGradationNext(sArchToRag);
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
    }

    void setStandByTransitions(MouseUtilitiesGradationAssistance state)
    { // This state is the initial state. So no transitions needed here
        state.addFunctionShow(delegate (EventHandler e)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Table surface should be touchable again");

            m_surfaceTableTouched = false; // Giving the possibility for the user to touch the surface again
            m_surfaceRagTouched = true;
            m_timer.stopTimer();
            m_assistancePicturalController.setGradationToMinimum();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(delegate (EventHandler e)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Hide function called for StandBy state");

            m_surfaceTableTouched = true;
            m_surfaceRagTouched = false; // From now on, if the user touches the rag, the surface will be populated (at least the callback supposed to trigger it will be fired.

            // Play sound to get the user's attention from audio on top of visually
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);

            // Start timer for gradation
            m_timer.startTimerOneShot();

            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
    }

    void setCubeRagTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistancePicturalController.show, MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Assistance stimulate level 1 should be displayed now"));
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Assistance reminder should be displayed now"));
        state.addFunctionShow(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(delegate (EventHandler e)
        {
            m_assistancePicturalController.hide(MouseUtilities.getEventHandlerEmpty());
            m_timer.stopTimer();

            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
    }

    void setReminderTableTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

            //m_timer.stopTimer();
            //m_assistancePicturalController.setGradationToMinimum();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_reminderController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setMessageCueTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.addFunctionShow(m_assistanceCueingController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerWithDebugMessage(m_debug, "Assistance reminder should be displayed now"));

        state.setFunctionHide(m_assistanceCueingController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setConnectWithArchTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistanceConnectWithArchController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());


        state.setFunctionHide(m_assistanceConnectWithArchController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setSolutionTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(m_assistanceSolutionController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_assistanceSolutionController.hide, MouseUtilities.getEventHandlerEmpty());
    }

    void setRagInteractionSurfaceTransitions(MouseUtilitiesGradationAssistance state)
    {
        state.addFunctionShow(delegate (EventHandler e)
        {
            //m_timer.stopTimer();
            m_reminderController.setGradationToMinimum();
        }, MouseUtilities.getEventHandlerEmpty());

        state.addFunctionShow(delegate (EventHandler e)
        {
            m_containerTableController.showInteractionSurfaceTable(MouseUtilities.getEventHandlerEmpty());
            m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
        }, MouseUtilities.getEventHandlerEmpty());

        
        state.addFunctionShow(m_reminderController.show, MouseUtilities.getEventHandlerEmpty());

        state.setFunctionHide(m_containerTableController.hideInteractionSurfaceTable, MouseUtilities.getEventHandlerEmpty());
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
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        fHide(new EventHandler(delegate (System.Object o, EventArgs e)
        {
            foreach (Action<EventHandler> fShow in fShows)
            {
                fShow(MouseUtilities.getEventHandlerEmpty());
            }
        }));
    }
}
