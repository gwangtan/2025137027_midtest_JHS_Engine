using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("�̵� ����")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpPower = 5f;
    public float gravity = -9.81f;

    [Header("ī�޶� ����")]
    public float mouseSensitivity = 400f;
    private float xRotation = 0f;

    [Header("������Ʈ")]
    public Animator animator;
    private CharacterController controller;
    private Camera playerCamera;

    private Vector3 moveDirection;
    private float currentSpeed;
    private Vector3 velocity;

    [Header("�޺� ����")]
    public float attackDuration = 0.6f;  // �� ���� �ִϸ��̼� ����
    private int attackIndex = 0;          // 0=Attack, 1=Attack1, 2=Attack2
    private bool isAttacking = false;
    private bool queuedAttack = false;    // �� ���� ����
    private float attackCooldownTime = 0f; // ��Ÿ�� �� �ð�

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
    // �̵�
    // ---------------------------
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = transform.forward * vertical + transform.right * horizontal;
        moveDirection.Normalize();

        // ���� �� �̵� ����
        currentSpeed = (isAttacking) ? 0f :
                       (moveDirection.magnitude >= 0.1f) ? (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed) : 0f;

        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    // ---------------------------
    // ȸ��
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
    // ���� & �߷�
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
    // Animator ������Ʈ
    // ---------------------------
    void UpdateAnimator()
    {
        float animatorSpeed = Mathf.Clamp01(currentSpeed / runSpeed);
        animator.SetFloat("Speed", animatorSpeed);
    }

    // ---------------------------
    // ���� �Է� ó��
    // ---------------------------
    void HandleAttackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ��Ÿ�� ����, �� �޺� �߿��� ����
            if (!isAttacking && Time.time < attackCooldownTime)
                return;

            if (isAttacking)
            {
                if (!queuedAttack && attackIndex <= 2)
                    queuedAttack = true; // �� ���� ����
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

        attackIndex = Mathf.Min(attackIndex + 1, 3); // Attack2 ���Ĵ� 3���� ����
    }

    void EndAttack()
    {
        isAttacking = false;

        if (queuedAttack && attackIndex <= 2)
        {
            // ����� ������ ������ ��� ���� ���� ����, ��Ÿ�� ����
            queuedAttack = false;
            DoAttack();
        }
        else
        {
            // Idle�� ���ư� ��쿡�� ��Ÿ�� ����
            switch (attackIndex)
            {
                case 1: attackCooldownTime = Time.time + 2f; break;    // Attack
                case 2: attackCooldownTime = Time.time + 2f; break;    // Attack1
                case 3: attackCooldownTime = Time.time + 2.5f; break;  // Attack2
            }

            // Attack2 �Ϸ� �� Idle�� ����
            if (attackIndex > 2)
                attackIndex = 0;
            // Attack / Attack1 �� Ŭ�� ������ attackIndex ���� �� Idle�� �ٷ� �� ���ư�
        }
    }
}
