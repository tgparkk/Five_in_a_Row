using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public BoardManager boardManager;
    public Button restartButton;
    
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;
    private bool isBlackTurn = true;
    
    public UnityEngine.UI.Text statusText;
    
    [Header("AI Settings")]
    public bool useAI = true;
    public bool aiPlaysBlack = false;
    private AIPlayer aiPlayer;
    private float aiMoveDelay = 0.5f;
    private float aiMoveTimer = 0f;
    private bool aiThinking = false;
    
    private bool isGameOver = false;
    
    [Header("Victory UI")]
    public GameObject victoryPanel;
    public Text victoryText;

    private void Awake()
    {
        // 싱글톤 패턴 구현 (씬 전환 후에도 참조 가능)
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // UIManager의 모드 설정 적용
        useAI = UIManager.vsAIMode;
        
        // 게임 시작 시 초기 상태 설정
        isGameOver = false;
        
        // Initialize board manager if not set
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();
            
        // Initialize AI if AI mode is enabled
        if (useAI)
        {
            int aiStoneType = aiPlaysBlack ? 1 : 2;
            aiPlayer = new AIPlayer(boardManager, aiStoneType, boardManager.boardSize);
            
            if (aiPlaysBlack && isBlackTurn)
            {
                aiThinking = true;
                aiMoveTimer = aiMoveDelay;
            }
            
            statusText.text = aiPlaysBlack ? "컴퓨터(흑돌) 차례입니다" : "플레이어(흑돌) 차례입니다";
        }
        else
        {
            statusText.text = "흑돌 차례입니다";
        }
        
        // 승리 패널 비활성화
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        
        // 재시작 버튼에 리스너 추가
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    public void OnStonePlaced(int player, Vector2Int pos)
    {
        if (isGameOver) return;

        if (CheckWin(pos, player))
        {
            isGameOver = true;
            string winner;
        
            if (useAI)
            {
                bool isAIWin = (player == 1 && aiPlaysBlack) || (player == 2 && !aiPlaysBlack);
                winner = isAIWin ? "컴퓨터" : "플레이어";
            }
            else
            {
                winner = player == 1 ? "흑돌" : "백돌";
            }
        
            statusText.text = $"{winner} 승리!";
            GameOver(player);
        }
        else
        {
            isBlackTurn = !isBlackTurn;
        
            if (useAI)
            {
                bool isNextAI = (aiPlaysBlack && isBlackTurn) || (!aiPlaysBlack && !isBlackTurn);
                string nextPlayerText = isNextAI ? "컴퓨터" : "플레이어";
                string nextColorText = isBlackTurn ? "흑돌" : "백돌";
                statusText.text = $"{nextPlayerText}({nextColorText}) 차례입니다";
            
                if (isNextAI)
                {
                    aiThinking = true;
                    aiMoveTimer = aiMoveDelay;
                }
            }
            else
            {
                string next = isBlackTurn ? "흑돌" : "백돌";
                statusText.text = $"{next} 차례입니다";
            }
        }
    }
    
    void Update()
    {
        // 게임이 종료되었으면 AI 행동 중지
        if (isGameOver) return;
        
        // AI's turn
        if (useAI && aiThinking)
        {
            aiMoveTimer -= Time.deltaTime;
            if (aiMoveTimer <= 0)
            {
                aiThinking = false;
                MakeAIMove();
            }
        }
    }
    
    private void MakeAIMove()
    {
        if (isGameOver) return;
        
        Vector2Int aiMove = aiPlayer.GetNextMove();
        if (boardManager.IsValid(aiMove) && boardManager.GetBoardValue(aiMove) == 0)
        {
            boardManager.PlaceStone(aiMove);
        }
    }

    // 승리 조건 체크 함수
    bool CheckWin(Vector2Int pos, int player)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            int count = 1;
            count += CountStonesInDirection(pos, dir, player);
            count += CountStonesInDirection(pos, -dir, player);

            if (count >= 5)
                return true;
        }
        return false;
    }

    int CountStonesInDirection(Vector2Int start, Vector2Int dir, int player)
    {
        int count = 0;
        Vector2Int pos = start + dir;
        while (boardManager.IsValid(pos) && boardManager.GetBoardValue(pos) == player)
        {
            count++;
            pos += dir;
        }
        return count;
    }
    
    void GameOver(int winningPlayer)
    {
        string winner;
        
        if (useAI)
        {
            bool isAIWin = (winningPlayer == 1 && aiPlaysBlack) || (winningPlayer == 2 && !aiPlaysBlack);
            winner = isAIWin ? "컴퓨터" : "플레이어";
        }
        else
        {
            winner = winningPlayer == 1 ? "흑돌" : "백돌";
        }

        // 게임 종료 상태 설정
        isGameOver = true;
        
        // 승리 메시지 표시
        if (statusText != null)
            statusText.text = winner + " 승리!";
            
        // 승리 패널 활성화
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            
            // 승리 메시지 설정
            if (victoryText != null)
                victoryText.text = winner + " 승리!";
        }

        // BoardManager도 게임 종료 상태로 설정
        if (boardManager != null)
            boardManager.gameOver = true;

        Debug.Log($"{winner} 승리! 게임 종료");
    }
    
    // 게임 재시작 함수
    public void RestartGame()
    {
        Debug.Log("Restart button clicked - GameManager");
        
        // 현재 씬 이름을 직접 가져와서 다시 로드
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Reloading scene: " + currentSceneName);
        
        // 씬 다시 로드
        SceneManager.LoadScene(currentSceneName);
    }
    
    // 메인 메뉴로 돌아가는 함수
    public void ReturnToMainMenu()
    {
        Debug.Log("Return to main menu clicked");
        SceneManager.LoadScene("OpeningScene");
    }
}