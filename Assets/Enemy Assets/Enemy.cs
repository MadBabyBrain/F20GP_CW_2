using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int currPos;
    private List<Vector3> path;
    public void init(List<Vector3> path)
    {
        this.path = path;
        this.currPos = 1;
        InvokeRepeating("move", 1f, 1f);
    }

    void move()
    {
        if (currPos == path.Count - 1) Destroy(this.gameObject);
        this.transform.position = this.path[this.currPos] + Vector3.one * 0.5f;
        // yield return new WaitForSecondsRealtime(1);
        this.currPos++;
    }
}
