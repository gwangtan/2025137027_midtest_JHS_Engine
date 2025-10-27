using UnityEngine;
using System.Collections;

public class BossPattern1 : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public float detectionRange = 20f;
    private Transform player;
    private bool playerInRange = false;

    [Header("����1 ���� ����")]
    public GameObject warningPrefab;
    public GameObject attackPrefab;
    public float warningTime = 1f;     // ��� ǥ�� �ð�
    public float riseTime = 1.5f;      // �ڱ�ġ�� �ð�
    public int attackCount = 3;        // 1ȸ ���ݴ� �ڱ�ħ ��
    public int damage = 20;

    [Header("�ִϸ��̼�")]
    public Animator animator;
    public string attackTrigger = "GroundAttack";

    [Header("��Ÿ")]
    public float attackInterval = 3f;  // ���� �ݺ� ����
    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogWarning("Player�� ã�� ���߽��ϴ�!");

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

            // ���
            if (warningPrefab != null)
            {
                GameObject warning = Instantiate(warningPrefab, targetPos, Quaternion.identity);
                Destroy(warning, warningTime);
            }
            yield return new WaitForSeconds(warningTime);

            // ���� ������Ʈ ����
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

        // ���� ��ٿ�
        yield return new WaitForSeconds(attackInterval);
        isAttacking = false;

        // ������ ���� �� ������ Idle ���
        if (animator != null) animator.Play("Idle");
    }

}
