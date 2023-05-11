using System.Collections;
using UnityEngine;

public class GameCameraController : MonoBehaviour
{
    [Header("Расположение")]
    [SerializeField] private GameObject _followTarget;
    [SerializeField] private Transform spawnPoint;

    private Camera _camera;
    private bool _shouldFollow = false;

    private Color _nightColor = new(0x44 / 255f, 0x03 / 255f, 0x32 / 255f, 0f);
    private Color _dayColor = new(0xDA / 255f, 0xE9 / 255f, 0xD8 / 255f, 0f);

    private DeviceType _deviceType; 
    [SerializeField] private DeviceController _deviceController;
    
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

            var scale = _followTarget.transform.localScale.x;

            Vector3 followOffset()
            {
                switch (_deviceType)
                {
                    case DeviceType.Tablet when scale == -2: return new Vector3(-20, 0, 0);
                    case DeviceType.Tablet when scale == 2 : return new Vector3(20, 0, 0);
                    case DeviceType.Phone when scale == -2 : return new Vector3(-25, 0, 0);
                    case DeviceType.Phone when scale == 2 : return new Vector3(25, 0, 0);
                }

                return default;
            }
            
            var moveSpeed = 5.0f;
            var capyPosition = _followTarget.transform.position;
            Vector3 targetPosition = new Vector3(capyPosition.x, capyPosition.y, transform.position.z) + followOffset();
            transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60;
        
        if (_deviceController.DeviceType == null)
            _deviceController.SyncDeviceType();

        _deviceType = _deviceController.DeviceType;
        
        if (_deviceType == DeviceType.Tablet)
            _camera.orthographicSize = 32;
        if(_deviceType == DeviceType.Phone)
            _camera.orthographicSize = 19;
    }

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        MenuBar.PlayButtonClicked += OnPlayClick;
        CapyCharacter.OnCapyDied += OnCapyDie;
        ZoneController.OnZoneAchieved += ChangeGameBackground;
    }

    private void OnDisable()
    {
        MenuBar.PlayButtonClicked -= OnPlayClick;
        CapyCharacter.OnCapyDied -= OnCapyDie;
        ZoneController.OnZoneAchieved -= ChangeGameBackground;
    }
}