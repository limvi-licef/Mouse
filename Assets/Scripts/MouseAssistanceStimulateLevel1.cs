/*Copyright 2022 Guillaume Spalla

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

/**
 * Assistance to who a cube, that has 3 gradation level: fix with discrete colors, vivid colors, and that follows the user
 * */
public class MouseAssistanceStimulateLevel1 : MouseAssistanceAbstract
{
    public Transform m_hologramView;
    MouseCubeOpening m_hologramController;
    Vector3 m_hologramOriginalLocalPos;

    Transform m_lightView;
    MouseUtilitiesLight m_lightController;

    MouseUtilitiesGradationManager m_gradationManager;

    public Transform m_surfaceWithStarsView;
    public Transform m_surfaceWithStarsViewTarget; // If provided, allows to adjust the size of the stars to the size of this surface

    public Transform m_help;

    public EventHandler m_eventHologramStimulateLevel1Gradation1Or2Touched;

    public AudioClip m_audioClipToPlayOnTouchInteractionSurface;
    public AudioListener m_audioListener;

    public bool m_hasFocus;

    private void Awake()
    {
        // Variables
        m_hasFocus = false;

        // Children
        m_hologramView = transform.Find("CubeOpening");
        m_hologramController = m_hologramView.GetComponent<MouseCubeOpening>();
        m_hologramOriginalLocalPos = m_hologramView.localPosition;

        m_lightView = transform.Find("Light");
        m_lightController = m_lightView.GetComponent<MouseUtilitiesLight>();

        m_surfaceWithStarsView = transform.Find("SurfaceWithStars");

        m_help = transform.Find("Help");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Setting up the gradation manger
        m_gradationManager = transform.GetComponent<MouseUtilitiesGradationManager>();
        m_gradationManager.addNewAssistanceGradation("Default", callbackGradationDefault);
        //m_gradationManager.addNewAssistanceGradation("Low", callbackGradationLow);
        m_gradationManager.addNewAssistanceGradation("LowVivid", callbackGradationLowVivid);
        //m_gradationManager.addNewAssistanceGradation("HighFollow", callbackGradationHighFollow);

        // Callbacks
        m_hologramController.m_eventCubetouched += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramStimulateLevel1Gradation1Or2Touched?.Invoke(this, EventArgs.Empty);

        });

        MouseUtilitiesHologramInteractions interactions = m_help.GetComponent<MouseUtilitiesHologramInteractions>();
        if (interactions == null)
        {
            interactions = m_help.gameObject.AddComponent<MouseUtilitiesHologramInteractions>();
        }
        interactions.s_touched += new EventHandler(delegate (System.Object o, EventArgs e)
        {
            m_eventHologramStimulateLevel1Gradation1Or2Touched?.Invoke(this, EventArgs.Empty);

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Help touched");

        });
        SolverHandler solver = m_help.GetComponent<SolverHandler>();
        if (solver == null)
        {
            solver = m_help.gameObject.AddComponent<SolverHandler>();
        }
        solver.TrackedTargetType = Microsoft.MixedReality.Toolkit.Utilities.TrackedObjectType.Head;
    }

    public bool hasFocus()
    {
        return m_hasFocus;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool m_mutexShow = false;
    public override void show(EventHandler eventHandler)
    {
        if (m_mutexShow == false)
        {
            m_mutexShow = true;

            MouseUtilitiesAnimation animator = m_hologramView.gameObject.AddComponent<MouseUtilitiesAnimation>();

            MouseUtilities.adjustObjectHeightToHeadHeight(m_help);
            m_hologramController.backupScaling();

            if (m_surfaceWithStarsViewTarget != null)
            { // Then adjusting the size of the stars to the size of this object
                Vector3 newScale = new Vector3();
                newScale.x = m_surfaceWithStarsViewTarget.localScale.x;
                newScale.y = m_surfaceWithStarsView.localScale.y; // Important not to change the height scale
                newScale.z = m_surfaceWithStarsViewTarget.localScale.z;

                m_surfaceWithStarsView.localScale = newScale;
            }

            m_help.gameObject.SetActive(true);
            m_surfaceWithStarsView.gameObject.SetActive(true);
            eventHandler?.Invoke(this, EventArgs.Empty);
            m_mutexShow = false;
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Mutex locked - Request ignored");
        }   
    }

    public void setPositionToOriginalLocation()
    {
        m_hologramView.localPosition = m_hologramOriginalLocalPos;
    }

    public override void hide(EventHandler eventHandler)
    {
        setGradationToMinimum();

        if (m_hologramView.gameObject.activeSelf)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube is going to be hidden");

            EventHandler[] eventHandlers = new EventHandler[] { new EventHandler(delegate (System.Object o, EventArgs e)
        {
            

            m_hologramView.gameObject.SetActive(false);
            m_hologramView.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            Destroy(m_hologramView.GetComponent<MouseUtilitiesAnimation>());

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube should be hidden now. New local position:" + m_hologramView.localPosition.ToString());
        }), eventHandler };

            m_hologramController.closeCube(new EventHandler(delegate (System.Object o, EventArgs e)
            {
                m_hologramView.gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(eventHandlers);
            }));
            m_lightView.gameObject.SetActive(false);
        }
        else if (m_surfaceWithStarsView.gameObject.activeSelf)
        {
            m_surfaceWithStarsView.gameObject.SetActive(false);
            m_help.gameObject.SetActive(false);
            eventHandler?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Assistance stimulate level 1 is disabled - no hide action to take");
        }
    }

    public bool increaseGradation()
    {
        return m_gradationManager.increaseGradation();
    }

    public bool decreaseGradation()
    {
        return m_gradationManager.decreaseGradation();
    }

    public void setGradationToMinimum()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Gradation set to minimum");

        m_gradationManager.setGradationToMinimum();
    }

    void callbackGradationDefault(System.Object o, EventArgs e)
    {
        m_surfaceWithStarsView.gameObject.SetActive(true);
        m_help.gameObject.SetActive(true);
        MouseUtilities.adjustObjectHeightToHeadHeight(m_help);
        m_hologramView.gameObject.SetActive(false);

        m_lightView.gameObject.SetActive(false);
        m_hologramController.GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;
        m_hologramController.setScalingToOriginal();

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Showing default gradation");
    }

    void callbackGradationLow (System.Object o, EventArgs e)
    {
        m_surfaceWithStarsView.gameObject.SetActive(false);
        m_help.gameObject.SetActive(false);

        MouseUtilities.adjustObjectHeightToHeadHeight(m_hologramView, m_hologramOriginalLocalPos.y);
        m_hologramView.gameObject.SetActive(true);

        m_hologramController.updateMaterials("Mouse_Clean_Bottom", "Mouse_Clean_Top-Left", "Mouse_Clean_Top-Right");
        m_lightView.gameObject.SetActive(false);
        m_hologramController.GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;
        m_hologramController.setScalingToOriginal();
    }

    void callbackGradationLowVivid(System.Object o, EventArgs e)
    {
        m_help.gameObject.SetActive(false);

        MouseUtilities.adjustObjectHeightToHeadHeight(m_hologramView, m_hologramOriginalLocalPos.y);
        m_hologramView.gameObject.SetActive(true);
        m_hologramController.updateMaterials("Mouse_Clean_Bottom_Vivid", "Mouse_Clean_Top-Left_Vivid", "Mouse_Clean_Top-Right_Vivid");
        m_lightView.gameObject.SetActive(true);
        m_hologramController.GetComponent<Billboard>().enabled = true;
        GetComponent<RadialView>().enabled = false;
        m_hologramController.setScalingToOriginal(); // setting back the scaling to the original one.

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Showing low vivid gradation");
    }

    void callbackGradationHighFollow(System.Object o, EventArgs e)
    {
        // Add an offset in y to the cube to avoid that they remain too much "in front" of the user
        GetComponent<RadialView>().enabled = true;
        m_hologramView.gameObject.GetComponent<Billboard>().enabled = false; // The billboard is present on the parent object so that the cube can be shifted to avoid that it is too much in front of the user.
        m_lightView.gameObject.SetActive(false);

        // reducing the scale of the cube to have it less intrusive
        m_hologramController.setScalingReduced();
        m_hologramView.transform.localPosition = new Vector3(m_hologramView.localPosition.x, m_hologramOriginalLocalPos.y, m_hologramView.localPosition.z);

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Showing high gradation");
    }

    public void openingCube(EventHandler eventHandler)
    {
        m_hologramController.openCube(eventHandler);
    }
}