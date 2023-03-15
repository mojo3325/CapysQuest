using System.Collections;
using UnityEngine;

public class Capy : MonoBehaviour
{

    [SerializeField] private LayerMask levelLayer;
    [SerializeField] private LayerMask iceLevelLayer;

    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _capsuleCollider;
    private bool _isGrounded;
    public bool _isIceGrounded;
    private bool _isActiveJetpack = false;
    private bool _isActiveGravity = false;
    private bool _isActiveHelmet = false;

    private void OnEnable()
    {
        EventManager.OnRightTap.AddListener(AddRightImpulse);
        EventManager.OnLeftTap.AddListener(AddLeftImpulse);
        EventManager.OnCapyDie.AddListener(ResetCapyState);        
    }

    private void ResetCapyState(DieType dieType)
    {
        _animator.SetBool("IsRunning", true);
        _isActiveGravity = false;
        _isActiveJetpack = false;
        _isActiveHelmet = false;
    }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void AddRightImpulse()
    {
        Vector3 scale = transform.localScale;
        scale.x = 2;


        if (_isActiveGravity)
        {

            if (_isGrounded)
            {
                Vector3 gravityScale = transform.localScale;
                gravityScale.y = (gravityScale.y == 2) ? -2 : 2;
                transform.localScale = gravityScale;
                _rigidBody.gravityScale = (_rigidBody.gravityScale == 12) ? -12: 12;
                _rigidBody.velocity = Vector3.zero;

                if(transform.rotation.z < 0.69f || transform.rotation.z > 0.69f)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    _rigidBody.velocity = Vector3.zero;
                }
            }
        }

        if (_isGrounded && (transform.rotation.z < -0.69f || transform.rotation.z > 0.69f))
        {
            
            transform.localScale = scale;

            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;
        }
        else if(_isGrounded && !_isActiveJetpack && !_isActiveGravity)
        {

            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(-2f, -6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;
            
        }
        else if (_isActiveJetpack && !_isActiveGravity)
        {

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

        if (_isActiveGravity)
        {

            if (_isGrounded)
            {
                Vector3 gravityScale = transform.localScale;
                gravityScale.y = (gravityScale.y == 2) ? -2 : 2;
                transform.localScale = gravityScale;
                _rigidBody.gravityScale = (_rigidBody.gravityScale == 12) ? -12 : 12;
                _rigidBody.velocity = Vector3.zero;

                if (transform.rotation.z < 0.69f || transform.rotation.z > 0.69f)
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    _rigidBody.velocity = Vector3.zero;
                }
            }
        }

        if (_isGrounded && (transform.rotation.z < -0.69f || transform.rotation.z > 0.69f))
        {
            transform.localScale = scale;

            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;
        }

        if (_isGrounded && !_isActiveJetpack)
        {
            transform.localScale = scale;
            transform.eulerAngles = new Vector3(0, 0, Random.Range(2f, 6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;

        }
        if (_isActiveJetpack)
        {
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
    private IEnumerator JetpackOffAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isActiveJetpack = false;
        _animator.SetBool("IsRunning", true);
    }

    void CheckIsGrounded()
    {
        if (_isActiveGravity)
        {
            Vector2 gravityDir = _rigidBody.gravityScale > 0 ? Vector2.down : Vector2.up;
            float scale = transform.localScale.y * Mathf.Sign(_rigidBody.gravityScale);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, gravityDir, scale + 0.1f, levelLayer);
            _isGrounded = hit.collider != null;

            Debug.DrawLine(transform.position, hit.point, Color.red);
        }else
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
        if (other.gameObject.tag == "enemy")
        {
            EventManager.OnCapyDie.Invoke(DieType.Enemy);
        }

        if (other.gameObject.tag == "zone1")
        {
            EventManager.OnZoneAchieved.Invoke(ZoneType.zone_1);
        }
        if (other.gameObject.tag == "zone2")
        {
            EventManager.OnZoneAchieved.Invoke(ZoneType.zone_2);
        }

        if (other.gameObject.tag == "zone3")
        {
            EventManager.OnZoneAchieved.Invoke(ZoneType.zone_3);
        }

        if (other.gameObject.tag == "zone4")
        {
            EventManager.OnZoneAchieved.Invoke(ZoneType.zone_4);
        }

        if(other.gameObject.tag == "Time")
        {
            EventManager.OnTimeClimed.Invoke();
        }

        if (other.gameObject.tag == "fly")
        {
            _isActiveHelmet = false;
            _isActiveGravity = false;
            _isActiveJetpack = true;
            _animator.SetTrigger("Jetpack");
            StartCoroutine(JetpackOffAfter(16f));
        }

        if (other.gameObject.tag == "Helmet")
        {
            _isActiveGravity = false;
            _isActiveJetpack = false;
            _isActiveHelmet = true;
            _animator.SetTrigger("Helmet");
        }

        if(other.gameObject.tag == "Gravity")
        {
            _isActiveHelmet = false;
            _isActiveJetpack = false;
            _isActiveGravity = true;
            _rigidBody.gravityScale = -12;
            Vector3 gravityScale = transform.localScale;
            gravityScale.y = -2 ;
            transform.localScale = gravityScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "enemy")
        {
            EventManager.OnCapyDie.Invoke(DieType.Enemy);
        }

        if (other.gameObject.tag == "river")
        {
            EventManager.OnCapyDie.Invoke(DieType.River);
        }

        if (other.gameObject.tag == "Finish")
        {
            EventManager.OnFinishZoneAchieved.Invoke();
        }

        if (other.gameObject.tag == "jumper")
        {
            _rigidBody.AddForce(Vector2.up * 230f, ForceMode2D.Impulse);
        }

        if (other.gameObject.tag == "megaJumper")
        {
            _rigidBody.AddForce(Vector2.up * 220f, ForceMode2D.Impulse);
        }

        if(other.gameObject.tag == "Jumper3")
        {
            _rigidBody.AddForce(Vector2.right * 200f, ForceMode2D.Impulse);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
