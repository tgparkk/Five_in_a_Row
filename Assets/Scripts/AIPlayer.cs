using UnityEngine;
using System.Collections.Generic;

public class AIPlayer
{
    private BoardManager boardManager;
    private int playerStoneType; // 1 for black, 2 for white
    private int aiStoneType;
    private System.Random random = new System.Random();
    private int boardSize;

    // Value maps for offensive and defensive play
    private int[,] valueMap;
    
    // Direction vectors for checking stone patterns
    private readonly Vector2Int[] directions = {
        new Vector2Int(1, 0),   // horizontal
        new Vector2Int(0, 1),   // vertical
        new Vector2Int(1, 1),   // diagonal down-right
        new Vector2Int(1, -1)   // diagonal up-right
    };

    public AIPlayer(BoardManager boardManager, int aiStoneType, int boardSize)
    {
        this.boardManager = boardManager;
        this.aiStoneType = aiStoneType;
        this.playerStoneType = aiStoneType == 1 ? 2 : 1;
        this.boardSize = boardSize;
        this.valueMap = new int[boardSize, boardSize];
    }
    
    public Vector2Int GetNextMove()
    {
        // Reset value map
        ResetValueMap();
        
        // Evaluate all empty positions
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (boardManager.GetBoardValue(pos) == 0) // empty position
                {
                    EvaluatePosition(pos);
                }
            }
        }
        
        // Find the position with the highest value
        Vector2Int bestMove = FindBestMove();
        return bestMove;
    }
    
    private void ResetValueMap()
    {
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                valueMap[x, y] = 0;
            }
        }
    }
    
    private void EvaluatePosition(Vector2Int pos)
    {
        // Evaluate AI's offensive value
        int offensiveValue = EvaluatePositionForPlayer(pos, aiStoneType);
        
        // Evaluate defensive value (blocking player)
        int defensiveValue = EvaluatePositionForPlayer(pos, playerStoneType);
        
        // Combine values with a slight preference for offense
        valueMap[pos.x, pos.y] = offensiveValue * 11 / 10 + defensiveValue;
        
        // Add some randomness to avoid predictability
        valueMap[pos.x, pos.y] += random.Next(5);
        
        // Favor center positions at the start of the game
        int centerDistance = Mathf.Abs(pos.x - boardSize / 2) + Mathf.Abs(pos.y - boardSize / 2);
        if (centerDistance <= 2)
        {
            valueMap[pos.x, pos.y] += 5;
        }
    }
    
    private int EvaluatePositionForPlayer(Vector2Int pos, int stoneType)
    {
        int totalValue = 0;
        
        foreach (Vector2Int dir in directions)
        {
            totalValue += EvaluateDirection(pos, dir, stoneType);
            totalValue += EvaluateDirection(pos, new Vector2Int(-dir.x, -dir.y), stoneType);
        }
        
        return totalValue;
    }
    
    private int EvaluateDirection(Vector2Int pos, Vector2Int dir, int stoneType)
    {
        int count = 0;
        bool blocked = false;
        Vector2Int currentPos = pos + dir;
        
        // Count consecutive stones in this direction
        while (boardManager.IsValid(currentPos) && count < 4)
        {
            if (boardManager.GetBoardValue(currentPos) == stoneType)
            {
                count++;
                currentPos += dir;
            }
            else if (boardManager.GetBoardValue(currentPos) != 0)
            {
                // Blocked by opponent's stone
                blocked = true;
                break;
            }
            else
            {
                // Empty space
                break;
            }
        }
        
        // Check if we're blocked at the end
        if (boardManager.IsValid(currentPos) && boardManager.GetBoardValue(currentPos) != 0 && boardManager.GetBoardValue(currentPos) != stoneType)
        {
            blocked = true;
        }
        
        // Value based on consecutive stones and whether it's blocked
        // These values can be tweaked for different AI behaviors
        if (count == 0) return blocked ? 0 : 1;
        if (count == 1) return blocked ? 2 : 5;
        if (count == 2) return blocked ? 10 : 50;
        if (count == 3) return blocked ? 50 : 500;
        if (count >= 4) return 10000; // Winning move
        
        return 0;
    }
    
    private Vector2Int FindBestMove()
    {
        int maxValue = -1;
        List<Vector2Int> bestMoves = new List<Vector2Int>();
        
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (boardManager.GetBoardValue(new Vector2Int(x, y)) == 0) // empty
                {
                    if (valueMap[x, y] > maxValue)
                    {
                        maxValue = valueMap[x, y];
                        bestMoves.Clear();
                        bestMoves.Add(new Vector2Int(x, y));
                    }
                    else if (valueMap[x, y] == maxValue)
                    {
                        bestMoves.Add(new Vector2Int(x, y));
                    }
                }
            }
        }
        
        // If we have multiple equally good moves, choose one randomly
        if (bestMoves.Count > 0)
        {
            return bestMoves[random.Next(bestMoves.Count)];
        }
        
        // Fallback: find any valid move
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (boardManager.GetBoardValue(new Vector2Int(x, y)) == 0)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        
        // Board is full (shouldn't reach here in normal gameplay)
        return new Vector2Int(0, 0);
    }
}