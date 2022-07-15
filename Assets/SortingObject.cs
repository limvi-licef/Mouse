using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPBehave;
using System;
using System.Reflection;

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
                Conditions["PersonWatchedObject"] = false;
                Conditions["PersonDroppedObjectOutsideStoringArea"] = false;
                Conditions["PersonDidNotComeToObject"] = false;

                MATCH.Inferences.Time inferenceObjectSorted = new Inferences.Time("Object sorted",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferenceObjectSorted.AddCallback(CallbackObjectStored);

                MATCH.Inferences.Time inferencePersonPassedByObject = new Inferences.Time("Person passed by the object",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonPassedByObject.AddCallback(CallbackPersonPassedByObject);

                MATCH.Inferences.Time inferencePersonGrabbedObject = new Inferences.Time("Person grabbed object",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonGrabbedObject.AddCallback(CallbackPersonGrabbedObject);

                MATCH.Inferences.Time inferencePersonWatchedObject = new Inferences.Time("Person watched object",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonWatchedObject.AddCallback(CallbackPersonWatchedObject);

                MATCH.Inferences.Time inferencePersonLocatedObjectOutsideStoringArea = new Inferences.Time("Person dropped object outside storing area",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonLocatedObjectOutsideStoringArea.AddCallback(CallbackPersonDroppedObjectOutsideStoringArea);

                MATCH.Inferences.Time inferencePersonDidNotComeToObject = new Inferences.Time("Person did not come to object since 2 minutes",
                    new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 35, 0));
                inferencePersonDidNotComeToObject.AddCallback(CallbackPersonDidNotComeToObject);

                InferenceManager.RegisterInference(inferenceObjectSorted);
                InferenceManager.RegisterInference(inferencePersonPassedByObject);
                InferenceManager.RegisterInference(inferencePersonGrabbedObject);
                InferenceManager.RegisterInference(inferencePersonWatchedObject);
                InferenceManager.RegisterInference(inferencePersonLocatedObjectOutsideStoringArea);
                InferenceManager.RegisterInference(inferencePersonDidNotComeToObject);

                /*Tree = new Root(
                    new Action(() => Debug.Log("Hello world!")));*/

                //new Action()

                Tree = new Root(Conditions,
                    new Selector(
                        new BlackboardCondition("ObjectStored", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                            new Sequence(
                                new NPBehave.Action(() => Debug.Log("Assistances Epsilon / Object stored")),
                                new WaitUntilStopped())),
                        new Selector(
                            new BlackboardCondition("PersonPassedByObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                new Sequence(
                                    new Selector(
                                        new BlackboardCondition("PersonGrabbedObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                            new Selector(
                                                new BlackboardCondition("PersonDroppedObjectOutsideStoringArea", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, new Sequence(
                                                    new NPBehave.Action(() => Debug.Log("Assistances Delta")),
                                                    new WaitUntilStopped())),
                                                new Sequence(
                                                    new NPBehave.Action(() => Debug.Log("Hide all assistances")),
                                                    new WaitUntilStopped())
                                                )),
                                        new BlackboardCondition("PersonGrabbedObject", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART, 
                                                new Sequence(
                                                    new NPBehave.Action(() => Debug.Log("Assistances Alpha")),
                                                    new WaitUntilStopped()))),
                                    //new BlackboardCondition("PersonGrabbedObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, 
                                    new BlackboardCondition("PersonDroppedObjectOutsideStoringArea", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, new Sequence(
                                        new NPBehave.Action(() => Debug.Log("Assistances Delta")),
                                        new WaitUntilStopped()))))),
                            new Selector(
                                new BlackboardCondition("PersonWatchedObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART, 
                                    new Sequence(
                                        new NPBehave.Action(() => Debug.Log("Assistances Beta")),
                                        new WaitUntilStopped())),
                               new BlackboardCondition("PersonWatchedObject", Operator.IS_EQUAL, false, Stops.IMMEDIATE_RESTART,
                                    new BlackboardCondition("PersonDidNotComeToObject", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                                        new Sequence (
                                            new NPBehave.Action(() => Debug.Log("Assistances Gamma")),
                                            new WaitUntilStopped()))))));

//#if UNITY_EDITOR
                NPBehave.Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
                debugger.BehaviorTree = Tree;
//#endif

                Tree.Start();
            }

            void CallbackObjectStored(System.Object o, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
                Conditions["ObjectStored"] = !(bool)Conditions["ObjectStored"];
            }

            void CallbackPersonPassedByObject(System.Object o, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
                Conditions["PersonPassedByObject"] = !(bool)Conditions["PersonPassedByObject"];
            }

            void CallbackPersonGrabbedObject(System.Object o, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
                Conditions["PersonGrabbedObject"] = !(bool)Conditions["PersonGrabbedObject"];
            }

            void CallbackPersonWatchedObject(System.Object o, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
                Conditions["PersonWatchedObject"] = !(bool)Conditions["PersonWatchedObject"];
            }

            void CallbackPersonDidNotComeToObject(System.Object o, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
                Conditions["PersonDidNotComeToObject"] = !(bool)Conditions["PersonDidNotComeToObject"];
            }

            void CallbackPersonDroppedObjectOutsideStoringArea(System.Object o, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");
                Conditions["PersonDroppedObjectOutsideStoringArea"] = !(bool)Conditions["PersonDroppedObjectOutsideStoringArea"];
            }
        }

        
    }
}
