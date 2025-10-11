using UnityEngine;

public class CameraFollowFixedOffset : MonoBehaviour
{
    public Transform target;      // 따라갈 대상
    public float xOffset = -5f;   // x축 거리
    public float zOffset = 1.7f;  // z축 거리

    private float fixedY;         // 고정된 y좌표

    void Start()
    {
        // 시작 시 카메라의 y값 고정
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 대상 위치 기준으로 x, z에 오프셋 적용
        transform.position = new Vector3(
            target.position.x + xOffset,
            fixedY,
            target.position.z + zOffset
        );
    }
}
