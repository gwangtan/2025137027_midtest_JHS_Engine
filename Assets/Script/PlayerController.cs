using UnityEngine;
using System.Collections;

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
    private bool queuedAttack = false;
    private float attackCooldownTime = 0f;

    //  카메라 회전 효과 관련
    private Quaternion originalCamRot;
    private Coroutine camEffectCoroutine;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;

        if (animator == null)
            animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerCamera != null)
            originalCamRot = playerCamera.transform.localRotation;
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

        currentSpeed = (isAttacking) ? 0f :
                       (moveDirection.magnitude >= 0.1f) ? (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) : 0f;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    // ---------------------------
    // 마우스 회전
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
            if (!isAttacking && Time.time < attackCooldownTime)
                return;

            if (isAttacking)
            {
                if (!queuedAttack && attackIndex <= 2)
                    queuedAttack = true;
            }
            else
            {
                DoAttack();
            }
        }
    }

    // ---------------------------
    // 공격 실행
    // ---------------------------
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

        //  공격별 카메라 회전 효과 적용
        if (camEffectCoroutine != null)
            StopCoroutine(camEffectCoroutine);

        if (attackClip == "Attack")
            camEffectCoroutine = StartCoroutine(SmoothCameraSwing(new Vector3(12f, 12f, 4f)));
        else if (attackClip == "Attack1")
            camEffectCoroutine = StartCoroutine(SmoothCameraSwing(new Vector3(4f, 12f, -6f)));
        else if (attackClip == "Attack2")
            camEffectCoroutine = StartCoroutine(SmoothCameraSwing(new Vector3(0f, 0f, 4f)));

        CancelInvoke(nameof(EndAttack));
        Invoke(nameof(EndAttack), attackDuration);

        attackIndex = Mathf.Min(attackIndex + 1, 3);
    }

    // ---------------------------
    // 공격 종료
    // ---------------------------
    void EndAttack()
    {
        isAttacking = false;

        if (queuedAttack && attackIndex <= 2)
        {
            queuedAttack = false;
            DoAttack();
        }
        else
        {
            switch (attackIndex)
            {
                case 1: attackCooldownTime = Time.time + 1f; break;
                case 2: attackCooldownTime = Time.time + 1f; break;
                case 3: attackCooldownTime = Time.time + 1.5f; break;
            }

            if (attackIndex > 2)
                attackIndex = 0;
        }
    }

    // ---------------------------
    // 카메라 부드러운 회전 타격 효과
    // ---------------------------
    IEnumerator SmoothCameraSwing(Vector3 targetEuler)
    {
        Quaternion startRot = playerCamera.transform.localRotation;
        Quaternion targetRot = startRot * Quaternion.Euler(targetEuler);

        float halfDuration = attackDuration * 0.5f; // 전반: 회전, 후반: 복귀
        float t = 0f;

        // 1️⃣ 전반부 - 부드럽게 회전
        while (t < 1f)
        {
            t += Time.deltaTime / halfDuration;
            playerCamera.transform.localRotation = Quaternion.Slerp(startRot, targetRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        // 2️⃣ 후반부 - 부드럽게 복귀
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / halfDuration;
            playerCamera.transform.localRotation = Quaternion.Slerp(targetRot, startRot, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        playerCamera.transform.localRotation = startRot;
    }
}
