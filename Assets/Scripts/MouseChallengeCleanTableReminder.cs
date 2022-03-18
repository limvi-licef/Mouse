using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Reflection;

public class MouseChallengeCleanTableReminder : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    Transform m_hologramClockView;
    Transform m_hologramWindowReminderButtonOkView;
    Transform m_hologramWindowReminderView;
    Transform m_hologramWindowReminderButtonBackView;

    public event EventHandler m_eventHologramClockTouched;
    public event EventHandler m_eventHologramWindowButtonOkTouched;
    public event EventHandler m_eventHologramWindowButtonBackTouched;

    MouseUtilitiesGradationManager m_gradationManager;

    Vector3 m_positionLocalOrigin;

    bool m_mutexHiding = false;

    private void Awake()
    {
        // Let's get the children
        m_hologramClockView = gameObject.transform.Find("MouseChallengeCleanTableAssistanceReminderClock");
        m_hologramWindowReminderView = gameObject.transform.Find("MouseChallengeCleanTableAssistanceReminderWindow");

        m_hologramWindowReminderButtonOkView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonOk");
        m_hologramWindowReminderButtonBackView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonBack");

        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Connect callbacks
        InteractableOnTouchReceiver touchReceiver = m_hologramClockView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>();
        touchReceiver.OnTouchStart.AddListener(callbackOnClockTouched);
        m_hologramWindowReminderButtonOkView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowOkButtonTouched);
        m_hologramWindowReminderButtonBackView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowBackButtonTouched);

        m_gradationManager = transform.GetComponent<MouseUtilitiesGradationManager>();
        if (m_gradationManager == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "MouseUtilitiesGradationManager component not present in that gameobject, whereas it is required. The object will most likely not work properly");
        }
        m_gradationManager.addNewAssistanceGradation("Default", callbackGradationDefault);
        m_gradationManager.addNewAssistanceGradation("HighFollow", callbackGradationHighFollow);

        m_positionLocalOrigin = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
       //m_hologramClock.RotateAround(transform.position, Vector3.up, 20 * Time.deltaTime);
    }

    void callbackOnClockTouched()
    {
        m_eventHologramClockTouched?.Invoke(this, EventArgs.Empty);

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        MouseUtilitiesAnimation animator = m_hologramClockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
         {
             // When the clock has diseappeared, make the windows appearing
             animator = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
             animator.animateAppearInPlace(m_debug, MouseUtilities.getEventHandlerEmpty());

             Destroy(m_hologramClockView.GetComponent<MouseUtilitiesAnimation>());
         }));
    }

    void callbackOnWindowOkButtonTouched()
    {
        MouseUtilitiesAnimation animator = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
            resetObject();
        }));
    }

    void callbackOnWindowBackButtonTouched()
    {
        m_eventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
    }

    public void hide(EventHandler e)
    {
        if (m_mutexHiding == false)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");
            m_mutexHiding = true;

            if (m_hologramClockView == null || m_hologramWindowReminderView == null)
            {
                //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "At least one of the object required for this animation is not initialized, so nothing will happen");

                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - at least one reminder component is not initialized");

                m_mutexHiding = false;
            }
            else if (m_hologramClockView.gameObject.activeSelf == false && m_hologramWindowReminderView.gameObject.activeSelf == false)
            {
                //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "The two objects required for this animation are already hidden, so no animation will be started");

                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - one reminder component is already hidden");

                m_mutexHiding = false;
            }
            else
            {
                //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance is going to be hidden");

                GameObject temp;

                if (m_hologramClockView.gameObject.activeSelf)
                {
                    temp = m_hologramClockView.gameObject;
                }
                else
                { // Only two children for know so fine this way. But to be extended if it happens that more children are added in the future
                    temp = m_hologramWindowReminderView.gameObject;
                }

                EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object oe, EventArgs ee)
            {
                //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance hidden");

                gameObject.SetActive(false);

                resetObject();

                m_mutexHiding = false;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - object hidden");


                Destroy(temp.GetComponent<MouseUtilitiesAnimation>());

            }), e };

                MouseUtilitiesAnimation animator = temp.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(m_debug, eventHandlers);
            }
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - Request ignored");
        }

           
    }

    public void show(EventHandler eventHandler)
    {
        if (gameObject.activeSelf)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Object already active - no animation will be started");
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance is going to be displayed");

            EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Assistance reminder should be visible now");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
            }), eventHandler };

            

            MouseUtilitiesAnimation animator = gameObject.AddComponent<MouseUtilitiesAnimation>();
            animator.animateAppearInPlaceToScaling(gameObject.transform.localScale/*new Vector3(0.1f, 0.1f, 0.1f)*/, m_debug, eventHandlers);
        }
        
    }

    void callbackGradationDefault(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;
    }

    void callbackGradationHighFollow(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = false;
        GetComponent<RadialView>().enabled = true;
    }

    public bool increaseGradation()
    {
        return m_gradationManager.increaseGradation();
    }

    public bool decreaseGradation()
    {
        return m_gradationManager.decreaseGradation();
    }

    public void setGradationToMinimum()
    {
        m_gradationManager.setGradationToMinimum();
    }

    // Be aware that this function does not send the object back to its original position
    void resetObject()
    {
        m_hologramClockView.gameObject.SetActive(true);
        m_hologramWindowReminderView.gameObject.SetActive(false);

        m_hologramClockView.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        m_hologramWindowReminderView.gameObject.transform.localScale = new Vector3(1, 1, 1);

        m_gradationManager.setGradationToMinimum();
    }

    public void setObjectToOriginalPosition()
    {
        transform.localPosition = m_positionLocalOrigin;
    }
}
