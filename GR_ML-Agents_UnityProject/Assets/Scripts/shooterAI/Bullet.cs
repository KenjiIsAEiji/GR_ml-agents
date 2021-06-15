using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject hitObject = collisionInfo.gameObject;
        if(hitObject.CompareTag("Agent")){
            int id = hitObject.GetComponent<ShooterAgent>().agentId;
            GameManager.Instance.EndEpisode(id);
        }
    }
}
