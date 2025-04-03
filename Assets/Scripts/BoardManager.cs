using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardSize = 15;                       // 오목판 크기
    public GameObject blackStonePrefab;              // 흑돌 프리팹
    public GameObject whiteStonePrefab;              // 백돌 프리팹
    
    private Vector2 origin;
    private float cellSize;

    private int[,] board;                            // 0 = 빈칸, 1 = 흑돌, 2 = 백돌
    private bool isBlackTurn = true;                 // 턴 관리
    
    public GameManager gameManager; // GameManager 참조

    void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        
        var sr = GetComponent<SpriteRenderer>();
        Vector2 boardWorldSize = sr.bounds.size;

        // 15×15 오목판 → 14칸 간격
        cellSize = boardWorldSize.x / (boardSize - 1);

        // 스프라이트 좌하단 좌표
        origin = (Vector2)transform.position - boardWorldSize * 0.5f;
    }
    
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

            if (IsValid(gridPos) && board[gridPos.x, gridPos.y] == 0)
            {
                PlaceStone(gridPos);
            }
            
            Debug.Log($"GridPos = {gridPos.x},{gridPos.y}");
        }
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        Vector2 local = worldPos - origin;
        return new Vector2Int(
            Mathf.RoundToInt(local.x / cellSize),
            Mathf.RoundToInt(local.y / cellSize)
        );
    }


    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return (Vector3)(origin + new Vector2(gridPos.x, gridPos.y) * cellSize);
    }

    public void PlaceStone(Vector2Int gridPos)
    {
        int stoneType = isBlackTurn ? 1 : 2;
        board[gridPos.x, gridPos.y] = stoneType;

        GameObject prefab = isBlackTurn ? blackStonePrefab : whiteStonePrefab;
        Vector3 spawnPos = GridToWorld(gridPos);
        Instantiate(prefab, spawnPos, Quaternion.identity);

        // GameManager에게 전달 (현재 턴의 플레이어가 돌을 놓았다는 의미)
        gameManager.OnStonePlaced(stoneType, gridPos);

        // 턴 전환은 마지막에!
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