using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

public class MouseUtilitiesGradationManager : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    List<AssistanceGradation> m_assistanceGradation;

    int m_assistanceGradationIndexCurrent;

    public struct AssistanceGradation
    {
        public AssistanceGradation (string name, EventHandler e)
        {
            id = name;
            callback = e;
        }

        public string id;
        public EventHandler callback;
    }

    private void Awake()
    {
        m_assistanceGradationIndexCurrent = -1; // i.e. no assistance in the list.
        m_assistanceGradation = new List<AssistanceGradation>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addNewAssistanceGradation(string id, EventHandler callback)
    {
        m_assistanceGradation.Add(new AssistanceGradation(id, callback));

        if (m_assistanceGradationIndexCurrent == -1)
        { // If this is the first gradation, then select it as the current one
            m_assistanceGradationIndexCurrent = 0;
        }
    }

    /*
     * Return true if max gradation is reached, false otherwise
     * */
    public bool increaseGradation()
    {
        int nbGradations = m_assistanceGradation.Count;
        bool toReturn = false;

        if(m_assistanceGradationIndexCurrent < nbGradations)
        {
            m_assistanceGradationIndexCurrent++;

            m_assistanceGradation[m_assistanceGradationIndexCurrent].callback?.Invoke(this, EventArgs.Empty);
        }


        if (m_assistanceGradationIndexCurrent == nbGradations - 1)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Maximum gradation level reached");
            toReturn = true;
        }



        return toReturn;
    }

    /*
     * Return true if min gradation is reached, false otherwise
     * */
    public bool decreaseGradation()
    {
        bool toReturn = false;

        if (m_assistanceGradationIndexCurrent > 0) // 0 being the first element of the list, i.e. the minimal one
        {
            m_assistanceGradationIndexCurrent--;

            m_assistanceGradation[m_assistanceGradationIndexCurrent].callback?.Invoke(this, EventArgs.Empty);
        }

        if (m_assistanceGradationIndexCurrent == 0)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Minimum gradation level reached");

            toReturn = true;
        }

        return toReturn;
    }

    public void setGradationToMinimum()
    {
        if (m_assistanceGradationIndexCurrent == 0)
        { // If gradation is already to the minimum, then nothing to do
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Gradation already at minimal level - nothing to do");
        }
        else
        {
            m_assistanceGradationIndexCurrent = 0;
            m_assistanceGradation[m_assistanceGradationIndexCurrent].callback?.Invoke(this, EventArgs.Empty);
        }
    }
}
