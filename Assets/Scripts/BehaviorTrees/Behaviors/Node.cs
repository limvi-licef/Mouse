/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WUG.BehaviorTreeVisualizer;

public abstract class Node : NodeBase
{
    private string m_lastStatusReason { get; set; } = "";

    public int EvaluationCount;

    public virtual NodeStatus Run()
    {
        NodeStatus nodeStatus = OnRun();

        if (LastNodeStatus != nodeStatus || !m_lastStatusReason.Equals(StatusReason))
        {
            LastNodeStatus = nodeStatus;
            m_lastStatusReason = StatusReason;
            OnNodeStatusChanged(this);
        }

        EvaluationCount++;

        if (nodeStatus != NodeStatus.Running)
        {
            Reset();
        }

        return nodeStatus;
    }

    public void Reset()
    {
        EvaluationCount = 0;
        OnReset();
    }

    protected abstract NodeStatus OnRun();
    protected abstract void OnReset();
}
