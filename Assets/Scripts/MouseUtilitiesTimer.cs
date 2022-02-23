using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;

public class MouseUtilitiesTimer : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public int m_timerDuration = 2; // in Seconds
    bool m_timerStart;
    int m_timerDurationInternal; // To convert the seconds in FPS, as the timer uses the Update function to run

    public event EventHandler m_eventTimerFinished;

    // Start is called before the first frame update
    void Start()
    {
        m_timerStart = false;
        m_timerDurationInternal = m_timerDuration * 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timerStart)
        {
            m_timerDurationInternal -= 1;

            if (m_timerDurationInternal > 0)
            {
                /*if ((m_timerDuration * 60) % m_timerDurationInternal == 0)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Timer: seconds remaining: " + (m_timerDurationInternal / 60).ToString());
                }*/
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Timer finished!");
               
                m_timerStart = false;
                m_timerDurationInternal = m_timerDuration * 60;

                m_eventTimerFinished?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void startTimerOneShot()
    {
        if ( m_timerStart == false )
        {
            m_timerStart = true;
            m_timerDurationInternal = m_timerDuration * 60;
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Timer already running, so will not be started - please call again this function when this timer will be finished");
        }
    }

    public void stopTimer()
    {
        if (m_timerStart)
        {
            m_timerStart = false;

        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No timer currently running, so nothing to do");
        }
    }
}
