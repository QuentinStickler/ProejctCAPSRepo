using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int score = 0;
    public float health = 100f;

    public void SetScore(int points) { score += points; }

    private void Start()
    {
        score = 0;
    }

    private void Update()
    {
        if(score >= 300)
        {
            Application.Quit();
        }
    }
}
