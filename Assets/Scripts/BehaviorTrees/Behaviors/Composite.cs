/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Composite : Node
{
    protected int CurrentChildIndex = 0;

    protected Composite(string displayName, params Node[] childNodes)
    {
        Name = displayName;

        ChildNodes.AddRange(childNodes.ToList());
    }
}
