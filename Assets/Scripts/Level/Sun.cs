using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Transform followTarget;

    private void Start()
    {
        Application.targetFrameRate = 60; 
    }

    private void FixedUpdate()
    {
        if (followTarget.gameObject.activeSelf)
        {
            var followOffset = new Vector3(30, 0, 0);
        
            transform.position = new Vector3(followTarget.position.x + followOffset.x, followTarget.position.y + 10, transform.position.z);
        }
    }
}
