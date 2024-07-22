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
    [SerializeField] private LayerMask groundLayer; // 用于地面检测的 Layer
    [SerializeField] private LayerMask platformLayer; // 用于平台检测的 Layer
    [SerializeField] Transform groundCheck; // 地面检测的起始点
    [SerializeField] private float groundCheckRadius = 0.2f; // 地面检测的半径

    private bool headHit;
    [SerializeField] private Transform headCheck; // 头部检测的起始点
    [SerializeField] private float headCheckDistance = 0.2f; // 头部检测的距离
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
        // 检测地面
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);

        //检测平台
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        //检测头部
        headHit = Physics2D.Raycast(headCheck.position, Vector2.up, headCheckDistance, groundLayer);

    }

    private void Update()
    {
        //复活角色(调试用)    
        //Respawn();

        //翻转角色
        Flip();

        //控制输入
        ApplyMovement();

        // 更新颜色
        UpdateColor();

        if (isOnPlatform) isGrounded = true;

        //摔死了，man
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

        // 记录坠落高度
        IsFallingLogic();
        //Debug.Log(fallDistance);

        isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;

        stateMachine.Update();
    }

    public void Respawn()
    {
        //修改死亡动画位置使其匹配视觉效果
        Vector3 newPosition = anim.transform.position;
        newPosition.y += 0.12f;
        anim.transform.position = newPosition;

        RespawnSystemAction?.Invoke();

        isDead = false;
        killedByFall = false;
        killedByNail = false;
        anim.SetBool("Dead", false);
        Debug.Log("运行了172行");

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
            float t = fallDistance/4 ; // 计算插值比例
            sr.color = Color.Lerp(normalColor, fallColor, t); // 颜色插值
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
        //修改死亡动画位置使其匹配视觉效果
        Vector3 newPosition = anim.transform.position;
        newPosition.y -= 0.12f;
        anim.transform.position = newPosition;
    }

    private void IsFallingLogic() //坠落逻辑
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

    private void OnDrawGizmos()  //检测射线
    {
        if (groundCheck == null || headCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(headCheck.position, headCheck.position + Vector3.up *headCheckDistance);
    }

    private void ApplyMovement()
    {
        if (isDead)  //空格重开
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                RespawnSystemAction?.Invoke();
                Respawn();
            }
            Debug.Log("运行到274行");
            return;
        }

        jumpInput = Input.GetButtonDown("Jump");
        //跳跃逻辑
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
        
        // 跳跃时间控制
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
        //跳跃顶头时的处理情况
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
