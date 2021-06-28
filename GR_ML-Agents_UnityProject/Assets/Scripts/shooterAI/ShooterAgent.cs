using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAgent : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float firePower = 10f;
    [SerializeField] float coolTime = 2.0f;
    private bool firing = false;
    [SerializeField] Transform muzzleTransform;
    
    public int agentId;

    Rigidbody agentRb;
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float rotateSpeed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        agentRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // rotate
        Vector3 rotatingVec = Vector3.zero;
        if(Input.GetKey(KeyCode.J)) rotatingVec += Vector3.down;
        if(Input.GetKey(KeyCode.L)) rotatingVec += Vector3.up;
        // Debug.Log(rotatingVec);

        this.transform.Rotate(rotatingVec * rotateSpeed * Time.deltaTime);

        // bullet firing
        if(Input.GetKey(KeyCode.Space)){
            //Debug.Log("trigger down");
            if(!firing){
                StartCoroutine(FireTimer());
            }
        }
    }

    void FixedUpdate()
    {
        // movement
        Vector3 vel = Vector3.zero;

        if(Input.GetKey(KeyCode.W)) vel.z += 1.0f;
        
        if(Input.GetKey(KeyCode.S)) vel.z += -1.0f;
        
        if(Input.GetKey(KeyCode.A)) vel.x += -1.0f;
        
        if(Input.GetKey(KeyCode.D)) vel.x += 1.0f;

        vel = vel * moveSpeed;
        // Debug.Log(vel);

        agentRb.AddRelativeForce(vel * agentRb.mass * agentRb.drag / (1f - agentRb.drag * Time.fixedDeltaTime));
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.CompareTag("bullet")){
            this.gameManager.EndEpisode(this.agentId);
        }
    }

    public void AgentRestart(){
        StopCoroutine(FireTimer());
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
    }

    IEnumerator FireTimer()
    {
        BulletFire();
        firing = true;
        //Debug.Log("now Firing");
        yield return new WaitForSeconds(coolTime);
        firing = false;
    }
}
