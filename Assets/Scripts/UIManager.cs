using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // 실제 게임 씬 이름

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}