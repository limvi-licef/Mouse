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

public class MouseUtilitiesContextualInferencesFactory : MonoBehaviour
{
        private static MouseUtilitiesContextualInferencesFactory m_instance;
        public static MouseUtilitiesContextualInferencesFactory Instance { get { return m_instance; } }

        public MouseAssistanceDialog m_refDialogAssistance;

        private void Awake()
        {
            if (m_instance != null && m_instance != this)
            {
                Destroy(this);
            }
            else
            {
                m_instance = this;
            }
        }

        public void createDistanceLeavingInferenceOneShot(MouseUtilitiesContextualInferences inferenceManager, string inferenceId, EventHandler toTrigger, GameObject refObject, float trigerringDistance = 2.0f)
        {
            MouseUtilitiesInferenceDistanceLeaving inference = new MouseUtilitiesInferenceDistanceLeaving(inferenceId, delegate (System.Object o, EventArgs e)
            {
                inferenceManager.unregisterInference(inferenceId);
                toTrigger?.Invoke(o, e);
            }, refObject, trigerringDistance);
            inferenceManager.registerInference(inference);
        }

        public void createDistanceComingInferenceOneShot(MouseUtilitiesContextualInferences inferenceManager, string inferenceId, EventHandler toTrigger, GameObject refObject, float trigerringDistance = 1.5f)
        {
            MouseUtilitiesInferenceDistanceComing inference = new MouseUtilitiesInferenceDistanceComing(inferenceId, delegate (System.Object o, EventArgs e)
            {
                inferenceManager.unregisterInference(inferenceId);
                toTrigger?.Invoke(o, e);
            }, refObject, trigerringDistance);
            inferenceManager.registerInference(inference);
        }

        /**
         * This inference creates 2 nested inferences: the first is trigerred when the user comes close to the object. It DOES NOT trigger the provided EventHandler yet. Instead, it creates a new inference trigerred if the user leave the place where the object is displayed. And here the EventHandler is triggered.
         * The reason to implement those inferences this way is that in case we have several assistances in a row that can trigger if the user is at a certain distance, then they will all trigger at once. With this way of doing, the next inference will be triggered only if the user first come closer and then leaves again
         * */
        public void createDistanceComingAndLeavingInferenceOneShot(MouseUtilitiesContextualInferences inferenceManager, string inferenceId, EventHandler toTrigger, GameObject refObject, float trigerringDistanceComing = 1.5f, float trigerringDistanceLeaving = 2.0f)
        {
            createDistanceComingInferenceOneShot(inferenceManager, inferenceId, delegate (System.Object o, EventArgs e)
            {
                createDistanceLeavingInferenceOneShot(inferenceManager, inferenceId + "Internal", toTrigger, refObject, trigerringDistanceLeaving);
            }, refObject, trigerringDistanceComing);
        }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
