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

            /*if (withAnimation)
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
            }*/
            m_text.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e) {
                EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object oo, EventArgs ee) {
                    Destroy(m_button.gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexShow = false;
            }), eventHandler };

                m_button.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(m_debug, temp);

                Destroy(m_text.gameObject.GetComponent<MouseUtilitiesAnimation>());

                // m_mutexShow = false;
            }));

        }

        
    }

    // With animation, compatible with the gradation manager
    /*public void show(EventHandler e)
    {
        show(true, e);
    }*/

    bool m_mutexHide = false;
    public void hide (EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true;

            /*if (withAnimation)
            {
                EventHandler[] temp = new EventHandler[] {eventHandler, new EventHandler(delegate (System.Object o, EventArgs e) {
                gameObject.transform.localScale = new Vector3(1,1,1);
                gameObject.SetActive(false);

                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

                    m_mutexHide = false;
            })};

                gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
            }
            else
            {
                gameObject.SetActive(false);
                m_mutexHide = false;
            }*/

            m_text.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e) {
                EventHandler[] temp = new EventHandler[] {new EventHandler(delegate (System.Object oo, EventArgs ee) {
                    Destroy(m_button.gameObject.GetComponent<MouseUtilitiesAnimation>());
                    m_mutexHide = false;
            }), eventHandler };

                m_button.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);

                Destroy(m_text.gameObject.GetComponent<MouseUtilitiesAnimation>());

                // m_mutexShow = false;
            }));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
