using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateMaze : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public int startPointCount = 2; // 출발 지점의 개수
    private int[,] maze;
    private List<Stack<Vector2Int>> pathStacks = new List<Stack<Vector2Int>>(); // 여러 스택 관리

    // 상하좌우 방향 벡터
    private Vector2Int[] directions = {
        new Vector2Int(0, 1),  // 위쪽
        new Vector2Int(1, 0),  // 오른쪽
        new Vector2Int(0, -1), // 아래쪽
        new Vector2Int(-1, 0)  // 왼쪽
    };

    void Start()
    {
        GenerateMaze();
        PrintMaze(); // 콘솔에 미로를 출력 (디버그용)
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // 여러 출발 지점을 무작위로 설정
        for (int i = 0; i < startPointCount; i++)
        {
            Vector2Int startPos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            maze[startPos.x, startPos.y] = 1; // 1은 통로, 0은 벽
            Stack<Vector2Int> newStack = new Stack<Vector2Int>();
            newStack.Push(startPos);
            pathStacks.Add(newStack);
        }

        while (pathStacks.Count > 0)
        {
            for (int i = pathStacks.Count - 1; i >= 0; i--)
            {
                if (pathStacks[i].Count > 0)
                {
                    Vector2Int currentPos = pathStacks[i].Peek();
                    List<Vector2Int> neighbors = GetValidNeighbors(currentPos);

                    if (neighbors.Count > 0)
                    {
                        // 랜덤한 방향으로 이동
                        Vector2Int chosenDirection = neighbors[Random.Range(0, neighbors.Count)];
                        Vector2Int nextPos = currentPos + chosenDirection;

                        // 이동한 위치와 현재 위치 사이를 통로로 만듦
                        maze[nextPos.x, nextPos.y] = 1;
                        maze[currentPos.x + chosenDirection.x / 2, currentPos.y + chosenDirection.y / 2] = 1;

                        pathStacks[i].Push(nextPos);
                    }
                    else
                    {
                        // 막힌 경우, 백트래킹
                        pathStacks[i].Pop();
                    }
                }

                // 스택이 비면 리스트에서 제거
                if (pathStacks[i].Count == 0)
                {
                    pathStacks.RemoveAt(i);
                }
            }
        }
    }

    List<Vector2Int> GetValidNeighbors(Vector2Int pos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighborPos = pos + direction * 2;

            if (neighborPos.x >= 0 && neighborPos.x < width && neighborPos.y >= 0 && neighborPos.y < height)
            {
                int midX = pos.x + direction.x;
                int midY = pos.y + direction.y;

                // 현재 위치와 목표 위치 사이의 중간 지점이 통로로 열려있는 경우도 허용 (T자 또는 +자 교차점)
                if (maze[neighborPos.x, neighborPos.y] == 0 || maze[midX, midY] == 1)
                {
                    neighbors.Add(direction);
                }
            }
        }

        return neighbors;
    }

    void PrintMaze()
    {
        for (int y = 0; y < height; y++)
        {
            string row = "";
            for (int x = 0; x < width; x++)
            {
                row += (maze[x, y] == 1) ? " " : "#";
            }
            Debug.Log(row);
        }
    }
}
