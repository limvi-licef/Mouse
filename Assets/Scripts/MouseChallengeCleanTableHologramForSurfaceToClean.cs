using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit;
using System;


public class MouseChallengeCleanTableHologramForSurfaceToClean : MonoBehaviour, IMixedRealityTouchHandler  /*IMixedRealityGestureHandler, IMixedRealityPointerHandler, IMixedRealityHandJointHandler , IMixedRealityTouchHandler*/
{

    //public WorldAnchorManager m_worldAnchorManager;
    public MouseDebugMessagesManager m_debugMessages;
    public Material m_matWhenTouched;
    //public bool m_updateAnchor;

    public event EventHandler CubeTouchedEvent;

    // Start is called before the first frame update
    void Start()
    {
        //m_storeInitialized = false;
        //Debug.Log("--------------------");
        //Debug.Log("[MouseWorldAnchorLocalManager::Start] Called for object " + transform.name);

        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "Start", MouseDebugMessagesManager.MessageLevel.Info, "--------------------");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "Start", MouseDebugMessagesManager.MessageLevel.Info, "Called for object " + transform.name);

        //m_worldAnchorManager.AttachAnchor(gameObject);
    }

    /*void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerClicked] Called");

        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerClicked", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }*/

    /*void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerDragged] Called");

        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDragged", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }*/

    /*void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerDown] Called");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Called");

        if (eventData.Pointer is IMixedRealityNearPointer )
        {
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Current material: " + gameObject.GetComponent<Renderer>().material.ToString());
            //Material newMaterial = Resources.Load("Materials/Mouse_Standard_Red", typeof(Material)) as Material;
            gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "New material: " + gameObject.GetComponent<Renderer>().material.ToString());

            IMixedRealityNearPointer tempPointer = (IMixedRealityNearPointer)eventData.Pointer;
            if (tempPointer.IsNearObject)
            {
                float distance = 0.0f;
                tempPointer.TryGetDistanceToNearestSurface(out distance);
                m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Pointer close to object. Distance: " + distance.ToString());
            }

        }
    }*/

    /*void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerUp] Called");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerUp", MouseDebugMessagesManager.MessageLevel.Info, "Called");        
    }*/

    /*void IMixedRealityGestureHandler.OnGestureCanceled(InputEventData i)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnGestureCanceled] Called");
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnGestureCanceled", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void IMixedRealityGestureHandler.OnGestureCompleted(InputEventData i)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnGestureCompleted] Called");
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnGestureCompletes", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void IMixedRealityGestureHandler.OnGestureStarted(InputEventData i)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnGestureStarted] Called");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnGestureStarted", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void IMixedRealityGestureHandler.OnGestureUpdated(InputEventData i)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnGestureUpdated] Called");
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnGestureUpdated", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }*/

    /*void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint, Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>> eventData)
    {
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnHandJointsUpdated", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }*/

    /*void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        m_debugMessages.displayMessage("MouseCubeInteractions", "OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {

    }*/

    public void  onTouch(/*UnityEngine.Events.UnityEvent ev, string s*/)
    {
        m_debugMessages.displayMessage("MouseCubeInteractions", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");

        /*m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "touchHandler", MouseDebugMessagesManager.MessageLevel.Info, "Pointer close to object");*/
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "touchHandler", MouseDebugMessagesManager.MessageLevel.Info, "Current material: " + gameObject.GetComponent<Renderer>().material.ToString());
        //Material newMaterial = Resources.Load("Materials/Mouse_Standard_Red", typeof(Material)) as Material;
        gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "touchHandler", MouseDebugMessagesManager.MessageLevel.Info, "New material: " + gameObject.GetComponent<Renderer>().material.ToString());

        /*if (ev is IMixedRealityNearPointer)
        {
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Current material: " + gameObject.GetComponent<Renderer>().material.ToString());
            gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "New material: " + gameObject.GetComponent<Renderer>().material.ToString());

            IMixedRealityNearPointer tempPointer = (IMixedRealityNearPointer)ev;
            if (tempPointer.IsNearObject)
            {
                float distance = 0.0f;
                tempPointer.TryGetDistanceToNearestSurface(out distance);
                m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Pointer close to object. Distance: " + distance.ToString());
            }

        }*/
    }

    /*public void onClick()
    {
        m_debugMessages.displayMessage("MouseCubeInteractions", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Object clicked");
        gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
    }*/

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
        CubeTouchedEvent?.Invoke(this, EventArgs.Empty);
        
        m_debugMessages.displayMessage("MouseCubeInteractions", "IMixedRealityTouchHandler.OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Touched");
    }

    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {
       
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}