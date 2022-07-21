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

namespace MATCH
{
    namespace Assistances
    {
        /**
 * Used as an "escape" where the user can ask to be reminded later about the challenge
 * */
        public class ReminderOneClockMoving : MonoBehaviour, IAssistance
        {


            Transform m_clockView; // Because we can have multiple clocks

            Transform m_hologramWindowReminderView;
            Dialog m_dialogController;

            public event EventHandler EventHologramClockTouched;
            public event EventHandler EventHologramWindowButtonOkTouched;
            public event EventHandler EventHologramWindowButtonBackTouched;

            Vector3 m_positionLocalOrigin;
            float m_yOffsetOrigin;

            // As the clock does not have an attached script, storing the required information here
            Vector3 m_clockScalingOriginal;
            //Vector3 m_clockScalingReduced;
            //Vector3 m_clockOriginalPosition;

            List<Transform> m_objectsToBeClose;

            bool m_newObjectToFocus;
            Transform m_newObjectToFocusTransform;

            private void Awake()
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");

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
                m_dialogController = m_hologramWindowReminderView.GetComponent<Dialog>();
                m_dialogController.setDescription("Tr�s bien! J'apparaitrai de nouveau demain � la m�me heure. Est-ce que cela vous convient?", 0.15f);
                m_dialogController.addButton("Parfait!", true);
                m_dialogController.m_buttonsController[0].s_buttonClicked += new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    EventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
                });
                m_dialogController.addButton("Je me suis tromp� de bouton! Revenir en arri�re...", true);
                m_dialogController.m_buttonsController[1].s_buttonClicked += delegate
                {
                    EventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
                };


                // Getting informations related to the clock
                m_clockScalingOriginal = new Vector3(0.1f, 0.1f, 0.1f); // harcoding the scaling like this might create some issues, but getting the scaling directly from the gameobject does not work with the Hololens (although it works here in Unity) 
                //m_clockScalingReduced = m_clockScalingOriginal / 3.0f;
                //m_clockOriginalPosition = m_clockView.localPosition;

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

            public void AddObjectToBeClose(Transform o) // The parent the clock will belong to
            {
                m_objectsToBeClose.Add(o);

                Utilities.HologramInteractions interactions = o.gameObject.GetComponent<Utilities.HologramInteractions>();

                if (interactions == null)
                {
                    interactions = o.gameObject.AddComponent<Utilities.HologramInteractions>();
                }

                interactions.EventFocusOn += CallbackOnObjectFocus;
            }

            public Transform GetTransform()
            {
                return transform;
            }

            void CallbackOnClockTouched(System.Object sender, EventArgs e)
            {// If a clock is touched, all other clocks are hidden

                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Clock touched callback called");

                MATCH.Utilities.Utility.AnimateDisappearInPlace(m_clockView.gameObject, new Vector3(0.1f, 0.1f, 0.1f), delegate
                {
                    m_hologramWindowReminderView.position = new Vector3(m_hologramWindowReminderView.position.x, m_clockView.position.y, m_hologramWindowReminderView.position.z);

                    m_dialogController.Show(MATCH.Utilities.Utility.GetEventHandlerEmpty());
                    EventHologramClockTouched?.Invoke(this, EventArgs.Empty);
                });
            }

            void CallbackOnObjectFocus(System.Object sender, EventArgs e)
            {
                if (m_clockView.gameObject.activeSelf)
                { // I.e. not active if the dialog is displayed
                    if (m_newObjectToFocusTransform == null || (m_newObjectToFocusTransform.name != ((GameObject)sender).name))
                    { // To avoid doing unnecessary processes if the situation did not change
                        /*string currentObjectName = "not initialized yet";

                        if (m_newObjectToFocusTransform != null)
                        {
                            currentObjectName = ((UnityEngine.GameObject)sender).name;
                        }*/

                        m_newObjectToFocusTransform = ((GameObject)sender).transform;
                        m_newObjectToFocus = true;
                    }
                }
            }

            /*void CallbackOnWindowOkButtonTouched()
            {
                EventHologramWindowButtonOkTouched?.Invoke(this, EventArgs.Empty);
            }*/

            /*void CallbackOnWindowBackButtonTouched()
            {
                EventHologramWindowButtonBackTouched?.Invoke(this, EventArgs.Empty);
            }*/

            bool m_mutexHide = false;
            public void Hide(EventHandler e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");

                if (m_mutexHide == false)
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Mutex locked");
                    m_mutexHide = true;

                    //GameObject temp;

                    if (m_clockView.gameObject.activeSelf)
                    {
                        MATCH.Utilities.Utility.AnimateDisappearInPlace(m_clockView.gameObject, new Vector3(0.1f, 0.1f, 0.1f), delegate
                        {
                            DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Mutex unlocked - clock hidden");
                            m_mutexHide = false;
                        });
                    }
                    else
                    { // Only two children for know so fine this way. But to be extended if it happens that more children are added in the future
                        m_dialogController.Hide(delegate
                        {
                            DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Mutex unlocked - dialog hidden");

                            m_mutexHide = false; //.unlockMutex();
                            e?.Invoke(this, EventArgs.Empty);
                        });
                    }


                }
            }

            bool m_mutexShow = false;
            bool m_showFirstTime = false;
            public void Show(EventHandler eventHandler)
            {
                m_clockScalingOriginal = new Vector3(0.1f, 0.1f, 0.1f);
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Clock is going to appear to scaling: " + m_clockScalingOriginal);

                if (m_mutexShow == false)
                {
                    m_mutexShow = true;

                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Mutex locked");

                    MATCH.Utilities.Utility.AdjustObjectHeightToHeadHeight(transform, m_yOffsetOrigin);

                    MATCH.Utilities.Animation animator = m_clockView.gameObject.AddComponent<MATCH.Utilities.Animation>();

                    animator.animateAppearInPlaceToScaling(m_clockScalingOriginal, new EventHandler(delegate (System.Object oo, EventArgs ee)
                    {
                        eventHandler?.Invoke(this, EventArgs.Empty);

                        if (m_showFirstTime == false)
                        {
                            Utilities.HologramInteractions temp = m_clockView.GetComponent<Utilities.HologramInteractions>();
                            temp.EventTouched += CallbackOnClockTouched;

                            m_showFirstTime = true;
                        }

                        Destroy(m_clockView.gameObject.GetComponent<MATCH.Utilities.Animation>());
                        m_mutexShow = false;
                    }
                        ));
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Mutex already locked - nothing to do");
                }
            }

            public void ShowHelp(bool show)
            {
                // Todo
            }

            // Be aware that this function does not send the object back to its original position
            /*void resetObject()
            {
                m_hologramWindowReminderView.gameObject.transform.localScale = new Vector3(1, 1, 1);
                m_hologramWindowReminderView.gameObject.SetActive(false);
            }*/

            public void SetObjectToOriginalPosition()
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Changing position");

                transform.localPosition = m_positionLocalOrigin;
            }
        }

    }
}


