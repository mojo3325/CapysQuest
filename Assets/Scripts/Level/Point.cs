using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] private AudioClip pointClip;
    private AudioSource audioSource;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy")
        {
            audioSource.PlayOneShot(pointClip);
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
