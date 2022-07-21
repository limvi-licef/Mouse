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

namespace MATCH
{
    namespace Inferences
    {
        public class DistanceComing : Inference
        {
            readonly GameObject ObjectToMonitor;
            readonly float MinDistanceToTrigger;

            public DistanceComing(string id, EventHandler callback, GameObject gameObject, float minDistanceToTrigger) : base(id, callback)
            {
                ObjectToMonitor = gameObject;
                MinDistanceToTrigger = minDistanceToTrigger;
            }

            public override bool Evaluate()
            {
                bool toReturn = false;

                float tempDistance = Vector3.Distance(Camera.main.transform.position, ObjectToMonitor.transform.position);

                if (tempDistance < MinDistanceToTrigger)
                {
                    toReturn = true;
                }

                return toReturn;
            }
        }
    }
}