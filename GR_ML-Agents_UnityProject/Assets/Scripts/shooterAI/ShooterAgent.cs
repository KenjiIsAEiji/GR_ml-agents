using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAgent : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float firePower = 10f;
    [SerializeField] float bulletLifeTime = 1.5f;
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
        if(Input.GetKeyDown(KeyCode.Space)){
            GameObject bullet = Instantiate(
                bulletPrefab,
                muzzleTransform.position,
                Quaternion.identity
            );

            bullet.GetComponent<Rigidbody>().AddForce(muzzleTransform.forward * firePower,ForceMode.Impulse);
            Destroy(bullet,bulletLifeTime);
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
}
