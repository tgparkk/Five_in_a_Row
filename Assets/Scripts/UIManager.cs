using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public string gameSceneName = "GameScene"; // 실제 게임 씬 이름
    
    // 게임 모드를 저장하기 위한 정적 변수 (씬 전환 후에도 유지됨)
    public static bool vsAIMode = false;

    // 사람과 대결 모드 시작
    public void StartVsPlayerMode()
    {
        Debug.Log("Starting Player vs Player mode");
        vsAIMode = false;
        SceneManager.LoadScene(gameSceneName);
    }
    
    // 컴퓨터와 대결 모드 시작
    public void StartVsAIMode()
    {
        Debug.Log("Starting Player vs AI mode");
        vsAIMode = true;
        SceneManager.LoadScene(gameSceneName);
    }
}