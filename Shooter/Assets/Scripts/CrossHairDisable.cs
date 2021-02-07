using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairDisable : MonoBehaviour
{
    public bool isEnabled;
    public GameObject crossHair;
    public void ManipulateCrosshair()
    {
        isEnabled = !isEnabled;
        crossHair.SetActive(isEnabled);
    }
}
