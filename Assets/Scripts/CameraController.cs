using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Make the camera to follow the target point's position and rotation
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}
