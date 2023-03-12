using System.Collections;
using UnityEngine;

public enum DieType
{
    Enemy, River, Timer
}

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject waterPrefab;

    private AudioSource audioSource;
    private Coroutine timeCountCoroutine;
    private bool _shouldFollow = false;
    private bool _isMute = false;
    public float _timeCount = 5f;
    private bool _isTimeOver = false;
    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
        set { _isDead = value; }
    }

    public bool IsTimeOver
    {
        get { return _isTimeOver; }
        set { _isTimeOver = value; }
    }
    public float TimeCount
    {
        get { return _timeCount; }
        set { _timeCount = value; }
    }
    public bool IsMute
    {
        get { return _isMute; }
        set { _isMute = value; }
    }

    public void OnPlayClick()
    {
        if (timeCountCoroutine != null)
        {
            StopCoroutine( timeCountCoroutine );
        }
        _timeCount = 55f;
        _isDead = false;
        _isTimeOver = false;
        _shouldFollow = true;
        followTarget.SetActive(true);
        timeCountCoroutine = StartCoroutine(TimeCountCoroutine());
        EventManager.OnTimeChange.Invoke(_timeCount);
    }

    public void OnCapyDie(DieType dieType)
    {
        _isDead = true;
        if(timeCountCoroutine!= null)
        {
            StopCoroutine(timeCountCoroutine);
        }
        _shouldFollow = false;
        var capyPosition = followTarget.transform.position;
        if (dieType == DieType.Enemy || dieType == DieType.Timer)

            BloodSpawn(capyPosition);

        if (dieType == DieType.River)

            WaterSpawn(capyPosition);

        followTarget.SetActive(false);
        CapyToSpawn();
    }

    IEnumerator TimeCountCoroutine()
    {
        while (_timeCount != 0)
        {
            yield return new WaitForSeconds(2f);
            _timeCount-=1;
            EventManager.OnTimeChange.Invoke(_timeCount);
        }
    }

    private void CapyToSpawn()
    {
        followTarget.transform.position = spawnPoint.position;
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

    private void AddTimeByPoint()
    {
        _timeCount = 6;
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

        if (_timeCount <= 0 && _isTimeOver == false)
        {
            _isTimeOver = true;
            EventManager.OnTimeLost.Invoke();
            EventManager.OnCapyDie.Invoke(DieType.Timer);
        }
    }

    public void SoundMute()
    {
        EventManager.OnSoundMuteClick.Invoke();
        audioSource.mute = true; 
    }

    public void SoundUnmute()
    {
        EventManager.OnSoundUnmuteClick.Invoke();
        audioSource.mute = false;
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
        EventManager.OnPointReached.AddListener(AddTimeByPoint);
    }
}
