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
            bool hitSomething = false;

            //  ���� ���� Ÿ��
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
                hitSomething = true;
            }
            else
            {
                //  �������� �ʴ� ���� Ÿ��
                StationaryShooterMonster shooterMonster = other.GetComponent<StationaryShooterMonster>();
                if (shooterMonster != null)
                {
                    shooterMonster.TakeDamage(damage);
                    hitSomething = true;
                }
            }

            //  �ǰ� ���� (�� �� �ϳ��� �¾��� ��)
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
        // PlayerController ������ isAttacking �÷��� Ȱ��
        var isAttackingField = typeof(PlayerController).GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (bool)isAttackingField.GetValue(playerController);
    }
}
