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
public class MouseChallengeCleanTableReminderOneClockMoving : MouseAssistanceAbstract
{


    Transform m_clockView; // Because we can have multiple clocks

    Transform m_hologramWindowReminderView;
    MouseAssistanceDialog m_dialogController;

    public event EventHandler m_eventHologramClockTouched;
    public event EventHandler m_eventHologramWindowButtonOkTouched;
    public event EventHandler m_eventHologramWindowButtonBackTouched;

    Vector3 m_positionLocalOrigin;
    float m_yOffsetOrigin;

    // As the clock does not have an attached script, storing the required information here
    Vector3 m_clockScalingOriginal;
    Vector3 m_clockScalingReduced;
    Vector3 m_clockOriginalPosition;

    List<Transform> m_objectsToBeClose;

    bool m_newObjectToFocus;
    Transform m_newObjectToFocusTransform;

    private void Awake()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Initialize variables
        m_objectsToBeClose = new List<Transform>();

        m_positionLocalOrigin = transform.localPosition;
        m_yOffsetOrigin = transform.localPosition.y;

        m_newObjectToFocus = false;

        m_newObjectToFocusTransform = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Let's get the children
        m_clockView = gameObject.transform.Find("Clock");
        m_hologramWindowReminderView = gameObject.transform.Find("MouseAssistanceDialog"); //gameObject.transform.Find("Text");
        m_dialogController = m_hologramWindowReminderView.GetComponent<MouseAssistanceDialog>();
        m_dialogController.setDescription("Très bien! J'apparaitrai de nouveau demain à la même heure. Est-ce que cela vous convient?", 0.15f);
        m_dialogController.addButton("Parfait!");
        m_dialogController.m_buttonsController[0].s_buttonClicked += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
        });
        m_dialogController.addButton("Je me suis trompé de bouton! Revenir en arrière...");
        m_dialogController.m_buttonsController[1].s_buttonClicked += delegate
        {
            m_eventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
        };


        // Getting informations related to the clock
        m_clockScalingOriginal = new Vector3(0.1f, 0.1f, 0.1f); // harcoding the scaling like this might create some issues, but getting the scaling directly from the gameobject does not work with the Hololens (although it works here in Unity) 
        m_clockScalingReduced = m_clockScalingOriginal / 3.0f;
        m_clockOriginalPosition = m_clockView.localPosition;

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

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock touched callback called");

        MouseUtilities.animateDisappearInPlace(m_clockView.gameObject, new Vector3(0.1f, 0.1f, 0.1f), delegate
        {
            m_hologramWindowReminderView.position = new Vector3(m_hologramWindowReminderView.position.x, m_clockView.position.y, m_hologramWindowReminderView.position.z);

            m_dialogController.show(MouseUtilities.getEventHandlerEmpty());
            m_eventHologramClockTouched?.Invoke(this, EventArgs.Empty);
        });       
    }

    void callbackOnObjectFocus(System.Object sender, EventArgs e)
    {
        if (m_clockView.gameObject.activeSelf)
        { // I.e. not active if the dialog is displayed
            if (m_newObjectToFocusTransform == null || (m_newObjectToFocusTransform.name != ((GameObject)sender).name))
            { // To avoid doing unnecessary processes if the situation did not change
                string currentObjectName = "not initialized yet";

                if (m_newObjectToFocusTransform != null)
                {
                    currentObjectName = ((UnityEngine.GameObject)sender).name;
                }

                m_newObjectToFocusTransform = ((GameObject)sender).transform;
                m_newObjectToFocus = true;
            }
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

    bool m_mutexHide = false;
    public override void hide(EventHandler e)
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (m_mutexHide == false)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");
            m_mutexHide = true; 

            //GameObject temp;

            if (m_clockView.gameObject.activeSelf)
            {
                MouseUtilities.animateDisappearInPlace(m_clockView.gameObject, new Vector3(0.1f, 0.1f, 0.1f), delegate
                {
                    MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - clock hidden");
                    m_mutexHide = false;
                });
            }
            else
            { // Only two children for know so fine this way. But to be extended if it happens that more children are added in the future
                m_dialogController.hide(delegate
                {
                    MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - dialog hidden");

                    m_mutexHide = false; //.unlockMutex();
                    e?.Invoke(this, EventArgs.Empty);
                });
            }

            
        }
    }

    bool m_mutexShow = false;
    bool m_showFirstTime = false;
    public override void show(EventHandler eventHandler)
    {
        m_clockScalingOriginal = new Vector3(0.1f,0.1f, 0.1f);
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clock is going to appear to scaling: " + m_clockScalingOriginal);

        if (m_mutexShow == false)
        {
            m_mutexShow = true; 

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");

            MouseUtilities.adjustObjectHeightToHeadHeight(transform, m_yOffsetOrigin);

            MouseUtilitiesAnimation animator = m_clockView.gameObject.AddComponent <MouseUtilitiesAnimation>(); 
            
            animator.animateAppearInPlaceToScaling(m_clockScalingOriginal, new EventHandler(delegate (System.Object oo, EventArgs ee)
                {
                    eventHandler?.Invoke(this, EventArgs.Empty);

                    if (m_showFirstTime == false)
                    {
                        MouseUtilitiesHologramInteractions temp = m_clockView.GetComponent<MouseUtilitiesHologramInteractions>();
                        temp.s_touched += callbackOnClockTouched;

                        m_showFirstTime = true;
                    }

                    Destroy(m_clockView.gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexShow = false; 
                }
                ));
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex already locked - nothing to do");
        }
    }

    // Be aware that this function does not send the object back to its original position
    void resetObject()
    {
        m_hologramWindowReminderView.gameObject.transform.localScale = new Vector3(1, 1, 1);
        m_hologramWindowReminderView.gameObject.SetActive(false);
    }

    public void setObjectToOriginalPosition()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Changing position");

        transform.localPosition = m_positionLocalOrigin;
    }
}
