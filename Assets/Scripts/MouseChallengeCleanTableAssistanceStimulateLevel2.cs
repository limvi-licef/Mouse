using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class MouseChallengeCleanTableAssistanceStimulateLevel2 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public event EventHandler m_eventHologramHelpTouched;

    public Transform m_hologramHelp;
    Transform m_hologramLineView;
    public Transform m_hologramText;

    MouseLineToObject m_hologramLineController;

    Vector3 m_textOriginalScaling;

    void Awake()
    {
        // Children
        m_hologramLineView = gameObject.transform.Find("Line");
        m_hologramLineController = m_hologramLineView.GetComponent<MouseLineToObject>();
        m_hologramHelp = MouseUtilities.mouseUtilitiesFindChild(gameObject, "Help");
        m_hologramText = gameObject.transform.Find("Text");

        m_textOriginalScaling = m_hologramText.localScale;
    }

    // Start is called before the first frame update
    void Start()
    {
        if ( m_hologramLineView == null )
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "Line hologram not initialized properly");
        }

        // Callbacks
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_hologramHelp, callbackHelpTouched);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackHelpTouched()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        m_eventHologramHelpTouched?.Invoke(this, EventArgs.Empty);
    }

    void resetObject()
    {
        m_hologramHelp.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Destroy(m_hologramHelp.GetComponent<MouseUtilitiesAnimation>());

        //gameObject.SetActive(false);
    }

    bool m_mutexShow = false;
    public void show(EventHandler eventHandler)
    {
        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            // Showing button
            /* if (m_hologramHelp.gameObject.activeSelf)
             {
                 m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Help button already displayed, so no animation to run");

                 m_mutexShow = false;
             }
             else
             {
                 //gameObject.SetActive(true);

                 EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
             {
                 m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                 Destroy(m_hologramText.gameObject.GetComponent<MouseUtilitiesAnimation>());

                 m_mutexShow = false;
             }), eventHandler };

                 m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(new Vector3(0.1f, 0.1f, 0.1f), m_debug, temp);
             }*/

            // Showing text
            //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Showing text to scaling: " + m_textOriginalScaling.ToString());

            m_hologramText.position = m_hologramLineController.m_hologramOrigin.transform.position;
            MouseUtilities.adjustObjectHeightToHeadHeight(m_debug, m_hologramText);

            m_hologramText.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(m_textOriginalScaling, m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                Destroy(m_hologramText.gameObject.GetComponent<MouseUtilitiesAnimation>());

                EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object oo, EventArgs ee)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());

                m_mutexShow = false;
            }), eventHandler };

                m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(new Vector3(0.1f, 0.1f, 0.1f), m_debug, temp);

                // m_mutexShow = false;
            }));

            // Showing line
            if (m_hologramLineView.gameObject.activeSelf == false)
            {
                m_hologramLineView.GetComponent<MouseLineToObject>().show(eventHandler);
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Line already shown: nothing to do");
            }
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - request ignored");
        }

        
    }

    bool m_mutexHide = false;
    public void hide(EventHandler eventHandler)
    {
        if (m_mutexHide == false)
        {
            m_mutexHide = true;

            /*// Hiding button
            if (m_hologramHelp.gameObject.activeSelf == false)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Help button is already hidden, so no animation will be started");
                m_mutexHide = false;
            }
            else
            {
                
            }*/

            // Hiding text
            m_hologramText.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_hologramText.gameObject.SetActive(false);

                //m_hologramText.localScale = new Vector3(0.1f,0.1f,0.1f);

                Destroy(m_hologramText.gameObject.GetComponent<MouseUtilitiesAnimation>());

                EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object oo, EventArgs ee)
            {
                m_hologramHelp.gameObject.SetActive(false);

                m_hologramHelp.localScale = new Vector3(0.1f,0.1f,0.1f);

                //gameObject.SetActive(false); // When the cube is hidden, as there is no animation to hide the line, the complete object can be hidden

			    Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());

                m_mutexHide = false;
            }), eventHandler };

                m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
            }));

            // Hiding line
            if (m_hologramLineView.gameObject.activeSelf)
            {
                m_hologramLineView.GetComponent<MouseLineToObject>().hide(/*eventHandler*/ MouseUtilities.getEventHandlerEmpty()); // The eventhandler being already called above, we do not want it to be called twice, as this could create strange behaviors.
            }
            else
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Line already hidden: nothing to do");
            }

        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - request ignored");
        }
    }

    public void setArchStartAndEndPoint(Transform origin, Transform target)
    {
        m_hologramLineController.m_hologramOrigin = origin.gameObject;
        m_hologramLineController.m_hologramTarget = target.gameObject;
    }
}
