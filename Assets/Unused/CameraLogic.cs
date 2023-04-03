using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    
    public Transform playerTransform;
    
    public Vector3 posOffset;
    public Vector3 rotOffset;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position + posOffset;
        transform.localEulerAngles = rotOffset;
    }
}
