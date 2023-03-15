using UnityEngine;

public class Zone : MonoBehaviour
{
    private AudioSource audioSource;
    private bool _isChecked;

    private void OnEnable()
    {
        EventManager.OnSoundChangeClick.AddListener(SoundTurn);
        EventManager.OnCapyDie.AddListener(ResetZoneState);
    }

    private void ResetZoneState(DieType die)
    {
        _isChecked = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Capy" && _isChecked == false)
        {
            _isChecked = true;
            audioSource.Play();
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
