using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour
{
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;

    private bool doubleJump;

    public GameObject bullet;
    public GameObject spawnRight;
    public GameObject spawnLeft;
    public GameObject spawnUp;

    public Transform wallGrabPoint;
    public Transform wallGrabPointLeft;
    public bool canGrab, isGrabbing;
    private float gravityStore;
    public float wallJumpTime = .2f;
    private float wallJumpTimer;

    public float DashForce;
    public float StartDashTimer;
    float CurrentDashTimer;
    float DashDirection;
    bool isDashing;
    public float dashCooldown;
    float currentDashCooldown;

    public int totalLives = 3;
    public int currentLives;
    [SerializeField] private TextMeshProUGUI livesTag;
    public float timer;


    [SerializeField] private LayerMask jumpableGround;

    private float dirX;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling }
    private MovementState state = MovementState.idle;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        gravityStore = rb.gravityScale;

        currentLives = totalLives;
        timer = Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        livesTag.text = "Lives: " + currentLives;
        
        if (!PauseMenu.isPaused)
        {
            dirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

            if (wallJumpTimer <= 0)
            {
                if (isGrounded() && !Input.GetButton("Jump"))
                {
                    doubleJump = false;
                }

                if (Input.GetButtonDown("Jump"))
                {
                    if (isGrounded() || doubleJump)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                        doubleJump = !doubleJump;
                        // canGrab = true;
                    }
                }

                if (Input.GetKeyDown(KeyCode.LeftShift) && currentDashCooldown <= 0 && dirX != 0)
                {
                    // Debug.Log("can dash");
                    isDashing = true;
                    CurrentDashTimer = StartDashTimer;
                    currentDashCooldown = dashCooldown;
                    rb.velocity = Vector2.zero;
                    DashDirection = (int)dirX;
                }


                // https://www.youtube.com/watch?v=G3cGpnuzVHU 
                if (isDashing)
                {
                    rb.velocity = transform.right * DashDirection * DashForce;
                    CurrentDashTimer -= Time.deltaTime;

                    if (CurrentDashTimer <= 0)
                    {
                        // Debug.Log("cannot dash right now");
                        isDashing = false;
                        StartCoroutine(Timer());
                        //currentDashCooldown -= Time.deltaTime;
                        Debug.Log(currentDashCooldown);
                    }
                }

                canGrab = Physics2D.OverlapCircle(wallGrabPoint.position, .2f, jumpableGround) || Physics2D.OverlapCircle(wallGrabPointLeft.position, .2f, jumpableGround);


                isGrabbing = false;
                if (canGrab)// && !isGrounded())
                {
                    //Debug.Log("yes");
                    if ((dirX > 0) || (dirX < 0))
                    {
                        //Debug.Log("I can jump");
                        isGrabbing = true;
                    }
                }

                if (isGrabbing)
                {
                    rb.gravityScale = 0f;
                    rb.velocity = Vector2.zero;

                    //Debug.Log("maybe");
                    if (Input.GetButtonDown("Jump"))
                    {
                        //Debug.Log("i am able to jump off the wall");
                        wallJumpTimer = wallJumpTime;
                        rb.velocity = new Vector2(-dirX * moveSpeed, jumpForce);
                        rb.gravityScale = gravityStore;
                        isGrabbing = false;
                        doubleJump = true;
                        //canGrab = false;
                    }
                }
                else
                {
                    rb.gravityScale = gravityStore;
                }

                if (isGrounded() && canGrab && isGrabbing)
                {
                    if (!Input.GetKey(KeyCode.A) || (!Input.GetKey(KeyCode.D)))
                    {
                        canGrab = false;
                        isGrabbing = false;
                    }
                    else if (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D)))
                    {
                        canGrab = false;
                        isGrabbing = false;
                    }
                }
            }
            else
            {
                wallJumpTimer -= Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0))
            {
                //Debug.Log("test");
                if (Input.GetKey(KeyCode.A))
                {
                    //Debug.Log("left");
                    GameObject go = GameObject.Instantiate(bullet, spawnLeft.transform.position, Quaternion.identity);
                    go.GetComponent<Bullet>().xSpeed = -35.0f;
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    //Debug.Log("right");
                    GameObject go = GameObject.Instantiate(bullet, spawnRight.transform.position, Quaternion.identity);
                    go.GetComponent<Bullet>().xSpeed = 35.0f;
                }
                else if (Input.GetKey(KeyCode.W))
                {
                    //Debug.Log("up");
                    GameObject go = GameObject.Instantiate(bullet, spawnUp.transform.position, Quaternion.identity);
                    go.GetComponent<Bullet>().ySpeed = 35.0f;
                }
            }


            UpdateAnimationState();
            OnCollisionEnter2D(null);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentLives--;
            livesTag.text = "Lives: " + currentLives;
            // Debug.Log("ow");
            if (currentLives <= 0)
            {
                Debug.Log("dead");
                //Die();
                //Timer();
                restartLevel(2);
                currentLives = totalLives;
            }
        }
        else if (collision.gameObject.CompareTag("deathWall"))
        {
            // Debug.Log("dead");
            //Die();
            //Timer();
            restartLevel(2);
            currentLives = totalLives;
        }
    }

        private void UpdateAnimationState()
    {
        MovementState state;
        
        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idle;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }
    

    public bool isGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    IEnumerator Timer()
    {
        print(Time.time);
        yield return new WaitForSecondsRealtime(1);
        currentDashCooldown = 0;
        print(Time.time);
    }

    private void Die()
    {
        // https://www.youtube.com/watch?v=ynH51MiKutY
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void restartLevel(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
        currentLives = totalLives;
    }
}
