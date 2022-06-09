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
    public static void mouseUtilitiesAddTouchCallback(Transform transform, UnityEngine.Events.UnityAction callback)
    {
        GameObject gameObject = transform.gameObject;

        Interactable interactable = gameObject.GetComponent<Interactable>();

        if (interactable == null)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No interactable component to the gameobject: adding one");

            interactable = gameObject.AddComponent<Interactable>();
        }

        InteractableOnTouchReceiver receiver = interactable.GetReceiver<InteractableOnTouchReceiver>();

        if (receiver == null)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No touch receiver to the interactable gameobject: adding one");

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
        return delegate
        {

        };
    }

    public static EventHandler getEventHandlerWithDebugMessage(string debugMessage)
    {
        return new EventHandler(delegate (System.Object o, EventArgs e)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, debugMessage);
        });
    }

    public static void adjustObjectHeightToHeadHeight(Transform t, float originalLocalHeightPos=0.0f)
    {
        t.position = new Vector3(t.position.x, Camera.main.transform.position.y + originalLocalHeightPos-0.2f, t.position.z); // The -0.2 is to be more aligned with the hololens.
    }

    // This function keeps its local position relative to the new parent. I.e. if the current object local position is (0,0,0), it will remain (0,0,0) for the new parent
    public static void setParentToObject(Transform o, Transform p)
    {
        Vector3 localPos = o.localPosition;

        //o.parent = p;
        o.SetParent(p);
        o.localPosition = localPos;
    }

    /*
     * Add the animate component to the object, animate the object, and then destroy the component.
     * */
    public static void animateDisappearInPlace(GameObject gameObject, Vector3 scalingOriginal, EventHandler eventHandler)
    {
        gameObject.AddComponent<MouseUtilitiesAnimation>().animateDiseappearInPlace(new EventHandler(delegate (System.Object o, EventArgs e)
        {
            UnityEngine.Object.Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

            gameObject.SetActive(false);
            gameObject.transform.localScale = scalingOriginal;

            eventHandler?.Invoke(gameObject, EventArgs.Empty);
        }));
    }
    public static void animateDisappearInPlace(GameObject gameObject, Vector3 scalingOriginal)
    {
        animateDisappearInPlace(gameObject, scalingOriginal, getEventHandlerEmpty());
    }

    public static void animateAppearInPlace(GameObject gameObject, EventHandler eventHandler)
    {
        gameObject.SetActive(true);
        gameObject.AddComponent<MouseUtilitiesAnimation>().animateAppearInPlace(new EventHandler(delegate (System.Object o, EventArgs e)
        {
            UnityEngine.Object.Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

            eventHandler?.Invoke(gameObject, EventArgs.Empty);
        }));
    }
    public static void animateAppearInPlace(GameObject gameObject)
    {
        animateAppearInPlace(gameObject, getEventHandlerEmpty());
    }

   public static void animateAppearInPlace(GameObject gameObject, Vector3 scaling, EventHandler eventHandler)
    {
        gameObject.SetActive(true);

        MouseUtilitiesAnimation animator = gameObject.AddComponent<MouseUtilitiesAnimation>();

        animator.m_scalingstep.x = scaling.x / 50.0f;
        animator.m_scalingstep.y = scaling.y / 50.0f;
        animator.m_scalingstep.z = scaling.z / 50.0f;

        animator.animateAppearInPlaceToScaling(scaling, delegate 
        {
            UnityEngine.Object.Destroy(gameObject.GetComponent<MouseUtilitiesAnimation>());

            eventHandler?.Invoke(gameObject, EventArgs.Empty);
        });
    }
    public static void animateAppearInPlace(GameObject gameObject, Vector3 scaling)
    {
        animateAppearInPlace(gameObject, scaling, getEventHandlerEmpty());
    }

    public static void bringObject(Transform t)
    {
        t.position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z);
        t.LookAt(Camera.main.transform);
        t.Rotate(new Vector3(0, 1, 0), 180);
    }

    public static void showInteractionSurface(Transform gameobject, bool show)
    {
        gameobject.GetComponent<Renderer>().enabled = show; // To hide the surface while keeping it interactable, then the renderer is disabled if show==false;
        gameobject.GetComponent<BoundsControl>().enabled = show;
        gameobject.GetComponent<ObjectManipulator>().enabled = show;
    }

    public static void setColor(Transform gameobject, string colorName)
    {
        Renderer r = gameobject.GetComponent<Renderer>();
        r.material = Resources.Load(colorName, typeof(Material)) as Material;
    }

    /**
     * Convert a BitArray to int.
     * Be careful: the least significant bit is the first element of the array
     * */
    public static int convertBitArrayToInt(BitArray array)
    {
        int[] arrayInt = new int[1];
        array.CopyTo(arrayInt, 0);

        return arrayInt[0];
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