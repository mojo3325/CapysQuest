using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CapyCharacter : MonoBehaviour
{
    public static event Action<DieType, Vector3> OnCapyDied;
    public static event Action ProgressPointReached;
    public static event Action HelmetClaimed;
    public static event Action<float> JetpackClaimed;
    public static event Action<string> OnCodeGenerated;
    public static event Action OnFinishAchieved;
    public static event Action CapyEnabled;
    public static event Action CapyHelmetEnemyTouched;
    
    [Header("CapyController")]
    [SerializeField] private CapyController controller;

    [Header("Показатели Кэпи")]
    [SerializeField] private bool _isGrounded;
    [SerializeField] private bool _isIceGrounded;

    [Header("Звуки Кэпи")]
    [SerializeField] private AudioClip _boosterPickSound;
    [SerializeField] private AudioClip _helmetLoseSound;
    [SerializeField] private AudioClip _jetpackSound;
    [SerializeField] private AudioClip _jumperSound;
    [SerializeField] private AudioClip _capyJumpSound;

    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _capsuleCollider;
    private AudioSource _audioSource;

    public AudioSource AudioSource => _audioSource;
    public Animator Animator => _animator;


    private void OnEnable()
    {
        CapyEnabled?.Invoke();
        GameScreen.RightButtonClicked += AddRightImpulse;
        GameScreen.LeftButtonClicked += AddLeftImpulse;
    }

    private void OnDisable()
    {
        GameScreen.RightButtonClicked -= AddRightImpulse;
        GameScreen.LeftButtonClicked -= AddLeftImpulse;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _audioSource = GetComponent<AudioSource>();
    }

    public void AddRightImpulse()
    {
        Vector3 scale = transform.localScale;
        scale.x = 2;


        if (_isGrounded && (transform.rotation.z < -0.69f || transform.rotation.z > 0.69f))
        {

            PlayCapyJumpSound();

            transform.localScale = scale;

            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(130f, 140f), ForceMode2D.Impulse);
            _isGrounded = false;
        }
        if(_isGrounded && !controller.IsActiveJetpack)

        {
            PlayCapyJumpSound();

            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(-2f, -6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(130f, 140f), ForceMode2D.Impulse);
            _isGrounded = false;
            
        }

        if (controller.IsActiveJetpack)
        {
            PlayJetpackSound();

            transform.localScale = scale;
             transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * 70f, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }

    public void AddLeftImpulse()
    {
        Vector3 scale = transform.localScale;
        scale.x = -2;


        if (_isGrounded && (transform.rotation.z < -0.69f || transform.rotation.z > 0.69f))
        {

            PlayCapyJumpSound();

            transform.localScale = scale;

            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(130f, 140f), ForceMode2D.Impulse);
            _isGrounded = false;
        }

        if (_isGrounded && !controller.IsActiveJetpack)
        {
            PlayCapyJumpSound();

            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(2f, 6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(130f, 140f), ForceMode2D.Impulse);
            _isGrounded = false;

        }
        if (controller.IsActiveJetpack)
        {
            PlayJetpackSound();

            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * 70f, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        CheckIsGrounded();
        CapyMovement();
    }

    void CheckIsGrounded()
    {
        Vector2 pos = transform.position;
        Vector2 dir = Vector2.down;
        float height = _capsuleCollider.size.y;
        float width = _capsuleCollider.size.x;

        RaycastHit2D hit = Physics2D.CapsuleCast(pos, new Vector2(width, height), CapsuleDirection2D.Vertical, 0f, dir, 3f, controller.LevelLayer);
        RaycastHit2D iceHit = Physics2D.CapsuleCast(pos, new Vector2(width, height), CapsuleDirection2D.Vertical, 0f, dir, 3f, controller.IceLevelLayer);
        _isIceGrounded = iceHit.collider != null;
        _isGrounded = hit.collider != null;

        Debug.DrawLine(pos, hit.point, Color.yellow);
    }

    private void CapyMovement()
    {
        if(_isGrounded & transform.rotation.z < -0.69f || _isGrounded & transform.rotation.z > 0.69f)
        {
            transform.position = new Vector3(transform.position.x + 0f , transform.position.y, transform.position.z);
        }else
        {
            if(transform.localScale.x == -2)
            {
                if (_isIceGrounded)
                {
                    transform.position = new Vector3(transform.position.x - 25f * Time.fixedDeltaTime, transform.position.y + 0f, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x - 15f * Time.fixedDeltaTime, transform.position.y + 0f, transform.position.z);
                }
            }
            if(transform.localScale.x == 2)
            {
                if (_isIceGrounded)
                {
                    transform.position = new Vector3(transform.position.x + 25f * Time.fixedDeltaTime, transform.position.y + 0f, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x + 15f * Time.fixedDeltaTime, transform.position.y + 0f, transform.position.z);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Saw"))
        {
            if (controller.IsActiveHelmet)
            {
                PlayHelmetLoseSound();
                CapyHelmetEnemyTouched?.Invoke();
            }
            else if (!controller.IsActiveHelmet)
            {
                OnCapyDied?.Invoke(DieType.Enemy, transform.position);
            }
        }

        if (other.gameObject.CompareTag("fly"))
        {
            PlayBoosterPickSound();
            JetpackClaimed?.Invoke(16f);
        }

        if(other.gameObject.CompareTag("fly2"))
        {
            PlayBoosterPickSound();
            JetpackClaimed?.Invoke(25f);
        }

        if (other.gameObject.CompareTag("Helmet"))
        {
            PlayBoosterPickSound();
            HelmetClaimed?.Invoke();
        }

        if(other.gameObject.CompareTag("zone1Tree"))
        {
            if(!controller.IsActiveHelmet && !controller.IsActiveJetpack)
            {
                string code = Random.Range(1, 99).ToString();
                OnCodeGenerated?.Invoke(code);

                Debug.Log(code);
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        if(other.gameObject.CompareTag("zone2Platform"))
        {
            OnCodeGenerated?.Invoke("48Y");
        }

        if (other.gameObject.CompareTag("zone3BoldBlock"))
        {
            if (!controller.IsActiveHelmet && !controller.IsActiveJetpack)
            {
                string code = Random.Range(1, 99).ToString();

                OnCodeGenerated?.Invoke(code);
            }
        }

        if (other.gameObject.CompareTag("zone4Platform"))
        {
            if(!controller.IsActiveHelmet)
                OnCodeGenerated?.Invoke("87Q");
        }

        //if (other.gameObject.CompareTag("zone4Tree"))
        //{
        //    if (!controller.IsActiveHelmet)
        //    {
        //        string code = Random.Range(1, 99).ToString();
        //        OnCodeGenerated?.Invoke(code);
        //    }
        //}

        if (other.gameObject.CompareTag("zone4Container"))
        {
            if(!controller.IsActiveHelmet && !controller.IsActiveJetpack)
                OnCodeGenerated?.Invoke("21J");
        }

        if (other.gameObject.CompareTag("zone4Rock"))
        {
            if (!controller.IsActiveHelmet && !controller.IsActiveJetpack)
            {
                string code = Random.Range(1, 99).ToString();
                OnCodeGenerated?.Invoke(code);
            }
        }

        if (other.gameObject.CompareTag("Finish"))
        {
            OnFinishAchieved?.Invoke();
            other.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.CompareTag("enemy") || other.gameObject.CompareTag("Saw"))
        {
            if (controller.IsActiveHelmet)
            {
                PlayHelmetLoseSound();
                CapyHelmetEnemyTouched?.Invoke();
            }
            else if (!controller.IsActiveHelmet)
            {
                OnCapyDied?.Invoke(DieType.Enemy, transform.position);
            }
        }

        if (other.gameObject.CompareTag("river"))
        {
            OnCapyDied?.Invoke(DieType.River, transform.position);
        }


        if (other.gameObject.CompareTag("jumper"))
        {
            PlayJumperSound();
            _rigidBody.AddForce(Vector2.up * 130f, ForceMode2D.Impulse);
        }

        if (other.gameObject.CompareTag("megaJumper"))
        {
            PlayJumperSound();
            _rigidBody.AddForce(Vector2.up * 220f, ForceMode2D.Impulse);
        }

        if (other.gameObject.CompareTag("Jumper3"))
        {
            PlayJumperSound();
            _rigidBody.AddForce(Vector2.left * 200f, ForceMode2D.Impulse);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void PlayBoosterPickSound()
    {
        _audioSource.PlayOneShot(_boosterPickSound);
    }

    private void PlayHelmetLoseSound()
    {
        _audioSource.PlayOneShot(_helmetLoseSound);
    }

    private void PlayJetpackSound()
    {
        _audioSource.PlayOneShot(_jetpackSound);
    }

    private void PlayJumperSound()
    {
        _audioSource.PlayOneShot(_jumperSound);
    }

    private void PlayCapyJumpSound()
    {
        _audioSource.PlayOneShot(_capyJumpSound);
    }
}
