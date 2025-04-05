using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int boardSize = 15;
    public GameObject blackStonePrefab;
    public GameObject whiteStonePrefab;
    
    [HideInInspector]
    public bool gameOver = false;  // 게임 종료 여부
    
    private Vector2 origin;
    private float cellSize;

    private int[,] board;
    private bool isBlackTurn = true;
    
    public GameManager gameManager;

    void Awake()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        
        var sr = GetComponent<SpriteRenderer>();
        Vector2 boardWorldSize = sr.bounds.size;

        cellSize = boardWorldSize.x / (boardSize - 1);
        origin = (Vector2)transform.position - boardWorldSize * 0.5f;
    }
    
    void Start()
    {
        board = new int[boardSize, boardSize];
        gameOver = false;
    }

    void Update()
    {
        // 게임이 종료되었으면 클릭 무시
        if (gameOver) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPos = WorldToGrid(worldPos);

            if (IsValid(gridPos) && board[gridPos.x, gridPos.y] == 0)
            {
                PlaceStone(gridPos);
            }
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
        // 게임이 종료되었으면 돌을 놓지 않음
        if (gameOver) return;
        
        int stoneType = isBlackTurn ? 1 : 2;
        board[gridPos.x, gridPos.y] = stoneType;

        GameObject prefab = isBlackTurn ? blackStonePrefab : whiteStonePrefab;
        Vector3 spawnPos = GridToWorld(gridPos);
        Instantiate(prefab, spawnPos, Quaternion.identity);

        // GameManager에게 전달
        gameManager.OnStonePlaced(stoneType, gridPos);

        // 턴 전환
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