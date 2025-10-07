using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 5f;
    public float gravity = -9.81f;

    [Header("카메라 설정")]
    public float mouseSensitivity = 400f;
    private float xRotation = 0f;

    [Header("컴포넌트")]
    public Animator animator;
    private CharacterController controller;
    private Camera playerCamera;

    private Vector3 moveDirection;
    private float currentSpeed;
    private Vector3 velocity;

    [Header("콤보 공격")]
    public float attackDuration = 0.6f;  // 각 공격 애니메이션 길이
    private int attackIndex = 0;          // 0=Attack, 1=Attack1, 2=Attack2
    private bool isAttacking = false;
    private bool queuedAttack = false;    // 한 번만 예약
    private float attackCooldownTime = 0f; // 쿨타임 끝 시간

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        if (animator == null)
            animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleJumpAndGravity();
        HandleMovement();
        HandleAttackInput();
        UpdateAnimator();
    }

    // ---------------------------
    // 이동
    // ---------------------------
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection.Normalize();

        // 공격 중 이동 제한
        currentSpeed = (isAttacking) ? 0f :
                       (moveDirection.magnitude >= 0.1f) ? (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) : 0f;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    // ---------------------------
    // 회전
    // ---------------------------
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    // ---------------------------
    // 점프 & 중력
    // ---------------------------
    void HandleJumpAndGravity()
    {
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (controller.isGrounded && Input.GetButtonDown("Jump") && !isAttacking)
            velocity.y = jumpPower;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    // ---------------------------
    // Animator 업데이트
    // ---------------------------
    void UpdateAnimator()
    {
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("Speed", animatorSpeed);
    }

    // ---------------------------
    // 공격 입력 처리
    // ---------------------------
    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 쿨타임 적용, 단 콤보 중에는 무시
            if (!isAttacking && Time.time < attackCooldownTime)
                return;

            if (isAttacking)
            {
                if (!queuedAttack && attackIndex <= 2)
                    queuedAttack = true; // 한 번만 예약
            }
            else
            {
                DoAttack();
            }
        }
    }

    void DoAttack()
    {
        isAttacking = true;
        queuedAttack = false;

        string attackClip = "";

        switch (attackIndex)
        {
            case 0: attackClip = "Attack"; break;
            case 1: attackClip = "Attack1"; break;
            case 2: attackClip = "Attack2"; break;
        }

        animator.CrossFadeInFixedTime(attackClip, 0f);

        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackDuration);

        attackIndex = Mathf.Min(attackIndex + 1, 3); // Attack2 이후는 3으로 유지
    }

    void EndAttack()
    {
        isAttacking = false;

        if (queuedAttack && attackIndex <= 2)
        {
            // 예약된 공격이 있으면 즉시 다음 공격 실행, 쿨타임 무시
            queuedAttack = false;
            DoAttack();
        }
        else
        {
            // Idle로 돌아갈 경우에만 쿨타임 적용
            switch (attackIndex)
            {
                case 1: attackCooldownTime = Time.time + 2f; break;    // Attack
                case 2: attackCooldownTime = Time.time + 2f; break;    // Attack1
                case 3: attackCooldownTime = Time.time + 2.5f; break;  // Attack2
            }

            // Attack2 완료 후 Idle로 복귀
            if (attackIndex > 2)
                attackIndex = 0;
            // Attack / Attack1 중 클릭 없으면 attackIndex 유지 → Idle로 바로 안 돌아감
        }
    }
}
