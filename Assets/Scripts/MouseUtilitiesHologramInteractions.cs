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
using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

/**
 * This class aims at having a similar role than the Interaction MRTK component, but with providing the name of the sender with the interaction event.
 * */
public class MouseUtilitiesHologramInteractions : MonoBehaviour, IMixedRealityTouchHandler, IMixedRealityFocusHandler
{
    public event EventHandler s_touched;
    public event EventHandler s_focusOn;
    public event EventHandler s_focusOff;

    // Start is called before the first frame update
    void Start()
    {

    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        s_touched?.Invoke(this, EventArgs.Empty);
    }

    // Here because it has to be to complete the implementation of the interface
    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData) { }
    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData) { }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        s_focusOn?.Invoke(this.gameObject, EventArgs.Empty);
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        s_focusOff?.Invoke(this.gameObject, EventArgs.Empty);
    }
}
