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
    #endregion
    #region Components
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CapsuleCollider2D cd;
    [HideInInspector] public Animator anim;
    #endregion
    #region Color and state
    [HideInInspector] public Color normalColor;
    [SerializeField] private float red = 1.7f;
    [HideInInspector] public Color fallColor;
    public Color deadColor;
    [HideInInspector] public Color bounceColor;
    [HideInInspector] public Color drillColor;
    [HideInInspector] public Color invertColor;
    #endregion
    #region Move info
    private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 13f;
    public float bounceHeight = 4f;
    public float drillSpeed = 2f;
    public float invertGravityThreshold = 8f;
    [SerializeField] private float xInput;
    private float jumpingGravity;
    private float defaultGravity;
    private bool jumpInput;
    private bool isJumping;
    #endregion
    #region Character States
    [SerializeField] private LayerMask groundLayer; // ���ڵ������ Layer
    [SerializeField] private LayerMask platformLayer; // ����ƽ̨���� Layer
    [SerializeField] Transform groundCheck; // ���������ʼ��
    [SerializeField] private float groundCheckRadius = 0.2f; // ������İ뾶

    private bool headHit;
    [SerializeField] private Transform headCheck; // ͷ��������ʼ��
    [SerializeField] private float headCheckDistance = 0.2f; // ͷ�����ľ���
    private float jumpTimer;
    private bool isJumpLocked;
    private float jumpTimeCounter;
    private float jumpDuration = 0.5f;
    private float lockedYPosition;

    public bool isDead;
    private bool killedByFall;
    public bool killedByNail;
    public bool isFalling;

    public bool isOnPlatform;
    public bool isGrounded;
    public bool isMoving;

    private bool isFacingRight = true;

    private float highestPos;
    public float fallDistance;
    public float fallRed;
    #endregion

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

        // Set initial state
        stateMachine.SetState(idleState);
    }

    private void FixedUpdate()
    {
        // ������
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);

        //���ƽ̨
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        //���ͷ��
        headHit = Physics2D.Raycast(headCheck.position, Vector2.up, headCheckDistance, groundLayer);

    }

    private void Update()
    {
        //�����ɫ(������)    
        //Respawn();

        //��ת��ɫ
        Flip();

        //��������
        ApplyMovement();

        // ������ɫ
        UpdateColor();

        if (isOnPlatform) isGrounded = true;

        //ˤ���ˣ�man
        if (fallDistance > red)
        {
            if (isGrounded)
            {
                Die();
                if (!killedByNail) killedByFall = true;
            }
        }

        if (isGrounded)
        {
            fallDistance = 0;
            if(!isJumping)  rb.gravityScale = defaultGravity;
        }

        if (!isGrounded)
        {
            rb.gravityScale = jumpingGravity;
        }

        // ��¼׹��߶�
        IsFallingLogic();
        //Debug.Log(fallDistance);

        isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;

        stateMachine.Update();
    }

    public void Respawn()
    {
        //�޸���������λ��ʹ��ƥ���Ӿ�Ч��
        Vector3 newPosition = anim.transform.position;
        newPosition.y += 0.12f;
        anim.transform.position = newPosition;

        RespawnSystemAction?.Invoke();

        isDead = false;
        killedByFall = false;
        killedByNail = false;
        anim.SetBool("Dead", false);
        Debug.Log("������172��");

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

        if (fallDistance >= fallRed && fallDistance <= 3)
        {
            float t = fallDistance/4 ; // �����ֵ����
            sr.color = Color.Lerp(normalColor, fallColor, t); // ��ɫ��ֵ
        }
        else if (fallDistance > 3)
        {
            sr.color = fallColor;
        }
        else sr.color = normalColor;

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
        Vector3 newPosition = anim.transform.position;
        newPosition.y -= 0.12f;
        anim.transform.position = newPosition;
    }

    private void IsFallingLogic() //׹���߼�
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
        }
        else
        {
            fallDistance = highestPos - transform.position.y;
            //Debug.Log("Fall Distance: " + fallDistance);
        }
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
                RespawnSystemAction?.Invoke();
                Respawn();
            }
            Debug.Log("���е�274��");
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
