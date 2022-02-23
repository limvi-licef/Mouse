using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;

public class MouseAssistanceChallengeSuccess : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public EventHandler m_eventHologramTouched;

    //public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    //public AudioListener m_audioListener;

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