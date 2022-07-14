/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Decorator : Node
{
    public Decorator(string displayName, Node node)
    {
        Name = displayName;
        ChildNodes.Add(node);
    }
}
