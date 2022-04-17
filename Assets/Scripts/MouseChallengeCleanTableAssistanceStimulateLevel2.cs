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
using System;
using System.Reflection;

/**
 * Assistance showing an arch between two objects, with a text at the beginning, and a help cube at the end.
 * */
public class MouseChallengeCleanTableAssistanceStimulateLevel2 : MonoBehaviour
{
    public event EventHandler m_eventHologramHelpTouched;

    public Transform m_hologramHelp;
    Transform m_hologramLineView;
    public Transform m_textView;
    MouseAssistanceDialog m_textController;

    MouseLineToObject m_hologramLineController;

    Vector3 m_textOriginalScaling;

    void Awake()
    {
        // Children
        m_hologramLineView = gameObject.transform.Find("Line");
        m_hologramLineController = m_hologramLineView.GetComponent<MouseLineToObject>();
        m_hologramHelp = MouseUtilities.mouseUtilitiesFindChild(gameObject, "Help");
        m_textView = gameObject.transform.Find("TextNew");
        m_textController = m_textView.GetComponent<MouseAssistanceDialog>();

        m_textOriginalScaling = m_textView.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        if ( m_hologramLineView == null )
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "Line hologram not initialized properly");
        }

        // Callbacks
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_hologramHelp, callbackHelpTouched);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackHelpTouched()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_eventHologramHelpTouched?.Invoke(this, EventArgs.Empty);
    }

    void resetObject()
    {
        m_hologramHelp.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Destroy(m_hologramHelp.GetComponent<MouseUtilitiesAnimation>());
    }

    bool m_mutexShow = false;
    public void show(EventHandler eventHandler)
    {
        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            m_textView.position = m_hologramLineController.m_hologramOrigin.transform.position;
            MouseUtilities.adjustObjectHeightToHeadHeight(m_textView);
            // Trick to start the line to the text position, i.e. to start at user's head's position
            m_hologramLineController.m_hologramOrigin.transform.position = m_textView.position;
            
            m_textController.show(delegate
            {
                m_textController.enableBillboard(true);
                m_mutexShow = false;
            });

            // Showing line
                m_hologramLineView.GetComponent<MouseLineToObject>().show(delegate  {
                    EventHandler[] temp = new EventHandler[] { delegate 
                    {
                        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                        Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());

                        m_mutexShow = false;
                    }, eventHandler };

                    MouseUtilities.adjustObjectHeightToHeadHeight(m_hologramHelp);

                    m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(new Vector3(0.1f, 0.1f, 0.1f), temp);
                });
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - request ignored");
        }
    }

    bool m_mutexHide = false;
    public void hide(EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true;

            m_textController.hide(eventHandler);
            MouseUtilities.animateDisappearInPlace(m_hologramHelp.gameObject, new Vector3(0.1f, 0.1f, 0.1f), delegate {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked - all elements from the arch should be hidden");
                m_mutexHide = false;
            });

            // Hiding text
            /*m_textView.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(new EventHandler(delegate (System.Object o, EventArgs e)
            {
                //m_textView.gameObject.SetActive(false);

                Destroy(m_textView.gameObject.GetComponent<MouseUtilitiesAnimation>());

                EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object oo, EventArgs ee)
            {
                m_hologramHelp.gameObject.SetActive(false);

                m_hologramHelp.localScale = new Vector3(0.1f,0.1f,0.1f);

			    Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());

                m_mutexHide = false;
            }), eventHandler };

                m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(temp);
            }));*/

            // Hiding line
            if (m_hologramLineView.gameObject.activeSelf)
            {
                m_hologramLineView.GetComponent<MouseLineToObject>().hide(MouseUtilities.getEventHandlerEmpty()); // The eventhandler being already called above, we do not want it to be called twice, as this could create strange behaviors.
            }
            else
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Line already hidden: nothing to do");
            }
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - request ignored");
        }
    }

    public void setArchStartAndEndPoint(Transform origin, Transform target)
    {
        m_hologramLineController.m_hologramOrigin = origin.gameObject;
        m_hologramLineController.m_hologramTarget = target.gameObject;
    }
}
