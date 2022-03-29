using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Reflection;
using System.Linq;

public class MouseChallengeCleanTableReminderSeveralClocksFixed : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    List<Transform> m_clocksView; // Because we can have multiple clocks
    Transform m_clockRefView; // The clock object already implemented, that will be duplicated on request of the user

    Transform m_hologramWindowReminderButtonOkView;
    Transform m_hologramWindowReminderView;
    Transform m_hologramWindowReminderButtonBackView;

    public event EventHandler m_eventHologramClockTouched;
    public event EventHandler m_eventHologramWindowButtonOkTouched;
    public event EventHandler m_eventHologramWindowButtonBackTouched;

    MouseUtilitiesGradationManager m_gradationManager;

    List<Vector3> m_positionsLocalOrigin;

    // As the clock does not have an attached script, storing the required information here
    Vector3 m_clockScalingOriginal;
    Vector3 m_clockScalingReduced;

    //bool m_mutexHiding = false;
    MouseUtilitiesMutex m_mutexHide;

    private void Awake()
    {
        // Let's get the children
        m_clockRefView = gameObject.transform.Find("Clock");
        m_hologramWindowReminderView = gameObject.transform.Find("Text");

        m_hologramWindowReminderButtonOkView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonOk");
        m_hologramWindowReminderButtonBackView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonBack");

        m_mutexHide = new MouseUtilitiesMutex(m_debug);

    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        m_clocksView = new List<Transform>();
        m_positionsLocalOrigin = new List<Vector3>();

        // Connect callbacks
        //InteractableOnTouchReceiver touchReceiver = m_hologramClockView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>();
        //touchReceiver.OnTouchStart.AddListener(callbackOnClockTouched);
        m_hologramWindowReminderButtonOkView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowOkButtonTouched);
        m_hologramWindowReminderButtonBackView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowBackButtonTouched);

        m_gradationManager = transform.GetComponent<MouseUtilitiesGradationManager>();
        if (m_gradationManager == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "MouseUtilitiesGradationManager component not present in that gameobject, whereas it is required. The object will most likely not work properly");
        }
        m_gradationManager.addNewAssistanceGradation("Default", callbackGradationDefault);
        m_gradationManager.addNewAssistanceGradation("HighFollow", callbackGradationHighFollow);

        //m_positionLocalOrigin = transform.localPosition;

        // Getting informations related to the clock
        m_clockScalingOriginal = m_clockRefView.localScale;
        m_clockScalingReduced = m_clockScalingOriginal / 3.0f;
    }

    // Update is called once per frame
    void Update()
    {
       //m_hologramClock.RotateAround(transform.position, Vector3.up, 20 * Time.deltaTime);
    }

    public void addClock(Transform parent) // The parent the clock will belong to
    {
        m_clocksView.Add(Instantiate(m_clockRefView));

        MouseUtilitiesHologramInteractions temp = m_clocksView.Last().GetComponent<MouseUtilitiesHologramInteractions>();
        //touchReceiver.OnTouchStart.AddListener(callbackOnClockTouched);
        temp.s_touched += callbackOnClockTouched;

        m_positionsLocalOrigin.Add(m_clocksView.Last().localPosition);
    }


    void callbackOnClockTouched(System.Object sender, EventArgs e)
    {// If a clock is touched, all other clocks are hidden

        foreach (Transform clock in m_clocksView)
        {
            if (clock != (Transform)sender)
            { // We want to hide only the clocks that have not been touched by the sender
                MouseUtilitiesAnimation animator = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs ee)
                {
                    clock.gameObject.SetActive(false);

                    // When the clock has diseappeared, make the text appearing, after moving it to the place of the hidden clock
                    m_hologramWindowReminderView.position = clock.position;
                    MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animatorText.animateAppearInPlace(m_debug, MouseUtilities.getEventHandlerEmpty());

                    Destroy(clock.GetComponent<MouseUtilitiesAnimation>());
                }));
            }
        }

        m_eventHologramClockTouched?.Invoke(this, EventArgs.Empty); // Informing the world that a clock has been touched

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        /*TODO-done MouseUtilitiesAnimation animator = m_hologramClockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
         {
             m_hologramClockView.gameObject.SetActive(false);

             // When the clock has diseappeared, make the windows appearing
             MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
             animatorText.animateAppearInPlace(m_debug, MouseUtilities.getEventHandlerEmpty());

             Destroy(m_hologramClockView.GetComponent<MouseUtilitiesAnimation>());
         }));*/
    }

    void callbackOnWindowOkButtonTouched()
    {
        m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);

        /*MouseUtilitiesAnimation animator = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
            resetObject();
        }));*/
    }

    void callbackOnWindowBackButtonTouched()
    {
        m_eventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
    }

    public void hide(EventHandler e)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (/*m_mutexHiding == false*/ m_mutexHide.isLocked() == false)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");
            //m_mutexHiding = true;
            m_mutexHide.lockMutex();

            GameObject temp = null;

            // For each clock displayed, hiding it
            foreach (Transform clock in m_clocksView)
            {
                if ( clock.gameObject.activeSelf)
                {
                    MouseUtilitiesAnimation animator = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {
                        clock.gameObject.SetActive(false);
                        Destroy(clock.GetComponent<MouseUtilitiesAnimation>());

                        // Unlocking the mutex at the last element of the list
                        if (clock == m_clocksView.Last())
                        {
                            //m_mutexHiding = false;
                            m_mutexHide.unlockMutex();
                            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");
                        }
                    }));
                }
            }

            // If the text is displayed, hiding it
            /*if (m_hologramClockView.gameObject.activeSelf)
            {
                temp = m_hologramClockView.gameObject;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock is going to be hidden");
            }
            else if (m_hologramWindowReminderView.gameObject.activeSelf)
            { // Only two children for know so fine this way. But to be extended if it happens that more children are added in the future
                temp = m_hologramWindowReminderView.gameObject;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Text is going to be hidden");
            }*/
            
            /*if(temp == null)
            { // Means all components are already hidden, so nothing to do
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "All objects are already hidden, so nothing do do excepting unlocking the mutex");
                m_mutexHiding = false;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex unlocked");
            }
            else*/
            if (m_hologramWindowReminderView.gameObject.activeSelf)
            { // Means text needs to be hidden
                EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object oe, EventArgs ee)
            {
                //m_mutexHiding = false;
                m_mutexHide.unlockMutex();
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");

                Destroy(temp.GetComponent<MouseUtilitiesAnimation>());
                resetObject();
            }), e };

                MouseUtilitiesAnimation animator = temp.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(m_debug, eventHandlers);
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "All objects are already hidden, so nothing do do excepting unlocking the mutex");
                //m_mutexHiding = false;
                m_mutexHide.unlockMutex();
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex unlocked");
            }
            

            /*if (m_hologramClockView == null || m_hologramWindowReminderView == null)
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

                //gameObject.SetActive(false);

                resetObject();

                m_mutexHiding = false;
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - object hidden");


                Destroy(temp.GetComponent<MouseUtilitiesAnimation>());

            }), e };

                MouseUtilitiesAnimation animator = temp.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(m_debug, eventHandlers);
            }*/
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - Request ignored");
        }

           
    }

    bool m_mutexShow = false;
    public void show(EventHandler eventHandler)
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            int i = 0;
            foreach (Transform clock in m_clocksView)
            {
                if (clock.gameObject.activeSelf)
                {
                    /*MouseUtilitiesAnimation animator = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {
                        clock.gameObject.SetActive(false);
                        Destroy(clock.GetComponent<MouseUtilitiesAnimation>());

                        // Unlocking the mutex at the last element of the list
                        if (clock == m_clocksView.Last())
                        {
                            m_mutexHiding = false;
                            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");
                        }
                    }));*/

                    MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, clock, m_positionsLocalOrigin[i].y);

                    EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
                    {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");

                    Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
    
                    // Unlocking the mutex at the last element of the list
                        if (clock == m_clocksView.Last())
                        {
                            m_mutexShow = false;
                            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");
                        }

                    }), eventHandler };

                    MouseUtilitiesAnimation anim = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    anim.animateAppearInPlaceToScaling(clock.gameObject.transform.localScale, m_debug, eventHandlers);
                }
                i++;
            }

            /*m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");

            MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, transform, m_positionLocalOrigin.y);     

            EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

                m_mutexShow = false;

            }), eventHandler };

            //TODO MouseUtilitiesAnimation animator = m_hologramClockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            //TODO animator.animateAppearInPlaceToScaling(m_hologramClockView.gameObject.transform.localScale, m_debug, eventHandlers);*/
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex already locked - nothing to do");
        }

        /*if (gameObject.activeSelf)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Object already active - no animation will be started");
        }
        else
        {
            //gameObject.transform.position = new Vector3(gameObject.transform.position.x, Camera.main.transform.position.y + gameObject.transform.localPosition.y, gameObject.transform.position.z);
            MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, transform, m_positionLocalOrigin.y);

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Reminder assistance is going to be displayed");

            EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Assistance reminder should be visible now");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
            }), eventHandler };

            

            MouseUtilitiesAnimation animator = gameObject.AddComponent<MouseUtilitiesAnimation>();
            animator.animateAppearInPlaceToScaling(gameObject.transform.localScale, m_debug, eventHandlers);
        }*/
        
    }

    void callbackGradationDefault(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;

        // Only one clock is going to be shown to follow the user. We take the first one.
        Transform clockFollower = m_clocksView.First();

        show(MouseUtilities.getEventHandlerEmpty());

        clockFollower.transform.localScale = m_clockScalingOriginal;
        //clockFollower.localPosition = new Vector3(clockFollower.localPosition.x, clockFollower.localPosition.y, clockFollower.localPosition.z - 0.25f);// Removing the offset
    }

    void callbackGradationHighFollow(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = false;
        GetComponent<RadialView>().enabled = true;

        // Only one clock is going to be shown to follow the user. We take the first one.
        Transform clockFollower = m_clocksView.First();

        foreach (Transform clock in m_clocksView)
        {
            if (clock != clockFollower)
            { // We want to hide only the clocks that have not been touched by the sender
                MouseUtilitiesAnimation animator = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object oo, EventArgs ee)
                {
                    clock.gameObject.SetActive(false);

                    // When the clock has diseappeared, make the text appearing, after moving it to the place of the hidden clock
                    m_hologramWindowReminderView.position = clock.position;
                    MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animatorText.animateAppearInPlace(m_debug, MouseUtilities.getEventHandlerEmpty());

                    Destroy(clock.GetComponent<MouseUtilitiesAnimation>());
                }));
            }
        }

        clockFollower.transform.localScale = m_clockScalingReduced;
        //m_hologramClockView.localPosition = new Vector3(m_hologramClockView.localPosition.x, m_hologramClockView.localPosition.y, m_hologramClockView.localPosition.z + 0.25f); // Adding an offset to avoid that the object remains in front of the user
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
        //m_hologramClockView.gameObject.SetActive(true);
        //m_hologramWindowReminderView.gameObject.SetActive(false);

        //TODO m_hologramClockView.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        m_hologramWindowReminderView.gameObject.transform.localScale = new Vector3(1, 1, 1);
        //TODO m_hologramClockView.gameObject.SetActive(false);
        m_hologramWindowReminderView.gameObject.SetActive(false);

        m_gradationManager.setGradationToMinimum();
    }

    public void setObjectsToOriginalPosition()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Changing position");

        for (int i = 0; i < m_clocksView.Count; i++)
        {
            m_clocksView[i].localPosition = m_positionsLocalOrigin[i];
        }
    }
}
