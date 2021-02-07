using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    public ParticleSystem blood;
    private void Awake()
    {
        if(this.gameObject)
            Destroy(this.gameObject,1f);
    }

}
