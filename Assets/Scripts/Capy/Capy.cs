using System.Collections;
using UnityEngine;

public class Capy : MonoBehaviour
{

    [SerializeField] private LayerMask levelLayer;
    [SerializeField] private GameController gameController;
    private Animator _animator;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _capsuleCollider;
    private bool _isGrounded;
    private bool _isActiveJetpack = false;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    public void AddRightImpulse()
    {

        if (_isGrounded & transform.rotation.z < -0.69f || _isGrounded & transform.rotation.z > 0.69f)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;
        }
        else if(_isGrounded && !_isActiveJetpack)
        {
            if (transform.localScale.x != 2)
            {
                transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
            }

            transform.eulerAngles = new Vector3(0, 0, Random.Range(-2f, -6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;

        }
        else if (_isActiveJetpack)
        {
            if (transform.localScale.x != 2)
            {
                transform.localScale = new Vector3(2, transform.localScale.y, transform.localScale.z);
            }

            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * 70f, ForceMode2D.Impulse);
            _isGrounded = false;
        }
    }

    public void AddLeftImpulse()
    {

        if (_isGrounded & transform.rotation.z < -0.69f || _isGrounded & transform.rotation.z > 0.69f)
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;
        }
        else if (_isGrounded)
        {
            if (transform.localScale.x != -2)
            {
                transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
            }

            transform.eulerAngles = new Vector3(0, 0, Random.Range(2f, 6f));
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.AddForce(transform.up * Random.Range(120f, 130f), ForceMode2D.Impulse);
            _isGrounded = false;

        }
        else if (_isActiveJetpack)
        {
            if (transform.localScale.x != -2)
            {
                transform.localScale = new Vector3(-2, transform.localScale.y, transform.localScale.z);
            }

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

    //private void CheckIsGrounded()
    //{
    //    _isGrounded = false;
    //    _isGrounded = Physics2D.OverlapCircle(transform.position, 3f, levelLayer);
    //}


    // ƒŒ¬ŒÀ‹ÕŒ ’Œ–ŒÿŒ –¿¡Œ“¿ﬁŸ¿ﬂ œ–Œ¬≈– ¿
    //private void CheckIsGrounded()
    //{
    //    Vector3 pos = transform.position;
    //    Vector3 dir = Vector3.down;

    //    // Cast rays to check if Capy is grounded
    //    RaycastHit2D hitFoot = Physics2D.Raycast(pos, dir, 3f, levelLayer);
    //    RaycastHit2D hitHead = Physics2D.Raycast(pos + new Vector3(0, 3f, 0), dir, 1.5f, levelLayer);
    //    RaycastHit2D hitButt = Physics2D.Raycast(pos + new Vector3(0, -3f, 0), dir, 1.5f, levelLayer);
    //    _isGrounded = hitFoot || hitHead || hitButt;

    //}
    private IEnumerator JetpackOffAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isActiveJetpack = false;
        _animator.SetBool("IsRunning", true);
    }


    private void CheckIsGrounded()
    {
        Vector2 pos = transform.position;
        Vector2 dir = Vector2.down;
        float height = _capsuleCollider.size.y;
        float width = _capsuleCollider.size.x;

        RaycastHit2D hit = Physics2D.CapsuleCast(pos, new Vector2(width, height), CapsuleDirection2D.Vertical, 0f, dir, 6f, levelLayer);
        _isGrounded = hit.collider != null;
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
                transform.position = new Vector3(transform.position.x - 15f * Time.fixedDeltaTime, transform.position.y + 0f, transform.position.z);
            }
            if(transform.localScale.x == 2)
            {
                transform.position = new Vector3(transform.position.x + 15f * Time.fixedDeltaTime, transform.position.y + 0f, transform.position.z);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (other.gameObject.tag == "enemy")
        //{
        //    gameController.OnCapyDie(DieType.Enemy);

        //}

        if (other.gameObject.tag == "point")
        {
            gameController.TimeCount = 6;
        }

        if (other.gameObject.tag == "zone2")
        {
            gameController.TimeCount = 6;
            gameController.Zone2Achieved = true;
        }

        if (other.gameObject.tag == "zone3")
        {
            gameController.TimeCount = 6;
            gameController.Zone3Achieved = true;
        }

        if (other.gameObject.tag == "zone4")
        {
            gameController.TimeCount = 6;
            gameController.Zone4Achieved = true;
        }

        if (other.gameObject.tag == "snap")
        {
            _animator.SetTrigger("Cap");
        }

        if (other.gameObject.tag == "fly")
        {
            Destroy(other.gameObject);
            _isActiveJetpack = true;
            _animator.SetTrigger("Jetpack");
            StartCoroutine(JetpackOffAfter(16f));
        }

        if (other.gameObject.tag == "vader")
        {
            _animator.SetTrigger("Vader");
        }

        if (other.gameObject.tag == "eat")
        {
            gameController.TimeCount = 6;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "enemy")
        {
            gameController.OnCapyDie(DieType.Enemy);
        }

        //if (other.gameObject.tag == "river")
        //{
        //    gameController.OnCapyDie(DieType.River);
        //}

        if (other.gameObject.tag == "Finish")
        {
            gameController.FinishAchieved = true;
        }

        if (other.gameObject.tag == "jumper")
        {
            _rigidBody.AddForce(Vector2.up * 230f, ForceMode2D.Impulse);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
