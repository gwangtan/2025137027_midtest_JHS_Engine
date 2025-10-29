using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Inspector���� ��ư�� ���� ����
    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    // ���� ��ư�� (����)
    public void QuitGame()
    {
        Debug.Log("���� ����");
        Application.Quit();
    }
}
