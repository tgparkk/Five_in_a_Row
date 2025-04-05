using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public string gameSceneName = "GameScene";
    
    // 게임 모드를 저장하기 위한 정적 변수
    public static bool vsAIMode = false;
    
    // UI 버튼 참조
    public Button vsPlayerButton;
    public Button vsAIButton;

    void Start()
    {
        Debug.Log("UIManager Start");
        
        // 버튼에 이벤트 리스너 등록
        if (vsPlayerButton != null)
        {
            vsPlayerButton.onClick.RemoveAllListeners();
            vsPlayerButton.onClick.AddListener(StartVsPlayerMode);
            Debug.Log("VS Player 버튼 리스너 등록 완료");
        }
        
        if (vsAIButton != null)
        {
            vsAIButton.onClick.RemoveAllListeners();
            vsAIButton.onClick.AddListener(StartVsAIMode);
            Debug.Log("VS AI 버튼 리스너 등록 완료");
        }
    }

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