using System.Collections;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region states
    private StateMachine stateMachine;
    public PlayerState idleState;
    public PlayerState moveState;
    public PlayerState airState;
    public PlayerState bouncingState;
    public PlayerState drillingState;
    public PlayerState invertedState;
    public PlayerState slopeState;
    #endregion
    #region Components
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CapsuleCollider2D cd;
    [HideInInspector] public Animator anim;
    #endregion
    #region Color and state
    [HideInInspector] public Color normalColor;
    [SerializeField] private float fallRed=1.5f;
    [SerializeField] private float red = 1.7f;
    [HideInInspector] public Color fallColor;
    public Color deadColor;
    private float bounce = 4f;
    public Color bounceColor;
    [HideInInspector] public Color drillColor;
    [HideInInspector] public Color invertColor;
    private bool isColorCoroutineRunning;
    #endregion
    #region Move info
    private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 13f;
    public float bounceHeight = 18f;
    public float drillSpeed = 2f;
    public float invertGravityThreshold = 8f;
    public float xInput;
    private float jumpingGravity;
    private float defaultGravity;
    private bool jumpInput;
    public bool isJumping;
    #endregion
    #region Character States
    [SerializeField] private LayerMask groundLayer; // ���ڵ������ Layer
    [SerializeField] private LayerMask platformLayer; // ����ƽ̨���� Layer
    public Transform groundCheck; // ���������ʼ��
    public float groundCheckRadius = 0.35f; // ������İ뾶

    private bool headHit;
    [SerializeField] private Transform headCheck; // ͷ��������ʼ��
    [SerializeField] private float headCheckDistance = 0.2f; // ͷ�����ľ���
    private float jumpTimer;
    private bool isJumpLocked;
    private float jumpTimeCounter;
    private float jumpDuration = 0.5f;
    private float lockedYPosition;

    public bool isDead;
    [SerializeField] private bool killedByFall;
    public bool killedByNail;
    public bool isFalling;

    public bool isOnPlatform;
    public bool isGrounded;
    public bool isMoving;

    public bool isFacingRight = true;

    private float highestPos;
    public float fallDistance;
    #endregion
    #region Slope
    [SerializeField] private bool isOnRightSlope;
    [SerializeField] private bool isOnLeftSlope;
    #endregion
    [SerializeField] private float testFloat;
    public System.Action RespawnSystemAction;

    private void Start()
    {
        highestPos = transform.position.y;

        defaultGravity = 8;
        jumpingGravity = 3;

        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        cd = GetComponent<CapsuleCollider2D>();

        stateMachine = new StateMachine();

        idleState = new IdleState(this);
        moveState = new MoveState(this);
        airState = new AirState(this);
        bouncingState = new BouncingState(this);
        drillingState = new DrillingState(this);
        invertedState = new InvertedState(this);
        slopeState = new SlopeState(this);

        // Set initial state
        stateMachine.SetState(idleState);
    }

    private void FixedUpdate()
    {
        // ������
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);

        //���ƽ̨
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        //���б��
        isOnRightSlope = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);

        //���ͷ��
        headHit = Physics2D.Raycast(headCheck.position, Vector2.up, headCheckDistance, groundLayer);
    }

    private void Update()
    {
        //��ת��ɫ
        Flip();

        //��������
        ApplyMovement();

        // ������ɫ
        UpdateColor();

        //ˤ���ˣ�man
        FallDistanceState();

        //��������ж�
        GroundLogic();

        // ��¼׹��߶�
        IsFallingLogic();
        //Debug.Log(fallDistance);

        isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;

        stateMachine.Update();
    }

    private void FallDistanceState()
    {
        if (fallDistance > red && isGrounded && fallDistance < bounce)
        {
            Die();
            if (!killedByNail) killedByFall = true;
        }
        else if (fallDistance > bounce && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, bounceHeight);
            //if (isGrounded)
            //{
            //    ChangeState(bouncingState);
            //}
        }
    }

    private void GroundLogic()
    {
        if (isOnPlatform) isGrounded = true;
        if (isGrounded)
        {
            //fallDistance = 0;
            if (!isJumping) rb.gravityScale = defaultGravity;
        }
        if (!isGrounded)
        {
            rb.gravityScale = jumpingGravity;
        }
    }

    public void Respawn()
    {
        //��������
        isDead = false;
        killedByFall = false;
        anim.SetBool("Dead", false);
        sr.color = normalColor;
        //�޸���������λ��ʹ��ƥ���Ӿ�Ч��
        if (killedByNail)
        {
            killedByNail = false;
            return;
        }
        Vector3 newPosition = anim.transform.position;
        newPosition.y += 0.12f;
        anim.transform.position = newPosition;
    }

    private void UpdateColor()
    {
        if (isDead)
        {
            if (killedByFall == true)
            {
                sr.color = fallColor;
                return;
            }
            else if (killedByNail == true)
            {
                sr.color = deadColor;
                return;
            }
        }

        if (isDead) return;

        if (fallDistance >= fallRed && fallDistance <= red)
        {
            float t = fallDistance/4 ; // �����ֵ����
            sr.color = Color.Lerp(normalColor, fallColor, t); // ��ɫ��ֵ
        }
        else if (fallDistance > red && fallDistance < bounce)
        {
            sr.color = fallColor;
        }

        else if(fallDistance > bounce)
        {
            sr.color = bounceColor;
            if (isGrounded && !isColorCoroutineRunning)
            {
                StartCoroutine(SetDefaultColor(0.8f));
            }
        }
    }

    private IEnumerator SetDefaultColor(float waitForSeconds)
    {
        isColorCoroutineRunning = true;
        yield return new WaitForSeconds(waitForSeconds);
        if (!isDead) sr.color = normalColor;
        isColorCoroutineRunning = false;
    }

    private void Flip()
    {
        if (xInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
        else if (xInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void Die()
    {
        anim.SetBool("Dead", true);
        isDead = true;
        rb.velocity = new Vector2(0, 0);
        //�޸���������λ��ʹ��ƥ���Ӿ�Ч��
        if (killedByNail) return;
        Vector3 newPosition = anim.transform.position;
        newPosition.y -= 0.12f;
        anim.transform.position = newPosition;
    }

    //private void IsFallingLogic() //׹���߼�
    //{
    //    if (rb.velocity.y < 0 && !isGrounded)
    //    {
    //        isFalling = true;
    //    }
    //    else
    //    {
    //        isFalling = false;
    //    }


    //    if (!isFalling)
    //    {
    //        highestPos = transform.position.y;
    //        fallDistance = 0;
    //    }
    //    else
    //    {
    //        fallDistance = highestPos - transform.position.y;
    //    }
    //}

    private float IsFallingLogic() //׹���߼�
    {
        if (rb.velocity.y < 0 && !isGrounded)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        if (!isFalling)
        {
            highestPos = transform.position.y;
            fallDistance = 0;
        }
        else
        {
            fallDistance = highestPos - transform.position.y;
        }

        return fallDistance;
    }

    private void OnDrawGizmos()  //�������
    {
        if (groundCheck == null || headCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(headCheck.position, headCheck.position + Vector3.up *headCheckDistance);
    }

    private void ApplyMovement()
    {
        if (isDead)  //�ո��ؿ�
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                Respawn();
                RespawnSystemAction?.Invoke();
            }
            return;
        }

        jumpInput = Input.GetButtonDown("Jump");
        //��Ծ�߼�
        JumpLogic();

        xInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(xInput * moveSpeed, rb.velocity.y);
    }

    private void JumpLogic()
    {
        if (jumpInput && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJumping = true;
            jumpTimeCounter = 0f;
            //isJumpLocked = false;
            ChangeState(airState);
        }
        
        // ��Ծʱ�����
        if (isJumping)
        {
            rb.gravityScale = jumpingGravity;

            jumpTimeCounter += Time.deltaTime;
            if (jumpTimeCounter >= jumpDuration)
            {
                isJumping = false;
                isJumpLocked = false;
            }
        }
        //��Ծ��ͷʱ�Ĵ������
        if (isJumping && headHit)
        {
            isJumpLocked = true;
            lockedYPosition = transform.position.y;
        }
        if (isJumpLocked)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            transform.position = new Vector2(transform.position.x, lockedYPosition);
        }
    }

    public void ChangeState(PlayerState newState)
    {
        stateMachine.SetState(newState);
    }
}
