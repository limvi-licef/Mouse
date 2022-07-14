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
namespace MATCH
{
    namespace Inferences
    {
        public class Manager : MonoBehaviour
        {
            Dictionary<string, Inference> m_inferences;

            private void Awake()
            {
                m_inferences = new Dictionary<string, Inference>();
            }

            // Start is called before the first frame update
            /*void Start()
            {

            }*/

            // Update is called once per frame
            void Update()
            {
                try
                {
                    foreach (KeyValuePair<string, Inference> inference in m_inferences)
                    {
                        // Mettre tout ça dans un thread? Pour pas que ce soit bloquant?
                        if (inference.Value.Evaluate())
                        {
                            inference.Value.TriggerCallback();
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    DebugMessagesManager.Instance.displayMessage("Manager", "Update", DebugMessagesManager.MessageLevel.Warning, "Inference dictionary has changed, no update performed this frame"); // Class and method names are hard coded for performance reasons.
                }
            }

            public void RegisterInference(Inference inference)
            {
                if (m_inferences.ContainsKey(inference.Id) == false)
                {
                    m_inferences.Add(inference.Id, inference);
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Inference " + inference.Id + " added");
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "Inference already registered - nothing to do");
                }
            }

            public void UnregisterInference(Inference inference)
            {
                UnregisterInference(inference.Id);
            }

            public void UnregisterInference(string id)
            {
                if (m_inferences.ContainsKey(id))
                {
                    m_inferences.Remove(id);
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Inference " + id + " unregistered");
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "No inference registered - nothing to do");
                }
            }

            public Inference GetInference(string id)
            {
                Inference toReturn = null;

                if (m_inferences.ContainsKey(id))
                {
                    toReturn = m_inferences[id];
                }

                return toReturn;
            }
        }

        public abstract class Inference
        {
            public string Id { get; private set; }
            public event EventHandler Callbacks;
			protected EventArgs m_callbackArgs;

            protected Inference(string id, EventHandler callback)
            {
                Id = id;
                Callbacks += callback;
				m_callbackArgs = EventArgs.Empty;
            }

            protected Inference(string id)
            {
                Id = id;
            }

            public void AddCallback(EventHandler callback)
            {
                Callbacks += callback;
            }

            public abstract bool Evaluate();

            //public string getId() => m_id;
            public void TriggerCallback() => Callbacks?.Invoke(this, m_callbackArgs);

            ~Inference()
            {
                for (int i = 0; i < Callbacks.GetInvocationList().Length; i ++)
                {
                    Callbacks -= (EventHandler) Callbacks.GetInvocationList()[i];
                }
            }
        }

        public class DistanceLeaving : Inference
        {
            readonly GameObject m_gameObject;
            readonly float m_minDistanceToTrigger;

            public DistanceLeaving(string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger) : base(id, callback)
            {
                m_gameObject = gameObject;
                m_minDistanceToTrigger = minDistanceToTrigger;
            }

            public override bool Evaluate()
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

        public class DistanceComing : Inference
        {
            readonly GameObject m_gameObject;
            readonly float m_minDistanceToTrigger;

            public DistanceComing(string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger) : base(id, callback)
            {
                m_gameObject = gameObject;
                m_minDistanceToTrigger = minDistanceToTrigger;
            }

            public override bool Evaluate()
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

        public class Time : Inference
        {
            readonly DateTime m_time;
            bool m_useOneMinuteTrigger;
            DateTime m_timeOneMinuteTrigger;

            public Time(string id, DateTime time, EventHandler callback) : base(id, callback)
            {
                m_time = time;

                m_useOneMinuteTrigger = false;

                AdminMenu.Instance.addButton("One minute trigger for " + id, CallbackOneMinuteTrigger);

            }

            public Time(string id, DateTime time):base(id)
            {
                m_time = time;

                AdminMenu.Instance.addButton("One minute trigger for " + id, CallbackOneMinuteTrigger);
            }

            public override bool Evaluate()
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
                        //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");

                        m_useOneMinuteTrigger = false;
                        toReturn = true;
                    }
                }


                return toReturn;
            }

            public void CallbackOneMinuteTrigger()
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Callback one minute triggered called");

                m_timeOneMinuteTrigger = DateTime.Now;
                m_useOneMinuteTrigger = true;
            }
        }


        public class MouseUtilitiesInferenceObjectInInteractionSurface : Inference
        {
            Assistances.InteractionSurface m_surface;
            Utilities.PhysicalObjectInformation m_objectdetected;
            BoxCollider m_Collider;

            string lastObject;

            public MouseUtilitiesInferenceObjectInInteractionSurface(string id, EventHandler callback, string objectName, Assistances.InteractionSurface surface) : base(id, callback)
            {
                m_surface = surface;

                //m_objectdetected = null;
                lastObject = null;
                m_objectdetected = null;
                //m_objectdetected.setObjectParams("TEst", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0)); //FOR TEST

                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Inference Object Launched");
                ObjectRecognition.ObjectInformation.Instance.unregisterCallbackToObject(objectName);
                ObjectRecognition.ObjectInformation.Instance.registerCallbackToObject(objectName, callbackObjectDetection);
            }

            //In the evaluate, the last object is saved for avoid spam when an object is in a surface. The return true is sent just one time (the first time that the object is detected in the surface).
            public override bool Evaluate()
            {
                bool toReturn = false;
                m_Collider = m_surface.GetInteractionSurface().gameObject.GetComponent<BoxCollider>();
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
                Utilities.EventHandlerArgObject objectInfo = (Utilities.EventHandlerArgObject)e;

                m_objectdetected = new Utilities.PhysicalObjectInformation();
                m_objectdetected = objectInfo.m_object;

            }
        }


public class MouseUtilitiesInferenceObjectOutInteractionSurface : Inferences.Inference
{
    Assistances.InteractionSurface m_surface;
    Utilities.PhysicalObjectInformation m_objectdetected;
    BoxCollider m_Collider;

    string lastObject;

    public MouseUtilitiesInferenceObjectOutInteractionSurface(string id, EventHandler callback, string objectName, Assistances.InteractionSurface surface) : base(id, callback)
    {
        m_surface = surface;
        lastObject = null;
        m_objectdetected = null;
        ObjectRecognition.ObjectInformation.Instance.unregisterCallbackToObject(objectName);
        ObjectRecognition.ObjectInformation.Instance.registerCallbackToObject(objectName, callbackObjectDetection);
    }

    public override bool Evaluate()
    {
        bool toReturn = false;
        m_Collider = m_surface.GetInteractionSurface().gameObject.GetComponent<BoxCollider>();

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
        Utilities.EventHandlerArgObject objectInfo = (Utilities.EventHandlerArgObject)e;
        m_objectdetected = objectInfo.m_object;
        m_callbackArgs = new Utilities.EventHandlerArgObject(m_objectdetected);
    }
}

	
}
    }