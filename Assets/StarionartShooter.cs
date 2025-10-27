using UnityEngine;
using System.Collections;

public class BossPattern1 : MonoBehaviour
{
    [Header("플레이어 감지")]
    public float detectionRange = 20f;
    private Transform player;
    private bool playerInRange = false;

    [Header("패턴1 공격 설정")]
    public GameObject warningPrefab;
    public GameObject attackPrefab;
    public float warningTime = 1f;     // 경고 표시 시간
    public float riseTime = 1.5f;      // 솟구치는 시간
    public int attackCount = 3;        // 1회 공격당 솟구침 수
    public int damage = 20;

    [Header("애니메이션")]
    public Animator animator;
    public string attackTrigger = "GroundAttack";

    [Header("기타")]
    public float attackInterval = 3f;  // 패턴 반복 간격
    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogWarning("Player를 찾지 못했습니다!");

        if (animator == null) animator = GetComponent<Animator>();

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (player == null) return;

        Vector3 flatPlayerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        float distance = Vector3.Distance(transform.position, flatPlayerPos);
        playerInRange = distance <= detectionRange;
    }

    IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (playerInRange && !isAttacking)
            {
                StartCoroutine(Pattern1());
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Pattern1()
    {
        isAttacking = true;

        if (animator != null) animator.SetTrigger(attackTrigger);

        for (int i = 0; i < attackCount; i++)
        {
            Vector3 targetPos = player.position;

            // 경고
            if (warningPrefab != null)
            {
                GameObject warning = Instantiate(warningPrefab, targetPos, Quaternion.identity);
                Destroy(warning, warningTime);
            }
            yield return new WaitForSeconds(warningTime);

            // 공격 오브젝트 생성
            if (attackPrefab != null)
            {
                GameObject attack = Instantiate(attackPrefab, targetPos, Quaternion.identity);
                Rigidbody rb = attack.GetComponent<Rigidbody>();
                if (rb != null) rb.velocity = Vector3.up * 10f;

                BossAttackProjectile proj = attack.GetComponent<BossAttackProjectile>();
                if (proj == null) proj = attack.AddComponent<BossAttackProjectile>();
                proj.damage = damage;

                yield return new WaitForSeconds(riseTime);
                Destroy(attack);
            }

            yield return new WaitForSeconds(0.2f);
        }

        // 공격 쿨다운
        yield return new WaitForSeconds(attackInterval);
        isAttacking = false;

        // 마지막 공격 후 강제로 Idle 재생
        if (animator != null) animator.Play("Idle");
    }

}
