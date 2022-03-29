using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Reflection;
using System.Linq;

public class MouseUtilitiesMutex
{
    bool m_mutex;
    MouseDebugMessagesManager m_debug;

    public MouseUtilitiesMutex (MouseDebugMessagesManager debug)
    {
        m_debug = debug;
        m_mutex = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void lockMutex()
    {
        m_mutex = true;
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex locked");
    }

    public void unlockMutex()
    {
        m_mutex = false;
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Mutex unlocked");
    }

    public bool isLocked()
    {
        return m_mutex;
    }
}
