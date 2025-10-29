using UnityEngine;
using System.Collections;

public class PlayerKnockbackController : MonoBehaviour
{
    private bool checkingWall = false;

    public void StartWallCheck()
    {
        if (!checkingWall)
            StartCoroutine(CheckWallCollision());
    }

    IEnumerator CheckWallCollision()
    {
        checkingWall = true;
        float timer = 3f;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    if (CameraShake.Instance != null)
                        CameraShake.Instance.TriggerShake(0.3f, 0.5f);
                    break;
                }
            }
            yield return null;
        }

        checkingWall = false;
    }
}
