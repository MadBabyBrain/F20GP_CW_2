using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[SerializeField]
public class Enemy : MonoBehaviour
{
    public float health;
    private float maxHealth;
    public float speed;

    public float waitTime;
    public float distMoved;

    

    public IEnumerator _init_(EnemyScriptableObject _stats, List<Vector3> _path, float waitTime)
    {
        path = _path;
        stats = _stats;
        this.waitTime = waitTime;
        this.distMoved = 0f;

        // set enemy parameters
        health = stats.hp;
        this.maxHealth = stats.hp;
        speed = stats.speed;

        // metadata
        pathIndex = 1;
        initialised = true;

        GameObject o = GameObject.Find("HealthBar");
        this.healthBar = GameObject.Instantiate(o, this.transform.position, Quaternion.identity);
        this.healthBar.transform.name = "Enemy Health";
        //this.healthText.GetComponent<TextMeshProUGUI>().fontSize = 30;
        this.healthBar.transform.SetParent(GameObject.Find("Canvas").transform);
        this.healthBar.SetActive(false);

        this.cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        yield return new WaitForSecondsRealtime(waitTime);
        this.transform.position = this.path[0] + Vector3.one * 0.5f;
        this.healthBar.SetActive(true);
    }

    private void move()
    {
        if (pathIndex == path.Count - 1)
        {
            GameObject.Find("HomeBase").GetComponent<HomeBaseLogic>().takeDamage(1);
            Destroy(this.healthBar);
            Destroy(gameObject);
            return;
        }

        // TODO: Terrain offset by 1/2 in every direction, fix in implementation
        // gameObject.transform.position = path[pathIndex++];

        this.transform.position = Vector3.MoveTowards(this.transform.position, this.path[this.pathIndex] + Vector3.one * 0.5f, this.speed * Time.deltaTime);
        this.transform.LookAt(new Vector3(this.path[this.pathIndex].x + 0.5f, this.transform.position.y, this.path[this.pathIndex].z + 0.5f), Vector3.up);
        this.transform.Rotate(new Vector3(0, 90, 0));

        this.distMoved += this.speed * Time.deltaTime;

        this.healthBar.transform.position = cam.WorldToScreenPoint(this.transform.position);
        this.healthBar.transform.Find("GreenHealth").gameObject.GetComponent<Image>().fillAmount = (float)this.health/(float)this.maxHealth;
        this.healthBar.transform.Find("HealthText").gameObject.GetComponent<TextMeshProUGUI>().text = "HP: " + this.health + "/" + this.maxHealth;

        if (Vector3.Distance(this.transform.position, this.path[this.pathIndex] + Vector3.one * 0.5f) < 0.01f)
        {
            this.pathIndex++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // make sure object has been initialised
        if (!initialised)
        {
            Debug.Log("Please initialise object with _init_() call");
            return;
        }

        if (this.waitTime <= 0)
        {
            move();
        }
        else
        {
            this.waitTime -= Time.deltaTime;
        }
        // InvokeRepeating("move", 0f, speed);
    }

    public void takeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && this.alive)
        {
            this.alive = false;
            GameObject.Find("Main").GetComponent<MapGenerator>().addMoney(5);
            Destroy(this.healthBar);
            Destroy(gameObject);
        }
    }

    // returns the number of elements left to traverse in the path
    public float getpos()
    {
        return this.distMoved;
    }

    // private members
    public EnemyScriptableObject stats;
    public List<Vector3> path;
    public int pathIndex;
    public bool initialised = false;   // used to determine if _init_() has been called and member fields have been set
    public GameObject healthBar;
    public Camera cam;
    public bool alive = true;

}
