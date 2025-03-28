using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public Button restartButton; // 선택: Inspector에서 재시작 버튼 연결
    
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;
    private bool isBlackTurn = true;
    
    //public Text statusText;

    private bool isGameOver = false;
    public UnityEngine.UI.Text statusText; // 승리 메시지 등 표시할 UI Text
    
    void Start()
    {
        statusText.text = "흑돌 차례입니다";
    }

    public void OnStonePlaced(int player, Vector2Int pos)
    {
        if (isGameOver) return;

        if (CheckWin(pos, player))
        {
            isGameOver = true;
            string winner = player == 1 ? "흑돌" : "백돌";
            statusText.text = $"{winner} 승리!";
            GameOver(player);
        }
        else
        {
            string next = player == 1 ? "백돌" : "흑돌";
            statusText.text = $"{next} 차례입니다";
        }
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        string winner = winningPlayer == 1 ? "흑돌" : "백돌";

        // 1. 승리 메시지 표시
        if (statusText != null)
            statusText.text = winner + " 승리!";

        // 2. 게임 진행 중단 (예: 입력 막기)
        // 방법 1: GameManager 스스로 비활성화
        this.enabled = false;

        // 방법 2: BoardManager에서 클릭 비활성화
        if (boardManager != null)
            boardManager.enabled = false;

        // 3. 선택: 게임 재시작 버튼 활성화
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        Debug.Log($"{winner} 승리! 게임 종료");
    }
    
}