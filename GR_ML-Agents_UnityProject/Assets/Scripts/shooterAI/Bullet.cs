using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameManager gameManagerRef;
    public Transform shootOrigin;
    [SerializeField] float shootingRange = 2.0f;

    public float bulletBaseDamage = 6f;
    [SerializeField] private AnimationCurve damageCurve;

    [SerializeField] float lifeTime = 3.0f;

    void Start()
    {
        // start remove timer
        StartCoroutine(RemoveTimer());
    }

    void Update()
    {
        // shootingRange comparison
        if(GetDistanceRatio() > 1.0f){
            RemoveBullet();
        }
    }

    private float GetDistanceRatio()
    {
        Vector3 relative = this.transform.position - shootOrigin.position;
        float ratio = relative.sqrMagnitude / (shootingRange * shootingRange);
        
        //Debug.Log("DistanceRatio: " + ratio);
        return ratio;
    }

    public float GetDamage
    {
        get
        {
            float damage = bulletBaseDamage * damageCurve.Evaluate(Mathf.Clamp01(GetDistanceRatio()));
            // Debug.Log("Damage! : " + damage);

            return damage;
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

    public void RemoveBullet()
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
