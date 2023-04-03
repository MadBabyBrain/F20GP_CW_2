using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathTrail : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    public float waitTime;
    public float speed = 10;

    public void _init_(List<Vector3> _path, float _waitTime) {
        pathIndex = 0;

        path = _path;
        waitTime = _waitTime;
        waited = _waitTime;

        obj = Instantiate(obj, new Vector3(1f, -100000000f, 1f), Quaternion.identity);
        obj.transform.position = path[0] + Vector3.one * 0.5f;
        trail = obj.GetComponent<TrailRenderer>();

        initialised = true;
    }

    void Update() {
        if (!initialised)
            return;

        // if (waited <= 0f)
        // {
            move();
        // }
        // else
        // {
        //     waited -= Time.deltaTime;
        // }
    }

    void move() { 
        // reset
        if (pathIndex == path.Count - 1)
        {
            trail.emitting = false;
            pathIndex = 0;
            trail.time = 0f;
            obj.transform.position = path[pathIndex] + Vector3.one * 0.5f;
            trail.time = 1f;
            trail.emitting = true;
        }

        // activate trail once at start
        // if (!trail.enabled) trail.enabled = true;

        obj.transform.position = Vector3.MoveTowards(obj.transform.position, path[pathIndex] + Vector3.one * 0.5f, speed * Time.deltaTime);
        if (Vector3.Distance(obj.transform.position, path[pathIndex] + Vector3.one * 0.5f) < 0.01f)
        {
            pathIndex++;
        }

        waited = waitTime;
    }

    public TrailRenderer trail;
    public int pathIndex;
    public float waited;
    public bool initialised  = false;

        public List<Vector3> path;
}
