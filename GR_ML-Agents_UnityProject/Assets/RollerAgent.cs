using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class RollerAgent : Agent
{
    public Transform target;
    Rigidbody rBody;

    public override void Initialize()
    {
        this.rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if(this.transform.position.y < 0){
            this.rBody.angularVelocity = Vector3.zero;
            this.rBody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0.0f,0.0f,0.0f);
        }

        target.position = new Vector3(
            Random.value * 8 - 4,
            0.5f,
            Random.value * 8 - 4
        );
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.position);
        sensor.AddObservation(this.transform.position);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actions.ContinuousActions[0];
        controlSignal.y = actions.ContinuousActions[1];
        rBody.AddForce(controlSignal * 10);

        float distanceToTarget = Vector3.Distance(this.transform.position, target.position);

        if(distanceToTarget < 1.42f){
            AddReward(1.0f);
            EndEpisode();
        }

        if(this.transform.position.y < 0){
            EndEpisode();
        }
    }
}
