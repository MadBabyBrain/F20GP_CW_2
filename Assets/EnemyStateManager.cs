using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class EnemyStateManager : MonoBehaviour
{
    // states
    public EnemyStateBase currentState;
    public EnemyStateAttack attack = new EnemyStateAttack();
    public EnemyStateWalking walking = new EnemyStateWalking();

    public GameObject player;
    public GameObject target;

    public List<Vector3> path;
    public int pathIndex = 0;

    public float health = 100;
    public float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentState = walking;
        currentState.enter(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.execute(this);
    }

    public void updateState(EnemyStateBase e)
    {
        currentState.exit(this);
        currentState = e;
        currentState.enter(this);
    }

    public void takeDamage(float damage){
        health -= damage;

        if (health <= 0) {
            Destroy(gameObject);
        }
    } 
}
