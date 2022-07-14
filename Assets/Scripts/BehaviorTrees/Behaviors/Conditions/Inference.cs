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
using System;
using WUG.BehaviorTreeVisualizer;
using System.Reflection;

namespace MATCH
{
    namespace BehaviorTrees
    {
        namespace Conditions
        {
            public class Inference : Condition
            {
                NodeStatus m_toReturn;
                readonly string m_message;

                public Inference(string text, MATCH.Inferences.Inference inference) : base(text)
                {
                    m_message = text;
                    //m_toReturn = toReturn;
                    m_toReturn = NodeStatus.Failure;

                    inference.AddCallback(CallbackInferenceEvaluated);
                }

                protected void CallbackInferenceEvaluated(System.Object o, EventArgs e)
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");

                    m_toReturn = NodeStatus.Success;
                }

                protected override void OnReset()
                {

                }

                protected override NodeStatus OnRun()
                {
                    //Debug.Log("Condition " + m_message + " condition evaluated. Result: " + m_toReturn);

                    return m_toReturn;
                }
            }
        }
    }
    }
