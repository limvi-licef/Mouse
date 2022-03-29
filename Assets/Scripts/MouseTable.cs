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
public class MouseTable : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    public Transform m_interactionSurfaceTableView;
    MouseChallengeCleanTableSurfaceToPopulateWithCubes m_interactionSurfaceTableController;
    Transform m_assistanceStimulateLevel1View;
    MouseAssistanceChallengeSuccess m_assistanceChallengeSuccessController;

    public event EventHandler m_eventInteractionSurfaceTableTouched;
    public event EventHandler m_eventInteractionSurfaceCleaned;

    // Start is called before the first frame update
    void Start()
    {
        // Children
        m_interactionSurfaceTableView = gameObject.transform.Find("InteractionSurfaceTable");
        m_interactionSurfaceTableController = m_interactionSurfaceTableView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>();

        // Sanity check
        if (m_interactionSurfaceTableView.GetComponent<MouseChallengeCleanTableSurfaceToPopulateWithCubes>() == null)
        {
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Error, "The m_hologramInteractionSurface object should have a MouseChallengeCleanTableSurfaceToPopulateWithCubes component");
        }

        // Connect the callbacks
        m_interactionSurfaceTableView.GetComponent<TapToPlace>().OnPlacingStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished);
        m_interactionSurfaceTableView.GetComponent<BoundsControl>().ScaleStopped.AddListener(callbackHologramInteractionSurfaceMovedFinished); // Use the same callback than for taptoplace as the process to do is the same
        m_interactionSurfaceTableView.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(delegate()
        {
            m_eventInteractionSurfaceTableTouched?.Invoke(this, EventArgs.Empty);
        }); // Only have to forward the event
        m_interactionSurfaceTableController.m_eventSurfaceCleaned += new EventHandler(delegate (System.Object o, EventArgs e) { m_eventInteractionSurfaceCleaned?.Invoke(this, EventArgs.Empty); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackHologramInteractionSurfaceMovedFinished()
    {
        m_debug.displayMessage("MousePopulateSurfaceTableWithCubes", "callbackOnTapToPlaceFinished", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        // Bring specific components to the center of the interaction surface
        gameObject.transform.position = m_interactionSurfaceTableView.transform.position;
        m_interactionSurfaceTableView.transform.localPosition = new Vector3(0, 0f, 0);
    }

    public void hideInteractionSurfaceTable(EventHandler eventHandler)
    {
        m_interactionSurfaceTableController.resetCubesStates(eventHandler);
    }

    public void showInteractionSurfaceTable(EventHandler eventHandler)
    {
        m_interactionSurfaceTableController.showInteractionCubesTablePanel(eventHandler);
    }
}
