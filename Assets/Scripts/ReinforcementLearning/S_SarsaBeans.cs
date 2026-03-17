using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_SarsaBeans : S_Sarsa
{
    // This is Beans, the alternative SARSA Algorithm
    // States: Distance to Food (and Water), from 0 to 10 * 6.4 = 64 States
    // Actions: Up, Down, Left, Right, Food, Water

    protected override int UseAction(int actionNumber, S_NPC NPC)
     {
         if (actionNumber <= 3)
         {
             switch (actionNumber)
             {
                case 0: // move to food
                    NPC.MoveNPC(GetAxisDistanceToItem(true, NeedsItemGOs[0], NPC), isQuickTesting);
                    Debug.Log("Toward Food");
                    AddToActionLog("Toward Food");
                    break;
                case 1: // move to water
                    NPC.MoveNPC(GetAxisDistanceToItem(true, NeedsItemGOs[1], NPC), isQuickTesting);
                    Debug.Log("Toward Water");
                    AddToActionLog("Toward Water");
                    break;
                case 2: // move away from food
                    NPC.MoveNPC(GetAxisDistanceToItem(false, NeedsItemGOs[0], NPC), isQuickTesting);
                    Debug.Log("Away Food");
                    AddToActionLog("Away from Food");
                    break;
                case 3: // move away from water
                    NPC.MoveNPC(GetAxisDistanceToItem(false, NeedsItemGOs[1], NPC), isQuickTesting);
                    Debug.Log("Away Water");
                    AddToActionLog("Away from Water");
                    break;
                default:
                    Debug.Log("No Action Found");
                    break;
             }
         }
         else
         {
             NPC.UseNeedsAction(randomiseTaskSpawning, needsList[actionNumber - 4].id);
            AddToActionLog(needsList[actionNumber - 4].actionLogMessage);
         }
        return actionNumber;
     }
     protected Vector3 GetAxisDistanceToItem(bool towardNeed, GameObject item, S_NPC NPC)
     {
         float x = Mathf.Abs(NPC.transform.position.x) - Mathf.Abs(item.transform.position.x); // if its positive, its up; if its negative, its down
         float z = Mathf.Abs(NPC.transform.position.z) - Mathf.Abs(item.transform.position.z); // if its positive, its left; if its negative, its right
         if (Mathf.Abs(x) > Mathf.Abs(z)) // if its closer to x axis, move either up or down
         {
            if (towardNeed)
            {
                if (x > 0) return new Vector3(NPC.transform.position.x - 1f, 1.5f, NPC.transform.position.z); //down
                else return new Vector3(NPC.transform.position.x + 1f, 1.5f, NPC.transform.position.z); //up
            }
            else
            {
                if (x > 0) return new Vector3(NPC.transform.position.x + 1f, 1.5f, NPC.transform.position.z); //up
                else return new Vector3(NPC.transform.position.x - 1f, 1.5f, NPC.transform.position.z); //down
            }
         }
         else // else its closer to z axis and move either left or right
         {
            if (towardNeed)
            {
                if (z > 0) return new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z - 1f); //right
                else return new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z + 1f); //left
            }
            else
            {
                if (z > 0) return new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z + 1f); //left
                else return new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z - 1f); //right 
            }
                
         }
     }

     private float DistanceToNeedItem(Enum_Desires id, S_NPC NPC)
     {
        foreach(GameObject item in NeedsItemGOs)
        {
            if(id == item.GetComponent<S_Tasks>().desire) return Vector3.Distance(NPC.transform.position, item.transform.position);
        }
        return 0;
     }

     public override int GetState(S_NPC NPC)
     {
         return Mathf.RoundToInt(DistanceToNeedItem(currentHighestNeed.id, NPC) * 6.4f);
     }
}
