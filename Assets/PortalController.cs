using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [Header("포탈 설정")]
    public GameObject portalEffect;          // 포탈 시각 효과 (비활성 상태로 시작)
    public string[] stageNames = { "Stage1", "Stage2", "Stage3", "Stage4", "Stage5" };

    private bool isPortalActive = false;

    void Start()
    {
        if (portalEffect != null)
            portalEffect.SetActive(false);
    }

    void Update()
    {
        // 현재 씬에 살아있는 몬스터가 있는지 체크
        MonsterController[] monsters = FindObjectsOfType<MonsterController>();
        bool allDead = true;

        foreach (MonsterController monster in monsters)
        {
            if (monster != null && monster.gameObject.activeInHierarchy)
            {
                allDead = false;
                break;
            }
        }

        if (allDead && !isPortalActive)
        {
            ActivatePortal();
        }
    }

    void ActivatePortal()
    {
        isPortalActive = true;
        if (portalEffect != null)
            portalEffect.SetActive(true);
        Debug.Log("모든 몬스터가 처치됨! 포탈이 열렸습니다!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"포탈 트리거 감지됨: {other.name}");

        if (!isPortalActive)
        {
            Debug.Log("포탈이 아직 비활성 상태입니다.");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Player 태그 아님.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int index = System.Array.IndexOf(stageNames, currentScene);

        Debug.Log($"현재씬: {currentScene} / 인덱스: {index}");

        if (index != -1 && index < stageNames.Length - 1)
        {
            string nextStage = stageNames[index + 1];
            Debug.Log($"씬 전환 시도: {nextStage}");
            SceneManager.LoadScene(nextStage);
        }
        else
        {
            Debug.Log("다음 스테이지 없음 또는 이름 불일치");
        }
    }

}
