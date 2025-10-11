using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;
    public PlayerController playerController;

    private void OnTriggerEnter(Collider other)
    {
        // 공격 중일 때만 대미지 적용
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
        // PlayerController 내부의 isAttacking 플래그 활용
        var isAttackingField = typeof(PlayerController).GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (bool)isAttackingField.GetValue(playerController);
    }
}
