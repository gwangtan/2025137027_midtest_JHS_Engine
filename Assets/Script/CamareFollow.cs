using UnityEngine;

public class CameraFollowFixedOffset : MonoBehaviour
{
    public Transform target;      // ���� ���
    public float xOffset = -5f;   // x�� �Ÿ�
    public float zOffset = 1.7f;  // z�� �Ÿ�

    private float fixedY;         // ������ y��ǥ

    void Start()
    {
        // ���� �� ī�޶��� y�� ����
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ��� ��ġ �������� x, z�� ������ ����
        transform.position = new Vector3(
            target.position.x + xOffset,
            fixedY,
            target.position.z + zOffset
        );
    }
}
