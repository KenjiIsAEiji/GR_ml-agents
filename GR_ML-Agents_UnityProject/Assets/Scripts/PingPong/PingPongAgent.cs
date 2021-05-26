using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PingPongAgent : Agent
{
    public int agentId;
    public GameObject ball;
    Rigidbody ballRb;

    // 初期化時に呼ばれる
    public override void Initialize()
    {
        this.ballRb = this.ball.GetComponent<Rigidbody>();
    }

    // 観察取得時に呼ばれる
    public override void CollectObservations(VectorSensor sensor)
    {
        // 各ベクトルをエージェントから見た方向に変更
        float dir = (agentId == 0) ? 1.0f : -1.0f;

        //ボールの一座標(x,z)
        sensor.AddObservation(this.ball.transform.localPosition.x * dir);   
        sensor.AddObservation(this.ball.transform.localPosition.z * dir);
        
        //ボールの速度
        sensor.AddObservation(this.ballRb.velocity.x * dir);
        sensor.AddObservation(this.ballRb.velocity.x * dir);
        
        // エージェント(パドル位置)
        sensor.AddObservation(this.transform.localPosition.x * dir);
    }

    // パドルとボールの衝突開始時に呼ばれる
    void OnCollisionEnter(Collision collisionInfo)
    {
        // 報酬
        AddReward(0.1f);
    }

    // 行動実行時に呼ばれる
    public override void OnActionReceived(ActionBuffers actions)
    {
        // エージェントの移動
        float dir = (agentId == 0) ? 1.0f : -1.0f;
        int action = (int)actions.DiscreteActions[0];
        Vector3 pos = this.transform.localPosition;

        if(action == 1){
            pos.x -= 0.2f * dir;
        }else if(action == 2){
            pos.x += 0.2f * dir;
        }

        if(pos.x < -4.0f) pos.x = -4.0f;
        if(pos.x > 4.0f) pos.x = 4.0f;
        this.transform.localPosition = pos;
    }

    // ヒューリスティックモードでの行動決定時に呼ばれる
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var DiscreteActions = actionsOut.DiscreteActions;
        DiscreteActions[0] = 0;
        if(Input.GetKey(KeyCode.A)) DiscreteActions[0] = 1;
        if(Input.GetKey(KeyCode.D)) DiscreteActions[0] = 2;
    }
}
