using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class MonsterController : MonoBehaviour
{
    [Header("기본 설정")]
    public float moveSpeed = 3f;
    public float detectionRange = 15f;
    public int maxHealth = 30;

    [Header("파괴 연출")]
    public GameObject[] bodyParts; // 6개의 파츠 모델
    public float explosionForce = 250f;
    public float explosionRadius = 2f;
    public float destroyDelay = 5f; // 몇 초 후 파편 삭제

    private int currentHealth;
    private CharacterController controller;
    private Transform player;
    private bool isAttackingPlayer = false;
    private bool isDead = false;

    public GameObject deathEffectPrefab; // Inspector에 넣기

    public AudioClip deathSound;
    private AudioSource audioSource;

    public GameObject body; // Body 모델 직접 넣기


    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        var playerCC = player.GetComponent<CharacterController>();
        if (playerCC != null)
            Physics.IgnoreCollision(controller, playerCC);

        // 시작 시 파츠 비활성화
        foreach (var part in bodyParts)
        {
            if (part != null)
                part.SetActive(false);
        }
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;
            controller.Move(direction * moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (body != null)
            body.SetActive(false);

        controller.enabled = false; // 이동 정지
        StopAllCoroutines(); // 플레이어 공격 중단

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        // 본체의 시각적 모델 비활성화
        foreach (var mesh in GetComponentsInChildren<SkinnedMeshRenderer>())
            mesh.enabled = false;

        foreach (var part in bodyParts)
        {
            if (part == null) continue;
            part.SetActive(true);

            Rigidbody rb = part.GetComponent<Rigidbody>();
            Collider col = part.GetComponent<Collider>();

            if (rb == null) rb = part.AddComponent<Rigidbody>();
            if (col == null) col = part.AddComponent<BoxCollider>();

            rb.mass = 0.5f;
            rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);

            part.layer = LayerMask.NameToLayer("Debris");

            Destroy(part, destroyDelay);
        }



        Destroy(gameObject, destroyDelay + 0.5f);

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player") && !isAttackingPlayer)
        {
            StartCoroutine(DamagePlayerRoutine(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            isAttackingPlayer = false;
        }
    }

    IEnumerator DamagePlayerRoutine(GameObject playerObj)
    {
        isAttackingPlayer = true;
        PlayerHealth playerHealth = playerObj.GetComponent<PlayerHealth>();

        while (isAttackingPlayer)
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(2); foreach (var mesh in GetComponentsInChildren<SkinnedMeshRenderer>())
                mesh.enabled = false;
            yield return new WaitForSeconds(1.5f);
        }
    }
}
