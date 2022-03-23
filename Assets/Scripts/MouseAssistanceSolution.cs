using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;

public class MouseAssistanceSolution : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool m_mutexShow = false;
    public void show(bool withAnimation, EventHandler eventHandler)
    {
        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, transform);

            if (withAnimation)
            {
                EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e) {
                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexShow = false;
            }), eventHandler };

                gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(m_debug, temp);
            }
            else
            {
                gameObject.SetActive(true);

                m_mutexShow = false;
            }
        }

        
    }

    bool m_mutexHide = false;
    public void hide(bool withAnimation, EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true;

            if (withAnimation)
            {
                EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e) {
                gameObject.transform.localScale = new Vector3(1,1,1);
                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
                gameObject.SetActive(false);
                   m_mutexHide = false;
            }), eventHandler };

                gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
            }
            else
            {
                gameObject.SetActive(false);
                m_mutexHide = false;
            }
        }

        
    }
}
