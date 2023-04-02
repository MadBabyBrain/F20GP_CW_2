using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeBaseLogic : MonoBehaviour
{

    private float health;
    private TextMeshProUGUI hpText;

    // Start is called before the first frame update
    void Start()
    {
        this.hpText = GameObject.Find("LifeText").GetComponent<TextMeshProUGUI>();
        this.health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        this.hpText.text = "Lives: " + this.health;
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
