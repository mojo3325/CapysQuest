using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{

    private AudioSource audioSource;
    public GameController gameController;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy" && gameController.IsMute == false) 
        {
            audioSource.Play();
        }
    }
}
