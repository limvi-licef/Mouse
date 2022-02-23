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
}