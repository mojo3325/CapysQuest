using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

public class CapyController : MonoBehaviour
{
    public static event Action<float> OnTimeChanged;
    public static event Action OnTimeLost;
    public static event Action CapyDiedThreeTimes;

    [Header("Эффекты")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;

    [Header("Capy")]
    [SerializeField] private CapyCharacter capy;

    [Header("Лэеры уровня")]
    [SerializeField] private LayerMask levelLayer;
    [SerializeField] private LayerMask iceLevelLayer;

    [Header("Показатели Кэпи")]
    [SerializeField] private bool _isActiveJetpack = false;
    [SerializeField] private bool _isActiveHelmet = false;
    [SerializeField] private float _timeCount = 5f;
    [SerializeField] private int CapyDieCount;


    public LayerMask LevelLayer => levelLayer;
    public LayerMask IceLevelLayer => iceLevelLayer;

    public bool IsActiveJetpack { get { return _isActiveJetpack; }}
    public bool IsActiveHelmet { get { return _isActiveHelmet; } }

    [Header("Звук")]
    [SerializeField] private SoundState soundState;


    [Header("Коорутины")]
    private Coroutine _timeCountCoroutine;
    private Coroutine _jetpackCoroutine;


    private void OnEnable()
    {
        // Capy CHARACTER Died

        CapyCharacter.OnCapyDied += OnCapyDied;
        CapyCharacter.OnCapyDied += CountCapyDies;

        // Capy Enabled
        CapyCharacter.CapyEnabled += SetupCapy;

        //Capy boosters pick
        CapyCharacter.TimeClaimed += AddTime;

        //Ad controller
        IntAdController.OnAdSuccessClosed += ResetDieCount;

        //Menu bar controller
        MenuBar.PlayButtonClicked += OnPlayClick;
        SettingsController.SoundChanged += SoundTurn;

        //Zone Controller
        ZoneController.OnZoneAchieved += AddTimeByZone;

        //Capy Behaviour Events
        CapyCharacter.CapyHelmetEnemyTouched += OnTouchWithHelmet;
        CapyCharacter.JetpackClaimed += OnJetpackClaimed;
        CapyCharacter.HelmetClaimed += OnHelmetClaimed;
    }

    private void OnDisable()
    {
        // Capy CHARACTER Died

        CapyCharacter.OnCapyDied -= OnCapyDied;
        CapyCharacter.OnCapyDied -= CountCapyDies;

        // Capy Enabled
        CapyCharacter.CapyEnabled -= SetupCapy;

        //Capy boosters pick
        CapyCharacter.TimeClaimed -= AddTime;

        //Ad controller
        IntAdController.OnAdSuccessClosed -= ResetDieCount;

        //Menu bar controller
        MenuBar.PlayButtonClicked -= OnPlayClick;
        SettingsController.SoundChanged -= SoundTurn;

        //Zone Controller
        ZoneController.OnZoneAchieved -= AddTimeByZone;

        //Capy Behaviour Events
        CapyCharacter.CapyHelmetEnemyTouched -= OnTouchWithHelmet;
        CapyCharacter.JetpackClaimed -= OnJetpackClaimed;
        CapyCharacter.HelmetClaimed -= OnHelmetClaimed;
    }

    public void OnPlayClick()
    {
        if (_timeCountCoroutine != null)
            StopCoroutine(_timeCountCoroutine);

        _timeCount = 5f;
        _timeCountCoroutine = StartCoroutine(TimeCount());
        OnTimeChanged?.Invoke(_timeCount);
    }


    private void OnCapyDied(DieType dieType, Vector3 position)
    {
        if (_timeCountCoroutine != null)
            StopCoroutine(_timeCountCoroutine);
        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);

        ParticleSpawn(dieType, position);
    }

    private void CountCapyDies(DieType D, Vector3 V)
    {
        if (CapyDieCount >= 2)
        {
            CapyDiedThreeTimes?.Invoke();
        }
        else
        {
            CapyDieCount += 1;
        }
    }

    private void ResetCapyState()
    {
        ResetCapyAnimations();
        ResetCapyBoosters();
    }

    private void ResetCapyAnimations()
    {
        capy.Animator.SetBool("IsRunning", true);
    }

    private void ResetCapyBoosters()
    {
        _isActiveJetpack = false;
        _isActiveHelmet = false;

        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);
    }

    private void AddTime()
    {
        _timeCount = 6;
        OnTimeChanged?.Invoke(_timeCount);
    }

    private void AddTimeByZone(ZoneType zone)
    {
        _timeCount = 6;
        OnTimeChanged?.Invoke(_timeCount);
    }

    private void SoundTurn(SoundState state)
    {
        soundState = state;
    }

    private void SetupCapy()
    {
        capy.AudioSource.mute = (soundState == SoundState.On) ? false : true;
        ResetCapyState();
    }

    private void ResetDieCount()
    {
        CapyDieCount = 0;
    }

    private void ParticleSpawn(DieType dieType, Vector3 targetPosition)
    {
        for (var i = 0; i < 15; i++)
        {
            var direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * 80f;
            var position = new Vector3(targetPosition.x, targetPosition.y + 5, 0);
            var particle = Instantiate(dieType == DieType.Enemy ? bloodPrefab : waterPrefab, position, Quaternion.identity);
            particle.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
    }

    private IEnumerator TimeCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            _timeCount -= 1;
            OnTimeChanged?.Invoke(_timeCount);

            if (_timeCount <= 0)
            {
                OnTimeLost?.Invoke();
                yield break;
            }
        }
    }

    private IEnumerator JetpackOffAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isActiveJetpack = false;
        capy.Animator.SetBool("IsRunning", true);
    }

    private void OnTouchWithHelmet()
    {
        _isActiveHelmet = false;
        capy.Animator.SetBool("IsRunning", true);
    }

    private void OnJetpackClaimed(float delay)
    {
        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);

        _isActiveHelmet = false;
        _isActiveJetpack = true;
        capy.Animator.SetBool("IsRunning", false);
        capy.Animator.SetTrigger("Jetpack");
        _jetpackCoroutine = StartCoroutine(JetpackOffAfter(delay));
    }

    private void OnHelmetClaimed()
    {
        if (_jetpackCoroutine != null)
            StopCoroutine(_jetpackCoroutine);

        _isActiveJetpack = false;
        _isActiveHelmet = true;
        capy.Animator.SetBool("IsRunning", false);
        capy.Animator.SetTrigger("Helmet");
    }
}
