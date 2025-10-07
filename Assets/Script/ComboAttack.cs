using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ComboAttackController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;
    private CharacterController controller;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 720f; // 회전 속도

    [Header("Combo Attack")]
    public float comboResetTime = 1f;  // 콤보 초기화 시간
    private int attackIndex = 0;        // 현재 공격 단계
    private bool isAttacking = false;   // 공격 중인지
    private float lastAttackTime;       // 마지막 공격 입력 시간

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
        // 공격 중이면 이동 불가
        if (isAttacking) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(h, 0, v);

        if (move.magnitude > 0.1f)
        {
            // 캐릭터 회전
            Vector3 dir = move.normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            // 이동
            controller.Move(dir * moveSpeed * Time.deltaTime);
        }

        // 블렌드 트리용 Speed 파라미터
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

    // 애니메이션 이벤트에서 호출
    public void EndAttack()
    {
        isAttacking = false;
    }
}
