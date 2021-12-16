using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GameLogger : MonoBehaviour
{
    private StreamWriter sw;
    [SerializeField] private int gameCount = 0;

    private List<string> gameScoreDatas;

    private string[] agentLabel = {"A","B"};

    // Start is called before the first frame update
    void Start()
    {
        sw = new StreamWriter(@"./Assets/LogData/gameLog.csv", true, Encoding.GetEncoding("Shift_JIS"));
        string[] s1 = {"Game Count", "AgentID:Now Score"};
        string s2 = string.Join(",",s1);
        sw.WriteLine(s2);
        
        gameScoreDatas = new List<string>();

        gameCount = 0;
        gameScoreDatas.Add(gameCount.ToString());
    }

    public void ScoreLog(int agentID, int nowScore)
    {
        string data = agentLabel[agentID] + " " + nowScore.ToString();

        gameScoreDatas.Add(data);

        Debug.Log("logging this data: "+ data);
    }

    public void GameEndLog()
    {
        string outLine = string.Join(",",gameScoreDatas);
        sw.WriteLine(outLine);
        gameScoreDatas = new List<string>();

        gameCount++;
        gameScoreDatas.Add(gameCount.ToString());
    }

    void OnApplicationQuit()
    {
        sw.Close();
    }
}
