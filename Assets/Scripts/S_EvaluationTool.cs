using UnityEngine;

public class S_EvaluationTool : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject grid;
    [SerializeField] private GameObject textPrefab;
    [SerializeField] private S_JSONReader reader;
    [SerializeField] private S_Sarsa sarsa;
    [SerializeField] private TextAsset qTableJson;
    [SerializeField] private int actionAmount = 6;
    [SerializeField] private int stateAmount = 64;
    private GameObject[] gridList;

    private void Start()
    {
        PopulateGrid();
    }
    public void PopulateGrid()
    {
        reader.ExtractFiles(sarsa, qTableJson, stateAmount, actionAmount);

        int unexploredActionCounter = 0;
        // Initialise amount of grids per need
        gridList = new GameObject[sarsa.needsList.Length];
        for (int a = 0; a < sarsa.needsList.Length; a++)
        {
            gridList[a] = Instantiate(grid, canvas.transform);

            // Initialise highest actions in the texts
            for (int i = 0; i < stateAmount; i++)
            {
                float highestReward = 0;
                int bestAction = 7;
                GameObject currentText = Instantiate(textPrefab, gridList[a].transform);

                for (int j = 0; j < sarsa.needsList[a].qTable.GetLength(1); j++)
                {
                    if (highestReward < sarsa.needsList[a].qTable[i, j])
                    {
                        highestReward = sarsa.needsList[a].qTable[i, j];
                        bestAction = j;
                    }
                    if (sarsa.needsList[a].qTable[i, j] == 0.1f) unexploredActionCounter++;
                }
                currentText.GetComponent<TMPro.TMP_Text>().text = bestAction.ToString();
            }
        }
        Debug.Log($"{unexploredActionCounter}/{actionAmount*stateAmount * sarsa.needsList.Length} actions have not been explored.");
    }
}
