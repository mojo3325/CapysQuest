using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector3 startPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += ResetPhysics;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= ResetPhysics;
    }

    private void ResetPhysics()
    {
        rb.velocity = Vector2.zero;
        transform.localPosition = startPosition;
    }
}
