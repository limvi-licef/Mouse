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
        public class FocusedOnObject : Inference
        {
            GameObject ObjectToFocus;
            Utilities.HologramInteractions InteractionComponent;
            int SecondsToFocus = 3;

            DateTime StartTime;
            bool FocusOn;

            public FocusedOnObject(string id, EventHandler callback, GameObject objectToFocus, int secondsToFocus): base(id, callback)
            {
                FocusOn = false;

                ObjectToFocus = objectToFocus;

                if (ObjectToFocus.TryGetComponent<Utilities.HologramInteractions>(out InteractionComponent) == false)
                {
                     InteractionComponent = ObjectToFocus.AddComponent<Utilities.HologramInteractions>();
                }

                SecondsToFocus = secondsToFocus;

                InteractionComponent.EventFocusOn += CallbackFocusOn;
                InteractionComponent.EventFocusOff += CallbackFocusOff;
            }

            public override bool Evaluate()
            {
                bool toReturn = false;

                if (FocusOn)
                {
                    TimeSpan elapsed = DateTime.Now.Subtract(StartTime);
                    if (elapsed.Seconds >= SecondsToFocus)
                    {
                        toReturn = true;
                    }
                }

                return toReturn;
            }

            void CallbackFocusOn(System.Object o, EventArgs e)
            {
                FocusOn = true;
                StartTime = DateTime.Now;
            }

            void CallbackFocusOff(System.Object o, EventArgs e)
            {
                FocusOn = false;
            }
        }
    }
}