using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class S_JSONReader : MonoBehaviour
{
    private S_QTableJSON qTableObject = new S_QTableJSON();

    public void ExtractFiles(S_Sarsa sarsa, TextAsset tableJson, int stateAmount, int actionAmount)
    {
        // populate the QTableJSON object and get amount of actions/tables
        qTableObject = JsonUtility.FromJson<S_QTableJSON>(tableJson.text);

        // populate S_Needs list
        int index = 0;
        foreach (string q in qTableObject.qTableJsonStrings)
        {
            string[] qValuesStrings = q.Split(";");
            sarsa.needsList[index].qTable = new float[stateAmount, actionAmount];
            for (int value = 0; value < qValuesStrings.Length; value++)
            {
                // set list in S_Sarsa with the newly extracted list
                sarsa.needsList[index].qTable[value / actionAmount, value % actionAmount] = float.Parse(qValuesStrings[value], CultureInfo.CurrentCulture); // CultureInfo.InvariantCulture
            }
            index++;
        }
    }

    public void SaveFiles(S_Sarsa sarsa, float exploreVal, string qTableName, string NPC)
    {
        qTableObject.name += 1;
        qTableObject.NPCname = NPC;
        qTableObject.exploreValue = exploreVal;
        qTableObject.qTableJsonStrings = new string[sarsa.needsList.Length];
        qTableObject.actionLogJSON = sarsa.GetActionLog().ToString();
        qTableObject.needValuesAtEndOfRun = new float[sarsa.needsList.Length];

        for (int index = 0; index < sarsa.needsList.Length; index++)
        {
            qTableObject.needValuesAtEndOfRun[index] = sarsa.needsList[index].currentNeedsStat;
            for (int q = 0; q < sarsa.needsList[index].qTable.GetLength(0); q++)
            {
                for (int v = 0; v < sarsa.needsList[index].qTable.GetLength(1); v++)
                {
                    if (CheckIfEmptyString(index))
                        qTableObject.qTableJsonStrings[index] = sarsa.needsList[index].qTable[q, v].ToString();
                    else
                        qTableObject.qTableJsonStrings[index] = qTableObject.qTableJsonStrings[index] + ";" + sarsa.needsList[index].qTable[q, v].ToString();

                }
                //qTableObject.qTableJsonStrings[index] = qTableObject.qTableJsonStrings[index] + "\r\n"; // line break for windows only
            }
            //qTableObject.qTableJsonStrings[index] = qTableObject.qTableJsonStrings[index] + "\r\n" + "\r\n"; // line break for windows only
        }
        WriteInJSON(qTableName);
    }

    private void WriteInJSON(string qTableName)
    {
        File.WriteAllText(Application.dataPath + "/Jsons/" + qTableName + qTableObject.name + qTableObject.NPCname + ".txt", JsonUtility.ToJson(qTableObject));
    }

    private bool CheckIfEmptyString(int index)
    {
        return qTableObject.qTableJsonStrings[index] == null || qTableObject.qTableJsonStrings[index] == "";
    }
}
