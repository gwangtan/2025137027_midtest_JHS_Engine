using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHealth = 100;
    private int currentHealth;
    public Slider hpSlider;

    [Header("피격 화면 효과")]
    public Image damageOverlay;          // Canvas 위의 붉은 이미지 연결
    public float flashSpeed = 0.2f;      // 붉게 번지는 속도
    public float fadeSpeed = 1.5f;       // 서서히 사라지는 속도
    public float maxAlpha = 0.4f;        // 최대 투명도 (0~1)

    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        hpSlider.value = 1f;

        if (damageOverlay != null)
            damageOverlay.color = new Color(1f, 0f, 0f, 0f);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"플레이어 체력: {currentHealth}");

        hpSlider.value = (float)currentHealth / maxHealth;

        //  피격 효과 실행
        if (damageOverlay != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageFlash()
    {
        // 1️붉게 번쩍
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flashSpeed;
            float a = Mathf.Lerp(0f, maxAlpha, t);
            damageOverlay.color = new Color(1f, 0f, 0f, a);
            yield return null;
        }

        // 2️서서히 사라짐
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            float a = Mathf.Lerp(maxAlpha, 0f, t);
            damageOverlay.color = new Color(1f, 0f, 0f, a);
            yield return null;
        }

        damageOverlay.color = new Color(1f, 0f, 0f, 0f);
    }

    void Die()
    {
        Debug.Log("플레이어 사망!");
        // 사망 처리 로직 추가 가능
    }
}
