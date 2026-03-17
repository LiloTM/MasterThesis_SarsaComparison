using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SarsaClementine : S_Sarsa
{
    // This is Clementine, the SARSA Algorithm
    // States: 8x8 grid positions
    // Actions: Up, Down, Left, Right, Food, Water

    protected override int UseAction(int actionNumber, S_NPC NPC)
    {
        if(actionNumber <= 3)
        {
            switch (actionNumber)
            {
                case 0: //up
                    NPC.MoveNPC(new Vector3(NPC.transform.position.x + 1f, 1.5f, NPC.transform.position.z), isQuickTesting);
                    AddToActionLog("MoveUp");
                    break;
                case 1: //down
                    NPC.MoveNPC(new Vector3(NPC.transform.position.x - 1f, 1.5f, NPC.transform.position.z), isQuickTesting);
                    AddToActionLog("MoveDown");
                    break;
                case 2: //left
                    NPC.MoveNPC(new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z + 1f), isQuickTesting);
                    AddToActionLog("MoveLeft");
                    break;
                case 3: // right
                    NPC.MoveNPC(new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z - 1f), isQuickTesting);
                    AddToActionLog("MoveRight");
                    break;
                default:
                    Debug.Log("No Action Found");
                    break;
            }
        }
        else
        {
            NPC.UseNeedsAction(randomiseTaskSpawning, needsList[actionNumber-4].id);
            AddToActionLog(needsList[actionNumber - 4].actionLogMessage);
        }
        return actionNumber;
    }
    public override int GetState(S_NPC NPC)
    {
        //x + z * column; transforms position into int in the array
        return Mathf.RoundToInt(NPC.transform.position.x) + Mathf.RoundToInt(NPC.transform.position.z) * 8;
    }
}
