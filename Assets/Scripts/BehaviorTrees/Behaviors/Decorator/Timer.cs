/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WUG.BehaviorTreeVisualizer;

public class Timer : Decorator
{
    private float m_startTime;
    private bool m_useFixedTime;
    private float m_timeToWait;

    public Timer (float timeToWait, Node childNode, bool useFixedTime = false): base($"Timer for {timeToWait}", childNode)
    {
        m_useFixedTime = useFixedTime;
        m_timeToWait = timeToWait;
    }

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

        if (EvaluationCount == 0)
        {
            StatusReason = $"Starting timer for {m_timeToWait}. Child node status is: {originalStatus}";
            m_startTime = m_useFixedTime ? Time.fixedTime : Time.time;
        }

        float elapsedTime = Time.fixedTime - m_startTime;

        if(elapsedTime>m_timeToWait)
        {
            StatusReason = $"Timer complete - child node status is: {originalStatus}";
            return NodeStatus.Success;
        }

        StatusReason = $"Timer is {elapsedTime} out of {m_timeToWait}. Child node status is: {originalStatus}";
        return NodeStatus.Running;
    }
}
