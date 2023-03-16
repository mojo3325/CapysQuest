using System;
using UnityEngine;

public class Zone : MonoBehaviour
{
    private AudioSource audioSource;
    private bool _isChecked;

    private void OnEnable()
    {
        EventManager.OnSoundChangeClick.AddListener(SoundTurn);
        EventManager.OnPlayClick.AddListener(ResetZoneState);
    }

    private void ResetZoneState()
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy" && _isChecked == false)
        {

            _isChecked = true;
            audioSource.Play();

            System.Random random = new System.Random();
            int randomNumber = random.Next(8, 64); // √енерируем число от 8 до 63 включительно (двузначное в восьмеричной системе)

            string octalNumber = Convert.ToString(randomNumber, 8); // ѕреобразуем число в восьмеричную систему

            Console.WriteLine("—лучайное двузначное число в восьмеричной системе: {0}", octalNumber);

        }
    }

    private void SoundTurn(SoundState state)
    {
        audioSource.mute = (state == SoundState.Off) ? true : false ;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
