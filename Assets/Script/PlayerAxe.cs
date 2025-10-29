using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;
    public PlayerController playerController;

    [Header("Sound Settings")]
    public AudioSource audioSource;   // 오디오 소스에 미리 히트 사운드(AudioClip) 연결

    private void OnTriggerEnter(Collider other)
    {
        if (playerController != null && other.CompareTag("Monster") && IsAttacking())
        {
            bool hitSomething = false;

            // 1️⃣ 일반 몬스터
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                hitSomething = true;
            }
            else
            {
                // 2️⃣ 고정형 몬스터
                StationaryShooterMonster shooterMonster = other.GetComponent<StationaryShooterMonster>();
                if (shooterMonster != null)
                {
                    shooterMonster.TakeDamage(damage);
                    hitSomething = true;
                }
                else
                {
                    // 3️⃣ ✅ 보스 패턴1 (BossPattern1)
                    BossController boss = other.GetComponent<BossController>();
                    if (boss != null)
                    {
                        boss.TakeDamage(damage);
                        hitSomething = true;
                    }
                }
            }

            // 4️⃣ 피격 반응 (둘 중 하나라도 맞았을 때)
            if (hitSomething)
            {
                if (CameraShake.Instance != null)
                    CameraShake.Instance.TriggerShake(0.15f, 0.2f);

                if (audioSource != null)
                    audioSource.Play();
            }
        }
    }

    private bool IsAttacking()
    {
        // PlayerController 내부의 isAttacking 플래그 활용
        var isAttackingField = typeof(PlayerController).GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (bool)isAttackingField.GetValue(playerController);
    }
}
