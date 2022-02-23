using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;

public class MouseUtilitiesRefuseChallenge : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    int m_layerBitMask = 1 << 9; // I.e. only the object in the "Mouse" layer will be considered by the raycast.

    public event EventHandler m_eventChallengeRefused;

    bool m_palmFacingUser;
    bool m_statusEventTriggered;

    // Start is called before the first frame update
    void Start()
    {
        m_palmFacingUser = false;
        m_statusEventTriggered = false;

        gameObject.GetComponent<HandConstraintPalmUp>().OnFirstHandDetected.AddListener(delegate ()
        {
            m_palmFacingUser = true;

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Palm detected");
        });

        gameObject.GetComponent<HandConstraintPalmUp>().OnLastHandLost.AddListener(delegate ()
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Palm not detected anymore");

            m_palmFacingUser = false;
        });
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                20.0f,
                /*Physics.DefaultRaycastLayers*/ m_layerBitMask))
        {
            // If the Raycast has succeeded and hit a hologram
            // hitInfo's point represents the position being gazed at
            // hitInfo's collider GameObject represents the hologram being gazed at

            //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Object focused by user: " + hitInfo.transform.gameObject.name);

            if (m_palmFacingUser)
            {
                if (m_statusEventTriggered == false)
                {
                    m_eventChallengeRefused?.Invoke(this, EventArgs.Empty);
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Event triggered, thanks to object: " + hitInfo.transform.gameObject.name);
                    m_statusEventTriggered = true;
                }
            }
            else
            {
                m_statusEventTriggered = false; // The palm is not opened anymore, so we reseat the boolean ensuring that only one event is trigerred when the palm is opened AND an object is focused by the user.
            }
        }
    }
}
