using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// ゲーム管理クラス
/// </summary>

public class GameManager : MonoBehaviour
{
    // public Agent[] agents;
    public GameObject[] agents;
    public List<GameObject> bullets;

    [SerializeField] Vector2 spawnRange;

    // Start is called before the first frame update
    void Start()
    {
        AgentReset();
    }
    
    public void AgentReset()
    {
        // エージェントの初期化(位置(x,z)や向きをリセット)
        // サイドもランダムに変化させる(x軸反転)
        Vector3 pos0, pos1;
        pos0 = pos1 = new Vector3(5.0f,0.5f,0);

        Vector2 randomOffset = new Vector2(
            Random.Range(-spawnRange.x,spawnRange.x),
            Random.Range(-spawnRange.y,spawnRange.y)
        );
        pos0.x += randomOffset.x;
        pos0.z += randomOffset.y;

        randomOffset = new Vector2(
            Random.Range(-spawnRange.x,spawnRange.x),
            Random.Range(-spawnRange.y,spawnRange.y)
        );
        pos1.x += randomOffset.x;
        pos1.z += randomOffset.y;
        
        if(Random.value < 0.5f){
            pos0.x = -pos0.x;
        }else{
            pos1.x = -pos1.x;
        }

        // agents[0].gameObject.transform.localPosition = pos0;
        // agents[1].gameObject.transform.localPosition = pos1;
        agents[0].transform.localPosition = pos0;
        agents[1].transform.localPosition = pos1;
        
        // ランダムな方向に初期化
        agents[0].transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        agents[1].transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        // agents[0].gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        // agents[1].gameObject.transform.localEulerAngles = new Vector3(0,Random.Range(-180.0f,180.0f),0);
        
        // 弾丸をすべてDestroy

    }

    // 得点処理(ヒットした側のエージェントから呼ばれる)
    public void EndEpisode(int agentId)
    {
        // 報酬を与える
        // if(agentId == 0){
        //     agents[0].AddReward(-1.0f);
        //     agents[1].AddReward(1.0f);
        // }else{
        //     agents[0].AddReward(1.0f);
        //     agents[1].AddReward(-1.0f);
        // }

        // // 
        // agents[0].EndEpisode();
        // agents[1].EndEpisode();
        Debug.Log("end episode");
        AgentReset();
    }
}
