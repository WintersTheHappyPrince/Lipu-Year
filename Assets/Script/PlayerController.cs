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
    [SerializeField] private float red = 1.6f;
    [HideInInspector] public Color fallColor;
    public Color deadColor;
    public float bounce = 4f;
    public Color bounceColor;
    [HideInInspector] public Color drillColor;
    [HideInInspector] public Color invertColor;
    public bool isColorCoroutineRunning;
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

        //��������ж�
        GroundLogic();

        //ˤ�����״̬
        FallDistanceState();

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
        else if (fallDistance > bounce)
        {
            StartBounceRotate(720);
            //rb.velocity = new Vector2(rb.velocity.x, bounceHeight);
            if (isGrounded) ChangeState(bouncingState);
        }
    }

    private void GroundLogic()
    {
        if (isGrounded)
        {
            //fallDistance = 0;
            if (!isJumping) rb.gravityScale = defaultGravity;
        }
        if (!isGrounded)
        {
            rb.gravityScale = jumpingGravity;
        }
        if (isOnPlatform) isGrounded = true;
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

    public void UpdateColor()
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

        if (fallDistance >= red && fallDistance < bounce)
        {
            sr.color = fallColor;
        }
        else if (fallDistance > bounce)
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
        //�޸���������λ��ʹ��ƥ���Ӿ�Ч��
        if (killedByNail) return;
        Vector3 newPosition = anim.transform.position;
        newPosition.y -= 0.12f;
        anim.transform.position = newPosition;
    }

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
        Gizmos.DrawLine(headCheck.position, headCheck.position + Vector3.up * headCheckDistance);
    }

    private void ApplyMovement()
    {
        if (isDead)  //�ո��ؿ�
        {
            if (Input.GetKeyDown(KeyCode.Space))
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


    private bool rotating; // �����Ƿ�������ת�����ⲿ�ж�
    private Coroutine bounceRotateCoroutine = null; // ���ڱ���Э�̵�����
    public void StopBounceRotate()
    {
        // ���Э���������У�ֹͣ��
        if (bounceRotateCoroutine != null)
        {
            StopCoroutine(bounceRotateCoroutine);
            bounceRotateCoroutine = null;
        }

        rotating = false;
        anim.transform.rotation = Quaternion.identity;
    }
    private void StartBounceRotate(float speed)
    {
        if (bounceRotateCoroutine == null)
        {
            bounceRotateCoroutine = StartCoroutine(BounceRotate(speed));
        }
    }
    private IEnumerator BounceRotate(float speed)
    {
        float totalRotation = 0f; // �ۼ���ת�Ƕ�
        float totalRotationChange = 0f; // �ۼ���ת�仯��
        rotating = true;

        while (rotating)
        {
            //ÿ֡��ת�Ƕ�
            float rotationAmount = speed * Time.deltaTime;

            if(!isFacingRight)
                anim.transform.Rotate(Vector3.forward, rotationAmount);
            else
                anim.transform.Rotate(Vector3.forward, -rotationAmount);

            //��ת�Ƕȷ���
            if (isFacingRight)
                // �ۼ���ת�Ƕ�
                totalRotation += rotationAmount;
            else
                totalRotation -= rotationAmount;

            // ������ת�仯��
            totalRotationChange += Mathf.Abs(rotationAmount);

            // ����Ƿ����360����ת�����ⲿ�ж�
            if (totalRotationChange >= 360f)
            {
                // ��ת��ɣ�ֹͣ��ת
                rotating = false;

                // ������ת�Ƕ�
                totalRotation = 0f;
                totalRotationChange = 0f;

                // ��ԭrotation��0,0,0
                anim.transform.rotation = Quaternion.identity;

                bounceRotateCoroutine = null;

                yield break;
            }
            yield return null;
        }
    }

    public void ChangeState(PlayerState newState)
    {
        stateMachine.SetState(newState);
    }
}
