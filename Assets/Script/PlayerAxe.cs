using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;
    public PlayerController playerController;

    [Header("Sound Settings")]
    public AudioSource audioSource;   // ����� �ҽ��� �̸� ��Ʈ ����(AudioClip) ����

    private void OnTriggerEnter(Collider other)
    {
        if (playerController != null && other.CompareTag("Monster") && IsAttacking())
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                // ���Ϳ��� ������ �ֱ�
                monster.TakeDamage(damage);

                // ī�޶� ��鸲
                if (CameraShake.Instance != null)
                {
                    CameraShake.Instance.TriggerShake(0.15f, 0.2f);
                }

                // �ǰ� ���� ���
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }
        }
    }

    private bool IsAttacking()
    {
        // PlayerController ������ isAttacking �÷��� Ȱ��
        var isAttackingField = typeof(PlayerController).GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (bool)isAttackingField.GetValue(playerController);
    }
}
