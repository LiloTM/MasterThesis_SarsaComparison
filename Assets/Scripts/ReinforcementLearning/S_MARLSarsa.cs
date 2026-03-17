using UnityEngine;

public class S_MARLSarsa : S_Sarsa
{
    public override float GainReward(int action, S_NPC NPC)
    {
        baseActionNumber = 5;
        // if out of bounds walk : reward runIntoWallReward
        if (NPC.transform.position.x > worldBorderMax ||
            NPC.transform.position.x < worldBorderMin ||
            NPC.transform.position.z > worldBorderMax ||
            NPC.transform.position.z < worldBorderMin)
        {
            return runIntoWallReward;
        }

        if (action >= baseActionNumber)
        {
            // if you use the needs' corresponding action on the wrong item or no item (i.e. drink where there's food)
            if (NPC.GetCurrentlyCollidingItem() == null) return useIncorrectItemReward;
            if (NPC.GetCurrentlyCollidingItem().GetComponent<S_Tasks>().desire != needsList[action - baseActionNumber].id) return useIncorrectItemReward;
            // if you use the needs' corresponding action on the correct item (i.e. eat where there's food) and have the highest need for it
            else if (needsList[action - baseActionNumber] == currentHighestNeed) return useCorrectItemReward;
            // if you use the needs' corresponding action on the correct item (i.e. eat where there's food) but there's another need "needier"
            else if (needsList[action - baseActionNumber] != currentHighestNeed) return useNonHighestNeedItemReward;
        }

        // learn distance to highest need and return it as a reward
        float distanceToNeed = GetDistanceToHighestNeed(NPC);
        // should the action be idle, double the distMultiplier in the reward to gain a lesser reward
        if (action == 4) return runIntoWallReward; 
        return distanceReward - (distanceToNeed * distanceMultiplier);
    }
    protected override int Exploit(int state, float[,] curQTable, S_NPC npc)
    {
        // EXPLOITATION
        int actionNr = 0;
        float stateReward = 0f;

        // Checks the entire table for the highest valued action.
        // Should a path be blocked, its value gets skipped, as its action will default to "idle" anyway, which is always last in the movement options and will therefore always be checked
        for (int i = 0; i < curQTable.GetLength(1); i++)
        {
            if (IsPathBlocked(i, npc)) continue;
            if (stateReward >= curQTable[state, i]) continue;
            stateReward = curQTable[state, i];
            actionNr = i;
        }
        return actionNr;
    }
    protected int ExecuteAction(Vector3 vector, S_NPC NPC, int actionNr, string actionLogMessage)
    {
        if (IsPathBlocked(vector))
        {
            AddToActionLog(actionLogMessage + " Blocked");
            return 4;
        }
        NPC.MoveNPC(vector, isQuickTesting);
        AddToActionLog(actionLogMessage);
        return actionNr;
    }
    private bool IsPathBlocked(int actionNr, S_NPC NPC)
    {
        switch (actionNr)
        {
            case 0: //up
                Vector3 moveTo1 = new Vector3(NPC.transform.position.x + 1f, 1.5f, NPC.transform.position.z);
                if (IsPathBlocked(moveTo1)) return true;
                break;
            case 1: //down
                Vector3 moveTo2 = new Vector3(NPC.transform.position.x - 1f, 1.5f, NPC.transform.position.z);
                if (IsPathBlocked(moveTo2)) return true;
                break;
            case 2: //left
                Vector3 moveTo3 = new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z + 1f);
                if (IsPathBlocked(moveTo3)) return true;
                break;
            case 3: // right
                Vector3 moveTo4 = new Vector3(NPC.transform.position.x, 1.5f, NPC.transform.position.z - 1f);
                if (IsPathBlocked(moveTo4)) return true;
                break;
            default:
                return false;
        }
        return false;
    }
    protected bool IsPathBlocked(Vector3 newPosition)
    {
        int pos = GetState(newPosition);
        if (pos > 63 || pos < 0) return false;  // check for border positions
        return world.isBlockedPosition[pos];
    }
    public override int GetState(Vector3 position)
    {
        //x + z * column; transforms position into int in the array
        return Mathf.RoundToInt(position.x) + Mathf.RoundToInt(position.z) * 8;
    }
}
