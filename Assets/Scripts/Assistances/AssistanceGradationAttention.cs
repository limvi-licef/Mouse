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
        public class AssistanceGradationAttention
        {
            List<IAssistance> Gradation;
            int GradationCurrent = -1;

            public AssistanceGradationAttention()
            {
                Gradation = new List<IAssistance>();
            }

            public IAssistance AddAssistance(IAssistance assistance)
            {
                Gradation.Add(assistance);
                if (GradationCurrent == -1)
                {
                    GradationCurrent = 0;
                }

                return assistance;
            }

            /**
             * Returns True if there is a new level of gradation shown, False if the current displayed level of gradation is the last one
             * */
            public bool ShowNextGradation(EventHandler callback)
            {
                bool toReturn = false;

                if (GradationCurrent > -1 && ++GradationCurrent < Gradation.Count)
                {
                    //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Next gradation is going to be shown");
                    Gradation[GradationCurrent].Show(callback);
                    toReturn = true;
                }
                else
                {
                    GradationCurrent--;
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "Maximum level of attention gradation reached");
                }

                return toReturn;
            }

            /**
             * Show the minimal level of gradation. I.e. even if a higher level of gradation is displayed, will go back to the minimal level
             * */
            public void ShowMinimalGradation (EventHandler callback)
            {
                if (GradationCurrent > -1)
                {
                    GradationCurrent = 0;
                    Gradation[GradationCurrent].Show(callback);
                    //DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Assistance should be shown now");
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "No assistances to display. Please add assistances first.");
                }
            }

            public void HideCurrentGradation(EventHandler callback)
            {
                if (GradationCurrent > -1)
                {
                    GradationCurrent = 0;
                    Gradation[GradationCurrent].Hide(callback);
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "No assistances to hide. Please add assistances first.");
                }
            }

            public void ShowHelpCurrentGradation(bool show)
            {
                if (GradationCurrent > -1)
                {
                    GradationCurrent = 0;
                    Gradation[GradationCurrent].ShowHelp(show);
                }
                else
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "No help to hide. Please add assistances first.");
                }
            }

            /**
             * Returns null if no assistances have been added yet
             * */
            public IAssistance GetCurrentAssistance()
            {
                IAssistance toReturn = null;

                if (GradationCurrent > -1)
                {
                    toReturn = Gradation[GradationCurrent];
                }

                return toReturn;
            }
        }
    }
}

