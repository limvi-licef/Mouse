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
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System;

/**
 * Describes the hologram that is used to populate the surface to clean.
 * Emits an event when touched
 * */
public class MouseChallengeCleanTableHologramForSurfaceToClean : MonoBehaviour, IMixedRealityTouchHandler
{
    public Material m_matWhenTouched;
    public event EventHandler CubeTouchedEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        MouseDebugMessagesManager.Instance.displayMessage("MouseCubeInteractions", "IMixedRealityTouchHandler.OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Touched");

        gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
        CubeTouchedEvent?.Invoke(this, EventArgs.Empty);
    }

    // Here because it has to be to complete the implementation of the interface
    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData) { }
    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData) { }

    // Update is called once per frame
    void Update()
    {

    }
}