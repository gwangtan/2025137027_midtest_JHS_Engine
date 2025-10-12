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
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                // 몬스터에게 데미지 주기
                monster.TakeDamage(damage);

                // 카메라 흔들림
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.TriggerShake(0.15f, 0.2f);
                }

                // 피격 사운드 재생
                if (audioSource != null)
                {
                    audioSource.Play();
                }
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
