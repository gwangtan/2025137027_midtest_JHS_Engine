using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    [Header("�÷��̾� ����")]
    public float detectionRange = 25f;
    private Transform player;

    [Header("����1")]
    public GameObject warningPrefab;   // ��� ������Ʈ
    public GameObject attackPrefab;    // ���� ������Ʈ
    public float warningTime = 1f;     // ��� ǥ�� �ð�
    public float riseTime = 1.5f;      // �ڱ�ġ�� �ð�
    public int attackCount = 3;        // 1ȸ ���ݴ� �ڱ�ħ ��
    public int damage = 20;            // ���ݷ�

    [Header("����")]
    public float attackInterval = 3f;  // ���� �ݺ� ����
    public Animator animator;
    public string groundAttackTrigger = "GroundAttack";

    [Header("ü��")]
    public int maxHealth = 200;
    private int currentHealth;
    public Slider bossHpSlider;

    [Header("����/����Ʈ")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public GameObject deathEffectPrefab;
    private AudioSource audioSource;

    [Header("�ı�")]
    public GameObject[] bodyParts;
    public GameObject mainBody;
    public float explosionForce = 300f;
    public float explosionRadius = 3f;
    public float destroyDelay = 6f;

    private bool isDead = false;
    private bool isAttacking = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (animator == null) animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
        if (bossHpSlider != null) bossHpSlider.value = 1f;

        foreach (var part in bodyParts)
            if (part != null) part.SetActive(false);

        StartCoroutine(PatternRoutine());
    }

    void Update()
    {
        if (isDead || player == null) return;
    }

    // -------------------------
    // ����1�� ���
    // -------------------------
    IEnumerator PatternRoutine()
    {
        while (!isDead)
        {
            if (!isAttacking && player != null)
            {
                yield return StartCoroutine(Pattern1());
            }
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator Pattern1()
    {
        isAttacking = true;

        if (animator != null)
            animator.SetTrigger(groundAttackTrigger);

        for (int i = 0; i < attackCount; i++)
        {
            if (player == null) break;
            Vector3 targetPos = player.position;

            // ��� ǥ��
            if (warningPrefab != null)
            {
                GameObject w = Instantiate(warningPrefab, targetPos, Quaternion.identity);
                Destroy(w, warningTime);
            }

            yield return new WaitForSeconds(warningTime);

            // ���� ������Ʈ ����
            if (attackPrefab != null)
            {
                GameObject atk = Instantiate(attackPrefab, targetPos, Quaternion.identity);
                Rigidbody rb = atk.GetComponent<Rigidbody>();
                if (rb != null) rb.velocity = Vector3.up * 10f;

                yield return new WaitForSeconds(riseTime);
                Destroy(atk);
            }

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(attackInterval);
        isAttacking = false;

        if (animator != null && !isDead)
            animator.Play("Idle");
    }

    // -------------------------
    // �ǰ� / ��� ó��
    // -------------------------
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        if (bossHpSlider != null)
            bossHpSlider.value = (float)currentHealth / maxHealth;

        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        if (animator != null && !isDead)
            animator.SetTrigger("Hit");
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        isAttacking = false;

        if (animator != null)
            animator.Play("Die");

        if (mainBody != null)
            mainBody.SetActive(false);

        if (deathSound != null && audioSource != null)
            audioSource.PlayOneShot(deathSound);

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        // ���� Ȱ��ȭ �� ����ȿ�� ����
        foreach (var part in bodyParts)
        {
            if (part == null) continue;
            part.SetActive(true);

            Rigidbody rb = part.GetComponent<Rigidbody>();
            Collider col = part.GetComponent<Collider>();

            if (rb == null) rb = part.AddComponent<Rigidbody>();
            if (col == null) col = part.AddComponent<BoxCollider>();

            rb.mass = 1f;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            part.layer = LayerMask.NameToLayer("Debris");

            Destroy(part, destroyDelay);
        }

        Destroy(gameObject, destroyDelay + 1f);
    }
}
