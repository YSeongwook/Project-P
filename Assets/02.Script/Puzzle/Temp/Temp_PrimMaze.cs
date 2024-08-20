using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

public class Temp_PrimMaze : MonoBehaviour
{
    [SerializeField] private int mapSize;
    [SerializeField] private GameObject tileReference;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Sprite straightTile, closedStraightTile, tTile, lTile, crossTile;

    [FoldoutGroup("Maze Setting")]
    [SerializeField] List<MazeSetting> mazeSettings;

    [Serializable]
    public class MazeSetting
    {
        public int startPointCount;
        public int EndPointsCount;
    }

    private Tile[,] tiles;
    private bool[,] visitated;

    private struct Tile
    {
        public Transform Transform;
        public Image Image;
        public TileType Type;
        public int TileShap;    // 0. ㅡ, 1. ㄱ, 2. T,  3. +, 4. closeTile
        public int TileRotate;  // 0. ↑,  1. →,  2. ↓,  3. ←
    }

    private List<Vector2Int> startPoints = new List<Vector2Int>();
    private Dictionary<Vector2Int, Vector2Int> endPoints = new Dictionary<Vector2Int, Vector2Int>();
    private Dictionary<Vector2Int, List<Vector2Int>> pathDic = new Dictionary<Vector2Int, List<Vector2Int>>();

    private void Awake()
    {
        tiles = new Tile[mapSize, mapSize];
        visitated = new bool[mapSize, mapSize];
    }

    private void Start()
    {
        //CreateTiles();
        CreateMazePath();
    }

    // 타일 생성
    private void CreateTiles()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                GameObject tile = Instantiate(tileReference, gridParent);
                tiles[i, j].Transform = tile.transform;
                tiles[i, j].Image = tile.transform.GetChild(0).GetComponent<Image>();
                tiles[i, j].Type = TileType.None;
                tiles[i, j].TileShap = UnityRandom.Range(0, 4);
                tiles[i, j].TileRotate = UnityRandom.Range(0, 4);
            }
        }
    }


    // 시작 및 끝 지점 랜덤 지정
    private void SetRandomPoint()
    {
        startPoints.Clear();
        endPoints.Clear();

        int index = 0;
        foreach (var setting in mazeSettings)
        {
            Vector2Int startPoint = new Vector2Int(UnityRandom.Range(0, mapSize), UnityRandom.Range(0, mapSize));
            startPoints.Add(startPoint);
            tiles[startPoint.x, startPoint.y].Type = TileType.StartPoint;

            SetRandomEndPoint(startPoint, setting.EndPointsCount);
        }
    }

    // 도착 지점 랜덤 설정
    private void SetRandomEndPoint(Vector2Int startPoint, int endPointCount)
    {
        for(int i = 0; i < endPointCount; i++)
        {
            Vector2Int endPoint = new Vector2Int(UnityRandom.Range(0, mapSize), UnityRandom.Range(0, mapSize));

            if(endPoint == startPoint)
            {
                i--;
                continue;
            }

            endPoints.Add(endPoint, startPoint);
            tiles[endPoint.x, endPoint.y].Type = TileType.EndPoint;
        }
    }

    // 경로 생성
    private void CreateMazePath()
    {
        pathDic.Clear();
        SetRandomPoint();

        foreach(var startPoint in startPoints)
        {
            PrimAlgorithm(startPoint);
        }        
    }

    //시작 지점의 다음 타일
    private Vector2Int SetStartPointNextTile(Vector2Int startPoint)
    {
        bool isLoop = true;
        Vector2Int nextTile = Vector2Int.zero;

        while (isLoop)
        {
            int RandomDir = UnityRandom.Range(0, 4);

            switch(RandomDir)
            {
                case 0:
                    // up
                    if (startPoint.y >= mapSize-1) continue;

                    nextTile += Vector2Int.up;
                    break;
                case 1:
                    // right
                    if (startPoint.x >= mapSize - 1) continue;

                    nextTile += Vector2Int.right;
                    break;
                case 2:
                    // down
                    if (startPoint.y <= 0) continue;

                    nextTile += Vector2Int.down;
                    break;
                case 3:
                    // left
                    if (startPoint.x <= 0) continue;

                    nextTile += Vector2Int.left;
                    break;
            }

            if(nextTile != Vector2Int.zero)
            {
                tiles[nextTile.x, nextTile.y].TileRotate = RandomDir;
                tiles[nextTile.x,nextTile.y].Type = TileType.Path;
                isLoop = false;
            }
        }

        
        return nextTile;
    }

    private void PrimAlgorithm(Vector2Int startPoint)
    {
        Dictionary<Vector2Int, int> frontier = new Dictionary<Vector2Int, int>();
        frontier.Add(startPoint, 0);
        visitated[startPoint.x, startPoint.y] = true;

        Vector2Int startNextTile = SetStartPointNextTile(startPoint);
        if (startNextTile != startPoint && !frontier.ContainsKey(startNextTile))
        {
            frontier.Add(startNextTile, 0);
            visitated[startNextTile.x, startNextTile.y] = true;
        }

        // 경로를 생성하고 각 도착 지점까지의 경로를 저장
        while (frontier.Count > 0)
        {
            // 가장 낮은 우선순위의 타일을 선택
            Vector2Int current = GetTileWithLowestPriority(frontier);
            frontier.Remove(current);

            // 현재 타일이 도착 지점이면 경로를 완성하고 다음 경로로 진행
            if (tiles[current.x, current.y].Type == TileType.EndPoint)
            {
                if (!pathDic.ContainsKey(current))
                {
                    pathDic[current] = new List<Vector2Int>();
                }
                pathDic[current].Add(current); // 도착 지점을 경로에 추가
                continue;
            }

            List<Vector2Int> neighbors = GetNeighbors(current);

            foreach (Vector2Int neighbor in neighbors)
            {
                if (!visitated[neighbor.x, neighbor.y])
                {
                    visitated[neighbor.x, neighbor.y] = true;
                    frontier[neighbor] = UnityRandom.Range(0, 100); // 무작위 가중치
                    tiles[neighbor.x, neighbor.y].Type = TileType.Path; // 경로로 설정

                    // 경로에 현재 타일을 추가
                    if (!pathDic.ContainsKey(neighbor))
                    {
                        pathDic[neighbor] = new List<Vector2Int>(pathDic[current]);
                    }
                    pathDic[neighbor].Add(current);
                }
            }
        }
    }


    private Vector2Int GetTileWithLowestPriority(Dictionary<Vector2Int, int> frontier)
    {
        Vector2Int lowestTile = default(Vector2Int);
        int lowestPriority = int.MaxValue;

        foreach (var tile in frontier)
        {
            if (tile.Value < lowestPriority)
            {
                lowestPriority = tile.Value;
                lowestTile = tile.Key;
            }
        }

        return lowestTile;
    }

    // 현재 위치의 이웃 타일 반환
    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (current.x > 0) neighbors.Add(new Vector2Int(current.x - 1, current.y)); // Left
        if (current.x < mapSize - 1) neighbors.Add(new Vector2Int(current.x + 1, current.y)); // Right
        if (current.y > 0) neighbors.Add(new Vector2Int(current.x, current.y - 1)); // Down
        if (current.y < mapSize - 1) neighbors.Add(new Vector2Int(current.x, current.y + 1)); // Up

        return neighbors;
    }
}
