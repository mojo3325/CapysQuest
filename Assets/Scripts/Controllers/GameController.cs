using System;
using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static event Action<float> OnTimeChanged;
    public static event Action OnTimeLost;

    [SerializeField] private GameObject _followTarget;
    [SerializeField] private AudioClip _bloodSound;
    [SerializeField] private AudioClip _waterSound;
    [SerializeField] private AudioClip _boosterPick;
    [SerializeField] private AudioClip _timeBoosterSound;
    [SerializeField] private AudioClip _helmetSound;
    [SerializeField] private Transform spawnPoint;


    private AudioSource _audioSource;
    private Camera _camera;
    private Coroutine _timeCountCoroutine;
    private bool _shouldFollow = false;
    private float _timeCount = 5f;
    private Color _nightColor = new Color(0x44 / 255f, 0x03 / 255f, 0x32 / 255f, 0f);
    private Color _dayColor = new Color(0xDA / 255f, 0xE9 / 255f, 0xD8 / 255f, 0f);

    public void OnPlayClick()
    {
        if (_timeCountCoroutine != null)
           StopCoroutine( _timeCountCoroutine );
        if (_followTarget.transform.position != spawnPoint.position)
        {
            _followTarget.SetActive(false);
            CapyToSpawn();
        }
            
        _timeCount = 555f;
        _shouldFollow = true;
        _followTarget.SetActive(true);
        _timeCountCoroutine = StartCoroutine(TimeCount());
        if (_camera.backgroundColor != _dayColor)
            StartCoroutine(SmoothBackgroundTransition(_dayColor, 1f));
        OnTimeChanged?.Invoke(_timeCount);
    }

    public void OnCapyDie(DieType dieType, Vector3 position)
    {
        if(_timeCountCoroutine!= null)
        {
            StopCoroutine(_timeCountCoroutine);
        }
        _shouldFollow = false;

        if (dieType == DieType.Enemy || dieType == DieType.Timer)
            _audioSource.PlayOneShot(_bloodSound);
        
        if (dieType == DieType.River)
            _audioSource.PlayOneShot(_waterSound);

        _followTarget.SetActive(false);
        CapyToSpawn();
    }

    IEnumerator TimeCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            _timeCount-=1;
            OnTimeChanged?.Invoke(_timeCount);

            if(_timeCount == 0)
            {
                OnTimeLost?.Invoke();
                yield break;
            }
        }
    }

    private void CapyToSpawn()
    {
        _followTarget.transform.position = spawnPoint.position;

        if (_followTarget.transform.localScale.x != 2)
        {
            _followTarget.transform.localScale = new Vector3(2, _followTarget.transform.localScale.y, _followTarget.transform.localScale.z);
        }
    }



    private void AddTime()
    {
        _timeCount = 6;
        _audioSource.PlayOneShot(_timeBoosterSound);
        OnTimeChanged?.Invoke(_timeCount);
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

    private void AddTimeByZone(ZoneType zoneType)
    {
        _timeCount = 6;
        OnTimeChanged?.Invoke(_timeCount);

        if(zoneType == ZoneType.zone_3)
        {
            StartCoroutine(SmoothBackgroundTransition(_nightColor, 1f));
        }   
    }

    private void PlayBoosterClaim()
    {
        _audioSource.PlayOneShot(_boosterPick);
    }

    private void PlayHelmetHit()
    {
        _audioSource.PlayOneShot(_helmetSound);
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

    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state == SoundState.On) ? false : true;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnCapyDied += OnCapyDie;
        CapyCharacter.OnTimeClaimed += AddTime;
        MenuBarController.SoundChanged += SoundTurn;
        BoosterController.OnBoosterClaimed += PlayBoosterClaim;
        CapyCharacter.OnEnemyTouchedWithHelm += PlayHelmetHit;
        CapyCharacter.OnZoneAchieved += AddTimeByZone;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= OnPlayClick;
        CapyCharacter.OnCapyDied -= OnCapyDie;
        CapyCharacter.OnTimeClaimed -= AddTime;
        MenuBarController.SoundChanged -= SoundTurn;
        BoosterController.OnBoosterClaimed -= PlayBoosterClaim;
        CapyCharacter.OnEnemyTouchedWithHelm -= PlayHelmetHit;
        CapyCharacter.OnZoneAchieved -= AddTimeByZone;
    }
}