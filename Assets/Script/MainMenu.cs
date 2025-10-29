using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Inspector에서 버튼에 연결 가능
    public void LoadStage1()
    {
        SceneManager.LoadScene("Stage1");
    }

    // 종료 버튼용 (선택)
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}
