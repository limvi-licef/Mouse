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
 * This class aims at having a similar role than the Interaction MRTK component, but with providing the name of ths sender with the interaction event.
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
