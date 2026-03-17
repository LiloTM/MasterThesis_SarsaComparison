using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Reinforcement Learning/Need")]
public class S_Need : ScriptableObject
{
    public Enum_Desires id;   // "name" of the action (hunger, thirst, ...); needs to correspond to the ID of the respective stats and items
    public float[,] qTable; // List for respective item, should its need be the currently highest; float[state][action]
    public float currentNeedsStat; // updated during runtime, taken from corresponding S_Stat
    public string actionLogMessage; // string attached to the actionlog 
}
