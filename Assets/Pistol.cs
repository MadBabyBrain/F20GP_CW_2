using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pistol : BaseWeapon
{
    private PlayerStateManager sm;

    public float shootRange = 5;
    private float cooldown = 0.5f;
    private float damage = 10f;
    private float timeSinceLastShoot = 0;


    public Pistol(PlayerStateManager sm){
        this.sm = sm;
    }

    public override void Attack(){
        Vector3 screenMousePos = sm.getMousePos();
        Vector3 heading = screenMousePos - sm.transform.position;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        direction.y = sm.transform.forward.y;
       
        if(sm.shoot.phase == InputActionPhase.Started && (Time.time-timeSinceLastShoot) > cooldown){
            timeSinceLastShoot = Time.time;
            RaycastHit hit;
            if(Physics.Raycast(sm.transform.position, direction, out hit, shootRange)){
                if(hit.collider.tag == "Enemy"){
                    hit.collider.gameObject.GetComponent<EnemyStateManager>().takeDamage(damage);
                }
            }
        }
    }
}
