using System.Collections;
using UnityEditor;
using UnityEngine;

public class S_Testing : MonoBehaviour
{
    [Header("JSON Reader")]
    [SerializeField] private S_JSONReader reader;
    [SerializeField] private TextAsset qTableJson;

    [Header("Reinforcement Learning Algorithms")]
    [SerializeField] private S_Sarsa[] sarsaList;
    
    [Header("NPCs")]
    [SerializeField] private S_NPC[] NPCList;

    [Header("Presets")]
    [SerializeField] private int actionAmount = 6;
    [SerializeField] private int stateAmount = 64;

    [Header("Testing")]
    [SerializeField] private int TestRoundNumber = 100;
    [SerializeField] private int TotalTurnsNumber = 10;

    [SerializeField] private bool isQuickTesting;
    [SerializeField] private bool randomiseTaskSpawning = false;
    [SerializeField] private float exploreValue = 0;

    [SerializeField] private float waitTimeBetweenActions = 0.2f;
    [SerializeField] private float waitTimeBetweenTurns = 0.5f;
    [SerializeField] private float increaseWaitTime = 0.2f;

    [Header("Q Table")]
    [SerializeField] private bool saveQTableOnQuitting = false;
    [SerializeField] private string qTableName;

    [Header("Populating a JSON Q Table")]
    [SerializeField] private float qTablePopulationValue = 0.1f;

    private S_World world;
    private void Awake()
    {
        InitWorld();

        // extract all Q Tables from the JSON file for all sarsa algorithms
        foreach (S_Sarsa sarsa in sarsaList)
        {
            reader.ExtractFiles(sarsa, qTableJson, stateAmount, actionAmount);
        }
    }
    private void Start()
    {
        // assign an NPC to each sarsa algorithm, then check whether there are NPCs left over and hand them to the last sarsa algorithm of the list
        for(int i = 0; i < sarsaList.Length; i++)
        {
            sarsaList[i].Init(exploreValue, randomiseTaskSpawning, qTablePopulationValue, isQuickTesting, world);
            InitStats();
            StartCoroutine(SelfRun(TestRoundNumber, TotalTurnsNumber, sarsaList[i], NPCList[i]));
        }
        if(NPCList.Length > sarsaList.Length)
        {
            for(int i = sarsaList.Length; i < NPCList.Length; i++)
            {
                StartCoroutine(SelfRun(TestRoundNumber, TotalTurnsNumber, sarsaList[sarsaList.Length - 1], NPCList[i]));
            }
        }
    }
    private void InitWorld()
    {
        // initialise the world with the positions the NPCs are currently blocking
        world = new S_World { isBlockedPosition = new bool[stateAmount] };
        ResetWorld();
    }
    private void ResetWorld()
    {
        for(int b = 0; b < world.isBlockedPosition.Length; b++)
        {
            if (world.isBlockedPosition[b]) world.isBlockedPosition[b] = false;
        }
        foreach (S_NPC npc in NPCList)
        {
            world.isBlockedPosition[Mathf.Clamp(Mathf.RoundToInt(npc.transform.position.x), 0, 7) + Mathf.Clamp(Mathf.RoundToInt(npc.transform.position.z) * 8, 0, 7)] = true;
        }
    }
    private void InitStats()
    {
        for(int i = 0; i < NPCList.Length; i++)
        {
            NPCList[i].SetDecreaseWaitTime(increaseWaitTime);
            foreach (S_Stat stat in NPCList[i].GetStats())
            {
                stat.currentValue = stat.startingValue;
            }
        }
    }

    IEnumerator SelfRun(int rounds, int totalTurns, S_Sarsa sarsa, S_NPC NPC)
    {
        UpdateStats(sarsa, NPC);
        ResetWorld();

        float[,] curQTable = sarsa.GetCurTable();
        int state = sarsa.GetState(NPC);
        int action = sarsa.SelectAction(exploreValue, curQTable, NPC);

        yield return new WaitForSeconds(waitTimeBetweenActions);
        UpdateSarsaRewards(sarsa, NPC, curQTable, state, action);

        // Update round and turn numbers, break if selfruns are over
        rounds--;
        if (rounds % 10 == 0) Debug.Log("Current rounds completed:" + rounds);
        if (rounds <= 0)
        {
            totalTurns--;
            Debug.Log("Total Turns left:" + totalTurns);
            if (totalTurns <= 0) yield break;
            NPC.ResetAgent();
            InitWorld();
            if (randomiseTaskSpawning)
            {
                foreach (GameObject go in sarsa.NeedsItemGOs)
                {
                    go.GetComponent<S_Tasks>().SetRandomPosition();
                }
            }
            yield return new WaitForSeconds(waitTimeBetweenTurns);
            reader.SaveFiles(sarsa, exploreValue, qTableName, NPC.name);
            StartCoroutine(SelfRun(TestRoundNumber, totalTurns, sarsa, NPC));
            yield break;
        }
        else StartCoroutine(SelfRun(rounds, totalTurns, sarsa, NPC));
    }

    // Update the current stats
    private static void UpdateStats(S_Sarsa sarsa, S_NPC NPC)
    {
        foreach (S_Need need in sarsa.needsList)
        {
            foreach (S_Stat stat in NPC.GetStats())
            {
                if (need.id == stat.id) need.currentNeedsStat = stat.currentValue;
            }
        }
    }
    // Update the rewards and q table in the sarsa
    private static void UpdateSarsaRewards(S_Sarsa sarsa, S_NPC NPC, float[,] curQTable, int state, int action)
    {
        float reward = sarsa.GainReward(action, NPC);
        sarsa.UpdateQTable(state, action, reward, curQTable, NPC);
    }

    // Populates a new JSON file with the preselected value using the first Sarsa in the list
    [ContextMenu("PopulateTable")]
    public void PopulateTable()
    {
        for (int index = 0; index < sarsaList[0].needsList.Length; index++)
        {
            sarsaList[0].needsList[index].qTable = new float[stateAmount,actionAmount];
            for (int i = 0; i < stateAmount; i++)
            {
                for (int j = 0; j < actionAmount; j++)
                {
                    sarsaList[0].needsList[index].qTable[i, j] = qTablePopulationValue;
                }
            }
        } 
        reader.SaveFiles(sarsaList[0], exploreValue, qTableName, "");
    }

    void OnApplicationQuit()
    {
        if (saveQTableOnQuitting)
        {
            for (int i = 0; i < sarsaList.Length; i++)
            {
                reader.SaveFiles(sarsaList[i], exploreValue, qTableName, NPCList[i].name);
            }
        }
    }
}
