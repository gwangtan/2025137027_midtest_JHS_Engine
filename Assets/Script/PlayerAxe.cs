using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        // ���� ���� ���� ����� ����
        if (playerController != null && other.CompareTag("Monster") && IsAttacking())
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                monster.TakeDamage(damage);
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
