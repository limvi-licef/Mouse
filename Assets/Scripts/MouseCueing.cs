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
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;

/**
 * Assistance to show a message with a "I don't understand" button
 * */
public class MouseCueing : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public event EventHandler m_eventHelpButtonClicked;

    public Transform m_text;
    Transform m_button;
    Transform m_hologramButtonClicked;

    private void Awake()
    {
        // Children
        m_text = gameObject.transform.Find("Text");
        m_button = gameObject.transform.Find("WindowMenu");
        m_hologramButtonClicked = m_button.Find("ButtonHelp");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Callback
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_hologramButtonClicked, callbackButtonHelpClicked);
    }

    void callbackButtonHelpClicked()
    {
        m_eventHelpButtonClicked?.Invoke(this, EventArgs.Empty);

        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Clicked");
    }

    bool m_mutexShow = false;
    public void show (EventHandler eventHandler)
    {
        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, transform);

            m_text.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e) {
                EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object oo, EventArgs ee) {
                    Destroy(m_button.gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexShow = false;
            }), eventHandler };

                m_button.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(m_debug, temp);

                Destroy(m_text.gameObject.GetComponent<MouseUtilitiesAnimation>());
            }));
        }
    }

    // With animation, compatible with the gradation manager
    bool m_mutexHide = false;
    public void hide (EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true;

            m_text.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e) {
                EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object oo, EventArgs ee) {
                    Destroy(m_button.gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexHide = false;
            }), eventHandler };

                m_button.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);

                Destroy(m_text.gameObject.GetComponent<MouseUtilitiesAnimation>());
            }));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
