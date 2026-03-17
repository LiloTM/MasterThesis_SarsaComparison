using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Sarsa : MonoBehaviour
{
    public S_Need[] needsList;

    public GameObject[] NeedsItemGOs;
    protected S_World world;

    protected string actionLog = "";
    public string GetActionLog() { return actionLog; }

    protected S_Need currentHighestNeed;
    protected float exploreValue;

    protected float qTablePopulationValue = 0.1f;
    protected bool randomiseTaskSpawning = false;
    protected bool isQuickTesting = false;


    [SerializeField] protected int worldBorderMin = 1;
    [SerializeField] protected int worldBorderMax = 7;

    [Header("Reward Values")]
    [SerializeField] protected float runIntoWallReward = 0.00001f;
    [SerializeField] protected float useCorrectItemReward = 0.9999f;
    [SerializeField] protected float useIncorrectItemReward = 0.0001f;
    [SerializeField] protected float useNonHighestNeedItemReward = 0.002f;
    [SerializeField] protected float distanceReward = 0.09f;
    [SerializeField] protected float distanceMultiplier = 0.001f;
    protected int baseActionNumber = 4;

    public void Init(float exploreValue, bool randomiseTaskSpawning, float qTablePopulationValue, bool isQuickTesting, S_World world)
    {
        this.exploreValue = exploreValue;
        this.randomiseTaskSpawning = randomiseTaskSpawning;
        this.qTablePopulationValue = qTablePopulationValue;
        this.isQuickTesting = isQuickTesting;
        this.world = world;
    }
    public int SelectAction(float exploitation, float[,] curQTable, S_NPC NPC)
    {
        float random = Random.value;
        int action = 0; 
        int state = GetState(NPC);
        if (random > exploitation) action = Exploit(state, curQTable, NPC);
        else action = Explore(state, curQTable);
        return UseAction(action, NPC);
    }
    protected virtual int UseAction(int actionNumber, S_NPC NPC)
    {
        // Insert the consequences of a chosen action in the children
        return actionNumber;
    }
    protected void AddToActionLog(string message)
    {
        actionLog = actionLog + message + "; ";
    }
    public float[,] GetCurTable()
    {
        currentHighestNeed = GetHighestNeed();
        return currentHighestNeed.qTable;
    }
    protected S_Need GetHighestNeed()
    {
        S_Need curHigh = needsList[0];
        foreach (S_Need need in needsList)
        {
            if (need.currentNeedsStat > curHigh.currentNeedsStat) curHigh = need; 
        }
        return curHigh;
    }
    protected virtual int Exploit(int state, float[,] curQTable, S_NPC npc)
    {
        // EXPLOITATION
        int actionNr = 0;
        float stateReward = 0f;

        for (int i = 0; i < curQTable.GetLength(1); i++)
        {
            if (stateReward < curQTable[state, i])
            {
                stateReward = curQTable[state, i];
                actionNr = i;
            }
        }
        return actionNr;
    }
    protected int Explore(int state, float[,] curQTable)
    {
        //EXPLORATION
        // Prefer exploration of previously unexplored actions by finding all and selecting a random one among them
        ArrayList list = new ArrayList();
        for (int i = 0; i < curQTable.GetLength(1); i++)
        {
            if (curQTable[state, i] == qTablePopulationValue) list.Add(i);
        }
        if (list.Count != 0) return (int)list[Random.Range(0, list.Count - 1)];

        // Otherwise return a random action
        return Random.Range(0, curQTable.GetLength(1));
    }
    public virtual float GainReward(int action, S_NPC NPC)
    {
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
        return distanceReward - (distanceToNeed * distanceMultiplier); 
    }
    protected float GetDistanceToHighestNeed(S_NPC NPC)
    {
        // learn distance to highest need
        foreach (GameObject item in NeedsItemGOs)
        {
            if (item.GetComponent<S_Tasks>().desire == currentHighestNeed.id) return Vector3.Distance(NPC.transform.position, item.transform.position);
        }
        return 0;
    }

    public void UpdateQTable(int state, int action, float reward, float[,] curQTable, S_NPC NPC)
    {
        int curState = GetState(NPC);
        int curAction = Exploit(curState, curQTable, NPC);
        // Q(s,a) = Q(s,a) + α [r + γ Q(s',a') - Q(s,a)]
        curQTable[state, action] = (float)(curQTable[state, action] + 0.1 * (reward + curQTable[curState, curAction] - curQTable[state, action]));
    }
    public virtual int GetState(S_NPC NPC) 
    {
        return 0; 
    }

    public virtual int GetState(Vector3 position)
    {
        return 0;
    }
}
