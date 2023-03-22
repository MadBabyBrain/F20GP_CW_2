using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackState : BaseState
{
    PlayerStateManager sm;
    Vector3 moveDirection;

    public AttackState(PlayerStateManager sm){
        this.sm = sm;
    }
    public override void EnterState(PlayerStateManager sm){
        sm.line.positionCount = 2;

    }


    public override void Update(){
       
        move();
        drawLine();
        if(sm.shoot.phase == InputActionPhase.Started){
            sm.weapon.Attack();
        }
    }

    // TODO: fix this
    private void drawLine(){
        sm.line.SetPosition(0,sm.transform.position);
        Vector3 endPos = sm.getMousePos();
        endPos.y=sm.transform.position.y;
        
        sm.line.SetPosition(1, endPos);
    }
 
    private void move(){
        moveDirection = getMoveDirection();
        sm.rb.velocity = new Vector3(moveDirection.x * sm.moveSpeed, sm.rb.velocity.y, moveDirection.z * sm.moveSpeed);
       
        if(sm.jump.phase == InputActionPhase.Started && sm.isGrounded()){
            sm.rb.AddForce(sm.transform.up * 10, ForceMode.Impulse);
        }
        
        
    }

    private Vector3 getMoveDirection(){
        float vertical=0;
        float horizontal=0;
        if(sm.up.phase == InputActionPhase.Started){
            vertical +=1;
        }
        if(sm.down.phase == InputActionPhase.Started){
            vertical -=1;
        }
        if(sm.left.phase == InputActionPhase.Started){
            horizontal -=1;
        }
        if(sm.right.phase == InputActionPhase.Started){
            horizontal +=1;
        }

        return new Vector3(horizontal, 0, vertical);
    }

}
