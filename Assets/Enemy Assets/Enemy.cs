using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Enemy : MonoBehaviour
{
    private int currPos;
    private List<Vector3> path;
    private Camera cam;
    [SerializeField]
    private GameObject health;
    private int hp;
    private bool toDestroy;
    public void init(List<Vector3> path, Camera cam, int health)
    {
        this.toDestroy = false;
        GameObject o = GameObject.Find("Text");
        this.health = GameObject.Instantiate(o, this.transform.position, Quaternion.identity);
        this.health.transform.name = "Enemy Health";
        // this.health.transform.parent = GameObject.Find("Canvas").transform;
        this.health.transform.SetParent(GameObject.Find("Canvas").transform);
        this.hp = health;
        this.cam = cam;
        this.path = path;
        this.currPos = 1;
        // InvokeRepeating("move", 1f, 1f);
    }
    private void Update()
    {
        move();
    }

    void move()
    {
        if (currPos == path.Count - 1) { this.toDestroy = true; }
        if (toDestroy)
        {
            Destroy(this.gameObject);
            Destroy(this.health);
            GameObject.Find("Main").GetComponent<MapGenerator>().addMoney(5);
        }
        // this.transform.position = this.path[this.currPos] + Vector3.one * 0.5f;
        this.transform.position = Vector3.MoveTowards(this.transform.position, this.path[this.currPos] + Vector3.one * 0.5f, Time.deltaTime);
        this.health.transform.position = cam.WorldToScreenPoint(this.transform.position);
        this.health.GetComponent<TextMeshProUGUI>().text = "Health: " + this.hp;
        // yield return new WaitForSecondsRealtime(1);
        // Debug.Log(Vector3.Distance(this.transform.position, this.path[this.currPos] + Vector3.one * 0.5f));
        if (Vector3.Distance(this.transform.position, this.path[this.currPos] + Vector3.one * 0.5f) < 0.01f)
        {
            this.currPos++;
        }
    }

    public int getPos()
    {
        return this.currPos;
    }
    public IEnumerator damage(int amount)
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (!this.toDestroy)
        {
            this.hp -= amount;
            this.health.GetComponent<TextMeshProUGUI>().text = "Health: " + this.hp;
            if (this.hp <= 0) { this.toDestroy = true; }
        }
    }
}
