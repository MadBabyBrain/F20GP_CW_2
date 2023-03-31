using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class EnemyStateManager : MonoBehaviour
{
    // states
    public EnemyStateBase currentState;
    public EnemyStateAttack attack;
    public EnemyStateWalking walking;

    public GameObject player;
    // public GameObject target;
    public EnemyScriptableObject stats;

    public List<Vector3> path;
    public int pathIndex = 0;

    public float health;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        health = stats.hp;
        speed = stats.speed;

        attack = this.gameObject.AddComponent<EnemyStateAttack>();
        walking = this.gameObject.AddComponent<EnemyStateWalking>();
        player = GameObject.FindGameObjectWithTag("Player");
        // attack = new EnemyStateAttack();
        // walking = new EnemyStateWalking();

        currentState = walking;
        currentState.enter(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.execute();
    }

    public void updateState(EnemyStateBase e)
    {
        currentState.exit();
        currentState = e;
        currentState.enter(this);
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
