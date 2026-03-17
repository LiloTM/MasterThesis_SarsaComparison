using System.Collections.Generic;
using UnityEngine;


public class S_MARLClementine : S_MARLSarsa
{
    // This is MARLClementine, the multi-agent SARSA Algorithm
    // States: 8x8 grid positions
    // Actions: Up, Down, Left, Right, Idle, Food, Water

    protected override int UseAction(int actionNumber, S_NPC NPC)
    {
        if (actionNumber <= 4)
        {
            switch (actionNumber)
            {
                case 0: //up
                    Vector3 moveTo1 = new Vector3(NPC.transform.position.x + 1f, 1.5f, NPC.transform.position.z);
                    return ExecuteAction(moveTo1, NPC, actionNumber, "MoveUp");
                case 1: //down
                    Vector3 moveTo2 = new Vector3(NPC.transform.position.x - 1f, 1.5f, NPC.transform.position.z);
                    return ExecuteAction(moveTo2, NPC, actionNumber, "MoveDown");
                case 2: //left
                    Vector3 moveTo3 = new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z + 1f);
                    return ExecuteAction(moveTo3, NPC, actionNumber, "MoveLeft");
                case 3: // right
                    Vector3 moveTo4 = new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z - 1f);
                    return ExecuteAction(moveTo4, NPC, actionNumber, "MoveRight");
                case 4: // idle / block
                    AddToActionLog("Idle");
                    Debug.Log("Idle");
                    break;
                default:
                    Debug.Log("No Action Found");
                    break;
            }
        }
        else
        {
            NPC.UseNeedsAction(randomiseTaskSpawning, needsList[actionNumber - 5].id);
            AddToActionLog(needsList[actionNumber - 5].actionLogMessage);
        }
        return actionNumber;
    }

    public override int GetState(S_NPC NPC)
    {
        //x + z * column; transforms position into int in the array
        return Mathf.RoundToInt(NPC.transform.position.x) + Mathf.RoundToInt(NPC.transform.position.z) * 8; 
    }
}