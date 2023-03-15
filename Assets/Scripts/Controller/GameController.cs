using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField] private AudioClip bloodSound;
    [SerializeField] private AudioClip waterSound;
    [SerializeField] private AudioClip boosterPick;
    [SerializeField] private AudioClip timeBoosterSound;

    private AudioSource audioSource;
    private Coroutine timeCountCoroutine;
    private bool _shouldFollow = false;
    private float _timeCount = 5f;

    public float TimeCount
    {
        get { return _timeCount; }
        set { _timeCount = value; }
    }
    public void OnPlayClick()
    {
        if (timeCountCoroutine != null)
        {
            StopCoroutine( timeCountCoroutine );
        }
        _timeCount = 5555f;
        _shouldFollow = true;
        followTarget.SetActive(true);
        timeCountCoroutine = StartCoroutine(TimeCountCoroutine());
        EventManager.OnTimeChange.Invoke(_timeCount);
    }

    public void OnCapyDie(DieType dieType)
    {
        if(timeCountCoroutine!= null)
        {
            StopCoroutine(timeCountCoroutine);
        }
        _shouldFollow = false;
        var capyPosition = followTarget.transform.position;

        if (dieType == DieType.Enemy || dieType == DieType.Timer)
        {
            audioSource.PlayOneShot(bloodSound);
            BloodSpawn(capyPosition);
        }

        if (dieType == DieType.River)
        {
            audioSource.PlayOneShot(waterSound);
            WaterSpawn(capyPosition);
        }


        followTarget.SetActive(false);
        CapyToSpawn();
    }

    IEnumerator TimeCountCoroutine()
    {
        while (_timeCount != 0)
        {
            yield return new WaitForSeconds(3f);
            _timeCount-=1;
            EventManager.OnTimeChange.Invoke(_timeCount);

            if(_timeCount == 0)
            {
                EventManager.OnTimeLost.Invoke();
                EventManager.OnCapyDie.Invoke(DieType.Timer);
                yield break;
            }
        }
    }

    private void CapyToSpawn()
    {
        followTarget.transform.position = spawnPoint.position;

        if (followTarget.transform.localScale.x != 2)
        {
            followTarget.transform.localScale = new Vector3(2, followTarget.transform.localScale.y, followTarget.transform.localScale.z);
        }
    }

    private void BloodSpawn(Vector3 targetPosition)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * 80f;
            Vector3 position = new Vector3(targetPosition.x, targetPosition.y + 5, 0);
            GameObject blood = Instantiate(bloodPrefab, position, Quaternion.identity);
            blood.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
    }

    private void WaterSpawn(Vector3 targetPosition)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f)).normalized * 80f;
            Vector3 position = new Vector3(targetPosition.x, targetPosition.y + 5, 0);
            GameObject water = Instantiate(waterPrefab, position, Quaternion.identity);
            water.GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
    }

    private void AddTime()
    {
        _timeCount = 6;
        audioSource.PlayOneShot(timeBoosterSound);
        EventManager.OnTimeChange.Invoke(_timeCount);
    }

    private void AddTimeByZone(ZoneType zoneType)
    {
        _timeCount = 6;
        EventManager.OnTimeChange.Invoke(_timeCount);
    }

    private void PlayBoosterPick()
    {
        audioSource.PlayOneShot(boosterPick);
    }

    private void FixedUpdate()
    {

        if (_shouldFollow && followTarget != null)
        {
            float moveSpeed = 5.0f;
            Vector3 capyPosition = followTarget.transform.position;
            Vector3 followOffset = followTarget.transform.localScale.x == -2 ? new Vector3(-25, 0, 0) : new Vector3(25, 0, 0);
            Vector3 targetPosition = new Vector3(capyPosition.x, capyPosition.y, transform.position.z) + followOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void SoundTurn(SoundState state)
    {
        audioSource.mute = (state == SoundState.On) ? false : true;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        EventManager.OnPlayClick.AddListener(OnPlayClick);
        EventManager.OnCapyDie.AddListener(OnCapyDie);
        EventManager.OnTimeClimed.AddListener(AddTime);
        EventManager.OnSoundChangeClick.AddListener(SoundTurn);
        EventManager.OnBoosterPick.AddListener(PlayBoosterPick);
        EventManager.OnZoneAchieved.AddListener(AddTimeByZone);
    }
}
