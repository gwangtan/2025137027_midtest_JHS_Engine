using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    [Header("��Ż ����")]
    public GameObject portalEffect;          // ��Ż �ð� ȿ�� (��Ȱ�� ���·� ����)
    public string[] stageNames = { "Stage1", "Stage2", "Stage3", "Stage4", "Stage5" };

    private bool isPortalActive = false;

    void Start()
    {
        if (portalEffect != null)
            portalEffect.SetActive(false);
    }

    void Update()
    {
        // ���� ���� ����ִ� ���Ͱ� �ִ��� üũ
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
        Debug.Log("��� ���Ͱ� óġ��! ��Ż�� ���Ƚ��ϴ�!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"��Ż Ʈ���� ������: {other.name}");

        if (!isPortalActive)
        {
            Debug.Log("��Ż�� ���� ��Ȱ�� �����Դϴ�.");
            return;
        }

        if (!other.CompareTag("Player"))
        {
            Debug.Log("Player �±� �ƴ�.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        int index = System.Array.IndexOf(stageNames, currentScene);

        Debug.Log($"�����: {currentScene} / �ε���: {index}");

        if (index != -1 && index < stageNames.Length - 1)
        {
            string nextStage = stageNames[index + 1];
            Debug.Log($"�� ��ȯ �õ�: {nextStage}");
            SceneManager.LoadScene(nextStage);
        }
        else
        {
            Debug.Log("���� �������� ���� �Ǵ� �̸� ����ġ");
        }
    }

}
