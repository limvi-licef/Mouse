using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;

public class MouseRag : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    Transform m_interactionSurfaceRagView;
    //Transform m_interactionLineView;
    Transform m_assistanceStimulateLevel2View;
    MouseChallengeCleanTableAssistanceStimulateLevel2 m_assistanceStimulateLevel2Controller;
    Transform m_assistanceReminderView;
    MouseChallengeCleanTableReminder m_assistanceReminderController;
    Transform m_assistanceCueingView;
    MouseCueing m_assistanceCueingController;
    Transform m_assistanceSolutionView;
    MouseAssistanceSolution m_assistanceSolutionController;


    public event EventHandler m_eventHologramHelpButtonTouched;
    public event EventHandler m_eventHologramInteractionSurfaceTouched;
    public event EventHandler m_eventHologramHelpButtonCueingTouched;
    public event EventHandler m_eventHologramReminderClockTouched;
    public event EventHandler m_eventHologramReminderOkTouched;
    public event EventHandler m_eventHologramReminderBackTouched;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference of the children
        m_interactionSurfaceRagView = gameObject.transform.Find("InteractionSurfaceRag");
        m_assistanceStimulateLevel2View = gameObject.transform.Find("MouseChallengeCleanTableAssistanceStimulateLevel2");
        m_assistanceStimulateLevel2Controller = m_assistanceStimulateLevel2View.GetComponent<MouseChallengeCleanTableAssistanceStimulateLevel2>();
        m_assistanceReminderView = gameObject.transform.Find("Mouse_AssistanceReminder");
        m_assistanceReminderController = m_assistanceReminderView.GetComponent<MouseChallengeCleanTableReminder>();
        m_assistanceCueingView = gameObject.transform.Find("CueingWindow");
        m_assistanceCueingController = m_assistanceCueingView.GetComponent<MouseCueing>();
        m_assistanceSolutionView = gameObject.transform.Find("SolutionWindow");
        m_assistanceSolutionController = m_assistanceSolutionView.GetComponent<MouseAssistanceSolution>();

        // Connect the callbacks
        m_interactionSurfaceRagView.GetComponent<TapToPlace>().OnPlacingStopped.AddListener(callbackHologramRagInteractionSurfaceMovedFinished);
        m_interactionSurfaceRagView.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackHologramRagInteractionSurfaceMovedFinished);
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_interactionSurfaceRagView, delegate ()
        {
            m_eventHologramInteractionSurfaceTouched?.Invoke(this, EventArgs.Empty);
        });
        m_assistanceStimulateLevel2Controller.m_eventHologramHelpTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventHologramHelpButtonTouched?.Invoke(this, EventArgs.Empty); }); 
        m_assistanceCueingController.m_eventHelpButtonClicked += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventHologramHelpButtonCueingTouched?.Invoke(this, EventArgs.Empty); }); 
        //m_assistanceSolutionController.m_
        m_assistanceReminderController.m_eventHologramClockTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventHologramReminderClockTouched?.Invoke(this, EventArgs.Empty); }); 
        m_assistanceReminderController.m_eventHologramWindowButtonOkTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventHologramReminderOkTouched?.Invoke(this, EventArgs.Empty); }); 
        m_assistanceReminderController.m_eventHologramWindowButtonBackTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventHologramReminderBackTouched?.Invoke(this, EventArgs.Empty); }); 

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void callbackHologramRagInteractionSurfaceMovedFinished()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        gameObject.transform.position = m_interactionSurfaceRagView.transform.position;
        m_interactionSurfaceRagView.transform.localPosition = new Vector3(0, 0f, 0);
    }

    public void hideAssistanceStimulateLevel2(EventHandler eventHandler)
    {
        if (m_assistanceStimulateLevel2View.gameObject.activeSelf)
        {
            m_assistanceStimulateLevel2Controller.hide(eventHandler);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 2 already hidden - nothing to do");
        }
    }

    // Appear in place
    public void showAssistanceStimulateLevel2(EventHandler eventHandler)
    { 
        if (m_assistanceStimulateLevel2View.gameObject.activeSelf == false)
        {
            m_assistanceStimulateLevel2Controller.show(eventHandler);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 2 already shown - nothing to do");
        }
    }

    public void hideAssistanceReminder(EventHandler e)
    {
        if (m_assistanceReminderView.gameObject.activeSelf)
        {
            m_assistanceReminderController.hide(e);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance reminder is disabled - no hide action to take");
        }
    }

    public void showAssistanceReminder(EventHandler e)
    {
        if (m_assistanceReminderView.gameObject.activeSelf == false)
        {
            m_assistanceReminderController.show(e);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance reminder is disabled - no hide action to take");
        }
    }

    public void showAssistanceCueing(EventHandler e)
    {
        if (m_assistanceCueingView.gameObject.activeSelf == false)
        {
            m_assistanceCueingController.show(true, e);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance cueing is already shown - no hide action to take");
        }
    }

    public void hideAssistanceCueing(EventHandler e)
    {
        if (m_assistanceCueingView.gameObject.activeSelf)
        {
            m_assistanceCueingController.hide(true, e);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance cueing is already hidden - no hide action to take");
        }
    }

    public void showAssistanceSolution(EventHandler e)
    {
        if (m_assistanceSolutionView.gameObject.activeSelf == false)
        {
            m_assistanceSolutionController.show(true, e);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance solution is already shown - no hide action to take");
        }
    }

    public void hideAssistanceSolution(EventHandler e)
    {
        if (m_assistanceSolutionView.gameObject.activeSelf)
        {
            m_assistanceSolutionController.hide(true, e);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance solution is already hidden - no hide action to take");
        }
    }
}
