using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBaseLogic : MonoBehaviour
{

    private float health;

    // Start is called before the first frame update
    void Start()
    {
        this.health = 100;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void takeDamage(float damage)
    {


        health -= damage;

        Debug.Log(health);

        if (health <= 0)
        {
            Debug.Log("gameOver");
            Time.timeScale = 0.1f;
        }
    }

}