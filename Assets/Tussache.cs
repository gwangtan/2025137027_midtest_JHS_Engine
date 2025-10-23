using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    [Header("����ü ����")]
    public float speed = 10f;           // �̵� �ӵ�
    public int damage = 10;              // ��������
    public float lifeTime = 5f;          // �ڵ� �ı� �ð�

    private void Start()
    {
        // ���� �ð� �� �ڵ� �ı�
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // �������� �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player �±׿� �浹���� ��
        if (other.CompareTag("Player"))
        {
            // PlayerHealth ��ũ��Ʈ ��������
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // ����ü ����
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Monster")) // ���� �ڽŰ� �浹�� ����
        {
            // ��, ���� �� �ٸ� �Ͱ� �ε����� ����
            Destroy(gameObject);
        }
    }
}
