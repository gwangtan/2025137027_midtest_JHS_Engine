using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ComboAttackController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f; // ȸ�� �ӵ�

    [Header("Combo Attack")]
    public float comboResetTime = 1f;  // �޺� �ʱ�ȭ �ð�
    private int attackIndex = 0;        // ���� ���� �ܰ�
    private bool isAttacking = false;   // ���� ������
    private float lastAttackTime;       // ������ ���� �Է� �ð�

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMovement();
        HandleAttack();
        ResetComboTimer();
    }

    void HandleMovement()
    {
        // ���� ���̸� �̵� �Ұ�
        if (isAttacking) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            // ĳ���� ȸ��
            Vector3 dir = move.normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            // �̵�
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }

        // ���� Ʈ���� Speed �Ķ����
        animator.SetFloat("Speed", move.magnitude);
    }

    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DoAttack();
        }
    }

    void DoAttack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;

        switch (attackIndex)
        {
            case 0:
                animator.SetTrigger("Attack");
                break;
            case 1:
                animator.SetTrigger("Attack1");
                break;
            case 2:
                animator.SetTrigger("Attack2");
                break;
        }

        attackIndex = (attackIndex + 1) % 3;
    }

    void ResetComboTimer()
    {
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            attackIndex = 0;
        }
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ��
    public void EndAttack()
    {
        isAttacking = false;
    }
}
