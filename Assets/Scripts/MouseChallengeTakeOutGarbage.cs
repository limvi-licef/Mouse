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

public class MouseChallengeTakeOutGarbage : MonoBehaviour
{
    public MouseUtilitiesContextualInferences m_inferenceManager;
    public MouseUtilitiesDisplayGraph m_graph;

    MouseUtilitiesGradationAssistanceManager m_gradationManager;
    

    MouseUtilitiesInferenceTime m_inference19h00;
    MouseUtilitiesInferenceTime m_inference19h30;

    EventHandler s_inference19h00;
    EventHandler s_inference19h30;

    public GameObject m_refInteractionSurface;
    public GameObject m_refCube;
    public GameObject m_refDialog;

    private void Awake()
    {
        // Initialize variables
        m_gradationManager = new MouseUtilitiesGradationAssistanceManager();

        
    }

    // Start is called before the first frame update
    void Start()
    {
        DateTime tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 0, 0);
        m_inference19h00 = new MouseUtilitiesInferenceTime("time19h00", tempTime, callbackTime19h00);

        tempTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 30, 0);
        m_inference19h30 = new MouseUtilitiesInferenceTime("time19h30", tempTime, callbackTime19h30);

        initializeScenario();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void initializeScenario()
    {
        // Instanciate assistances
        GameObject garbageInteractionSurfaceView = Instantiate(m_refInteractionSurface, gameObject.transform);
        MouseInteractionSurface garbageInteractionSurfaceController = garbageInteractionSurfaceView.GetComponent<MouseInteractionSurface>();
        garbageInteractionSurfaceController.setAdminButtons("garbage");
        garbageInteractionSurfaceController.setColor("Mouse_Purple_Glowing");
        garbageInteractionSurfaceController.showInteractionSurfaceTable(true);
        garbageInteractionSurfaceView.transform.localPosition = new Vector3(-0.92f, 0.383f, 3.881f);


        GameObject doorInteractionSurfaceView = Instantiate(m_refInteractionSurface, gameObject.transform);
        MouseInteractionSurface doorInteractionSurfaceController = doorInteractionSurfaceView.GetComponent<MouseInteractionSurface>();
        doorInteractionSurfaceController.setAdminButtons("door");
        doorInteractionSurfaceController.setColor("Mouse_Green_Glowing");
        doorInteractionSurfaceController.showInteractionSurfaceTable(true);
        doorInteractionSurfaceView.transform.localPosition = new Vector3(-1.032f, 0.544f, 2.617f);
        doorInteractionSurfaceController.setObjectResizable(true);

        GameObject exclamationMarkView = Instantiate(m_refCube, garbageInteractionSurfaceView.transform);
        MouseAssistanceBasic exclamationMarkController = exclamationMarkView.GetComponent<MouseAssistanceBasic>();
        exclamationMarkController.setMaterialToChild("Mouse_Exclamation");

        GameObject solutionView = Instantiate(m_refDialog, garbageInteractionSurfaceView.transform);
        MouseAssistanceDialog solutionController = solutionView.GetComponent<MouseAssistanceDialog>();
        solutionController.setDescription("Il est l'heure de sortir les poubelles!");

        GameObject highlightGarbageView = Instantiate(m_refCube, garbageInteractionSurfaceView.transform);
        MouseAssistanceBasic highlightGarbageController = highlightGarbageView.GetComponent<MouseAssistanceBasic>();
        highlightGarbageController.setAdjustHeightOnShow(false);
        highlightGarbageController.setMaterialToChild("Mouse_Cyan_Glowing");
        highlightGarbageController.setScale(0.2f, 0.6f, 0.2f);
        highlightGarbageController.setLocalPosition(0, -0.35f, 0);
        highlightGarbageController.setBillboard(false);

        GameObject highlightGarbageVividView = Instantiate(m_refCube, garbageInteractionSurfaceView.transform);
        MouseAssistanceBasic highlightGarbageVividController = highlightGarbageVividView.GetComponent<MouseAssistanceBasic>();
        highlightGarbageVividController.setAdjustHeightOnShow(false);
        highlightGarbageVividController.setMaterialToChild("Mouse_Orange_Glowing");
        highlightGarbageVividController.setScale(0.2f, 0.6f, 0.2f);
        highlightGarbageVividController.setLocalPosition(0, -0.35f, 0);
        highlightGarbageVividController.setBillboard(false);

        GameObject successView = Instantiate(m_refCube, garbageInteractionSurfaceView.transform);
        MouseAssistanceBasic successController = successView.GetComponent<MouseAssistanceBasic>();
        successController.setMaterialToChild("Mouse_Congratulation");

        // Add inferences
        m_inferenceManager.registerInference(m_inference19h00);

        // Set states
        MouseUtilitiesGradationAssistance sStandBy = m_gradationManager.addNewAssistanceGradation("StandBy");
        sStandBy.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceManager.registerInference(m_inference19h00);
        }, MouseUtilities.getEventHandlerEmpty());
        sStandBy.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        MouseUtilitiesGradationAssistance sHighlightGarbage = m_gradationManager.addNewAssistanceGradation("HighlightGarbage");
        sHighlightGarbage.setFunctionHideAndShow(highlightGarbageController);
        sHighlightGarbage.addFunctionShow(delegate (EventHandler e)
        {
            m_inferenceManager.registerInference(m_inference19h30);
        }, MouseUtilities.getEventHandlerEmpty());
        MouseUtilitiesGradationAssistance sExclamationMark = m_gradationManager.addNewAssistanceGradation("ExclamationMark");
        sExclamationMark.addFunctionShow(exclamationMarkController);
        sExclamationMark.addFunctionShow(highlightGarbageVividController);
        sExclamationMark.setFunctionHide(delegate (EventHandler e)
        {
            exclamationMarkController.hide(e);
            highlightGarbageVividController.hide(MouseUtilities.getEventHandlerEmpty());
        }, MouseUtilities.getEventHandlerEmpty());
        //sExclamationMark.setFunctionHideAndShow(exclamationMarkController);
        MouseUtilitiesGradationAssistance sSolution = m_gradationManager.addNewAssistanceGradation("Solution");
        sSolution.setFunctionHideAndShow(solutionController);
        MouseUtilitiesGradationAssistance sGarbageGrabbed = m_gradationManager.addNewAssistanceGradation("Garbage grabbed");
        sGarbageGrabbed.addFunctionShow(delegate(EventHandler e)
        {
           
        }, MouseUtilities.getEventHandlerEmpty());
        sGarbageGrabbed.setFunctionHide(delegate (EventHandler e)
        {
            e?.Invoke(this, EventArgs.Empty);
        }, MouseUtilities.getEventHandlerEmpty());
        MouseUtilitiesGradationAssistance sSuccess = m_gradationManager.addNewAssistanceGradation("Success");
        sSuccess.setFunctionHideAndShow(successController);

        
        // Connections between states
        s_inference19h00 += sStandBy.goToState(sHighlightGarbage);
        garbageInteractionSurfaceController.m_eventInteractionSurfaceTableTouched += sStandBy.goToState(/*sSuccess*/sGarbageGrabbed);
        highlightGarbageController.s_touched += sHighlightGarbage.goToState(sSolution);
        s_inference19h30 += sHighlightGarbage.goToState(sExclamationMark);
        garbageInteractionSurfaceController.m_eventInteractionSurfaceTableTouched += sHighlightGarbage.goToState(/*sSuccess*/sGarbageGrabbed);
        exclamationMarkController.s_touched += sExclamationMark.goToState(sSolution);
        highlightGarbageVividController.s_touched += delegate (System.Object o, EventArgs e) { exclamationMarkController.triggerTouch(); };
        
        garbageInteractionSurfaceController.m_eventInteractionSurfaceTableTouched += sExclamationMark.goToState(/*sSuccess*/sGarbageGrabbed);
        garbageInteractionSurfaceController.m_eventInteractionSurfaceTableTouched += sSolution.goToState(/*sSuccess*/sGarbageGrabbed);
        doorInteractionSurfaceController.m_eventInteractionSurfaceTableTouched += sGarbageGrabbed.goToState(sSuccess);
        successController.s_touched += sSuccess.goToState(sStandBy);

        m_gradationManager.setGradationInitial("StandBy");

        // Display graph
        m_graph.setManager(m_gradationManager);
    }

    void callbackTime19h00(System.Object o, EventArgs e)
    {
        m_inferenceManager.unregisterInference(m_inference19h00);

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Time 19h00 triggered");

        s_inference19h00?.Invoke(this, EventArgs.Empty);
    }

    void callbackTime19h30(System.Object o, EventArgs e)
    {
        m_inferenceManager.unregisterInference(m_inference19h30);

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Time 19h30 triggered");

        s_inference19h30?.Invoke(this, EventArgs.Empty);
    }
}
