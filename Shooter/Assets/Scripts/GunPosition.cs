using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPosition : MonoBehaviour
{

    public Transform lookAt;
    void Start()
    {
        transform.position = lookAt.transform.position;

    }
    void Update()
    {
        transform.position = lookAt.transform.position;
    }
}
