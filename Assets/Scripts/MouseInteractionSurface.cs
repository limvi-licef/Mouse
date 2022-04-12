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
    //public MouseChallengeCleanTableSurfaceToPopulateWithCubes m_interactionSurfaceTableController;
    //Transform m_assistanceStimulateLevel1View;
    //MouseAssistanceChallengeSuccess m_assistanceChallengeSuccessController;

    public event EventHandler m_eventInteractionSurfaceTableTouched;
    //public event EventHandler m_eventInteractionSurfaceCleaned;

    string m_color = "Mouse_Green_Glowing"; // Default color if the user does not set one

    private void Awake()
    {
        // Children
        m_interactionSurfaceView = gameObject.transform.Find("InteractionSurfaceChild");
    }

    // Start is called before the first frame update
    void Start()
    {
        //m_interactionSurfaceView.gameObject.SetActive(true); // As hidden by default


        //m_interactionSurfaceTableController = m_interactionSurfaceView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>();

        /*// Sanity check
        if (m_interactionSurfaceView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>() == null)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "The m_hologramInteractionSurface object should have a MouseChallengeCleanTableSurfaceToPopulateWithCubes component");
        }*/

        // Set color to children
        //m_interactionSurfaceView.GetComponent<Renderer>().material = m_color;

        // Connect the callbacks
        //m_interactionSurfaceTableView.GetComponent<TapToPlace>().OnPlacingStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished);
        m_interactionSurfaceView.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished); // Use the same callback than for taptoplace as the process to do is the same
        m_interactionSurfaceView.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(delegate()
        {
            m_eventInteractionSurfaceTableTouched?.Invoke(this, EventArgs.Empty);
        }); // Only have to forward the event
        //m_interactionSurfaceTableController.m_eventSurfaceCleaned += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventInteractionSurfaceCleaned?.Invoke(this, EventArgs.Empty); });
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

        showInteractionSurfaceTable(!(m_interactionSurfaceView.transform.Find("rigRoot").gameObject.activeSelf));
    }

    public void showInteractionSurfaceTable(bool show)
    {
        //m_interactionSurfaceTableController.resetCubesStates(eventHandler);

        /*string color = m_color;
        if (show == false)
        {
            color = "Mouse_White_Transparent";
        }*/

        //m_interactionSurfaceView.GetComponent<Renderer>().material = Resources.Load("Mouse_White_Transparent", typeof(Material)) as Material;
        m_interactionSurfaceView.GetComponent<Renderer>().enabled = show;
        m_interactionSurfaceView.GetComponent<BoundsControl>().enabled = show;
        m_interactionSurfaceView.transform.Find("rigRoot").gameObject.SetActive(show);
    }

    /*public void showInteractionSurfaceTable(EventHandler eventHandler)
    {
        m_interactionSurfaceTableController.showInteractionCubesTablePanel(eventHandler);
    }*/

    public void setAdminButton(string text)
    {
        // Add button to admin interface
        MouseUtilitiesAdminMenu.Instance.addSwitchButton(text, callbackShow);
    }
}
