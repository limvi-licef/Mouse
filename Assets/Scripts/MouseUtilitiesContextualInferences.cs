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

    //MouseUtilitiesMutex m_mutexDictionary;

    private void Awake()
    {
        m_inferences = new Dictionary<string, MouseUtilitiesInferenceAbstract>();
        //m_mutexDictionary = new MouseUtilitiesMutex();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       /* if (m_mutexDictionary.isLocked() == false)
        { // Running the loop only if the mutex is not locked
          // Evaluation of the inferences. If positive, then the callback is called
          */
            try
            {
                foreach (KeyValuePair<string, MouseUtilitiesInferenceAbstract> inference in m_inferences)
                {
            /*        if (m_mutexDictionary.isLocked())
                    { // Then stop the loop immediatly
                        break;
                    }*/

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
            
        //}
        
    }

    public void registerInference(MouseUtilitiesInferenceAbstract inference)
    {
        if (m_inferences.ContainsKey(inference.getId()) == false)
        {
            //m_mutexDictionary.lockMutex();
            m_inferences.Add(inference.getId(), inference);
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Inference " + inference.getId() + " added");
            //m_mutexDictionary.unlockMutex();
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
            //m_mutexDictionary.lockMutex();
            m_inferences.Remove(id);
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Inference " + id + " unregistered");
            //m_mutexDictionary.unlockMutex();
        }
        else
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "No inference registered - nothing to do");
        }
    }
}

public abstract class MouseUtilitiesInferenceAbstract
{
    string m_id;
    EventHandler m_callback;

    protected MouseUtilitiesInferenceAbstract(string id, EventHandler callback)
    {
        m_id = id;
        m_callback = callback;
    }

    public abstract bool evaluate();

    public string getId() => m_id;
    public void triggerCallback() => m_callback?.Invoke(this, EventArgs.Empty);
}

public class MouseUtilitiesInferenceDistanceFromObject : MouseUtilitiesInferenceAbstract
{
    GameObject m_gameObject;
    float m_minDistanceToTrigger;

    public MouseUtilitiesInferenceDistanceFromObject (string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger): base(id, callback)
    {
        m_gameObject = gameObject;
        m_minDistanceToTrigger = minDistanceToTrigger;
    }

    public override bool evaluate()
    {
        bool toReturn = false;

        float tempDistance = Vector3.Distance(Camera.main.transform.position, m_gameObject.transform.position);

        //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Distance: " + tempDistance);

        if (tempDistance > m_minDistanceToTrigger)
        {
            toReturn = true;
        }

        return toReturn;
    }
}