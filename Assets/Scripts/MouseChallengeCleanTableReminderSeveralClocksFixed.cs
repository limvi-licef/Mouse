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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Reflection;
using System.Linq;

/**
 * EXPERIMENTAL (i.e. read between the line: does not work)
 * Reminder where several fix clocks can be displayed
 * */
public class MouseChallengeCleanTableReminderSeveralClocksFixed : MonoBehaviour
{
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

        m_mutexHide = new MouseUtilitiesMutex();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        m_clocksView = new List<Transform>();
        m_positionsLocalOrigin = new List<Vector3>();

        // Connect callbacks
        m_hologramWindowReminderButtonOkView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowOkButtonTouched);
        m_hologramWindowReminderButtonBackView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowBackButtonTouched);

        m_gradationManager = transform.GetComponent<MouseUtilitiesGradationManager>();
        if (m_gradationManager == null)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "MouseUtilitiesGradationManager component not present in that gameobject, whereas it is required. The object will most likely not work properly");
        }
        m_gradationManager.addNewAssistanceGradation("Default", callbackGradationDefault);
        m_gradationManager.addNewAssistanceGradation("HighFollow", callbackGradationHighFollow);

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
                animator.animateDiseappearInPlace(new EventHandler(delegate (System.Object o, EventArgs ee)
                {
                    clock.gameObject.SetActive(false);

                    // When the clock has diseappeared, make the text appearing, after moving it to the place of the hidden clock
                    m_hologramWindowReminderView.position = clock.position;
                    MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animatorText.animateAppearInPlace(MouseUtilities.getEventHandlerEmpty());

                    Destroy(clock.GetComponent<MouseUtilitiesAnimation>());
                }));
            }
        }

        m_eventHologramClockTouched?.Invoke(this, EventArgs.Empty); // Informing the world that a clock has been touched

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void callbackOnWindowOkButtonTouched()
    {
        m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
    }

    void callbackOnWindowBackButtonTouched()
    {
        m_eventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
    }

    public void hide(EventHandler e)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexHide.isLocked() == false)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");
            m_mutexHide.lockMutex();

            GameObject temp = null;

            // For each clock displayed, hiding it
            foreach (Transform clock in m_clocksView)
            {
                if ( clock.gameObject.activeSelf)
                {
                    MouseUtilitiesAnimation animator = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animator.animateDiseappearInPlace(new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {
                        clock.gameObject.SetActive(false);
                        Destroy(clock.GetComponent<MouseUtilitiesAnimation>());

                        // Unlocking the mutex at the last element of the list
                        if (clock == m_clocksView.Last())
                        {
                            //m_mutexHiding = false;
                            m_mutexHide.unlockMutex();
                            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");
                        }
                    }));
                }
            }

            // If the text is displayed, hiding it
            if (m_hologramWindowReminderView.gameObject.activeSelf)
            { // Means text needs to be hidden
                EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object oe, EventArgs ee)
            {
                //m_mutexHiding = false;
                m_mutexHide.unlockMutex();
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");

                Destroy(temp.GetComponent<MouseUtilitiesAnimation>());
                resetObject();
            }), e };

                MouseUtilitiesAnimation animator = temp.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(eventHandlers);
            }
            else
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "All objects are already hidden, so nothing do do excepting unlocking the mutex");
                m_mutexHide.unlockMutex();
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex unlocked");
            }
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - Request ignored");
        }
    }

    bool m_mutexShow = false;
    public void show(EventHandler eventHandler)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            int i = 0;
            foreach (Transform clock in m_clocksView)
            {
                if (clock.gameObject.activeSelf)
                {
                    MouseUtilities.adjustObjectHeightToHeadHeight(clock, m_positionsLocalOrigin[i].y);

                    EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
                    {
                    MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");

                    Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
    
                    // Unlocking the mutex at the last element of the list
                        if (clock == m_clocksView.Last())
                        {
                            m_mutexShow = false;
                            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");
                        }

                    }), eventHandler };

                    MouseUtilitiesAnimation anim = clock.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    anim.animateAppearInPlaceToScaling(clock.gameObject.transform.localScale, eventHandlers);
                }
                i++;
            }
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex already locked - nothing to do");
        }        
    }

    void callbackGradationDefault(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;

        // Only one clock is going to be shown to follow the user. We take the first one.
        Transform clockFollower = m_clocksView.First();

        show(MouseUtilities.getEventHandlerEmpty());

        clockFollower.transform.localScale = m_clockScalingOriginal;
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
                animator.animateDiseappearInPlace(new EventHandler(delegate (System.Object oo, EventArgs ee)
                {
                    clock.gameObject.SetActive(false);

                    // When the clock has diseappeared, make the text appearing, after moving it to the place of the hidden clock
                    m_hologramWindowReminderView.position = clock.position;
                    MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
                    animatorText.animateAppearInPlace(MouseUtilities.getEventHandlerEmpty());

                    Destroy(clock.GetComponent<MouseUtilitiesAnimation>());
                }));
            }
        }

        clockFollower.transform.localScale = m_clockScalingReduced;
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
        m_hologramWindowReminderView.gameObject.transform.localScale = new Vector3(1, 1, 1);
        m_hologramWindowReminderView.gameObject.SetActive(false);

        m_gradationManager.setGradationToMinimum();
    }

    public void setObjectsToOriginalPosition()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Changing position");

        for (int i = 0; i < m_clocksView.Count; i++)
        {
            m_clocksView[i].localPosition = m_positionsLocalOrigin[i];
        }
    }
}
