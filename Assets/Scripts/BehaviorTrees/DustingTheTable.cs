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

using UnityEngine;
using UnityEngine.AI;
using WUG.BehaviorTreeVisualizer;
using System.Collections;
using System;

namespace MATCH
{
    namespace BehaviorTrees
    {
        public class DustingTheTable : MonoBehaviour, IBehaviorTree
        {
            public Inferences.Manager InferenceManager;

            private Coroutine m_BehaviorTreeRoutine;
            private readonly YieldInstruction m_WaitTime = new WaitForSeconds(.1f);

            private void Start()
            {
                //MyActivity = NavigationActivity.Waypoint;

                GenerateBehaviorTree();

                if (m_BehaviorTreeRoutine == null && BehaviorTree != null)
                {
                    m_BehaviorTreeRoutine = StartCoroutine(RunBehaviorTree());
                }
            }

            public NodeBase BehaviorTree { get; set; }

            private void GenerateBehaviorTree()
            {
                Debug.Log("Called");

                MATCH.Inferences.Time inference19h35 = new Inferences.Time("19h35",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));

                MATCH.Inferences.Time inferenceTableCleaned = new Inferences.Time("Table cleaned",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));

                InferenceManager.RegisterInference(inference19h35);
                InferenceManager.RegisterInference(inferenceTableCleaned);

                BehaviorTree =
                    new Sequence("==> Dusting the table",
                        new Sequence("==> Process",
                            new Conditions.Inference("Is it at least 19h35?", inference19h35),
                            new Selector("? It is at least 20h",
                                new Conditions.Inference ("Is the table cleaned?", inferenceTableCleaned),
                                new DisplayMessage("Table not cleaned"))),
                        new DisplayMessage("Nothing to do!"));
                /*new Sequence("Sequence 1",
                    new IsNavigationActivityTypeOf(NavigationActivity.PickupItem),
                    new Selector("Look for or move to items",
                        new Sequence("Look for items",
                            new Inverter("Inverter",
                                new AreItemsNearBy(5f)),
                                new SetNavigationActivityTo(NavigationActivity.Waypoint)),
                        new Sequence("Navigate to Item",
                            new NavigateToDestination()))),
                 new Sequence("Move to Waypoint",
                    new IsNavigationActivityTypeOf(NavigationActivity.Waypoint),
                            new NavigateToDestination(),
                            new Timer(2f,
                                new SetNavigationActivityTo(NavigationActivity.PickupItem))));*/
            }

            private IEnumerator RunBehaviorTree()
            {
                while (enabled)
                {
                    if (BehaviorTree == null)
                    {
                        $"{this.GetType().Name} is missing Behavior Tree. Did you set the BehaviorTree property?".BTDebugLog();
                        continue;
                    }

                    (BehaviorTree as Node).Run();

                    yield return m_WaitTime;
                }
            }

            private void OnDestroy()
            {
                if (m_BehaviorTreeRoutine != null)
                {
                    StopCoroutine(m_BehaviorTreeRoutine);
                }
            }
        }
    }
    
}
