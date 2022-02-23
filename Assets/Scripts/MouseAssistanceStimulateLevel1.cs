using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

public class MouseAssistanceStimulateLevel1 : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;

    public enum AssistanceGradation
    {
        Default = 0,
        Low_Vivid = 1,
        Mid_Audio = 2,
        High_Follow = 3
    }

    AssistanceGradation m_assistanceGradation;

    Transform m_hologramView;
    //Transform m_hologramFollowView;
    Transform m_lightView;
    MouseUtilitiesLight m_lightController;

    public EventHandler m_eventHologramStimulateLevel1Gradation1Or2Touched;

    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    public bool m_hasFocus;

    // Start is called before the first frame update
    void Start()
    {
        // Variables
        m_assistanceGradation = AssistanceGradation.Default;
        m_hasFocus = false;

        // Children
        m_hologramView = transform.Find("MouseChallengeCleanTableAssistanceStimulateLevel1");
        m_lightView = transform.Find("Light");
        m_lightController = m_lightView.GetComponent<MouseUtilitiesLight>();

        // Callbacks
        MouseUtilities.mouseUtilitiesAddTouchCallback(m_debug, m_hologramView, delegate () { m_eventHologramStimulateLevel1Gradation1Or2Touched?.Invoke(this, EventArgs.Empty); });

        /*InteractableOnFocusReceiver focusReceiver = m_hologramView.Find("FocusDetection").GetComponent<Interactable>().AddReceiver<InteractableOnFocusReceiver>();
        focusReceiver.OnFocusOn.AddListener(delegate () {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Stimulate level 1 focus on");
            m_hasFocus = true;
        }
        );
        focusReceiver.OnFocusOff.AddListener(delegate () {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Stimulate level 1 focus off");
            m_hasFocus = false;
        }
        );*/
    }

    public bool hasFocus()
    {
        return m_hasFocus;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void show(EventHandler eventHandler)
    {
        switch (m_assistanceGradation)
        {
            case AssistanceGradation.Default:
            case AssistanceGradation.Low_Vivid:
            case AssistanceGradation.Mid_Audio:
            case AssistanceGradation.High_Follow:
                if (m_hologramView.gameObject.activeSelf == false)
                {
                    MouseUtilitiesAnimation animator = m_hologramView.gameObject.AddComponent<MouseUtilitiesAnimation>();

                    EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
                    {
                        Destroy(animator);
                    }), eventHandler };

                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Target scaling: " + m_hologramView.transform.localScale.ToString());

                    animator.animateAppearInPlaceToScaling(m_hologramView.transform.localScale, m_debug, eventHandlers);
                }
                else
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 1 is enabled - no hide action to take");
                }

                break;
            default:
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Nothing to show, as assistance gradation level not managed / not recognized");
                break;
        }

    }

    public void hide(EventHandler eventHandler)
    {
        switch (m_assistanceGradation)
        {
            case AssistanceGradation.Default:
            case AssistanceGradation.Low_Vivid:
            case AssistanceGradation.Mid_Audio:
            case AssistanceGradation.High_Follow:
                if (m_hologramView.gameObject.activeSelf)
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube is going to be hidden");


                    EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
               {
                   m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube should be hidden now");

                   m_hologramView.gameObject.SetActive(false);
                   m_hologramView.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                   Destroy(m_hologramView.GetComponent<MouseUtilitiesAnimation>());
               }), eventHandler };

                    m_hologramView.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(m_debug, eventHandlers);
                    m_lightView.gameObject.SetActive(false);
                }
                else
                {
                    m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 1 is disabled - no hide action to take");
                }

                break;
            default:
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Nothing to hide, as assistance gradation level not managed / not recognized");
                break;
        }
    }

    public AssistanceGradation increaseGradation()
    {
        AssistanceGradation toReturn = m_assistanceGradation;

        int maxGradation = Enum.GetValues(typeof(AssistanceGradation)).Cast<int>().Max();

        if ((int)m_assistanceGradation < maxGradation)
        {
            m_assistanceGradation++;
            toReturn = m_assistanceGradation;

            // Do the modification of the holograms to take into account the new gradation
            updateHologramsToGradation();

        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Maximum gradation level reached");
        }

        return m_assistanceGradation;
    }

    public AssistanceGradation decreaseGradation()
    {
        AssistanceGradation toReturn = m_assistanceGradation;

        int minGradation = Enum.GetValues(typeof(AssistanceGradation)).Cast<int>().Min();

        if ((int)m_assistanceGradation > minGradation)
        {
            m_assistanceGradation++;
            toReturn = m_assistanceGradation;

            // Do the modification of the holograms to take into account the new gradation
            updateHologramsToGradation();

        }
        else
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Minimum gradation level reached");
        }

        return m_assistanceGradation;
    }

    public void setGradationToMinimum()
    {
        m_assistanceGradation = AssistanceGradation.Default;
        updateHologramsToGradation();
    }

    void updateHologramsToGradation()
    {
        switch (m_assistanceGradation)
        {
            case AssistanceGradation.Default:
                m_hologramView.GetComponent<Renderer>().material = Resources.Load("Mouse_Clean", typeof(Material)) as Material;
                m_lightView.gameObject.SetActive(false);
                m_hologramView.gameObject.GetComponent<SolverHandler>().enabled = false;
                m_hologramView.gameObject.GetComponent<SurfaceMagnetism>().enabled = false;
                //m_hologramDefaultView.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                break;
            case AssistanceGradation.Low_Vivid:
                m_hologramView.GetComponent<Renderer>().material = Resources.Load("Mouse_Clean_Vivid", typeof(Material)) as Material;
                m_lightView.gameObject.SetActive(true);
                m_hologramView.gameObject.GetComponent<SolverHandler>().enabled = false;
                m_hologramView.gameObject.GetComponent<SurfaceMagnetism>().enabled = false;
                //m_hologramDefaultView.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                break;
            case AssistanceGradation.Mid_Audio:
                m_audioListener.GetComponent<AudioSource>().PlayOneShot(m_audioClipToPlayOnTouchInteractionSurface);
                m_hologramView.gameObject.GetComponent<SolverHandler>().enabled = false;
                m_hologramView.gameObject.GetComponent<SurfaceMagnetism>().enabled = false;
                //m_hologramDefaultView.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                m_lightView.gameObject.SetActive(true);
                break;
            case AssistanceGradation.High_Follow:
                m_hologramView.gameObject.GetComponent<SolverHandler>().enabled = true;
                m_hologramView.gameObject.GetComponent<SurfaceMagnetism>().enabled = true;
                //m_hologramDefaultView.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                m_lightView.gameObject.SetActive(false);
                break;
            default:
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Nothing to show, as assistance gradation level not managed / not recognized");
                break;
        }
    }
}
