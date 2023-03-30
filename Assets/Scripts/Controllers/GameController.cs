using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Расположение")]
    [SerializeField] private GameObject _followTarget;
    [SerializeField] private Transform spawnPoint;

    [Header("Звуки смерти")]
    [SerializeField] private AudioClip _defaultDieSound;
    [SerializeField] private AudioClip _waterDieSound;
    [SerializeField] private AudioClip _timeLostSound;



    //HACK

    [SerializeField] private Transform Zone1;
    [SerializeField] private Transform Zone2;
    [SerializeField] private Transform Zone3;
    [SerializeField] private Transform Zone4;
    [SerializeField] private Transform Finish;



    //HACK





    private Camera _camera;
    private AudioSource _audioSource;
    private bool _shouldFollow = false;

    private Color _nightColor = new Color(0x44 / 255f, 0x03 / 255f, 0x32 / 255f, 0f);
    private Color _dayColor = new Color(0xDA / 255f, 0xE9 / 255f, 0xD8 / 255f, 0f);

    private void OnPlayClick()
    {
        if (_followTarget.transform.position != spawnPoint.position)
        {
            _followTarget.SetActive(false);
            CapyToSpawn();
        }
            
        _shouldFollow = true;
        _followTarget.SetActive(true);
        if (_camera.backgroundColor != _dayColor)
            StartCoroutine(SmoothBackgroundTransition(_dayColor, 1f));
    }

    private void OnCapyDie(DieType dieType, Vector3 position)
    {
        if (dieType == DieType.Enemy)
            PlayDefaultDieSound();
        if (dieType == DieType.River)
            PlayWaterDieSound();
        
        _shouldFollow = false;
        _followTarget.SetActive(false);
        CapyToSpawn();
    }


    private void CapyToSpawn()
    {
        _followTarget.transform.position = spawnPoint.position;
        _followTarget.transform.localRotation = Quaternion.identity;
        if (_followTarget.transform.localScale.x != 2)
        {
            _followTarget.transform.localScale = new Vector3(2, _followTarget.transform.localScale.y, _followTarget.transform.localScale.z);
        }
    }

    private IEnumerator SmoothBackgroundTransition(Color targetColor, float duration)
    {
        float t = 0f;
        Color startColor = _camera.backgroundColor;

        while (t < duration)
        {
            t += Time.deltaTime;
            _camera.backgroundColor = Color.Lerp(startColor, targetColor, t / duration);
            yield return null;
        }
    }

    private void ChangeGameBackground(ZoneType zoneType)
    {
        if(zoneType == ZoneType.zone_3)
        {
            StartCoroutine(SmoothBackgroundTransition(_nightColor, 1f));
        }   
    }

    private void FixedUpdate()
    {
        if (_shouldFollow && _followTarget != null)
        {
            float moveSpeed = 5.0f;
            Vector3 capyPosition = _followTarget.transform.position;
            Vector3 followOffset = _followTarget.transform.localScale.x == -2 ? new Vector3(-25, 0, 0) : new Vector3(25, 0, 0);
            Vector3 targetPosition = new Vector3(capyPosition.x, capyPosition.y, transform.position.z) + followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnCapyDied += OnCapyDie;
        ZoneController.OnZoneAchieved += ChangeGameBackground;
        CapyController.OnTimeLost += PlayTimeLostSound;
        SettingsController.SoundChanged += SoundTurn;


        //HACK

        GameScreen.Zone1Clicked += ToZone1;
        GameScreen.Zone2Clicked += ToZone2;
        GameScreen.Zone3Clicked += ToZone3;
        GameScreen.Zone4Clicked += ToZone4;
        GameScreen.FinishClicked += ToFinish;


        //HACK
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= OnPlayClick;
        CapyCharacter.OnCapyDied -= OnCapyDie;
        ZoneController.OnZoneAchieved -= ChangeGameBackground;
        CapyController.OnTimeLost -= PlayTimeLostSound;
        SettingsController.SoundChanged -= SoundTurn;

        //HACK

        GameScreen.Zone1Clicked -= ToZone1;
        GameScreen.Zone2Clicked -= ToZone2;
        GameScreen.Zone3Clicked -= ToZone3;
        GameScreen.Zone4Clicked -= ToZone4;
        GameScreen.FinishClicked -= ToFinish;


        //HACK
    }

    //HACK


    void ToZone1()
    {
        _followTarget.transform.localPosition = Zone1.localPosition;
    }

    void ToZone2()
    {
        _followTarget.transform.localPosition = Zone2.localPosition;
    }

    void ToZone3()
    {
        _followTarget.transform.localPosition = Zone3.localPosition;
    }

    void ToZone4()
    {
        _followTarget.transform.localPosition = Zone4.localPosition;
        _followTarget.transform.localScale = _followTarget.transform.localScale = new Vector3(-2, _followTarget.transform.localScale.y, _followTarget.transform.localScale.z);
    }

    void ToFinish()
    {
        _followTarget.transform.localPosition = Finish.localPosition;
        _followTarget.transform.localScale = _followTarget.transform.localScale = new Vector3(-2, _followTarget.transform.localScale.y, _followTarget.transform.localScale.z);
    }


    //HACK

    private void PlayDefaultDieSound()
    {
        _audioSource.PlayOneShot(_defaultDieSound);
    }

    private void PlayWaterDieSound()
    {
        _audioSource.PlayOneShot(_waterDieSound);
    }

    private void PlayTimeLostSound()
    {
        _audioSource.PlayOneShot(_timeLostSound);
    }

    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state == SoundState.On) ? false : true;
    }
}