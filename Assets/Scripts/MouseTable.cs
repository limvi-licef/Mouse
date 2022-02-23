using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class MouseTable : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    Transform m_interactionSurfaceTableView;
    MouseChallengeCleanTableSurfaceToPopulateWithCubes m_interactionSurfaceTableController;
    Transform m_assistanceReminderView;
    MouseChallengeCleanTableReminder m_assistanceReminderController;
    Transform m_assistanceStimulateLevel1View;
    MouseAssistanceStimulateLevel1 m_assistanceStimulateLevel1Controller;
    Transform m_assistanceChallengeSuccessView;
    MouseAssistanceChallengeSuccess m_assistanceChallengeSuccessController;


    public event EventHandler m_eventInteractionSurfaceTableTouched;
    public event EventHandler m_eventInteractionSurfaceCleaned;
    public event EventHandler m_eventReminderClockTouched;
    public event EventHandler m_eventReminderOkTouched;
    public event EventHandler m_eventReminderBackTouched;
    public event EventHandler m_eventAssistanceStimulateLevel1Touched;
    public event EventHandler m_eventAssistanceChallengeSuccessOk;

    MouseTable()
    {
        

    }

    // Start is called before the first frame update
    void Start()
    {
        // Children
        m_interactionSurfaceTableView = gameObject.transform.Find("InteractionSurfaceTable");
        m_interactionSurfaceTableController = m_interactionSurfaceTableView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>();
        m_assistanceReminderView = gameObject.transform.Find("MouseChallengeCleanTableAssistanceReminder");
        m_assistanceReminderController = m_assistanceReminderView.GetComponent<MouseChallengeCleanTableReminder>();
        m_assistanceStimulateLevel1View = gameObject.transform.Find("AssistanceStimulateLevel1");
        //MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_assistanceStimulateLevel1View, delegate () { m_eventAssistanceStimulateLevel1Touched?.Invoke(this, EventArgs.Empty); } );
        m_assistanceStimulateLevel1Controller = m_assistanceStimulateLevel1View.GetComponent<MouseAssistanceStimulateLevel1>();
        m_assistanceChallengeSuccessView = gameObject.transform.Find("Mouse_ChallengeCleanTableClose");
        m_assistanceChallengeSuccessController = m_assistanceChallengeSuccessView.GetComponent<MouseAssistanceChallengeSuccess>();


        // Sanity check
        if (m_interactionSurfaceTableView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>() == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "The m_hologramInteractionSurface object should have a MouseChallengeCleanTableSurfaceToPopulateWithCubes component");
        }

        // Connect the callbacks
        m_interactionSurfaceTableView.GetComponent<TapToPlace>().OnPlacingStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished);
        m_interactionSurfaceTableView.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished); // Use the same callback than for taptoplace as the process to do is the same
        m_interactionSurfaceTableView.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(delegate()
        {
            m_eventInteractionSurfaceTableTouched?.Invoke(this, EventArgs.Empty);
        }); // Only have to forward the event
        m_interactionSurfaceTableController.m_eventSurfaceCleaned += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventInteractionSurfaceCleaned?.Invoke(this, EventArgs.Empty); });
        m_assistanceReminderController.m_eventHologramClockTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventReminderClockTouched?.Invoke(this, EventArgs.Empty); });
        m_assistanceReminderController.m_eventHologramWindowButtonBackTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventReminderBackTouched?.Invoke(this, EventArgs.Empty); });
        m_assistanceReminderController.m_eventHologramWindowButtonOkTouched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventReminderOkTouched?.Invoke(this, EventArgs.Empty); });

        m_assistanceStimulateLevel1Controller.m_eventHologramStimulateLevel1Gradation1Or2Touched += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventAssistanceStimulateLevel1Touched?.Invoke(this, EventArgs.Empty); });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackHologramInteractionSurfaceMovedFinished()
    {
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Bring specific components to the center of the interaction surface

        //gameObject.transform.position = m_hologramInteractionSurface.transform.position;
        gameObject.transform.position = m_interactionSurfaceTableView.transform.position;
        m_interactionSurfaceTableView.transform.localPosition = new Vector3(0, 0f, 0);
    }

    public void hideInteractionSurfaceTable(EventHandler eventHandler)
    {
        m_interactionSurfaceTableController.resetCubesStates(eventHandler);
    }

    public void showInteractionSurfaceTable(EventHandler eventHandler)
    {
        m_interactionSurfaceTableController.showInteractionCubesTablePanel(eventHandler);
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

    public void hideAssistanceChallengeSuccess(EventHandler eventHandler)
    {
        if (m_assistanceChallengeSuccessView.gameObject.activeSelf)
        {
            m_assistanceChallengeSuccessController.hide(eventHandler);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance challenge success is disabled - no hide action to take");
        }
    }

    public void showAssistanceChallengeSuccess(EventHandler eventHandler)
    {
        if (m_assistanceChallengeSuccessView.gameObject.activeSelf == false)
        {
            m_assistanceChallengeSuccessController.show(eventHandler);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance challenge success is enabled - no show action to take");
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

    public void hideAssistanceStimulateLevel1(EventHandler eventHandler)
    { // We manage all the animations and the reset of the object here, as it is a simple cube, i.e. it does not have an attached customized component.

        /*if (m_assistanceStimulateLevel1View.gameObject.activeSelf)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube is going to be hidden");


            EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
               {
                   m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube should be hidden now");

                   m_assistanceStimulateLevel1View.gameObject.SetActive(false);
                   m_assistanceStimulateLevel1View.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                   Destroy(m_assistanceStimulateLevel1View.GetComponent<MouseUtilitiesAnimation>());
               }), eventHandler };

            m_assistanceStimulateLevel1View.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, eventHandlers);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 1 is disabled - no hide action to take");
        }*/

        m_assistanceStimulateLevel1Controller.hide(eventHandler);
        
    }

    public bool hasFocusAssistanceStimulateLevel1 ()
    {
        return m_assistanceStimulateLevel1Controller.hasFocus();
    }

    public MouseAssistanceStimulateLevel1.AssistanceGradation increaseGradationAssistanceStimulateLevel1()
    {
        return m_assistanceStimulateLevel1Controller.increaseGradation();
    }

    public MouseAssistanceStimulateLevel1.AssistanceGradation decreaseGradationAssistanceStimulateLevel1()
    {
        return m_assistanceStimulateLevel1Controller.decreaseGradation();
    }

    public void setGradationAssistanceStimulateLevel1ToMinimum()
    {
        m_assistanceStimulateLevel1Controller.setGradationToMinimum();
    }

    public void showAssistanceStimulateLevel1(EventHandler eventHandler)
    {
        /*if (m_assistanceStimulateLevel1View.gameObject.activeSelf == false)
        {
            MouseUtilitiesAnimation animator = m_assistanceStimulateLevel1View.gameObject.AddComponent<MouseUtilitiesAnimation>();

            EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
            {
                Destroy(animator);
            }), eventHandler };

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Target scaling: " + m_assistanceStimulateLevel1View.transform.localScale.ToString());

            animator.animateAppearInPlaceToScaling(m_assistanceStimulateLevel1View.transform.localScale, m_debug, eventHandlers);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 1 is enabled - no hide action to take");
        }*/

        m_assistanceStimulateLevel1Controller.show(eventHandler);
    }
}
