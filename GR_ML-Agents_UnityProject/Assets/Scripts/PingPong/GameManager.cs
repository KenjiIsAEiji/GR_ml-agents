using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

/// <summary>
/// ゲーム管理用クラス
/// </summary>
public class GameManager : MonoBehaviour
{
    public Agent[] agents;
    public GameObject ball;

    // スタート時に呼ばれる
    void Start()
    {
        Reset();
    }

    // エピソード開始時に呼ばれる
    public void Reset()
    {
        // エージェント(パドル)の位置をリセット
        agents[0].gameObject.transform.localPosition = new Vector3(0.0f,0.5f,-0.7f);
        agents[1].gameObject.transform.localPosition = new Vector3(0.0f,0.5f,0.7f);

        // ボールの位置をリセット
        ball.transform.localPosition = new Vector3(0.0f, 0.25f, 0.0f);

        // ランダムな方向に初速度を再設定
        float speed = 10.0f;
        float rad = Random.Range(45f, 135f) * Mathf.PI / 180.0f;
        Vector3 v0 = new Vector3(Mathf.Cos(rad) * speed, 0.0f, Mathf.Sin(rad) * speed);
        if(Random.value < 0.5f) v0.z = -v0.z;

        Rigidbody rd = ball.GetComponent<Rigidbody>();
        rd.velocity = v0;
    }

    // エピソード完了時に呼ばれる
    public void EndEpisode(int agentId)
    {
        // エピソード完了時(スコア発生時)
        // 得点する側にはプラス報酬
        // 失点する側にはマイナス報酬
        if(agentId == 0){
            agents[0].AddReward(1.0f);
            agents[1].AddReward(-1.0f);
        }else{
            agents[0].AddReward(-1.0f);
            agents[1].AddReward(1.0f);
        }

        // エピソード完了および環境をリセット
        agents[0].EndEpisode();
        agents[1].EndEpisode();
        Reset();
    }
}
