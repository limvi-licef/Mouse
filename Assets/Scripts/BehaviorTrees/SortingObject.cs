using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using System;

namespace MATCH
{
    namespace BehaviorTrees
    {
        public class SortingObject : MonoBehaviour
        {
            private NPBehave.Root Tree;
            private Blackboard Conditions;
            public Inferences.Manager InferenceManager;

            // Start is called before the first frame update
            void Start()
            {
                Conditions = new Blackboard(UnityContext.GetClock());
                Conditions["ObjectStored"] = false;
                Conditions["PersonPassedByObject"] = false;
                Conditions["PersonGrabbedObject"] = false;

                MATCH.Inferences.Time inferenceObjectSorted = new Inferences.Time("Object sorted",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferenceObjectSorted.AddCallback(CallbackObjectStored);

                MATCH.Inferences.Time inferencePersonPassedByObject = new Inferences.Time("Person passed by the object",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonPassedByObject.AddCallback(CallbackPersonPassedByObject);

                MATCH.Inferences.Time inferencePersonGrabbedObject = new Inferences.Time("Person grabbed object",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonGrabbedObject.AddCallback(CallbackPersonGrabbedObject);

                InferenceManager.RegisterInference(inferenceObjectSorted);
                InferenceManager.RegisterInference(inferencePersonPassedByObject);
                InferenceManager.RegisterInference(inferencePersonGrabbedObject);

                /*Tree = new Root(
                    new Action(() => Debug.Log("Hello world!")));*/

                //new Action()

                Tree = new Root(Conditions,
                    new Selector(
                        new BlackboardCondition("ObjectStored", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                new NPBehave.Action(() => Debug.Log("Object stored!")),
                                new WaitUntilStopped())),
                        new Selector(
                            new BlackboardCondition("PersonPassedByObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                new Selector(
                                    new BlackboardCondition("PersonGrabbedObject", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART, new Sequence(
                                    new NPBehave.Action(() => Debug.Log("Assistance!")),
                                    new WaitUntilStopped())),
                                    new BlackboardCondition("PersonGrabbedObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, new Sequence(
                                    new NPBehave.Action(() => Debug.Log("Hide all assistances!")),
                                    new WaitUntilStopped())))))));

//#if UNITY_EDITOR
                NPBehave.Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
                debugger.BehaviorTree = Tree;
//#endif

                Tree.Start();
            }

            void CallbackObjectStored(System.Object o, EventArgs e)
            {
                Conditions["ObjectStored"] = true;
            }

            void CallbackPersonPassedByObject(System.Object o, EventArgs e)
            {
                Conditions["PersonPassedByObject"] = true;
            }

            void CallbackPersonGrabbedObject(System.Object o, EventArgs e)
            {
                Conditions["PersonGrabbedObject"] = true;
            }
        }

        
    }
}