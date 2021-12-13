using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using UnityEngine.UI;

/// <summary>
/// ゲーム管理クラス
/// </summary>

public class GameManager : MonoBehaviour
{
    public Agent[] agents;
    // public GameObject[] agents;
    public List<GameObject> bullets;

    public int[] agentScore;

    [Header("-- Environment settings --")]
    [SerializeField] float spawnXOffset = 5f;
    [SerializeField] Vector2 spawnRange;

    public int maxEnvironmentSteps = 10000;

    [SerializeField] private int resetTimer;
    
    // -- reward settings --
    private float timeBonus = 1.0f;
    private float scoreReward = 0.025f;
    private float hitBaseReward = 0.01f;

    [Header("-- HUD Settings --")]
    [SerializeField] Text[] ScoreTexts;

    // Start is called before the first frame update
    void Start()
    {
        GameAllClear();
        AgentReset();
    }

    void FixedUpdate()
    {
        resetTimer += 1;    // 現在のステップ数をカウント

        // maxEnvironmentSteps以上になったらエピソードを中断
        // maxEnvironmentStepsが0の場合は中断せず、エピソードを無制限に継続する
        if(resetTimer >= maxEnvironmentSteps && maxEnvironmentSteps > 0){
            agents[0].EpisodeInterrupted();
            agents[1].EpisodeInterrupted();
            Debug.Log("Episode Interrupted");
            GameAllClear();
            AgentReset();
        }else{
            // step reward
            agents[0].AddReward(-(1/maxEnvironmentSteps));
            agents[1].AddReward(-(1/maxEnvironmentSteps));
        }
    }
    
    public void AgentReset()
    {
        // エージェントの初期化(位置(x,z)や向きをリセット)
        // サイドもランダムに変化させる(x軸反転)
        Vector3 pos0, pos1;
        pos0 = pos1 = new Vector3(spawnXOffset,0.5f,0);

        pos0.x = -pos0.x;

        // Vector2 randomOffset = new Vector2(
        //     Random.Range(-spawnRange.x,spawnRange.x),
        //     Random.Range(-spawnRange.y,spawnRange.y)
        // );
        // pos0.x += randomOffset.x;
        // pos0.z += randomOffset.y;

        // randomOffset = new Vector2(
        //     Random.Range(-spawnRange.x,spawnRange.x),
        //     Random.Range(-spawnRange.y,spawnRange.y)
        // );
        // pos1.x += randomOffset.x;
        // pos1.z += randomOffset.y;
        
        // if(Random.value < 0.5f){
        //     pos0.x = -pos0.x;
        // }else{
        //     pos1.x = -pos1.x;
        // }

        // agents[0].transform.localPosition = pos0;
        // agents[1].transform.localPosition = pos1;
        agents[0].gameObject.transform.localPosition = pos0;
        agents[1].gameObject.transform.localPosition = pos1;
        
        // ランダムな方向に初期化
        // agents[0].transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        // agents[1].transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        // agents[0].gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        // agents[1].gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        
        agents[0].GetComponent<ShooterAgent>().AgentRestart();
        agents[1].GetComponent<ShooterAgent>().AgentRestart();

        // 弾丸をすべてDestroy
        foreach(GameObject liveBullet in bullets){
            Destroy(liveBullet);
        }
        bullets.Clear();
    }

    private void GameAllClear()
    {
        EnvironmentParameters envParameter = Academy.Instance.EnvironmentParameters;
        
        agentScore = new int[agents.Length];
        
        // 環境パラメータからランダムな値(float)を取得し、スコア(int)に割り当て
        agentScore[0] = Mathf.RoundToInt(envParameter.GetWithDefault("agent_A", 0));
        agentScore[1] = Mathf.RoundToInt(envParameter.GetWithDefault("agent_B", 0));

        ScoreToUI();
        Debug.Log("All agent Score Refreshed!");

        // 現在のステップ数をリセット
        resetTimer = 0;
    }

    // ヒット処理(弾丸がヒットされたエージェントから呼ばれる)
    // damageRatioは与えたダメージ量に対応した報酬の倍率(0～1)
    public void BulletHit(int agentId, float damageRatio)
    {
        // ダメージを与えたエージェントに中程度の報酬
        if(agentId == 0){
            // agents[0].AddReward(-0.1f);
            agents[1].AddReward(hitBaseReward * damageRatio);
        }else{
            agents[0].AddReward(hitBaseReward * damageRatio);
            // agents[1].AddReward(-0.1f);
        }
    }

    // 得点処理(HPが0となった側のエージェントから呼ばれる)
    public void AgentDefeated(int agentId)
    {
        if(agentId == 0){
            // スコア差を算出
            int scoreGap = agentScore[1] - agentScore[0];
            
            // スコア差に応じて得点時の報酬を与える
            agents[1].AddReward(scoreReward * (4 - scoreGap));
            agentScore[1] = agentScore[1] + 1;
        }else{
            // スコア差を算出
            int scoreGap = agentScore[0] - agentScore[1];
            
            // スコア差に応じて得点時の報酬を与える
            agents[0].AddReward(scoreReward * (4 - scoreGap));
            agentScore[0] = agentScore[0] + 1;
        }
        
        Debug.Log("Agent" + agentId + " Defeated!");

        for(int i = 0; i < agentScore.Length; i++){
            // スコアが先に5以上になったらエピソード完了
            if(agentScore[i] >= 5){
                EndEpisode(i);
            }
        }
        ScoreToUI();
        AgentReset();
    }

    // 決着処理(得点処理で先に5点先取したら呼ばれる)
    public void EndEpisode(int agentId)
    {
        // All agents add reward
        if(agentId == 0){
            Debug.Log("agent 1 Win!!");
            agents[1].AddReward(2.0f - timeBonus * (resetTimer / maxEnvironmentSteps));
            agents[0].AddReward(-1.0f);
        }else{
            Debug.Log("agent 0 Win!!");
            agents[0].AddReward(2.0f - timeBonus * (resetTimer / maxEnvironmentSteps));
            agents[1].AddReward(-1.0f);
        }

        // All agents end Episode
        agents[0].EndEpisode();
        agents[1].EndEpisode();
        // Debug.Log("end episode" + " from AgentID " + agentId);
        AgentReset();
        GameAllClear();
    }

    public void ScoreToUI()
    {
        for(int i = 0; i < agentScore.Length; i++){
            ScoreTexts[i].text = agentScore[i].ToString();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawCube(new Vector3(spawnXOffset, 0.25f, 0), new Vector3(spawnRange.x * 2, 0.5f, spawnRange.y * 2));
        Gizmos.DrawCube(new Vector3(-spawnXOffset, 0.25f, 0), new Vector3(spawnRange.x * 2, 0.5f, spawnRange.y * 2));
    }
}
