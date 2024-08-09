using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public int width = 5;
    public int height = 5;
    public int minDistance = 5;
    [Tooltip("경로 생성의 최대 시도 횟수")]
    public int maxGenerationAttempts = 30; // 인스펙터에서 설정 가능한 최대 시도 횟수

    public List<Vector2Int> GeneratePath()
    {
        List<Vector2Int> path = null;
        int attemptCount = 0;

        while (attemptCount < maxGenerationAttempts)
        {
            path = TryGeneratePath();
            if (path != null && path.Count > 1)
            {
                DebugLogger.Log("경로 생성 성공");
                return path;
            }
            attemptCount++;
            DebugLogger.Log($"경로 생성 시도 {attemptCount}/{maxGenerationAttempts} 실패");
        }

        DebugLogger.LogError("경로 생성에 실패했습니다. 최대 시도 횟수를 초과했습니다.");
        return null;
    }

    private List<Vector2Int> TryGeneratePath()
    {
        List<Vector2Int> path = new List<Vector2Int>();
        HashSet<Vector2Int> occupied = new HashSet<Vector2Int>();

        Vector2Int startPoint = GetRandomPoint();
        Vector2Int endPoint = GetDistantPoint(startPoint, minDistance);

        Vector2Int currentPos = startPoint;
        path.Add(currentPos);
        occupied.Add(currentPos);

        while (true)
        {
            List<Vector2Int> possibleMoves = GetPossibleMoves(currentPos, occupied);

            if (possibleMoves.Count == 0)
            {
                DebugLogger.LogError("경로 생성 실패: 이동할 곳이 없습니다.");
                return null;
            }

            Vector2Int move = possibleMoves[Random.Range(0, possibleMoves.Count)];
            currentPos += move;

            path.Add(currentPos);
            occupied.Add(currentPos);

            if (currentPos == endPoint)
            {
                return path;
            }
        }
    }

    private Vector2Int GetRandomPoint()
    {
        return new Vector2Int(Random.Range(0, width), Random.Range(0, height));
    }

    private Vector2Int GetDistantPoint(Vector2Int startPoint, int minDistance)
    {
        Vector2Int endPoint;
        do
        {
            endPoint = GetRandomPoint();
        } while (Vector2Int.Distance(startPoint, endPoint) < minDistance);

        return endPoint;
    }

    private List<Vector2Int> GetPossibleMoves(Vector2Int pos, HashSet<Vector2Int> occupied)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = pos + dir;
            if (IsValidMove(newPos) && !occupied.Contains(newPos))
            {
                moves.Add(dir);
            }
        }

        return moves;
    }

    private bool IsValidMove(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
}
