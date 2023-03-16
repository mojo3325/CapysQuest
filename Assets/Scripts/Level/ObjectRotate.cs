using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, 120 * Time.fixedDeltaTime);
    }

}
