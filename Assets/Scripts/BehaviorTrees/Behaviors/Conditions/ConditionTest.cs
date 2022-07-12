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
using WUG.BehaviorTreeVisualizer;

namespace MATCH
{
    public class ConditionTest : Condition
    {
        readonly NodeStatus m_toReturn;
        readonly string m_message;

        public ConditionTest(string text, NodeStatus toReturn) : base(text)
        {
            m_message = text;
            m_toReturn = toReturn;
        }


        protected override void OnReset()
        {
        
        }

        protected override NodeStatus OnRun()
        {
            Debug.Log("Condition " + m_message + " test called");

            return m_toReturn;
        }
    }
}