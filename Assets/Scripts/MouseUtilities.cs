using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;
using System.Timers;

static class MouseUtilities
{
    // Utilities functions: to be moved to a dedicated namespace later?
    public static void mouseUtilitiesAddTouchCallback(MouseDebugMessagesManager debug, Transform transform, UnityEngine.Events.UnityAction callback)
    {
        GameObject gameObject = transform.gameObject;

        Interactable interactable = gameObject.GetComponent<Interactable>();

        if (interactable == null)
        {
            debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No interactable component to the gameobject: adding one");

            interactable = gameObject.AddComponent<Interactable>();
        }

        InteractableOnTouchReceiver receiver = interactable.GetReceiver<InteractableOnTouchReceiver>();

        if (receiver == null)
        {
            debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No touch receiver to the interactable gameobject: adding one");

            receiver = interactable.AddReceiver<InteractableOnTouchReceiver>();
        }

        receiver.OnTouchStart.AddListener(callback);
    }

    public static Transform mouseUtilitiesFindChild(GameObject parent, string childId)
    {
        return parent.transform.Find(childId);
    }

    public static EventHandler getEventHandlerEmpty()
    {
        return new EventHandler(delegate (System.Object o, EventArgs e)
        {

        });
    }

    public static EventHandler getEventHandlerWithDebugMessage(MouseDebugMessagesManager debug, string debugMessage)
    {
        return new EventHandler(delegate (System.Object o, EventArgs e)
        {
            debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, debugMessage);
        });
    }

    public static void adjustObjectHeightToHeadHeight(MouseDebugMessagesManager debug, Transform t, float originalLocalHeightPos=0.0f)
    {
        debug.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Adjusting object height to head position: x= " + t.position.x + " y= Camera position (" + Camera.main.transform.position.y + ") + local position (" + originalLocalHeightPos + ") z=" + t.position.z);

        t.position = new Vector3(t.position.x, Camera.main.transform.position.y + /*t.localPosition.y*/ originalLocalHeightPos, t.position.z);
    }

    // From https://forum.unity.com/threads/how-to-know-if-a-script-is-running-inside-unity-editor-when-using-device-simulator.921827/
    public static bool IsEditorSimulator()
    {
#if UNITY_EDITOR
        return !Application.isEditor;
#endif
        return false;
    }
    public static bool IsEditorGameView()
    {
#if UNITY_EDITOR
        return Application.isEditor;
#endif
        return false;
    }
}