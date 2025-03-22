using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardSize = 15;                       // 오목판 크기
    public GameObject blackStonePrefab;              // 흑돌 프리팹
    public GameObject whiteStonePrefab;              // 백돌 프리팹
    public float cellSize = 1.0f;                    // 격자 크기 (1 유닛당 1칸)
    public Vector2 boardOrigin = new Vector2(-7f, -7f); // 왼쪽 아래 기준 위치

    private int[,] board;                            // 0 = 빈칸, 1 = 흑돌, 2 = 백돌
    private bool isBlackTurn = true;                 // 턴 관리

    void Start()
    {
        board = new int[boardSize, boardSize];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = WorldToGrid(worldPos);

            if (IsInBounds(gridPos) && board[gridPos.x, gridPos.y] == 0)
            {
                PlaceStone(gridPos);
            }
        }
    }

    Vector2Int WorldToGrid(Vector2 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - boardOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - boardOrigin.y) / cellSize);
        return new Vector2Int(x, y);
    }

    Vector3 GridToWorld(Vector2Int gridPos)
    {
        float x = boardOrigin.x + gridPos.x * cellSize + cellSize / 2;
        float y = boardOrigin.y + gridPos.y * cellSize + cellSize / 2;
        return new Vector3(x, y, 0);
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardSize && pos.y >= 0 && pos.y < boardSize;
    }

    void PlaceStone(Vector2Int gridPos)
    {
        int stoneType = isBlackTurn ? 1 : 2;
        board[gridPos.x, gridPos.y] = stoneType;

        GameObject prefab = isBlackTurn ? blackStonePrefab : whiteStonePrefab;
        Instantiate(prefab, GridToWorld(gridPos), Quaternion.identity);

        isBlackTurn = !isBlackTurn;
    }
    
    public bool IsValid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < boardSize && pos.y >= 0 && pos.y < boardSize;
    }

    public int GetBoardValue(Vector2Int pos)
    {
        return board[pos.x, pos.y];
    }

}