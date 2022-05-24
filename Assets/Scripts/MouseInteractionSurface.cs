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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

/**
 * Manages the table interaction surface
 * */
public class MouseInteractionSurface : MonoBehaviour
{
    Transform m_interactionSurfaceView;

    public event EventHandler m_eventInteractionSurfaceTableTouched;
    public event EventHandler s_interactionSurfaceScaled;
    public event EventHandler s_interactionSurfaceMoved;

    string m_color = "Mouse_Green_Glowing"; // Default color if the user does not set one

    bool m_surfaceInitialized;

    private void Awake()
    {
        // Initialize variables
        m_surfaceInitialized = false;

        // Children
        m_interactionSurfaceView = gameObject.transform.Find("InteractionSurfaceChild");
    }

    // Start is called before the first frame update
    void Start()
    {
        BoundsControl boundsControl = m_interactionSurfaceView.GetComponent<BoundsControl>();

        boundsControl.ScaleStopped.AddListener(delegate
        {
            s_interactionSurfaceScaled?.Invoke(this, EventArgs.Empty);
        });

        ObjectManipulator objectManipulator = m_interactionSurfaceView.GetComponent<ObjectManipulator>();
        objectManipulator.OnManipulationEnded.AddListener(delegate (ManipulationEventData data)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Called");
            s_interactionSurfaceMoved?.Invoke(this, EventArgs.Empty);
        });
    }

    public Transform getInteractionSurface()
    {
        return m_interactionSurfaceView;
    }

    /**
     * The name of the color should reference an object present in the "resources" directory
     * **/
    public void setColor(string colorName)
    {
        MouseDebugMessagesManager.Instance.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Loading color");

        m_color = colorName;

        m_interactionSurfaceView.GetComponent<Renderer>().material = Resources.Load(m_color, typeof(Material)) as Material;
    }

    public void setScaling(Vector3 scaling)
    {
        m_interactionSurfaceView.localScale = scaling;
    }

    public void setLocalPosition(Vector3 position)
    {
        m_interactionSurfaceView.localPosition = position;
    }

    public Vector3 getLocalPosition()
    {
        return m_interactionSurfaceView.localPosition;
    }

    public Vector3 getLocalScale()
    {
        return m_interactionSurfaceView.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackHologramInteractionSurfaceMovedFinished()
    {
        MouseDebugMessagesManager.Instance.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Bring specific components to the center of the interaction surface
        gameObject.transform.position = m_interactionSurfaceView.transform.position;
        m_interactionSurfaceView.transform.localPosition = new Vector3(0, 0f, 0);
    }

    void callbackShow()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Callback showing / hiding interaction surface called");

        showInteractionSurfaceTable(!(m_interactionSurfaceView.GetComponent<BoundsControl>().enabled));
    }

    public void showInteractionSurfaceTable(bool show)
    {
        

        if (m_surfaceInitialized == false)
        {
            m_interactionSurfaceView.gameObject.SetActive(true); // If it happens that the surface is not displayed

            // Connect the callbacks
            m_interactionSurfaceView.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished); // Use the same callback than for taptoplace as the process to do is the same
            m_interactionSurfaceView.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(delegate ()
            {
                m_eventInteractionSurfaceTableTouched?.Invoke(this, EventArgs.Empty);
            }); // Only have to forward the event

            
            m_surfaceInitialized = true;

        }

        m_interactionSurfaceView.GetComponent<Renderer>().enabled = show; // To hide the surface while keeping it interactable, then the renderer is disabled if show==false;
        m_interactionSurfaceView.GetComponent<BoundsControl>().enabled = show;
    }

    public void callbackBring()
    {
        gameObject.transform.position = new Vector3(Camera.main.transform.position.x + 1.5f, Camera.main.transform.position.y - 0.5f, Camera.main.transform.position.z);

        MouseDebugMessagesManager.Instance.displayMessage("MouseUtilitiesAdminMenu", "callbackBringInteractionSurface", MouseDebugMessagesManager.MessageLevel.Info, "Called - Camera position: " + Camera.main.transform.position + " New position of the object: " + gameObject.transform.position);
    }

    public void setAdminButtons(string interfaceSurfaceId, MouseUtilitiesAdminMenu.Panels panel = MouseUtilitiesAdminMenu.Panels.Default)
    {
        MouseUtilitiesAdminMenu.Instance.addSwitchButton("Hide " + interfaceSurfaceId + " interaction surface", callbackShow, panel);
        MouseUtilitiesAdminMenu.Instance.addButton("Bring " + interfaceSurfaceId + " interaction surface", callbackBring, panel);
    }

    public void setObjectResizable(bool enable)
    {
        if (enable)
        {
            m_interactionSurfaceView.GetComponent<BoundsControl>().ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.NonUniform;
        }
        else
        {
            m_interactionSurfaceView.GetComponent<BoundsControl>().ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.Uniform;
        }
        
    }



    public void setPreventResizeY(bool prevent)
    {
        if (prevent)
        {
            m_interactionSurfaceView.GetComponent<BoundsControl>().FlattenAxis = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.FlattenModeType.FlattenY;
        }
        else
        {
            m_interactionSurfaceView.GetComponent<BoundsControl>().FlattenAxis = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.FlattenModeType.DoNotFlatten;
        }
    }

    /**
     * Trigger the touch event from the script.
     * */
    public void triggerTouchEvent()
    {
        m_eventInteractionSurfaceTableTouched?.Invoke(this, EventArgs.Empty);
    }
}
