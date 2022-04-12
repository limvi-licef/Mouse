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
 * Used as an "escape" where the user can ask to be reminded later about the challenge
 * */
public class MouseChallengeCleanTableReminderOneClockMoving : MonoBehaviour
{
    Transform m_clockView; // Because we can have multiple clocks

    //Transform m_hologramWindowReminderButtonOkView;
    Transform m_hologramWindowReminderView;
    //Transform m_hologramWindowReminderButtonBackView;
    MouseAssistanceDialog m_dialogController;

    public event EventHandler m_eventHologramClockTouched;
    public event EventHandler m_eventHologramWindowButtonOkTouched;
    public event EventHandler m_eventHologramWindowButtonBackTouched;

    MouseUtilitiesGradationManager m_gradationManager;

    Vector3 m_positionLocalOrigin;
    float m_yOffsetOrigin;

    // As the clock does not have an attached script, storing the required information here
    Vector3 m_clockScalingOriginal;
    Vector3 m_clockScalingReduced;
    Vector3 m_clockOriginalPosition;

    //bool m_mutexHiding = false;
    MouseUtilitiesMutex m_mutexHide;
    MouseUtilitiesMutex m_mutexShow;

    List<Transform> m_objectsToBeClose;

    bool m_newObjectToFocus;
    Transform m_newObjectToFocusTransform;

    private void Awake()
    {
        // Initialize variables
        m_objectsToBeClose = new List<Transform>();

        // Let's get the children
        m_clockView = gameObject.transform.Find("Clock");
        m_hologramWindowReminderView = gameObject.transform.Find("MouseAssistanceDialog"); //gameObject.transform.Find("Text");
        m_dialogController = m_hologramWindowReminderView.GetComponent<MouseAssistanceDialog>();
        m_dialogController.setDescription("Tr�s bien! J'apparaitrai de nouveau demain � la m�me heure. Est-ce que cela vous convient?", 0.15f);
        m_dialogController.addButton("Parfait!");
        m_dialogController.m_buttonsController[0].s_buttonClicked += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
        }); 
        m_dialogController.addButton("Je me suis tromp� de bouton! Revenir en arri�re...");
        m_dialogController.m_buttonsController[1].s_buttonClicked += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
        }); 

        //m_hologramWindowReminderButtonOkView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonOk");
        //m_hologramWindowReminderButtonBackView = m_hologramWindowReminderView.Find("WindowMenu").Find("ButtonBack");

        m_mutexHide = new MouseUtilitiesMutex();
        m_mutexShow = new MouseUtilitiesMutex();

        m_clockOriginalPosition = m_clockView.localPosition;
        m_positionLocalOrigin = transform.localPosition;
        m_yOffsetOrigin = transform.localPosition.y;

        m_newObjectToFocus = false;

        m_newObjectToFocusTransform = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Connect callbacks
        MouseUtilitiesHologramInteractions temp = m_clockView.GetComponent<MouseUtilitiesHologramInteractions>();
        temp.s_touched += callbackOnClockTouched;
        //m_hologramWindowReminderButtonOkView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowOkButtonTouched);
        //m_hologramWindowReminderButtonBackView.gameObject.GetComponent<Interactable>().AddReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackOnWindowBackButtonTouched);

        m_gradationManager = transform.GetComponent<MouseUtilitiesGradationManager>();
        if (m_gradationManager == null)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "MouseUtilitiesGradationManager component not present in that gameobject, whereas it is required. The object will most likely not work properly");
        }
        m_gradationManager.addNewAssistanceGradation("Default", callbackGradationDefault);
        m_gradationManager.addNewAssistanceGradation("HighFollow", callbackGradationHighFollow);

        // Getting informations related to the clock
        m_clockScalingOriginal = m_clockView.localScale;
        m_clockScalingReduced = m_clockScalingOriginal / 3.0f;        
    }

    // Update is called once per frame
    void Update()
    {
       if (m_newObjectToFocus)
        {
            m_newObjectToFocus = false; // Managed so disable to avoid this to be called each time

            gameObject.transform.position = m_newObjectToFocusTransform.position;
            m_positionLocalOrigin = gameObject.transform.localPosition;
        }
    }

    public void addObjectToBeClose(Transform o) // The parent the clock will belong to
    {
        m_objectsToBeClose.Add(o);

        MouseUtilitiesHologramInteractions interactions = o.gameObject.GetComponent<MouseUtilitiesHologramInteractions>();

        if (interactions == null)
        {
            interactions = o.gameObject.AddComponent<MouseUtilitiesHologramInteractions>();
        }

        interactions.s_focusOn += callbackOnObjectFocus;
    }


    void callbackOnClockTouched(System.Object sender, EventArgs e)
    {// If a clock is touched, all other clocks are hidden

        MouseUtilitiesAnimation animator = m_clockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animator.animateDiseappearInPlace(new EventHandler(delegate (System.Object o, EventArgs ee)
        {
            m_clockView.gameObject.SetActive(false);

            // When the clock has diseappeared, make the text appearing, after moving it to the place of the hidden clock
            //m_hologramWindowReminderView.position = m_clockView.position;
            MouseUtilities.adjustObjectHeightToHeadHeight(m_hologramWindowReminderView);

            m_dialogController.show(MouseUtilities.getEventHandlerEmpty());
            //MouseUtilitiesAnimation animatorText = m_hologramWindowReminderView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            //animatorText.animateAppearInPlace(MouseUtilities.getEventHandlerEmpty());

            Destroy(m_clockView.GetComponent<MouseUtilitiesAnimation>());
        }));

        m_eventHologramClockTouched?.Invoke(this, EventArgs.Empty); // Informing the world that a clock has been touched

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void callbackOnObjectFocus(System.Object sender, EventArgs e)
    {
        if (m_gradationManager.isGradationMax() == false)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called - Sender type: " + sender.GetType());


            if (m_newObjectToFocusTransform == null || (m_newObjectToFocusTransform.name != ((GameObject)sender).name))
            { // To avoid doing unnecessary processes if the situation did not change
                string currentObjectName = "not initialized yet";

                if (m_newObjectToFocusTransform != null)
                {
                    currentObjectName = ((UnityEngine.GameObject)sender).name;
                }

                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Seems to work - Currently focused object: " + currentObjectName + "| New one: " + ((GameObject)sender).name + ". If the 2 names are different, this is good news");
                m_newObjectToFocusTransform = ((GameObject)sender).transform;
                m_newObjectToFocus = true;
            }
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Maximum gradation reached - no focus process are performed");
        }
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

            //GameObject temp;

            if (m_clockView.gameObject.activeSelf)
            {
                //temp = m_clockView.gameObject;
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock is going to be hidden");
                EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object oe, EventArgs ee)
            {
                m_mutexHide.unlockMutex();
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - object hidden");

                Destroy(m_clockView.gameObject.GetComponent<MouseUtilitiesAnimation>());
                resetObject();
            }), e };

                MouseUtilitiesAnimation animator = m_clockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
                animator.animateDiseappearInPlace(eventHandlers);
            }
            else
            { // Only two children for know so fine this way. But to be extended if it happens that more children are added in the future
                //temp = m_hologramWindowReminderView.gameObject;
                //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Text is going to be hidden");
                m_dialogController.hide(new EventHandler(delegate(System.Object o, EventArgs ee)
                {
                    m_mutexHide.unlockMutex();
                    e?.Invoke(this, EventArgs.Empty);
                }));
            }

            
        }
    }

    public void show(EventHandler eventHandler)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexShow.isLocked() == false)
        {
            m_mutexShow.lockMutex();

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");

            MouseUtilities.adjustObjectHeightToHeadHeight(transform, m_yOffsetOrigin);

            EventHandler[] eventHandlers = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e)
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock should be visible now");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

                m_mutexShow.unlockMutex();

            }), eventHandler };

            MouseUtilitiesAnimation animator = m_clockView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            animator.animateAppearInPlaceToScaling(m_clockScalingOriginal, eventHandlers);
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

        show(MouseUtilities.getEventHandlerEmpty());

        m_clockView.transform.localScale = m_clockScalingOriginal;
    }

    void callbackGradationHighFollow(System.Object o, EventArgs e)
    {
        GetComponent<Billboard>().enabled = false;
        GetComponent<RadialView>().enabled = true;

        m_clockView.transform.localScale = m_clockScalingReduced;
        m_clockView.localPosition = new Vector3(-0.1f, 0.2f, 0);
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

    public void setObjectToOriginalPosition()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Changing position");

        transform.localPosition = m_positionLocalOrigin;
    }
}
