using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class CapyCharacter : MonoBehaviour
{
    public static event Action OnEnemyTouchedWithHelm;
    public static event Action<DieType, Vector3> OnCapyDied;
    public static event Action OnTimeClaimed;
    public static event Action<string> OnCodeGenerated;
    public static event Action OnFinishAchieved;

    [Header("Лэеры уровня")]
    [SerializeField] private LayerMask levelLayer;
    [SerializeField] private LayerMask iceLevelLayer;

    [Header("Звуки Кэпи")]
    [SerializeField] private AudioClip _boosterPickSound;
    [SerializeField] private AudioClip _timeBoosterPickSound;
    [SerializeField] private AudioClip _helmetLoseSound;
    [SerializeField] private AudioClip _capyJumpSound;
    [SerializeField] private AudioClip _jetpackSound;
    [SerializeField] private AudioClip _jumperSound;

    
    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _capsuleCollider;
    private AudioSource _audioSource;
    private bool _isGrounded;
    private bool _isIceGrounded;
    private bool _isActiveJetpack = false;
    private bool _isActiveHelmet = false;

    private void OnEnable()
    {
        GameScreen.RightButtonClicked += AddRightImpulse;
        GameScreen.LeftButtonClicked += AddLeftImpulse;
        MenuBar.PlayButtonClicked += ResetCapyState;
        MenuBarController.SoundChanged += SoundTurn;
    }

    private void ResetCapyState()
    {
        ResetCapyAnimations();
        ResetCapyBoosters();
    }

    private void ResetCapyAnimations()
    {
        _animator.SetBool("IsRunning", true);
    }

    private void ResetCapyBoosters()
    {
        _isActiveJetpack = false;
        _isActiveHelmet = false;
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

        if(_isGrounded && !_isActiveJetpack)
        {
            PlayCapyJumpSound();

            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(-2f, -6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(130f, 140f), ForceMode2D.Impulse);
            _isGrounded = false;
            
        }

        if (_isActiveJetpack)
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

        if (_isGrounded && !_isActiveJetpack)
        {
            PlayCapyJumpSound();

            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(2f, 6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(130f, 140f), ForceMode2D.Impulse);
            _isGrounded = false;

        }
        if (_isActiveJetpack)
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

        Debug.Log(_isActiveJetpack);
    }

    private IEnumerator JetpackOffAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isActiveJetpack = false;
        _animator.SetBool("IsRunning", true);
    }

    void CheckIsGrounded()
    {
        Vector2 pos = transform.position;
        Vector2 dir = Vector2.down;
        float height = _capsuleCollider.size.y;
        float width = _capsuleCollider.size.x;

        RaycastHit2D hit = Physics2D.CapsuleCast(pos, new Vector2(width, height), CapsuleDirection2D.Vertical, 0f, dir, 2f, levelLayer);
        RaycastHit2D iceHit = Physics2D.CapsuleCast(pos, new Vector2(width, height), CapsuleDirection2D.Vertical, 0f, dir, 2f, iceLevelLayer);
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
            if (_isActiveHelmet)
            {
                PlayHelmetLosekSound();
                _isActiveHelmet = false;
                OnEnemyTouchedWithHelm?.Invoke();
                _animator.SetBool("IsRunning", true);
            }
            else if (!_isActiveHelmet)
            {
                OnCapyDied?.Invoke(DieType.Enemy, transform.position);
            }
        }

        if(other.gameObject.CompareTag("Time"))
        {
            PlayTimePickSound();
            OnTimeClaimed?.Invoke();
        }

        if (other.gameObject.CompareTag("fly"))
        {
            PlayBoosterPickSound();
            _isActiveHelmet = false;
            _isActiveJetpack = true;
            _animator.SetTrigger("Jetpack");
            StartCoroutine(JetpackOffAfter(16f));
        }

        if(other.gameObject.CompareTag("fly2"))
        {
            PlayBoosterPickSound();
            _isActiveHelmet = false;
            _isActiveJetpack = true;
            _animator.SetTrigger("Jetpack");
            StartCoroutine(JetpackOffAfter(20f));
        }

        if (other.gameObject.CompareTag("Helmet"))
        {
            PlayBoosterPickSound();
            _isActiveJetpack = false;
            _isActiveHelmet = true;
            _animator.SetTrigger("Helmet");
        }

        if(other.gameObject.CompareTag("zone1Tree"))
        {
            if(!_isActiveHelmet && !_isActiveJetpack)
            {
                string code = Random.Range(1, 99).ToString();

                OnCodeGenerated?.Invoke(code);
            }
        }

        if(other.gameObject.CompareTag("zone2Platform"))
        {
            OnCodeGenerated?.Invoke("48Y");
        }

        if (other.gameObject.CompareTag("zone3BoldBlock"))
        {
            if (!_isActiveHelmet && !_isActiveJetpack)
            {
                string code = Random.Range(1, 99).ToString();

                OnCodeGenerated?.Invoke(code);
            }
        }

        if (other.gameObject.CompareTag("zone4Platform"))
        {
            if(!_isActiveHelmet)
                OnCodeGenerated?.Invoke("87Q");
        }

        if (other.gameObject.CompareTag("zone4Tree"))
        {
            if (!_isActiveHelmet)
            {
                string code = Random.Range(1, 99).ToString();
                OnCodeGenerated?.Invoke(code);
            }
        }

        if (other.gameObject.CompareTag("zone4Container"))
        {
            if(!_isActiveHelmet && !_isActiveJetpack)
                OnCodeGenerated?.Invoke("21J");
        }

        if (other.gameObject.CompareTag("zone4Rock"))
        {
            if (!_isActiveHelmet && !_isActiveJetpack)
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
            if (_isActiveHelmet)
            {
                PlayHelmetLosekSound();
                _isActiveHelmet = false;
                OnEnemyTouchedWithHelm?.Invoke();
                _animator.SetBool("IsRunning", true);
            }
            else if (_isActiveHelmet == false)
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
            _rigidBody.AddForce(Vector2.up * 230f, ForceMode2D.Impulse);
        }

        if (other.gameObject.CompareTag("megaJumper"))
        {
            PlayJumperSound();
            _rigidBody.AddForce(Vector2.up * 220f, ForceMode2D.Impulse);
        }

        if(other.gameObject.CompareTag("Jumper3"))
        {
            PlayJumperSound();
            _rigidBody.AddForce(Vector2.right * 200f, ForceMode2D.Impulse);
        }
       
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }

    private void OnDisable()
    {
        GameScreen.RightButtonClicked -= AddRightImpulse;
        GameScreen.LeftButtonClicked -= AddLeftImpulse;
        MenuBarController.SoundChanged -= SoundTurn;
    }

    private void PlayBoosterPickSound()
    {
        _audioSource.PlayOneShot(_boosterPickSound);
    }

    private void PlayTimePickSound()
    {
        _audioSource.PlayOneShot(_timeBoosterPickSound);
    }

    private void PlayHelmetLosekSound()
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

    private void SoundTurn(SoundState state)
    {
        _audioSource.mute = (state == SoundState.On) ? false : true;
    }

}
