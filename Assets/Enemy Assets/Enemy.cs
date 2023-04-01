using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Enemy : MonoBehaviour
{
    public float health;
    public float speed;

    void _init_(ref EnemyScriptableObject _stats, ref List<Vector3> _path) {
        path = _path;
        stats = _stats;

        // set enemy parameters
        health = stats.hp;
        speed = stats.speed;

        // metadata
        pathIndex = 0;
        initialised = true;
    }

    private void move() {
        if (pathIndex == path.Count - 1)
        {
            Destroy(gameObject);
            return;
        }

        // TODO: Terrain offset by 1/2 in every direction, fix in implementation
        gameObject.transform.position = path[pathIndex++];
    }

    // Update is called once per frame
    void Update()
    {
        // make sure object has been initialised
        if (!initialised) {
            Debug.Log("Please initialise object with _init_() call");
            return;
        }

        InvokeRepeating("move", 0f, speed);
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // returns the number of elements left to traverse in the path
    public int distanceLeftToTravel() {
        return ((path.Count - 1) - pathIndex);
    }

    // private members
    private EnemyScriptableObject stats;
    private List<Vector3> path;
    private int pathIndex;
    private bool initialised = false;   // used to determine if _init_() has been called and member fields have been set
}
