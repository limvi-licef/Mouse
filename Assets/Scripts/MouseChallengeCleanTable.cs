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

        // Initialization of the scenario
        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void initializeScenario()
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
        initialCueingController.setDescription("Que faites-vous typiquement après manger?", 0.2f);
        initialCueingController.addButton("Je ne sais pas", 0.2f);
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

        // Drawing the graph
        m_displayGraphController.setManager(m_assistanceGradationManager);

        // Add button to reset scenario
        MouseUtilitiesAdminMenu.Instance.addButton("Reset clean table challenge", delegate () { m_assistanceGradationManager.goBackToOriginalState(); });
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
            MouseUtilitiesInferenceDistanceFromObject inferenceAssistancePicturalDistance = new MouseUtilitiesInferenceDistanceFromObject("inferenceAssistancePicturalDistance", callbackInferenceDistanceAssistanceStimulateLevel1, m_assistancePicturalView.gameObject, 3.0f);

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
