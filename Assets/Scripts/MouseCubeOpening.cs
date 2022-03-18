using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;

public class MouseCubeOpening : MonoBehaviour
{
    public MouseDebugMessagesManager m_debug;
    Transform m_cubeTopLeftPartView;
    Transform m_cubeTopRightPartView;
    Transform m_cubeBottomPartView;

    bool m_mutexClosingOngoing;

    public event EventHandler m_eventCubetouched;

    // Start is called before the first frame update
    void Start()
    {
        // Children
        m_cubeTopRightPartView = gameObject.transform.Find("TopRightPart");
        m_cubeTopLeftPartView = gameObject.transform.Find("TopLeftPart");
        m_cubeBottomPartView = gameObject.transform.Find("BottomPart");

        m_mutexClosingOngoing = false;

        // Add callbacks
        gameObject.GetComponent<Interactable>().GetReceiver<InteractableOnTouchReceiver>().OnTouchStart.AddListener(callbackCubeTouched);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void callbackCubeTouched()
    {
        m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube touched");

        openCube(m_eventCubetouched);

    }

    public void updateMaterials(string materialNameBottom, string materialNameTopLeft, string materialNameTopRight)
    {
        m_cubeBottomPartView.GetComponent<Renderer>().material = Resources.Load(materialNameBottom, typeof(Material)) as Material;
        m_cubeTopLeftPartView.GetComponent<Renderer>().material = Resources.Load(materialNameTopRight, typeof(Material)) as Material; // What left when preparing the meterial is actually right here
        m_cubeTopRightPartView.GetComponent<Renderer>().material = Resources.Load(materialNameTopLeft, typeof(Material)) as Material; // What left when preparing the meterial is actually left here
    }

    public void openCube(EventHandler callback)
    {
        // Moving the parts
        Vector3 worldDestPosLeftPart = gameObject.transform.TransformPoint(new Vector3(0.75f, 0.5f, 0f));
        Vector3 worldDestPosRightPart = gameObject.transform.TransformPoint(new Vector3(-0.75f, 0.5f, 0f));
        MouseUtilitiesAnimation animatorLeftPart = m_cubeTopLeftPartView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animatorLeftPart.m_animationSpeed = 0.5f;
        MouseUtilitiesAnimation animatorRightPart = m_cubeTopRightPartView.gameObject.AddComponent<MouseUtilitiesAnimation>();
        animatorRightPart.m_animationSpeed = 0.5f;


        animatorLeftPart.animateMoveToPosition(worldDestPosLeftPart, m_debug, MouseUtilities.getEventHandlerEmpty());
        animatorRightPart.animateMoveToPosition(worldDestPosRightPart, m_debug, new EventHandler(delegate (System.Object o, EventArgs e) { callback?.Invoke(this, EventArgs.Empty); }));
    }

    public void closeCube(EventHandler callback)
    {
        if (m_mutexClosingOngoing)
        { // Do not accept a new request if the process of the current one is not yet finished
            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "A request to close the cube is currently being processed. This one is ignored.");
        }
        else
        {
            m_mutexClosingOngoing = true; // Locking the mutex

            m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Closing cube ...");

            // Moving the parts
            Vector3 worldDestPosLeftPart = gameObject.transform.TransformPoint(new Vector3(0.25f, 0.5f, 0f));
            Vector3 worldDestPosRightPart = gameObject.transform.TransformPoint(new Vector3(-0.25f, 0.5f, 0f));
            MouseUtilitiesAnimation animatorLeftPart = m_cubeTopLeftPartView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            animatorLeftPart.m_animationSpeed = 0.5f;
            MouseUtilitiesAnimation animatorRightPart = m_cubeTopRightPartView.gameObject.AddComponent<MouseUtilitiesAnimation>();
            animatorRightPart.m_animationSpeed = 0.5f;


            animatorLeftPart.animateMoveToPosition(worldDestPosLeftPart, m_debug, MouseUtilities.getEventHandlerEmpty());
            animatorRightPart.animateMoveToPosition(worldDestPosRightPart, m_debug, new EventHandler(delegate (System.Object o, EventArgs e) {
                m_debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Cube closed ");
                callback?.Invoke(this, EventArgs.Empty);
                m_mutexClosingOngoing = false; // Process finished: unlocking the mutex
            }));
        }
    }
        
}
