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

    [Header("-- Agents Settings --")]
    public int agentId;
    public Transform opponentTransform;

    // Start is called before the first frame update
    public override void Initialize()
    {
        this.agentRb = GetComponent<Rigidbody>();
    }

    // 
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.forward.x);
        sensor.AddObservation(this.transform.forward.z);

        Vector3 relativePosition = opponentTransform.position - this.transform.position;
        relativePosition = relativePosition.normalized;

        sensor.AddObservation(relativePosition.x);
        sensor.AddObservation(relativePosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // actions organizing ---------------------------------
        int verticalAction = (int)actions.DiscreteActions[0];
        int horizonAction = (int)actions.DiscreteActions[1];
        int rotateAction = (int)actions.DiscreteActions[2];
        int fire = (int)actions.DiscreteActions[3];
        
        // movement ---------------------------------
        Vector3 vel = Vector3.zero;

        if(verticalAction == 1) vel.z += 1.0f;
        if(verticalAction == 2) vel.z += -1.0f;

        if(horizonAction == 1) vel.x += -1.0f;
        if(horizonAction == 2) vel.x += 1.0f;

        vel = vel * moveSpeed;
        // Debug.Log(vel);
        agentRb.AddRelativeForce(vel * agentRb.mass * agentRb.drag / (1f - agentRb.drag * Time.fixedDeltaTime));

        // rotate ---------------------------------
        Vector3 rotatingVec = Vector3.zero;
        if(rotateAction == 1) rotatingVec += Vector3.down;
        if(rotateAction == 2) rotatingVec += Vector3.up;
        // Debug.Log(rotatingVec);
        this.transform.Rotate(rotatingVec * rotateSpeed * Time.deltaTime);

        // bullet firing ---------------------------------
        if(fire == 1){
            //Debug.Log("trigger down");
            if(!firing){
                StartCoroutine(FireTimer());
            }
        }

        // stepReward
        // AddReward(-0.001f);

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        
        // vertical
        if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)){
            discreteActions[0] = 0;
        }else if(Input.GetKey(KeyCode.W)){
            discreteActions[0] = 1;
        }else if(Input.GetKey(KeyCode.S)){
            discreteActions[0] = 2;
        }else{
            discreteActions[0] = 0;
        }

        // horizon
        if(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)){
            discreteActions[1] = 0;
        }else if(Input.GetKey(KeyCode.A)){
            discreteActions[1] = 1;
        }else if(Input.GetKey(KeyCode.D)){
            discreteActions[1] = 2;
        }else{
            discreteActions[1] = 0;
        }

        // rotate
        if(Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.L)){
            discreteActions[2] = 0;
        }else if(Input.GetKey(KeyCode.J)){
            discreteActions[2] = 1;
        }else if(Input.GetKey(KeyCode.L)){
            discreteActions[2] = 2;
        }else{
            discreteActions[2] = 0;
        }

        // firing
        if(Input.GetKey(KeyCode.Space)){
            discreteActions[3] = 1;
        }else{
            discreteActions[3] = 0;
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
