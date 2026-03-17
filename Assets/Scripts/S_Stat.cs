using UnityEngine;

[CreateAssetMenu(menuName = "Reinforcement Learning/Stat")]
public class S_Stat : ScriptableObject
{
    public Enum_Desires id;   // "name" of the action (hunger, thirst, ...); needs to correspond to the ID of the respective need
    public float startingValue = 1; // value at the beginning of a run, representing a stat like hunger or thirst
    public float currentValue; // current value of the stat, like hunger or thirst
    public float increaseAmount; // amount a stat increases by per second
}
