using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [Header("투사체 설정")]
    public float speed = 10f;           // 이동 속도
    public int damage = 10;              // 데미지량
    public float lifeTime = 5f;          // 자동 파괴 시간

    private void Start()
    {
        // 일정 시간 후 자동 파괴
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // 전방으로 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player 태그와 충돌했을 때
        if (other.CompareTag("Player"))
        {
            // PlayerHealth 스크립트 가져오기
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 투사체 제거
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Monster")) // 몬스터 자신과 충돌은 무시
        {
            // 벽, 지면 등 다른 것과 부딪히면 제거
            Destroy(gameObject);
        }
    }
}
