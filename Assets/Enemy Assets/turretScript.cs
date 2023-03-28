using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretScript : MonoBehaviour
{

    Transform Enemy;
    float distance;
    public float MaxDist;
    public Transform t_Head;
    // Start is called before the first frame update
    void Start()
    {
        Enemy = GameObject.FindGameObjectWithTag("enemy").transform;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(Enemy.position, transform.position);
        if(distance <= MaxDist){
            t_Head.LookAt(Enemy);
        }
    }
}
