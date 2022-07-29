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
using NPBehave;
using System;
using System.Reflection;

namespace MATCH
{
    namespace Assistances
    {
        public class QandDAssistances
        {
            List<IAssistance> AssistancesToDisplay;

            // Start is called before the first frame update
            public QandDAssistances() { 
                AssistancesToDisplay = new List<IAssistance>();
            }

            void AddAssistance(IAssistance assistance)
            {
                AssistancesToDisplay.Add(assistance);
            }

            void ShowOneHideOthers(int index, EventHandler callback)
            {
                for (int i = 0; i < AssistancesToDisplay.Count; i ++)
                {
                    if (i == index)
                    {
                        AssistancesToDisplay[i].Show(callback);
                    }
                    else
                    {
                        AssistancesToDisplay[i].Hide(Utilities.Utility.GetEventHandlerEmpty());
                    }
                }
            }
        }
    }
}