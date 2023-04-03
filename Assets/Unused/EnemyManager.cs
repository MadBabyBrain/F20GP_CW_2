using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public List<GameObject> turrets, enemies;
    public Collider[] es;

    public void run()
    {
        this.turrets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Turret"));
        InvokeRepeating("manage", 1f, 0.13f);
    }

    // private void Update()
    // {
    //     manage();
    // }
    private void manage()
    {
        // // print("Moving Enemies");
        // this.enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        // if (this.enemies.Count <= 0)
        // {
        //     CancelInvoke();
        // }
        // else
        // {
        //     foreach (GameObject t in this.turrets)
        //     {
        //         GameObject target = null;
        //         int max = int.MinValue;
        //         this.es = Physics.OverlapSphere(t.transform.position, 10f, LayerMask.GetMask("Enemies"));
        //         foreach (Collider e in es)
        //         {
        //             if (e.CompareTag("Enemy") && e.transform.gameObject)
        //             {
        //                 int pos = e.transform.GetComponent<Enemy>().getPos();
        //                 if (pos > max)
        //                 {
        //                     max = pos;
        //                     target = e.transform.gameObject;
        //                 }
        //             }
        //         }

        //         if (target != null)
        //         {
        //             t.transform.LookAt(new Vector3(target.transform.position.x, t.transform.position.y, target.transform.position.z), Vector3.up);
        //             t.transform.RotateAround(t.transform.position, Vector3.up, 90);
        //             StartCoroutine(target.GetComponent<Enemy>().damage(1));
        //         }
        //     }
        // }
    }
}
