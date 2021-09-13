using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameManager gameManagerRef;
    [SerializeField] float lifeTime = 3.0f;

    void Start()
    {
        StartCoroutine(RemoveTimer());
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        GameObject hitObject = collisionInfo.gameObject;
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
