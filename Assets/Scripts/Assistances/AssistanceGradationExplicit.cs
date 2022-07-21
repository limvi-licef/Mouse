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
        public class AssistanceGradationExplicit : MonoBehaviour
        {
            private NPBehave.Root Tree;
            private NPBehave.Blackboard Conditions;

            private Inferences.FocusedOnObject InferencePersonFocusedOnAssistance;
            Inferences.Manager InferencesManager;

            public enum AssistanceStatus
            {
                Stop = 0,
                Run = 1,
                Pause = 2
            }

            private List<AssistanceGradationAttention> AssistancesGradation;
            int CurrentAssistanceIndex; // Index in the list AssistancesGradation

            AssistanceStatus Status;

            private void Start()
            {
                Status = AssistanceStatus.Stop;
                CurrentAssistanceIndex = -1; // Means the process did not started yet
                
                InferencePersonFocusedOnAssistance = null; // Use InitializeInfereneFocusedAssistance to initialize it
                InferencesManager = new Inferences.Manager();

                // Initialize the BT
                Conditions = new NPBehave.Blackboard(UnityContext.GetClock());
                Conditions["AssistanceDisplayed"] = false;
                Conditions["AssistanceFocusedByPerson"] = false;
                Conditions["HelpButtonDisplayed"] = false;
                Conditions["HelpButtonClicked"] = false;

                Sequence sButtonClicked = new Sequence(new NPBehave.Action(() => IncreaseGradation(Utilities.Utility.GetEventHandlerEmpty())),
                    new NPBehave.WaitUntilStopped());

                BlackboardCondition cButtonClicked = new BlackboardCondition("HelpButtonClicked", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, sButtonClicked);

                BlackboardCondition cHelpButtonDisplayed = new BlackboardCondition("HelpButtonDisplayed", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, cButtonClicked);

                Selector srHelpButtonDisplayed = new Selector(
                    cHelpButtonDisplayed,
                    new NPBehave.Action(() => DisplayHelpButtons()));

                Sequence seAssistanceFocusedByPerson = new Sequence(
                    new NPBehave.Action(() => AssistancesGradation[CurrentAssistanceIndex].ShowMinimalGradation(Utilities.Utility.GetEventHandlerEmpty())),
                    new NPBehave.Action(() => Debug.Log("Here should be displayed a symbol to inform the person the system understood he looked at the assistance")),
                    srHelpButtonDisplayed);

                BlackboardCondition cAssistanceFocusedByPerson = new BlackboardCondition("AssistanceFocusedByPerson", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, seAssistanceFocusedByPerson);

                Selector srAssistanceDisplayed = new Selector(
                    cAssistanceFocusedByPerson,
                    new NPBehave.Action(()=>IncreaseAttentionGrabbingGradation()));

                BlackboardCondition cAssistanceDisplayed = new BlackboardCondition("AssistanceDisplayed", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, srAssistanceDisplayed);

                Selector srBegin = new Selector(
                    cAssistanceDisplayed,
                    new NPBehave.Action(() => ShowFirstAssistance()));

                Tree = new Root(Conditions, srBegin);
            }

            private void ShowFirstAssistance()
            {
                CurrentAssistanceIndex = 0;
                AssistancesGradation[CurrentAssistanceIndex].ShowMinimalGradation(Utilities.Utility.GetEventHandlerEmpty());
                Conditions["AssistanceDisplayed"] = true;
            }

            /**
             * Returns true if gradation have been increased, false if the maximum level has been reached
             * */
            private bool IncreaseGradation(EventHandler callback)
            {
                bool toReturn = false;

                if (CurrentAssistanceIndex != -1 && CurrentAssistanceIndex+1 < AssistancesGradation.Count)
                {
                    AssistancesGradation[CurrentAssistanceIndex].HideCurrentGradation(delegate(System.Object o, EventArgs e)
                    {
                        AssistancesGradation[++CurrentAssistanceIndex].ShowMinimalGradation(callback);
                    });

                    toReturn = true;
                }

                return toReturn;
            }

            /**
             * Is done for the current assistance
             * */
            private void IncreaseAttentionGrabbingGradation()
            {
                AssistancesGradation[CurrentAssistanceIndex].ShowNextGradation(Utilities.Utility.GetEventHandlerEmpty());

                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Next gradation for attention grabbing shown");
            }

            /**
             * The order you add the assistances is important: it determines their order of appearance
             * */
            public void AddAssistance(AssistanceGradationAttention assistance)
            {
                AssistancesGradation.Add(assistance);
            }

            public void DisplayHelpButtons()
            {
                AssistancesGradation[CurrentAssistanceIndex].ShowHelpCurrentGradation(true);
            }

            /**
             * This function start the assistance from scratch, i.e. it will not remuse if it has been interrupted. If you want to resume the assistance after it has been interrupted, please use the ResumeAssistance function
             * The callback is called when the process is finished (finished, i.e. not interrupted)
             * */
            public void RunAssistance(EventHandler callback)
            {
                Status = AssistanceStatus.Run;

                // Initialize the BT
                Tree.Start();
            }

            public void PauseAssistance()
            {
                Status = AssistanceStatus.Pause;
            }

            public void StopAssistance()
            {
                Status = AssistanceStatus.Stop;
            }

            public AssistanceStatus GetAssistanceStatus()
            {
                return Status;
            }

            private void InitializeInfereneFocusedAssistance(int indexAssistanceToMonitor)
            {
                if (InferencePersonFocusedOnAssistance != null)
                {
                    InferencesManager.UnregisterInference(InferencePersonFocusedOnAssistance);
                    InferencePersonFocusedOnAssistance = null;
                }

                InferencePersonFocusedOnAssistance = new Inferences.FocusedOnObject("AssistanceFocus", CallbackAssistanceFocused, AssistancesGradation[indexAssistanceToMonitor].GetCurrentAssistance().GetTransform().gameObject, 3);
            }

            private void CallbackAssistanceFocused(System.Object o, EventArgs e)
            {
                Conditions["AssistanceFocusedByPerson"] = true;
            }
        }

    }
}
