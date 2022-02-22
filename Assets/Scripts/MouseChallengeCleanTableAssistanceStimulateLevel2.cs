using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class MouseChallengeCleanTableAssistanceStimulateLevel2 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public event EventHandler m_eventHologramHelpTouched;

    Transform m_hologramHelp;
    Transform m_hologramLine;

    void Awake()
    {
        // Children
        m_hologramLine = gameObject.transform.Find("Line");
        m_hologramHelp = MouseUtilities.mouseUtilitiesFindChild(gameObject, "Help");
    }

    // Start is called before the first frame update
    void Start()
    {
        //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Get children
        //m_hologramHelp = MouseUtilities.mouseUtilitiesFindChild(gameObject, "Help");
        //m_hologramLine = gameObject.transform.Find("Line");//MouseUtilities.mouseUtilitiesFindChild(gameObject, "Line");

        if ( m_hologramLine == null )
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

        /*m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

            m_eventHologramHelpTouched?.Invoke(this, EventArgs.Empty);

            m_hologramHelp.gameObject.SetActive(false);

            // Let also delete the line
            m_hologramLine.GetComponent<MouseLineToObject>().hideLine();

            resetObject();
        }));*/
    }

    void resetObject()
    {
        m_hologramHelp.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        Destroy(m_hologramHelp.GetComponent<MouseUtilitiesAnimation>());

        gameObject.SetActive(false);
    }

   /* public void displayHelpButton (bool withAnimation)
    {
        if (gameObject.activeSelf)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Help button already displayed, so no animation to run");

            //
        }
        else
        {
            gameObject.SetActive(true);

            if (withAnimation)
            {
                m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(new Vector3(0.1f, 0.1f, 0.1f), m_debug, new EventHandler(delegate (System.Object o, EventArgs e)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                    Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());
                }));
            }
            else
            {
                m_hologramHelp.gameObject.SetActive(true);
            }
        }
    }*/

    // Additional event handler so that the user can execute an extra event.
    /*public void hideHelpButton(bool withAnimation, EventHandler eventHandler)
    {
        if (m_hologramHelp.gameObject.activeSelf == false)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Help button is already hidden, so no animation will be started");
        }
        else
        {
            if (withAnimation)
            {
                EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
            {
                //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                m_hologramHelp.gameObject.SetActive(false);

                m_hologramHelp.localScale = new Vector3(0.1f,0.1f,0.1f);

                Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());
            }), eventHandler };

                m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
            }
            else
            {
                m_hologramHelp.gameObject.SetActive(false);
            }
        }

        
    }*/

    /*public void displayLine(bool withAnimation)
    {
        if (m_hologramLine.GetComponent<MouseLineToObject>() == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "No MouseLineToObjects found");
        }
        else
        {
            m_hologramLine.GetComponent<MouseLineToObject>().displayLine(withAnimation);
        }
    }*/

    /*public void hideLine(bool withAnimation)
    {
        if(withAnimation)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "Not implemented yet with animation");
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

            m_hologramLine.GetComponent<MouseLineToObject>().hideLine();

            //m_hologramLine.gameObject.SetActive(false);
        }
    }*/

    public void show(EventHandler eventHandler)
    {
        // Showing button
        if (gameObject.activeSelf)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Help button already displayed, so no animation to run");

            //
        }
        else
        {
            gameObject.SetActive(true);

            EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

                Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());
            }), eventHandler };

            m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlaceToScaling(new Vector3(0.1f, 0.1f, 0.1f), m_debug, temp);
        }
        // Showing line
        if (m_hologramLine.gameObject.activeSelf == false)
        {
            m_hologramLine.GetComponent<MouseLineToObject>().show(eventHandler);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Line already shown: nothing to do");
        }
    }

    public void hide(EventHandler eventHandler)
    {
        // Hiding button
        if (m_hologramHelp.gameObject.activeSelf == false)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Help button is already hidden, so no animation will be started");
        }
        else
        {
			EventHandler[] temp = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
		    {
			    //m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");

			    m_hologramHelp.gameObject.SetActive(false);

			    m_hologramHelp.localScale = new Vector3(0.1f,0.1f,0.1f);

                gameObject.SetActive(false); // When the cube is hidden, as there is no animation to hide the line, the complete object can be hidden

			    Destroy(m_hologramHelp.gameObject.GetComponent<MouseUtilitiesAnimation>());
		    }), eventHandler };

			m_hologramHelp.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, temp);
        }

        // Hiding line
        if (m_hologramLine.gameObject.activeSelf)
        {
            m_hologramLine.GetComponent<MouseLineToObject>().hide(eventHandler);
        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Line already hidden: nothing to do");
        }
    }
}
