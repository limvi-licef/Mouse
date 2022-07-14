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

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

/**
 * Inherits MonoBehaviour because we want the inferences to be evaluated at each frame
 * Inferences are evaluated each frame: your responsibility to manage the triggers, and to unregister when it's ok for you.
 * Based more or less on the observer pattern.
 * */
public class MouseUtilitiesContextualInferences : MonoBehaviour
{
    Dictionary<string, MouseUtilitiesInferenceAbstract> m_inferences;

    private void Awake()
    {
        m_inferences = new Dictionary<string, MouseUtilitiesInferenceAbstract>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            try
            {
                foreach (KeyValuePair<string, MouseUtilitiesInferenceAbstract> inference in m_inferences)
                {
                    // Mettre tout ça dans un thread? Pour pas que ce soit bloquant?
                    if (inference.Value.evaluate())
                    {
                        inference.Value.triggerCallback();
                    }
                }
            }
            catch(InvalidOperationException)
            {
                MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Inference dictionary has changed, no update performed this frame");
            }        
    }

    public void registerInference(MouseUtilitiesInferenceAbstract inference)
    {
        if (m_inferences.ContainsKey(inference.getId()) == false)
        {
            m_inferences.Add(inference.getId(), inference);
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Inference " + inference.getId() + " added");
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Inference already registered - nothing to do");
        }
    }

    public void unregisterInference(MouseUtilitiesInferenceAbstract inference)
    {
        unregisterInference(inference.getId());
    }

    public void unregisterInference(string id)
    {
        if (m_inferences.ContainsKey(id))
        {
            m_inferences.Remove(id);
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Inference " + id + " unregistered");
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No inference registered - nothing to do");
        }
    }

    public MouseUtilitiesInferenceAbstract getInference(string id)
    {
        MouseUtilitiesInferenceAbstract toReturn = null;

        if (m_inferences.ContainsKey(id))
        {
            toReturn = m_inferences[id];
        }

        return toReturn;
    }
}

public abstract class MouseUtilitiesInferenceAbstract
{
    string m_id;
    EventHandler m_callback;
    protected EventArgs m_callbackArgs;

    protected MouseUtilitiesInferenceAbstract(string id, EventHandler callback)
    {
        m_id = id;
        m_callback = callback;
        m_callbackArgs = EventArgs.Empty;
    }

    public abstract bool evaluate();

    public string getId() => m_id;
    public void triggerCallback() => m_callback?.Invoke(this, m_callbackArgs);
}

public class MouseUtilitiesInferenceDistanceLeaving : MouseUtilitiesInferenceAbstract
{
    GameObject m_gameObject;
    float m_minDistanceToTrigger;

    public MouseUtilitiesInferenceDistanceLeaving (string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger): base(id, callback)
    {
        m_gameObject = gameObject;
        m_minDistanceToTrigger = minDistanceToTrigger;
    }

    public override bool evaluate()
    {
        bool toReturn = false;

        float tempDistance = Vector3.Distance(Camera.main.transform.position, m_gameObject.transform.position);

        if (tempDistance > m_minDistanceToTrigger)
        {
            toReturn = true;
        }

        return toReturn;
    }
}

public class MouseUtilitiesInferenceDistanceComing : MouseUtilitiesInferenceAbstract
{
    GameObject m_gameObject;
    float m_minDistanceToTrigger;

    public MouseUtilitiesInferenceDistanceComing(string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger) : base(id, callback)
    {
        m_gameObject = gameObject;
        m_minDistanceToTrigger = minDistanceToTrigger;
    }

    public override bool evaluate()
    {
        bool toReturn = false;

        float tempDistance = Vector3.Distance(Camera.main.transform.position, m_gameObject.transform.position);

        if (tempDistance < m_minDistanceToTrigger)
        {
            toReturn = true;
        }

        return toReturn;
    }
}

public class MouseUtilitiesInferenceTime : MouseUtilitiesInferenceAbstract
{
    DateTime m_time;
    bool m_useOneMinuteTrigger;
    DateTime m_timeOneMinuteTrigger;

    public MouseUtilitiesInferenceTime(string id, DateTime time, EventHandler callback): base(id, callback)
    {
        m_time = time;

        m_useOneMinuteTrigger = false;

        MouseUtilitiesAdminMenu.Instance.addButton("One minute trigger for " + id, callbackOneMinuteTrigger);

    }

    public override bool evaluate()
    {
        bool toReturn = false;

        if (m_useOneMinuteTrigger == false)
        {
            if (DateTime.Now.Hour == m_time.Hour && DateTime.Now.Minute == m_time.Minute)
            {
                toReturn = true;
            }
        }
        else
        {
            TimeSpan elapsed = DateTime.Now.Subtract(m_timeOneMinuteTrigger);

            if ( /*elapsed.Minutes >= 1*/ elapsed.Seconds >= 10)
            {
                m_useOneMinuteTrigger = false;
                toReturn = true;
            }
        }
        

        return toReturn;
    }

    public void callbackOneMinuteTrigger()
    {
        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Callback one minute triggered called");

        m_timeOneMinuteTrigger = DateTime.Now;
        m_useOneMinuteTrigger = true;
    }
}

public class MouseUtilitiesInferenceObjectInInteractionSurface : MouseUtilitiesInferenceAbstract
{
    MouseInteractionSurface m_surface;
    MousePhysicalObjectInformation m_objectdetected;
    BoxCollider m_Collider;

    string lastObject;

    public MouseUtilitiesInferenceObjectInInteractionSurface(string id, EventHandler callback, string objectName, MouseInteractionSurface surface) : base(id, callback)
    {
        m_surface = surface;
        
        //m_objectdetected = null;
        lastObject = null;
        m_objectdetected = null;
        //m_objectdetected.setObjectParams("TEst", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)); //FOR TEST

        MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Inference Object Launched");
        MouseUtilitiesObjectInformation.Instance.unregisterCallbackToObject(objectName);
        MouseUtilitiesObjectInformation.Instance.registerCallbackToObject(objectName, callbackObjectDetection);
    }

    //In the evaluate, the last object is saved for avoid spam when an object is in a surface. The return true is sent just one time (the first time that the object is detected in the surface).
    public override bool evaluate()
    {
        bool toReturn = false;
        m_Collider = m_surface.getInteractionSurface().gameObject.GetComponent<BoxCollider>();
        //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Evaluate ok");
        if (m_objectdetected != null) 
        {
            //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Object name ok");
            if (m_Collider.bounds.Contains(m_objectdetected.getCenter())) //check if the center of the object is in the surface area
            {
                //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "In collider");
                if (m_objectdetected.getObjectName() != lastObject) //last object for avoid spam when an object is in a surface
                {
                    toReturn = true;
                    lastObject = m_objectdetected.getObjectName();
                    //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Finally, object detected : " + m_objectdetected.getObjectName() + " with the center " + m_objectdetected.getCenter() + ". Center of storage : " + m_Collider.bounds.center);
                }
            }
            else // the object isn't in the surface
            {
                lastObject = null;
            }
        }
        return toReturn;
    }

    public void callbackObjectDetection(System.Object o, EventArgs e)
    {
        MouseEventHandlerArgObject objectInfo = (MouseEventHandlerArgObject)e;
        
        m_objectdetected = new MousePhysicalObjectInformation();
        m_objectdetected = objectInfo.m_object;
        
    }
}

public class MouseUtilitiesInferenceObjectOutInteractionSurface : MouseUtilitiesInferenceAbstract
{
    MouseInteractionSurface m_surface;
    MousePhysicalObjectInformation m_objectdetected;
    BoxCollider m_Collider;

    string lastObject;

    public MouseUtilitiesInferenceObjectOutInteractionSurface(string id, EventHandler callback, string objectName, MouseInteractionSurface surface) : base(id, callback)
    {
        m_surface = surface;
        lastObject = null;
        m_objectdetected = null;
        MouseUtilitiesObjectInformation.Instance.unregisterCallbackToObject(objectName);
        MouseUtilitiesObjectInformation.Instance.registerCallbackToObject(objectName, callbackObjectDetection);
    }

    public override bool evaluate()
    {
        bool toReturn = false;
        m_Collider = m_surface.getInteractionSurface().gameObject.GetComponent<BoxCollider>();

        if (m_objectdetected != null)
        {
            if (!m_Collider.bounds.Contains(m_objectdetected.getCenter()))
            {
                if (m_objectdetected.getObjectName() != lastObject) //last object for avoid spam when an object is out of a surface
                {
                    toReturn = true;
                    lastObject = m_objectdetected.getObjectName();
                }
            }
            else // the object is in the surface
            {
                lastObject = null;
            }
        }
        return toReturn;
    }

    public void callbackObjectDetection(System.Object o, EventArgs e)
    {
        MouseEventHandlerArgObject objectInfo = (MouseEventHandlerArgObject)e;
        //m_objectdetected = new MousePhysicalObjectInformation();
        m_objectdetected = objectInfo.m_object;
        m_callbackArgs = new MouseEventHandlerArgObject(m_objectdetected);
    }
}

/*
public class MouseUtilitiesInferenceSetInteractionSurfaceToObject : MouseUtilitiesInferenceAbstract
{
    MouseInteractionSurface m_surface;
    MousePhysicalObjectInformation m_objectdetected;

    public MouseUtilitiesInferenceSetInteractionSurfaceToObject(string id, EventHandler callback, string objectName, MouseInteractionSurface surface) : base(id, callback)
    {
        m_surface = surface;
        m_objectdetected = new MousePhysicalObjectInformation();
        MouseUtilitiesObjectInformation.Instance.unregisterCallbackObjectPosition(objectName);
        MouseUtilitiesObjectInformation.Instance.registerCallbackObjectPosition(objectName, callbackObjectDetection);
    }

    public override bool evaluate()
    {
        bool toReturn = false;

        if (m_objectdetected.getObjectName() != null)
        {
            m_surface.transform.localPosition = m_objectdetected.getCenter();
            toReturn = true;
        }
        return toReturn;
    }

    public void callbackObjectDetection(System.Object o, EventArgs e)
    {
        MouseEventHandlerArgObject objectInfo = (MouseEventHandlerArgObject)e;
        m_objectdetected = new MousePhysicalObjectInformation();
        m_objectdetected = objectInfo.m_object;
    }
}

*/



/*

public class MouseUtilitiesInferenceNearObject : MouseUtilitiesInferenceAbstract
{
    MousePhysicalObjectInformation m_objectdetected;
    float m_minDistanceToTrigger; //distance with camera

    public MouseUtilitiesInferenceNearObject(string id, EventHandler callback, string objectName, float minDistanceToTrigger) : base(id, callback)
    {
        m_minDistanceToTrigger = minDistanceToTrigger;
        //register objectname with position in the dictionnary
    }

    public override bool evaluate()
    {
        bool toReturn = false;

        float tempDistance = Vector3.Distance(Camera.main.transform.position, m_objectdetected.getCenter()); //distance object/camera

        if (tempDistance < m_minDistanceToTrigger)
        {
            toReturn = true;
        }

        return toReturn;
    }

}

/*
public class MouseUtilitiesInferenceDistanceComing : MouseUtilitiesInferenceAbstract
{
    GameObject m_gameObject;
    float m_minDistanceToTrigger;

    public MouseUtilitiesInferenceDistanceComing(string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger) : base(id, callback)
    {
        m_gameObject = gameObject;
        m_minDistanceToTrigger = minDistanceToTrigger;
    }

    public override bool evaluate()
    {
        bool toReturn = false;

        float tempDistance = Vector3.Distance(Camera.main.transform.position, m_gameObject.transform.position);

        if (tempDistance < m_minDistanceToTrigger)
        {
            toReturn = true;
        }

        return toReturn;
    }
}
*/