using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;
//using Microsoft.MixedReality.Toolkit.Experimental.Utilities;
using Microsoft.MixedReality.Toolkit;


public class MouseChallengeCubeInteractions : MonoBehaviour, IMixedRealityGestureHandler, IMixedRealityPointerHandler, IMixedRealityHandJointHandler //, IMixedRealityTouchHandler
{

    //public WorldAnchorManager m_worldAnchorManager;
    public MouseDebugMessagesManager m_debugMessages;
    public MousePopulateSurfaceTableWithCubes m_surfaceToPopulate;
    //public Material m_matWhenTouched;
    //public bool m_updateAnchor;

    // Start is called before the first frame update
    void Start()
    {
        
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
        
    }

    void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
    {
        
    }

    void IMixedRealityGestureHandler.OnGestureCanceled(InputEventData i)
    {
        
    }

    void IMixedRealityGestureHandler.OnGestureCompleted(InputEventData i)
    {
       
    }

    void IMixedRealityGestureHandler.OnGestureStarted(InputEventData i)
    {
        
    }

    void IMixedRealityGestureHandler.OnGestureUpdated(InputEventData i)
    {
        
    }

    void IMixedRealityHandJointHandler.OnHandJointsUpdated(InputEventData<IDictionary<Microsoft.MixedReality.Toolkit.Utilities.TrackedHandJoint, Microsoft.MixedReality.Toolkit.Utilities.MixedRealityPose>> eventData)
    {
        //m_debugMessages.displayMessage("MouseWorldAnchorLocalManager", "OnHandJointsUpdated", MouseDebugMessagesManager.MessageLevel.Info, "Called");
    }

    /*void IMixedRealityTouchHandler.OnTouchCompleted(HandTrackingInputEventData eventData)
    {

    }

    void IMixedRealityTouchHandler.OnTouchStarted(HandTrackingInputEventData eventData)
    {
        m_debugMessages.displayMessage("MouseChallengeCubeInteractions", "OnTouchStarted", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
    }

    void IMixedRealityTouchHandler.OnTouchUpdated(HandTrackingInputEventData eventData)
    {

    }*/

    public void  onTouch(/*UnityEngine.Events.UnityEvent ev, string s*/)
    {
        m_debugMessages.displayMessage("MouseChallengeCubeInteractions", "onTouch", MouseDebugMessagesManager.MessageLevel.Info, "Object touched");
        m_surfaceToPopulate.populateTablePanel();
    }

    public void onClick(/*UnityEngine.Events.UnityEvent ev, string s*/)
    {
        m_debugMessages.displayMessage("MouseChallengeCubeInteractions", "onClick", MouseDebugMessagesManager.MessageLevel.Info, "Object clicked");
    }

    // Update is called once per frame
    void Update()
    {

    }
}