using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// ゲーム管理クラス
/// </summary>

public class GameManager : MonoBehaviour
{
    public Agent[] agents;
    public GameObject[] Bullets;

    [SerializeField] Vector2 stageSize;

    // Start is called before the first frame update
    void Start()
    {
        AgentReset();
    }
    
    public void AgentReset()
    {
        // エージェントの初期化(位置や向きをリセット)
        // サイドもランダムに変化させる
        Vector3 pos0, pos1;
        pos0 = pos1 = new Vector3(5.0f,0.5f,0);

        // 弾丸をすべてDestroy

    }

    // 得点処理(ヒットした側のエージェントから呼ばれる)
    public void EndEpisode(int agentId)
    {
        // 報酬を与える
        if(agentId == 0){
            agents[0].AddReward(-1.0f);
            agents[1].AddReward(1.0f);
        }else{
            agents[0].AddReward(1.0f);
            agents[1].AddReward(-1.0f);
        }

        // 
        agents[0].EndEpisode();
        agents[1].EndEpisode();
        AgentReset();
    }
}
