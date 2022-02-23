using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;

public class MouseCueing : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public event EventHandler m_eventHelpButtonClicked; 

    Transform m_hologramButtonClicked;

    // Start is called before the first frame update
    void Start()
    {
        // Children
        m_hologramButtonClicked = gameObject.transform.Find("WindowMenu").Find("ButtonHelp");

        // Callback
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_hologramButtonClicked, callbackButtonHelpClicked);
    }

    void callbackButtonHelpClicked()
    {
        m_eventHelpButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    public void show (bool withAnimation, EventHandler eventHandler)
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

    public void hide (bool withAnimation, EventHandler eventHandler)
    {
        if (withAnimation)
        {
            EventHandler[] temp = new EventHandler[] {eventHandler, new EventHandler(delegate (System.Object o, EventArgs e) {
                gameObject.transform.localScale = new Vector3(1,1,1);
                gameObject.SetActive(false);

                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());
            })};

            gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
