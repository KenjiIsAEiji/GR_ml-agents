using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ShooterAgent : Agent
{
    [Header("-- Agent moving settings --")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 5.0f;
    Rigidbody agentRb;
    
    [Header("-- Game management --")]
    [SerializeField] GameManager gameManager;

    [Header("-- Agent bullet shooting settings --")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float firePower = 10f;
    [SerializeField] float coolTime = 2.0f;
    [SerializeField] Transform muzzleTransform;
    private bool firing = false;

    [Header("-- Agents observation settings --")]
    public int agentId;
    public Transform stageTransform;

    // Start is called before the first frame update
    public override void Initialize()
    {
        this.agentRb = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ステージ上のエージェント自身の位置
        sensor.AddObservation(stageTransform.InverseTransformPoint(this.transform.position));
        // 自身の速度とエージェント自身の正面および右方向それぞれの内積
        sensor.AddObservation(Vector3.Dot(agentRb.velocity,this.transform.forward));
        sensor.AddObservation(Vector3.Dot(agentRb.velocity,this.transform.right));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // actions organizing ---------------------------------
        var continuousAction = actions.ContinuousActions;
        var discreteAction = actions.DiscreteActions;

        float inputVertical = continuousAction[0];
        float inputHorizontal = continuousAction[1];
        float inputRotate = continuousAction[2];

        int fire = (int)discreteAction[0];
        
        // movement ---------------------------------
        Vector3 vel = new Vector3(inputHorizontal, 0, inputVertical);

        vel = vel * moveSpeed;
        // Debug.Log(vel);
        agentRb.AddRelativeForce(vel * agentRb.mass * agentRb.drag / (1f - agentRb.drag * Time.fixedDeltaTime));

        // rotate ---------------------------------
        // Debug.Log(rotatingVec);
        this.transform.Rotate(Vector3.down * inputRotate * rotateSpeed * Time.deltaTime);

        // bullet firing ---------------------------------
        if(fire == 1){
            //Debug.Log("trigger down");
            if(!firing){
                StartCoroutine(FireTimer());
            }
        }

        // stepReward
        AddReward(-0.001f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActions = actionsOut.ContinuousActions;
        var discreteActions = actionsOut.DiscreteActions;

        // vertical
        float input = 0;
        if(Input.GetKey(KeyCode.W)) input += 1.0f;
        if(Input.GetKey(KeyCode.S)) input += -1.0f;

        continuousActions[0] = input;

        // horizon
        input = 0;
        if(Input.GetKey(KeyCode.A)) input += -1.0f;
        if(Input.GetKey(KeyCode.D)) input += 1.0f;

        continuousActions[1] = input;

        // rotate
        input = 0;
        if(Input.GetKey(KeyCode.J)) input += 1.0f;
        if(Input.GetKey(KeyCode.L)) input += -1.0f;
        
        continuousActions[2] = input;

        // firing
        if(Input.GetKey(KeyCode.Space)){
            discreteActions[0] = 1;
        }else{
            discreteActions[0] = 0;
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     // rotate
    //     Vector3 rotatingVec = Vector3.zero;
    //     if(Input.GetKey(KeyCode.J)) rotatingVec += Vector3.down;
    //     if(Input.GetKey(KeyCode.L)) rotatingVec += Vector3.up;
    //     // Debug.Log(rotatingVec);

    //     this.transform.Rotate(rotatingVec * rotateSpeed * Time.deltaTime);

    //     // bullet firing
    //     if(Input.GetKey(KeyCode.Space)){
    //         //Debug.Log("trigger down");
    //         if(!firing){
    //             StartCoroutine(FireTimer());
    //         }
    //     }
    // }

    // void FixedUpdate()
    // {
    //     // movement
    //     Vector3 vel = Vector3.zero;

    //     if(Input.GetKey(KeyCode.W)) vel.z += 1.0f;
        
    //     if(Input.GetKey(KeyCode.S)) vel.z += -1.0f;
        
    //     if(Input.GetKey(KeyCode.A)) vel.x += -1.0f;
        
    //     if(Input.GetKey(KeyCode.D)) vel.x += 1.0f;

    //     vel = vel * moveSpeed;
    //     // Debug.Log(vel);

    //     agentRb.AddRelativeForce(vel * agentRb.mass * agentRb.drag / (1f - agentRb.drag * Time.fixedDeltaTime));
    // }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("bullet")){
            this.gameManager.EndEpisode(this.agentId);
        }
    }

    public void AgentRestart(){
        StopAllCoroutines();
        firing = false;
    }

    void BulletFire()
    {
        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzleTransform.position,
            Quaternion.identity
        );

        bullet.GetComponent<Rigidbody>().AddForce(muzzleTransform.forward * firePower,ForceMode.Impulse);
        bullet.GetComponent<Bullet>().gameManagerRef = gameManager;
        gameManager.bullets.Add(bullet);

        // Bullet shooting Rewerd
        // AddReward(0.1f);
    }

    IEnumerator FireTimer()
    {
        BulletFire();
        firing = true;
        // Debug.Log("now Firing");
        yield return new WaitForSeconds(coolTime);
        firing = false;
    }
}
