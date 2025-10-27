using UnityEngine;

public class GroundAttackDamage : MonoBehaviour
{
    public int damage = 15;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
                hp.TakeDamage(damage);
        }
    }
}
