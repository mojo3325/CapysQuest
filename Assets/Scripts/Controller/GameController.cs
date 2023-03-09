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
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject GameOverMenu;

    private AudioSource audioSource;
    private Coroutine timeCountCoroutine;
    private bool _shouldFollow = false;
    private bool _isMute = false;
    public float _timeCount = 5f;
    private bool _isTimeOver = false;
    private bool _isDead = false;

    private bool _zone2Achieved = false;
    private bool _zone3Achieved = false;
    private bool _zone4Achieved = false;
    private bool _finishAchieved = false;

    public bool Zone2Achieved
    {
        get { return _zone2Achieved; }
        set { _zone2Achieved = value; }
    }

    public bool Zone3Achieved
    {
        get { return _zone3Achieved; }
        set { _zone3Achieved = value; }
    }

    public bool Zone4Achieved
    {
        get { return _zone4Achieved; }
        set { _zone4Achieved = value; }
    }

    public bool FinishAchieved
    {
        get { return _finishAchieved; }
        set { _finishAchieved = value; }
    }
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
        _timeCount = 9999f;
        _isDead = false;
        _isTimeOver = false;
        ResetAllAchivements();
        _shouldFollow = true;
        followTarget.SetActive(true);
        MainMenu.SetActive(false);
        MenuUI.SetActive(false);
        GameUI.SetActive(true);
        timeCountCoroutine = StartCoroutine(TimeCountCoroutine());
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
        StartCoroutine(ShowMenuAfterDie());
    }


    IEnumerator TimeCountCoroutine()
    {
        while (_timeCount >= 1)
        {
            yield return new WaitForSeconds(2f);
            _timeCount-=1;
        }
    }

    private IEnumerator ShowMenuAfterDie()
    {
        yield return new WaitForSeconds(1.5f);

        GameUI.SetActive(false);
        MenuUI.SetActive(true);
        GameOverMenu.SetActive(true);
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
            OnCapyDie(DieType.Timer);
        }
    }

    private void ResetAllAchivements()
    {
        _zone2Achieved = false;
        _zone3Achieved = false;
        _zone4Achieved = false;
        _finishAchieved = false;
    }

    private void Update()
    {
        if(audioSource != null)
        {
            audioSource.mute = _isMute;
        }
    }

    public void SoundTurn()
    {
        IsMute = !IsMute;
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
