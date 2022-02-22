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

    public void show(bool withAnimation, EventHandler eventHandler)
    {
        if (withAnimation)
        {
            EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e) {
                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
            }), eventHandler };

            gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(m_debug, temp);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    public void hide(bool withAnimation, EventHandler eventHandler)
    {
        if (withAnimation)
        {
            EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object o, EventArgs e) {
                gameObject.transform.localScale = new Vector3(1,1,1);
                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
                gameObject.SetActive(false);
            }), eventHandler };

            gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
