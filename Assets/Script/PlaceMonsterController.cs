using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Collider))]
public class StationaryShooterMonster : MonoBehaviour
{
    [Header("기본 설정")]
    public float detectionRange = 15f;
    public float attackInterval = 2f;
    public int maxHealth = 30;
    private int currentHealth;
    private Transform player;

    [Header("투사체 설정")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 10f;

    [Header("시각 효과 (몸체 진동)")]
    public Transform[] bodyParts;
    public float shakeAmount = 0.05f;
    public float shakeSpeed = 10f;

    [Header("UI")]
    public Slider healthSlider;

    [Header("파괴 연출")]
    public GameObject[] explosionParts;
    public float explosionForce = 250f;
    public float explosionRadius = 2f;
    public float destroyDelay = 5f;

    [Header("사운드 및 이펙트")]
    public GameObject deathEffectPrefab;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private bool isDead = false;
    private bool playerInRange = false;
    private Vector3[] originalPositions;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
            healthSlider.value = 1f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        audioSource = GetComponent<AudioSource>();

        originalPositions = new Vector3[bodyParts.Length];
        for (int i = 0; i < bodyParts.Length; i++)
            if (bodyParts[i] != null)
                originalPositions[i] = bodyParts[i].localPosition;

        foreach (var part in explosionParts)
            if (part != null)
                part.SetActive(false);

        gameObject.tag = "Monster";

        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
        col.enabled = true;

        StartCoroutine(AttackRoutine());
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= detectionRange;

        if (playerInRange)
            ShakeBodyParts();
        else
            ResetBodyParts();
    }

    IEnumerator AttackRoutine()
    {
        while (!isDead)
        {
            if (playerInRange)
                ShootProjectile();

            yield return new WaitForSeconds(attackInterval);
        }
    }

    void ShootProjectile()
    {
        if (projectilePrefab == null || shootPoint == null || player == null) return;

        //  플레이어를 향한 방향 계산
        Vector3 dir = (player.position - shootPoint.position).normalized;

        //  투사체 회전 적용
        Quaternion lookRot = Quaternion.LookRotation(dir, Vector3.up);

        //  투사체 생성
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, lookRot);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.velocity = dir * projectileSpeed;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (healthSlider != null)
            healthSlider.value = (float)currentHealth / maxHealth;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        StopAllCoroutines();

        if (deathEffectPrefab != null)
            Instantiate(deathEffectPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        ResetBodyParts();

        foreach (var part in bodyParts)
            if (part != null)
                part.gameObject.SetActive(false);

        foreach (var mesh in GetComponentsInChildren<SkinnedMeshRenderer>())
            mesh.enabled = false;

        foreach (var part in explosionParts)
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
    }

    void ShakeBodyParts()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            if (bodyParts[i] == null) continue;

            Vector3 original = originalPositions[i];
            float offsetX = Mathf.Sin(Time.time * shakeSpeed + i) * shakeAmount;
            float offsetZ = Mathf.Cos(Time.time * shakeSpeed + i) * shakeAmount;
            bodyParts[i].localPosition = original + new Vector3(offsetX, 0f, offsetZ);
        }
    }

    void ResetBodyParts()
    {
        for (int i = 0; i < bodyParts.Length; i++)
        {
            if (bodyParts[i] == null) continue;
            bodyParts[i].localPosition = Vector3.Lerp(bodyParts[i].localPosition, originalPositions[i], Time.deltaTime * 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Monster] OnTriggerEnter: {other.name}");
    }
}
