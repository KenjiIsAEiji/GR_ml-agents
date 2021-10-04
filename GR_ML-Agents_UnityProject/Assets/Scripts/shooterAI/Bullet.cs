using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameManager gameManagerRef;

    [SerializeField] float lifeTime = 3.0f;

    public Transform shootOrigin;
    [SerializeField] float shootingRange = 2.0f;

    void Start()
    {
        StartCoroutine(RemoveTimer());
    }

    void Update()
    {
        Vector3 relative = this.transform.position - shootOrigin.position;
        
        // 
        if(relative.sqrMagnitude > (shootingRange * shootingRange)){
            RemoveBullet();
        }
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject hitObject = collisionInfo.gameObject;

        // 
        if(hitObject.CompareTag("wall") || hitObject.CompareTag("InsideWall") || hitObject.CompareTag("bullet")){
            RemoveBullet();
        }
    }

    private void RemoveBullet()
    {
        gameManagerRef.bullets.Remove(this.gameObject);
        Destroy(this.gameObject);
    }

    IEnumerator RemoveTimer()
    {
        // Debug.Log("Starting life timer this bullet");
        yield return new WaitForSeconds(lifeTime);
        RemoveBullet();
    }
}
