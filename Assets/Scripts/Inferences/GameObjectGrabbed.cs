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
        public class GameObjectGrabbed : Inference
        {
            GameObject ObjectToGrab;
            bool ObjectGrabbed;

            public GameObjectGrabbed(string id, EventHandler callback, GameObject objectToGrab): base(id, callback)
            {
                ObjectToGrab = objectToGrab;
                ObjectGrabbed = false;

                ObjectManipulator objectManipulator = objectToGrab.GetComponent<ObjectManipulator>();
                objectManipulator.OnManipulationStarted.AddListener(CallbackOnGrabbedStarted);
                objectManipulator.OnManipulationEnded.AddListener(CallbackOnGrabbedEnded);
            }

            public override bool Evaluate()
            {
                return ObjectGrabbed;
            }

            void CallbackOnGrabbedStarted(ManipulationEventData eventData)
            {
                //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called grabbed started");

                ObjectGrabbed = true;
                //eventData.
            }

            void CallbackOnGrabbedEnded(ManipulationEventData eventData)
            {
                //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called grabbed ending");

                ObjectGrabbed = false;
            }
        }

    }
}
