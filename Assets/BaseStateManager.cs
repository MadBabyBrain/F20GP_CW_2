using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStateManager : MonoBehaviour
{
    BaseState state;
    

    public float health;
    // Start is called before the first frame update
    void Start()
    {

    }

    
    // Update is called once per frame
    void Update()
    {
        state.Update();
    }

    public void takeDamage(float damage){
        health -= damage;
        if(health<=0){
            changeDeathState();
        }
    }

    public void changeDeathState(){
        
    }
}
