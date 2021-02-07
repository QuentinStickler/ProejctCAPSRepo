using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Transform target;
    void Start()
    {

        transform.LookAt(target);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
    }
}
