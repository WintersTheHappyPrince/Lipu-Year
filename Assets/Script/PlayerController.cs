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
    [SerializeField] private float red = 1.7f;
    [HideInInspector] public Color fallColor;
    public Color deadColor;

    public float bounce = 3.8f; //进入弹跳需要的格数
    public Color bounceColor;
    public float bounceHeight = 16f;

    public float drill = 6f; //进入钻地需要的格数
    public Color drillColor;
    public Color drillingColor = Color.black;

    [HideInInspector] public Color invertColor;
    public bool isColorCoroutineRunning;
    #endregion
    #region Move info
    private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 13f;
    public float drillSpeed = 2f;
    public float invertGravityThreshold = 8f;
    public float xInput;
    public float jumpingGravity;
    private float defaultGravity;
    private bool jumpInput;
    public bool isJumping;
    #endregion
    #region Character States
    [SerializeField] private LayerMask groundLayer; // 用于地面检测的 Layer
    [SerializeField] private LayerMask platformLayer; // 用于平台检测的 Layer
    public Transform groundCheck; // 地面检测的起始点
    public float groundCheckRadius = 0.35f; // 地面检测的半径

    private bool headHit;
    [SerializeField] private Transform headCheck; // 头部检测的起始点
    [SerializeField] private float headCheckDistance = 0.2f; // 头部检测的距离
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
        // 检测地面
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);

        //检测平台
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);

        //检测头部
        headHit = Physics2D.Raycast(headCheck.position, Vector2.up, headCheckDistance, groundLayer);
    }

    private void Update()
    {
        //翻转角色
        Flip();

        //控制输入
        ApplyMovement();

        // 更新颜色
        UpdateColor();

        //滞空重力
        GravityChange();

        //摔落距离状态
        FallDistanceState();
        //↑↓这两个方法代码位置不能交换(原理不明)
        // 记录坠落高度
        IsFallingLogic();

        isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;

        stateMachine.Update();
    }

    private void FallDistanceState()
    {
        //Debug.Log("fallDistance :" + fallDistance + "isGrounded = " + isGrounded);

        if (fallDistance > red && isGrounded && fallDistance < bounce)
        {
            Die();
            if (!killedByNail) killedByFall = true;
        }
        else if (fallDistance > bounce && fallDistance < drill)
        {
            StarAnimRotate();
            //rb.velocity = new Vector2(rb.velocity.x, bounceHeight);
            if (isGrounded) 
                ChangeState(bouncingState);
        }
        else if (fallDistance > drill)
        {
            if (!drillingCoroutineRunning)
                StartCoroutine(HandleDrillingState());

            ChangeState(drillingState);
        }
    }

    private void GravityChange()
    {
        //if (drillingCoroutineRunning) return;

        if (isGrounded)
        {
            if (!isJumping) 
                rb.gravityScale = defaultGravity;
        }
        else
        {
            rb.gravityScale = jumpingGravity;
        }

        //if (isOnPlatform) isGrounded = true;
    }

    public void Respawn()
    {
        //参数调整
        isDead = false;
        killedByFall = false;
        anim.SetBool("Dead", false);
        sr.color = normalColor;
        //修改死亡动画位置使其匹配视觉效果
        if (killedByNail)
        {
            killedByNail = false;
            return;
        }
        //Vector3 newPosition = anim.transform.position;
        //newPosition.y += 0.12f;
        //anim.transform.position = newPosition;
        anim.transform.localPosition = new Vector3(0, 0.25f, 0);
    }

    public void UpdateColor()
    {
        if (isDead)
        {
            sr.color = killedByFall ? fallColor : deadColor;
            return;

            //if (killedByFall == true)
            //{
            //    sr.color = fallColor;
            //    return;
            //}
            //else if (killedByNail == true)
            //{
            //    sr.color = deadColor;
            //    return;
            //}
        }

        if (fallDistance >= red && fallDistance < bounce)
        {
            sr.color = fallColor;
        }
        else if (fallDistance > bounce && fallDistance < drill)
        {
            sr.color = bounceColor;
            if (isGrounded && !isColorCoroutineRunning)
            {
                StartCoroutine(SetDefaultColor(0.8f));
            }
        }

        //else sr.color = normalColor;
    }

    public void StartSetDefaultColor(float waitForSeconds)
    {
        StartCoroutine(SetDefaultColor(waitForSeconds));
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
        //修改死亡动画位置使其匹配视觉效果
        if (killedByNail) return;
        Vector3 newPosition = anim.transform.position;
        newPosition.y -= 0.12f;
        anim.transform.position = newPosition;
    }

    private float IsFallingLogic() //坠落逻辑
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

    private void OnDrawGizmos()  //检测射线
    {
        if (groundCheck == null || headCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);


        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckDistance);
        //Gizmos.DrawLine(headCheck.position, headCheck.position + Vector3.up * headCheckDistance);
    }

    private void ApplyMovement()
    {
        if (isDead)  //空格重开
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Respawn();
                RespawnSystemAction?.Invoke();
            }
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
        if (drillingCoroutineRunning || rotating) return;

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

    #region AnimRotate

    private bool isAnimRotated;

    public void StarAnimRotate()
    {
        if (!isAnimRotated)
            StartCoroutine(AnimRotate());
    }

    private IEnumerator AnimRotate()
    {
        Debug.Log("旋转动画协程");

        isAnimRotated = true;

        anim.transform.Rotate(Vector3.back, 180f);

        yield return new WaitForSeconds(0.5f);

        isAnimRotated = false;
    }
    #endregion

    #region BounceLogic
    //此处实现动画功能，弹跳功能写在进入弹跳状态的代码中
    private bool rotating; // 控制是否正在旋转或者外部中断
    private Coroutine bounceRotateCoroutine = null; // 用于保存协程的引用
    public void StopBounceRotate()
    {
        // 如果协程正在运行，停止它
        if (bounceRotateCoroutine != null)
        {
            StopCoroutine(bounceRotateCoroutine);
            bounceRotateCoroutine = null;
        }

        rotating = false;
        anim.transform.rotation = Quaternion.identity;
    }
    public void StartBounceRotate(float speed)
    {
        if (bounceRotateCoroutine == null)
        {
            bounceRotateCoroutine = StartCoroutine(BounceRotate(speed));
        }
    }
    private IEnumerator BounceRotate(float speed)
    {
        float totalRotationChange = 0f; // 累计旋转变化量
        rotating = true;

        while (rotating)
        {
            //每帧旋转角度
            float rotationAmount = speed * Time.deltaTime;

            // 根据角色朝向进行旋转
            if (!isFacingRight)
                anim.transform.Rotate(Vector3.forward, rotationAmount);
            else
                anim.transform.Rotate(Vector3.forward, -rotationAmount);

            // 计算旋转变化量
            totalRotationChange += Mathf.Abs(rotationAmount);

            // 检查是否完成360度旋转或者外部中断
            if (totalRotationChange >= 360f)
            {
                // 旋转完成，停止旋转
                rotating = false;

                // 重置旋转角度
                totalRotationChange = 0f;

                // 复原rotation到0,0,0
                anim.transform.rotation = Quaternion.identity;

                ChangeState(airState);

                bounceRotateCoroutine = null;

                yield break;
            }
            yield return null;
        }
    }
    #endregion

    #region DrillLogic

    public bool drillingCoroutineRunning;

    private IEnumerator HandleDrillingState()
    {
        drillingCoroutineRunning = true;

        bool hasEnteredIgnoreCollision=false;
   
        Debug.Log("开始钻地协程");

        while (!hasEnteredIgnoreCollision) //第一次接触地面
        {
            bool isTouchingGround = cd.IsTouchingLayers(groundLayer);

            Debug.Log($"cd.IsTouchingLayers(Ground): {isTouchingGround}");

            //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
            cd.isTrigger = true;

            if (!hasEnteredIgnoreCollision && isTouchingGround)
            {
                hasEnteredIgnoreCollision = true;
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.velocity = new Vector2(rb.velocity.x, -drillSpeed);
                Debug.Log("忽略碰撞层");
            }
            yield return null;
        }

        while (true)  //钻入地面
        {
            bool isTouchingGround = cd.IsTouchingLayers(groundLayer);

            Debug.Log($"cd.IsTouchingLayers(Ground): {isTouchingGround}");

            if (hasEnteredIgnoreCollision && isTouchingGround)  //钻地进行时
            {
                rb.velocity = new Vector2(rb.velocity.x, -drillSpeed);
            }

            //yield return new WaitForSeconds(0.5f);

            if (hasEnteredIgnoreCollision && !isTouchingGround || isDead)  //退出钻地
            {
                Debug.Log("恢复碰撞层");
                //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
                cd.isTrigger = false;
                ChangeState(airState);  // 回到airState状态或者其他适合的状态
                rb.gravityScale = jumpingGravity;
                StarAnimRotate();
                highestPos = transform.position.y;
                //Debug.Log("重置玩家摔落高度");
                yield return new WaitForSeconds(1);    
                drillingCoroutineRunning = false;
                yield break;  // 退出协程
            }
            
            yield return null;  // 等待下一帧
        }    
    }

    #endregion

    public void ChangeState(PlayerState newState)
    {
        stateMachine.SetState(newState);
    }
}
