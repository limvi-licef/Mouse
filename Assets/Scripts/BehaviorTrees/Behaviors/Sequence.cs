/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WUG.BehaviorTreeVisualizer;

public class Sequence : Composite
{
    public Sequence(string displayName, params Node[] childNodes) : base(displayName, childNodes) { }

    protected override NodeStatus OnRun()
    {
        Node currentNode = (Node)ChildNodes[CurrentChildIndex];

        NodeStatus childNodeStatus = currentNode.Run();

        switch (childNodeStatus)
        {
            case NodeStatus.Failure:
                return childNodeStatus;
            case NodeStatus.Success:
                CurrentChildIndex++;
                break;
        }

        if (CurrentChildIndex >= ChildNodes.Count)
        {
            return NodeStatus.Success;
        }

        return childNodeStatus == NodeStatus.Success ? OnRun() : NodeStatus.Running;
    }

    protected override void OnReset()
    {
        CurrentChildIndex = 0;

        for (int i = 0; i < ChildNodes.Count; i++)
        {
            (ChildNodes[i] as Node).Reset();
        }
    }
}
