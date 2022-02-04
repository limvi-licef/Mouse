using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System;


public class MouseChallengeCleanTableHologramForSurfaceToClean : MonoBehaviour, IMixedRealityTouchHandler
{
    public MouseDebugMessagesManager m_debugMessages;
    public Material m_matWhenTouched;
    public event EventHandler CubeTouchedEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        m_debugMessages.displayMessage("MouseCubeInteractions", "IMixedRealityTouchHandler.OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Touched");

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