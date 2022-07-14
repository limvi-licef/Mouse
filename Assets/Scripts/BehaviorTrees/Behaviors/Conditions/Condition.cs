/**
 * Code from https://github.com/Yecats/UnityBehaviorTreeVisualizer
 * */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Condition : Node
{
    public Condition (string name)
    {
        Name = name;
    }
}
