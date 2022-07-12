/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WUG.BehaviorTreeVisualizer;

public class Inverter : Decorator
{
    public Inverter(string displayName, Node childNode) : base(displayName, childNode) { }

    protected override void OnReset()
    {
        
    }

    protected override NodeStatus OnRun()
    {
        if (ChildNodes.Count == 0 || ChildNodes[0] == null)
        {
            return NodeStatus.Failure;
        }

        NodeStatus originalStatus = (ChildNodes[0] as Node).Run();

        switch(originalStatus)
        {
            case NodeStatus.Failure:
                return NodeStatus.Success;
            case NodeStatus.Success:
                return NodeStatus.Failure;
        }

        return originalStatus;
    }
}
