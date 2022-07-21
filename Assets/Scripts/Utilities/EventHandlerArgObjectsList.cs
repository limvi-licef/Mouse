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
using Newtonsoft.Json.Linq;

/**
 * Used to have a lighting object to point to a provided gameobject, from the same direction than the user's gaze
 * */
namespace MATCH
{
    namespace Utilities
    {
        public class EventHandlerArgObjectsList : EventArgs
        {
            public List<PhysicalObjectInformation> m_objectList;

            public EventHandlerArgObjectsList(List<PhysicalObjectInformation> listofobjects)
            {
                m_objectList = listofobjects;
            }

            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
        }

        public class EventHandlerArgObject : EventArgs
        {
            public PhysicalObjectInformation ObjectDetected;

            public EventHandlerArgObject(PhysicalObjectInformation obj)
            {
                ObjectDetected = obj;
            }

            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }



        }
    }
}
