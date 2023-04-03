using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateManager : MonoBehaviour
{
    public float moveSpeed;
    public InputAction up;
    public InputAction left;
    public InputAction down;
    public InputAction right;
    public InputAction shoot;
    public InputAction jump;
    
    public Rigidbody rb;

    //states
    BaseState state;
    AttackState attackState;

    //weapons
    public BaseWeapon weapon;
    Pistol pistol;
    AssaultRifle assaultRifle;

    Camera camera;
    public LineRenderer line;
    public LayerMask layermask;

    public Vector3 jumpBoxSize; 

    public PlayerStateManager(){
        attackState = new AttackState(this);
        pistol = new Pistol(this);
        assaultRifle = new AssaultRifle(this);
    }

    void OnEnable(){
        up.Enable();
        left.Enable();
        down.Enable();
        right.Enable();
        shoot.Enable();
        jump.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        state = attackState;
        weapon = assaultRifle;
    }

    // Update is called once per frame
    void Update()
    {
        state.Update();
    }

    public Vector3 getMousePos(){
        // get raw mouse position and convert it to a world point
        Vector3 rawMousePos = Input.mousePosition;
        camera = GameObject.Find("PlayerCamera").GetComponent<Camera>();
        float dist = Vector3.Distance(transform.position, camera.transform.position);
        return camera.ScreenToWorldPoint(new Vector3(rawMousePos.x, rawMousePos.y, dist));
    }

    public bool isGrounded(){
        if(Physics.BoxCast(transform.position, jumpBoxSize, -transform.up, transform.rotation, 1, layermask)){
            return true;
        }
        return false;
    }
}
