using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] Transform cam;
    void LateUpdate()
    {
        this.transform.LookAt(this.transform.position + cam.forward);
    }
}
