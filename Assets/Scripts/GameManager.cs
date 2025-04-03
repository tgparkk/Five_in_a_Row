using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public Button restartButton; // 선택: Inspector에서 재시작 버튼 연결
    
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;
    private bool isBlackTurn = true;
    
    public UnityEngine.UI.Text statusText; // 승리 메시지 등 표시할 UI Text
    
    // AI related fields
    [Header("AI Settings")]
    public bool useAI = true; // 기본값은 true, 시작 시 UIManager 설정으로 덮어씌워짐
    public bool aiPlaysBlack = false;
    private AIPlayer aiPlayer;
    private float aiMoveDelay = 0.5f; // Delay before AI makes a move
    private float aiMoveTimer = 0f;
    private bool aiThinking = false;
    
    private bool isGameOver = false;
    
    [Header("Victory UI")]
    public GameObject victoryPanel;     // 승리 패널
    public Text victoryText;            // 승리 메시지 텍스트

    
    void Start()
    {
        // UIManager의 모드 설정 적용 (사람 vs 사람 또는 사람 vs AI)
        useAI = UIManager.vsAIMode;
        
        // Initialize board manager if not set
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();
            
        // Initialize AI if AI mode is enabled
        if (useAI)
        {
            int aiStoneType = aiPlaysBlack ? 1 : 2;
            aiPlayer = new AIPlayer(boardManager, aiStoneType, boardManager.boardSize);
            
            // If AI plays first (black), make its first move
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
            // 이제 턴 변경
            isBlackTurn = !isBlackTurn;
        
            // 다음 턴 메시지 업데이트
            if (useAI)
            {
                bool isNextAI = (aiPlaysBlack && isBlackTurn) || (!aiPlaysBlack && !isBlackTurn);
                string nextPlayerText = isNextAI ? "컴퓨터" : "플레이어";
                string nextColorText = isBlackTurn ? "흑돌" : "백돌";
                statusText.text = $"{nextPlayerText}({nextColorText}) 차례입니다";
            
                // AI 턴이면 AI 생각 시작
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
        // AI's turn
        if (useAI && aiThinking && !isGameOver)
        {
            aiMoveTimer -= Time.deltaTime;
            if (aiMoveTimer <= 0)
            {
                aiThinking = false;
                MakeAIMove();
            }
        }
    
        // Player's turn
        else if (!aiThinking && !isGameOver)
        {
            // 이 부분이 수정된 부분입니다
            bool isPlayerTurn = !useAI || 
                                (aiPlaysBlack && !isBlackTurn) || 
                                (!aiPlaysBlack && isBlackTurn);
            
            if (isPlayerTurn && Input.GetMouseButtonDown(0))
            {
                // 화면 → 월드 좌표
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 월드 좌표 → 격자 좌표
                Vector2Int gridPos = boardManager.WorldToGrid(worldPos);

                // 범위 체크 & 빈 칸 확인
                if (boardManager.IsValid(gridPos) && boardManager.GetBoardValue(gridPos) == 0)
                {
                    // 격자 좌표 → 정확한 월드 좌표로 돌 생성
                    boardManager.PlaceStone(gridPos);
                    Debug.Log($"Placed stone at {gridPos.x}, {gridPos.y}");
                }
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
            Debug.Log($"AI placed stone at {aiMove.x}, {aiMove.y}");
        }
    }

    // 승리 조건 체크 함수
    bool CheckWin(Vector2Int pos, int player)
    {
        // 4가지 방향: 가로, 세로, 두 대각선
        Vector2Int[] directions = {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1)
        };

        foreach (Vector2Int dir in directions)
        {
            int count = 1;
            // 한쪽 방향
            count += CountStonesInDirection(pos, dir, player);
            // 반대 방향
            count += CountStonesInDirection(pos, -dir, player);

            if (count >= 5)
                return true;
        }
        return false;
    }

    // 특정 방향으로 연속된 돌의 수를 세는 함수
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
    
    
    // 게임 종료 이벤트 처리 함수
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

        // 게임 진행 중단
        aiThinking = false;
        
        // BoardManager에서 클릭 비활성화
        if (boardManager != null)
            boardManager.enabled = false;

        Debug.Log($"{winner} 승리! 게임 종료");
    }
    
    // 게임 재시작 함수
    public void RestartGame()
    {
        Debug.Log("Restart button clicked"); // 디버그 로그 추가
    
        // 현재 씬 이름을 직접 사용
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log("Current scene: " + currentSceneName);
    
        // 씬 다시 로드
        SceneManager.LoadScene(currentSceneName);
    }
    
    // 메인 메뉴로 돌아가는 함수
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("OpeningScene");
    }
}