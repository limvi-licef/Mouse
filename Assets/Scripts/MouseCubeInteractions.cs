using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit;


public class MouseCubeInteractions : MonoBehaviour, IMixedRealityGestureHandler, IMixedRealityPointerHandler, IMixedRealityHandJointHandler, IMixedRealityTouchHandler
{

    //public WorldAnchorManager m_worldAnchorManager;
    public MouseDebugMessagesManager m_debugMessages;
    public Material m_matWhenTouched;
    //public bool m_updateAnchor;

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


    void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerClicked] Called");

        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerClicked", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerDragged] Called");

        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDragged", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerDown] Called");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        /*if (m_updateAnchor)
        {
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Destroying current anchor");
            m_worldAnchorManager.RemoveAnchor(gameObject);
        }*/

        if (eventData.Pointer is IMixedRealityNearPointer )
        {
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Pointer close to object");
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Current material: " + gameObject.GetComponent<Renderer>().material.ToString());
            //Material newMaterial = Resources.Load("Materials/Mouse_Standard_Red", typeof(Material)) as Material;
            gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "New material: " + gameObject.GetComponent<Renderer>().material.ToString());

        }
    }

    void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
    {
        //Debug.Log("[MouseWorldAnchorLocalManager::OnPointerUp] Called");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerUp", MouseDebugMessagesManager.MessageLevel.Info, "Called");
        /*if (m_updateAnchor)
        {
            m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerUp", MouseDebugMessagesManager.MessageLevel.Info, "Creating a new anchor and trying to save it");
            m_worldAnchorManager.AttachAnchor(gameObject);
        }*/
        
    }

    void IMixedRealityGestureHandler.OnGestureCanceled(InputEventData i)
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
    }

    void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint, Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>> eventData)
    {
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnHandJointsUpdated", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        m_debugMessages.displayMessage("MouseCubeInteractions", "OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {

    }

    public void touchHandler()
    {
        m_debugMessages.displayMessage("MouseCubeInteractions", "touchHandler", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");

        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Pointer close to object");
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "Current material: " + gameObject.GetComponent<Renderer>().material.ToString());
        //Material newMaterial = Resources.Load("Materials/Mouse_Standard_Red", typeof(Material)) as Material;
        gameObject.GetComponent<Renderer>().material = m_matWhenTouched;
        m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnPointerDown", MouseDebugMessagesManager.MessageLevel.Info, "New material: " + gameObject.GetComponent<Renderer>().material.ToString());
    }

    // Update is called once per frame
    void Update()
    {

    }
}