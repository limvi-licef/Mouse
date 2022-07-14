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

            protected Inference(string id, EventHandler callback)
            {
                Id = id;
                Callbacks += callback;
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
            public void TriggerCallback() => Callbacks?.Invoke(this, EventArgs.Empty);

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
    }

}
