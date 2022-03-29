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

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;

/**
 * Simple assistance to congratulate the user when he successfully finished a challenge
 * */
public class MouseAssistanceChallengeSuccess : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public EventHandler m_eventHologramTouched;

    // Start is called before the first frame update
    void Start()
    {
        // Callbacks
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, transform, delegate () { m_eventHologramTouched?.Invoke(this, EventArgs.Empty); });
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void show(EventHandler eventHandler)
    {
        if (gameObject.activeSelf == false)
        {
            MouseUtilitiesAnimation animator = gameObject.AddComponent<MouseUtilitiesAnimation>();

            EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
                    {
                        Destroy(animator);
                    }), eventHandler };

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Target scaling: " + transform.localScale.ToString());

            animator.animateAppearInPlaceToScaling(transform.localScale, m_debug, eventHandlers);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Success hologram is enabled - no hide action to take");
        }
    }

    public void hide(EventHandler eventHandler)
    {
        if (gameObject.activeSelf)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube is going to be hidden");


            EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
               {
                   m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube should be hidden now");

                   gameObject.SetActive(false);
                   transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                   Destroy(GetComponent<MouseUtilitiesAnimation>());
               }), eventHandler };

            gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, eventHandlers);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Success hologram is disabled - no hide action to take");
        }
    }
}