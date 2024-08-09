using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField] private float red = 1.8f;
    [HideInInspector] public Color fallColor;
    public Color deadColor;

    public float bounce = 3.65f; //进入弹跳需要的格数
    public Color bounceColor;
    public float bounceHeight = 15f;

    public float drill = 5.7f; //进入钻地需要的格数
    public Color drillColor;
    public Color drillingColor = Color.black;

    public float inverted = 7.3f;
    public bool isInverted;
    public Color invertedColor = Color.yellow;

    public bool isColorCoroutineRunning;

    #endregion
    #region Move info
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 13f;
    public float drillSpeed = 2f;

    public float xInput;
    private float jumpingGravity = 3f;
    private float defaultGravity = 8f;
    private bool jumpInput;
    public bool isJumping;
    #endregion
    #region Character States
    [SerializeField] private LayerMask groundLayer; // 用于地面检测的 Layer
    [SerializeField] private LayerMask platformLayer; // 用于平台检测的 Layer
    public Transform groundCheck; // 地面检测的起始点
    public float groundCheckRadius = 0.35f; // 地面检测的半径
    [SerializeField] private bool canDrill;
    [SerializeField] private Transform drillCheck;
    [SerializeField] private float drillCheckRadius;

    public bool headHit;
    [SerializeField] private Transform headCheck; // 头部检测的起始点
    [SerializeField] private float headCheckDistance = 0.2f; // 头部检测的距离
    private float jumpTimer;
    public bool isJumpLocked;
    public float jumpTimeCounter;
    private float jumpDuration = 0.5f;
    private float lockedYPosition;

    public bool isDead;
    [SerializeField] private bool killedByFall;
    public bool killedByNail;
    private bool isFallingCondition;
    public bool isFalling;

    public bool isOnPlatform;
    public bool isGrounded;
    public bool isMoving;

    public bool isFacingRight = true;

    private float highestPos;
    public float fallDistance;
    #endregion
    #region MobilePhoneInput
    public VariableJoystick variableJoystick;
    #endregion
    [SerializeField] private float testFloat;
    public System.Action RespawnSystemAction;
    public System.Action InvertedSystemAction;


    private void Start()
    {
        highestPos = transform.position.y;

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

        ResetInverted();
    }

    private void FixedUpdate()
    {
        // 检测地面
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        //isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius, groundLayer);

        //检测平台
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
        if (isOnPlatform) isGrounded = true;

        canDrill = Physics2D.OverlapCircle(drillCheck.position, drillCheckRadius, groundLayer | platformLayer);

        //检测头部
        headHit = Physics2D.OverlapCircle(headCheck.position, headCheckDistance, groundLayer);
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isOnPlatform = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer);
        if (isOnPlatform) isGrounded = true;
        canDrill = Physics2D.OverlapCircle(drillCheck.position, drillCheckRadius, groundLayer | platformLayer);

        //空格重开
        if (isDead)
        {
            if (killedByFall)
                sr.color = fallColor;
            else if (killedByNail)
                sr.color = deadColor;

            if(Input.GetButtonDown("Jump") || Input.GetButtonDown("JoyJump"))
                if (!isRespawnCorRunning)
                    StartCoroutine(Respawn());


            //手机端逻辑
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector3 pos = Input.GetTouch(i).position;

                if (pos.x > Screen.width / 2)
                {
                    Debug.Log("isJumpInput"+ jumpInput);
                    if (!isRespawnCorRunning)
                        StartCoroutine(Respawn());
                    break;
                }
            }

            return;
        }
        if (isRespawnCorRunning) return;

        //翻转角色
        Flip();

        //控制输入
        ApplyMovement();

        //跳跃逻辑
        JumpLogic();

        // 更新颜色
        UpdateColor();

        //滞空重力
        GravityChange();

        //摔落距离状态
        FallDistanceState();
        //↑↓这两个方法代码位置不能交换
        // 记录坠落高度
        IsFallingLogic();

        stateMachine.Update();

        InvertedLogic();

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    InvertedSetup();
        //}
    }

    private void FallDistanceState()
    {
        if (fallDistance > red && isGrounded && fallDistance < bounce)
        {
            Die();
            if (!killedByNail) killedByFall = true;
        }
        else if (fallDistance > bounce && fallDistance < drill)
        {
            if (isGrounded)
                ChangeState(bouncingState);
        }
        else if (fallDistance > drill && fallDistance < inverted)
        {
            StarAnimRotate();

            if (!drillingCoroutineRunning)
                StartCoroutine(nameof(HandleDrillingState));

            ChangeState(drillingState);
        }
        else if (fallDistance > inverted)
        {
            StopDrillingCoroutine();
            //Debug.Log("大于距离");
            if (isGrounded)
            {
                //Debug.Log("大于距离且触地");
                InvertedSetup();

                StartSetDefaultColor(0.1f);
            }
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

    }

    public void UpdateColor()
    {
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
        else if (fallDistance > drill && fallDistance < inverted)
        {
            sr.color = drillColor;
        }
        else if (fallDistance > inverted)
        {
            sr.color = invertedColor;
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

    public void ResetInverted()
    {
        //Debug.Log("PlayerPrefs IsPlayerInverted" + PlayerPrefs.GetInt("IsPlayerInverted"));
        //Debug.Log("Player isInverted:" + isInverted);
        if (PlayerPrefs.GetInt("IsPlayerInverted") == 1 && !isInverted)
            InvertedSetup();
        else if (PlayerPrefs.GetInt("IsPlayerInverted") == 0 && isInverted)
            InvertedSetup();
    }

    [SerializeField] private TransitionEffect transitionEffect;

    private bool isRespawnCorRunning;

    public IEnumerator Respawn()
    {
        isRespawnCorRunning = true;

        transitionEffect.PlayerDied();

        yield return new WaitUntil(() => !transitionEffect.isTransitioning);

        highestPos = PlayerPrefs.GetFloat("PlayerY", 0);
        RespawnSystemAction?.Invoke();

        //参数调整
        isDead = false;
        killedByFall = false;
        anim.SetBool("Dead", false);
        sr.color = normalColor;
        ChangeState(idleState);

        //修改（摔死）死亡动画位置使其匹配视觉效果
        anim.transform.localPosition = killedByNail ? new Vector3(0, 0.25f, 0) : new Vector3(0, 0.25f, 0);
        killedByNail = false;

        ResetInverted();

        yield return null;
        transitionEffect.PlayerRespawn();

        isRespawnCorRunning = false;
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX(2);
        anim.SetBool("Dead", true);
        isDead = true;
        rb.velocity = new Vector2(0, 0);
        //修改死亡动画位置使其匹配视觉效果

        isFalling = false;
        fallDistance = 0;

        anim.transform.localRotation = Quaternion.identity;

        if (killedByNail) return;//摔死调整动画动画位置使其匹配视觉效果
        Vector3 newPosition = anim.transform.position;
        newPosition.y -= 0.12f;
        anim.transform.position = newPosition;
    }

    private float IsFallingLogic() //坠落逻辑
    {
        //Debug.Log("isFallingCondition :" + isFallingCondition + "isFalling" + isFalling + "fallDistance = " + fallDistance + "isGrounded = " + isGrounded);

        isFallingCondition = isInverted ? rb.velocity.y > 0 : rb.velocity.y < 0;

        isFalling = isFallingCondition && !isGrounded;

        if (!isFalling)
        {
            highestPos = transform.position.y;
            fallDistance = 0;
        }
        else
        {
            fallDistance = isInverted ? transform.position.y - highestPos : highestPos - transform.position.y;
            //根据角色翻转状态决定
        }

        return fallDistance;
    }

    private void OnDrawGizmos()  //检测射线
    {
        if (groundCheck == null || headCheck == null || drillCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        //Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(drillCheck.position, drillCheckRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(headCheck.position, headCheckDistance);
        //Gizmos.DrawLine(headCheck.position, headCheck.position + Vector3.up * headCheckDistance);
    }

    private void ApplyMovement()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        //手机端逻辑
        xInput = xInput == 0 ? variableJoystick.Horizontal : xInput;

        rb.velocity = drillingCoroutineRunning ? new Vector2(xInput * drillSpeed / 2, rb.velocity.y) : new Vector2(xInput * moveSpeed, rb.velocity.y);

        isMoving = Mathf.Abs(rb.velocity.x) > 0.1f;
    }

    private void JumpLogic()
    {
        jumpInput = Input.GetButtonDown("Jump") || Input.GetButtonDown("JoyJump");

        //手机端逻辑
        for (int i = 0; i < Input.touchCount; i++)
        {
            Vector3 pos = Input.GetTouch(i).position;
            jumpInput = (pos.x > Screen.width / 2);
        }

        if (drillingCoroutineRunning) return;

        if (jumpInput && isGrounded)
        {
            if (!rotating) //bounce判断符
            {
                AudioManager.instance.PlaySFX(4, 0.8f);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isJumping = true;
                jumpTimeCounter = 0f;
                //isJumpLocked = false;
                ChangeState(airState);
            }
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
        //Debug.Log("旋转动画协程");

        isAnimRotated = true;

        anim.transform.Rotate(Vector3.back, 180f);

        yield return new WaitForSeconds(0.1f);

        isAnimRotated = false;
    }

    private IEnumerator AnimlocalRotation()
    {
        //Debug.Log("复原旋转动画协程");
        isAnimRotated = true;

        anim.transform.localRotation = Quaternion.identity;

        yield return null;

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
                anim.transform.localRotation = Quaternion.identity;

                ChangeState(airState);

                bounceRotateCoroutine = null;

                yield break;
            }
            yield return null;
        }
    }
    #endregion

    #region DrillLogic

    private bool drillingCoroutineRunning;
    public bool dangerOfNails; //玩家钻地时可以被钉子杀死

    private IEnumerator HandleDrillingState()
    {
        drillingCoroutineRunning = true;

        bool hasEnteredIgnoreCollision = false;

        dangerOfNails = true;

        //Debug.Log("开始钻地协程");

        while (!hasEnteredIgnoreCollision) //进入地面前
        {
            bool isTouchingGround = canDrill;

            //Debug.Log($"cd.IsTouchingLayers(Ground): {isTouchingGround}");

            //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);


            if (!hasEnteredIgnoreCollision && isTouchingGround) //第一次接触地面
            {
                AudioManager.instance.PlaySFX(1);
                cd.isTrigger = true;
                hasEnteredIgnoreCollision = true;
                rb.gravityScale = 0;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                //rb.velocity = new Vector2(rb.velocity.x, -drillSpeed);
                rb.velocity = isInverted ? new Vector2(rb.velocity.x, drillSpeed) : new Vector2(rb.velocity.x, -drillSpeed);
                //Debug.Log("忽略碰撞层");
            }
            yield return null;
        }//接触地面后进行下面的while循环

        //yield return new WaitForSeconds(0.2f);

        while (true)  //钻入地面
        {
            bool isTouchingGround = cd.IsTouchingLayers(groundLayer | platformLayer);

            //Debug.Log($"cd.IsTouchingLayers(Ground): {isTouchingGround}");

            if (hasEnteredIgnoreCollision && isTouchingGround)  //钻地进行时
            {
                //rb.velocity = new Vector2(rb.velocity.x, -drillSpeed);
                rb.velocity = isInverted ? new Vector2(rb.velocity.x, drillSpeed) : new Vector2(rb.velocity.x, -drillSpeed);
            }

            if (hasEnteredIgnoreCollision && !isTouchingGround || isDead)  //退出钻地
            {
                //Debug.Log("恢复碰撞层");
                //Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
                AudioManager.instance.PlaySFX(1);
                cd.isTrigger = false;
                ChangeState(airState);  // 回到airState状态或者其他适合的状态
                rb.gravityScale = jumpingGravity;
                StartCoroutine(AnimlocalRotation());
                highestPos = transform.position.y;

                //Debug.Log("重置玩家摔落高度");
                yield return new WaitForSeconds(0.2f);
                drillingCoroutineRunning = false;

                yield return null;
                dangerOfNails = false;

                yield break;  // 退出协程
            }

            yield return null;  // 等待下一帧
        }
    }

    private void StopDrillingCoroutine()
    {
        StopCoroutine(nameof(HandleDrillingState));
        cd.isTrigger = false;
        drillingCoroutineRunning = false;
        dangerOfNails = false;
        ChangeState(airState);
    }

    #endregion

    #region InvertedLogic

    public void InvertedSetup()
    {
        //Debug.Log("反转");
        // 如果已反转,复原,如果未反转,执行反转
        if (isInverted)
        {
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        }

        StartCoroutine(AnimlocalRotation());

        defaultGravity = -defaultGravity;
        jumpingGravity = -jumpingGravity;
        jumpForce = -jumpForce;
        bounceHeight = -bounceHeight;
        isFacingRight = !isFacingRight;

        AudioManager.instance.PlaySFX(1);
        //Debug.Log("Player's InvertedSystemAction");
        InvertedSystemAction?.Invoke();

        isInverted = !isInverted;
    }

    private void InvertedLogic()
    {
        //if (isInverted)
        //{
        //    if (rb.velocity.y < -0.5)
        //        dangerOfNails = true;
        //    else
        //        dangerOfNails = false;
        //}
    }

    #endregion

    public void ChangeState(PlayerState newState)
    {
        stateMachine.SetState(newState);
    }
}
