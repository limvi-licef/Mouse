/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WUG.BehaviorTreeVisualizer;

public class Selector : Composite
{
    public Selector(string displayName, params Node[] childNodes) : base(displayName, childNodes) { }

    protected override NodeStatus OnRun()
    {
        if (CurrentChildIndex >= ChildNodes.Count)
        {
            return NodeStatus.Failure;
        }

        NodeStatus nodeStatus = (ChildNodes[CurrentChildIndex] as Node).Run();

        switch(nodeStatus)
        {
            case NodeStatus.Failure:
                CurrentChildIndex++;
                break;
            case NodeStatus.Success:
                return NodeStatus.Success;
        }

        return NodeStatus.Running;
    }

    protected override void OnReset()
    {
        CurrentChildIndex = 0;

        for (int i = 0; i < ChildNodes.Count; i ++)
        {
            (ChildNodes[i] as Node).Reset();
        }
    }
}
