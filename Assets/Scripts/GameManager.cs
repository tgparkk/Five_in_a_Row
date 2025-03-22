using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public BoardManager boardManager;
    public Text statusText;

    private bool isGameOver = false;

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
        }
        else
        {
            string next = player == 1 ? "백돌" : "흑돌";
            statusText.text = $"{next} 차례입니다";
        }
    }

    bool CheckWin(Vector2Int pos, int player)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 0),   // 가로
            new Vector2Int(0, 1),   // 세로
            new Vector2Int(1, 1),   // 대각 ↘
            new Vector2Int(1, -1)   // 대각 ↗
        };

        foreach (Vector2Int dir in directions)
        {
            int count = 1;
            count += CountDirection(pos, dir, player);
            count += CountDirection(pos, -dir, player);

            if (count >= 5)
                return true;
        }

        return false;
    }

    int CountDirection(Vector2Int start, Vector2Int dir, int player)
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
}