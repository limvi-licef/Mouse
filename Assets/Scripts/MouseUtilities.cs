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
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;
using System.Timers;

/**
 * Static class containing various utilities functions
 * */
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

        t.position = new Vector3(t.position.x, Camera.main.transform.position.y + /*t.localPosition.y*/ originalLocalHeightPos-0.2f, t.position.z); // The -0.2 is to be more aligned with the hololens.
    }

    // This function keeps its local position relative to the new parent. I.e. if the current object local position is (0,0,0), it will remain (0,0,0) for the new parent
    public static void setParentToObject(Transform o, Transform p)
    {
        Vector3 localPos = o.localPosition;

        o.parent = p;
        o.localPosition = localPos;
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