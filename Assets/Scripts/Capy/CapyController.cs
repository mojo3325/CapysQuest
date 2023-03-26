using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class CapyController : MonoBehaviour
{
    public static event Action<float> OnTimeChanged;
    public static event Action OnTimeLost;
    public static event Action CapyDiedThreeTimes;

    [Header("Звуки")]
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;

    [Header("Capy")]
    [SerializeField] private CapyCharacter capy;

    private SoundState soundState;

    private int CapyDieCount;

    private float _timeCount = 5f;
    private Coroutine _timeCountCoroutine;


    private void OnEnable()
    {
        CapyCharacter.OnCapyDied += OnCapyDied;
        CapyCharacter.OnCapyDied += CountCapyDies;
        IntAdController.OnAdSuccessClosed += ResetDieCount;
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnTimeClaimed += AddTime;
        ZoneController.OnZoneAchieved += AddTimeByZone;
        MenuBarController.SoundChanged += SoundTurn;
        CapyCharacter.CapyEnabled += SetupCapyAudio;
    }

    private void OnDisable()
    {
        CapyCharacter.OnCapyDied -= OnCapyDied;
        CapyCharacter.OnCapyDied -= CountCapyDies;
        IntAdController.OnAdSuccessClosed -= ResetDieCount;
        MenuBar.PlayButtonClicked -= OnPlayClick;
        CapyCharacter.OnTimeClaimed -= AddTime;
        ZoneController.OnZoneAchieved -= AddTimeByZone;
        MenuBarController.SoundChanged -= SoundTurn;
        CapyCharacter.CapyEnabled -= SetupCapyAudio;
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
        {
            StopCoroutine(_timeCountCoroutine);
        }

        ParticleSpawn(dieType, position);
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

    private void SetupCapyAudio()
    {
        capy.audioSource.mute = (soundState == SoundState.On) ? false : true;
    }


    private void CountCapyDies(DieType D, Vector3 V)
    {
        if(CapyDieCount >= 2)
        {
            CapyDiedThreeTimes?.Invoke();
        }
        else
        {
            CapyDieCount += 1;
        }
    }

    private void ResetDieCount()
    {
        CapyDieCount = 0;
    }

    private void ParticleSpawn(DieType dieType, Vector3 targetPosition)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * 80f;
            Vector3 position = new Vector3(targetPosition.x, targetPosition.y + 5, 0);
            GameObject particle = Instantiate((dieType == DieType.Enemy || dieType == DieType.Timer) ? bloodPrefab : waterPrefab, position, Quaternion.identity);
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

            if (_timeCount == 0)
            {
                OnTimeLost?.Invoke();
                yield break;
            }
        }
    }
}
