using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityRandom = UnityEngine.Random;

public enum TileType
{
    None,
    Path,
    Wall,
    StartPoint,
    EndPoint
}

public enum TileShape
{
    
}

public class TempMazeCreate : MonoBehaviour
{
    [SerializeField] private int mapSize;
    [SerializeField] private GameObject tileReference;
    [SerializeField] private Transform gridParent;

    [FoldoutGroup("Maze Setting")]
    [SerializeField] List<MazeSetting> mazeSettings;

    public struct Tile
    {
        public Transform Transform;
        public Image Image;
        public int TileID;
        public TileType Type;
    }

    [Serializable]
    public class MazeSetting
    {
        public Vector2Int startPoint;
        public List<Vector2Int> EndPoints;
    }

    private Tile[,] tiles;
    private Dictionary<Vector2Int, List<Vector2Int>> pathList = new Dictionary<Vector2Int, List<Vector2Int>>();

    private void Start()
    {
        tiles = new Tile[mapSize, mapSize];

        CreateTiles();
        CreatePath();

        DrawMazeWithPaths();
        PrintStack();
    }

    private void CreateTiles()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                GameObject tile = Instantiate(tileReference, gridParent);
                tiles[i,j].Transform = tile.transform;
                tiles[i,j].Image = tile.GetComponentInChildren<Image>();
                tiles[i, j].TileID = i/ mapSize + j+1;
                tiles[i, j].Type = TileType.None;
            }
        }
    }

    // 시작 및 끝 지점 랜덤 지정
    private void SetRandomPoint()
    {
        foreach(var setting in mazeSettings)
        {
            setting.startPoint = new Vector2Int(UnityRandom.Range(0, mapSize), UnityRandom.Range(0, mapSize));
            tiles[setting.startPoint.y, setting.startPoint.x].Type = TileType.StartPoint;

            pathList.Add(setting.startPoint, new List<Vector2Int> { setting.startPoint });
            AddRandomPoints(setting);
        }
    }

    //끝 지점 랜덤 지정
    private void AddRandomPoints(MazeSetting setting)
    {
        int EndPointCount = setting.EndPoints.Count;
        List<Vector2Int> newEndPoint = new List<Vector2Int>();

        for(int i = 0;i < EndPointCount; i++)
        {
            Vector2Int endPoint = new Vector2Int(UnityRandom.Range(0, mapSize), UnityRandom.Range(0, mapSize));
            if(tiles[endPoint.y, endPoint.x].Type != TileType.None || Vector2Int.Distance(endPoint, setting.startPoint) < 2 )
            {
                i--;
                continue;
            }

            newEndPoint.Add(endPoint);
            tiles[endPoint.y, endPoint.x].Type = TileType.EndPoint;
        }

        setting.EndPoints = newEndPoint;
    }

    //경로 생성
    private void CreatePath()
    {
        pathList.Clear();
        SetRandomPoint();

        foreach(var maze in mazeSettings)
        {
            foreach(var endPoint in maze.EndPoints)
            {
                bool isEnd = false;
                while (true)
                {
                    Vector2Int CurrentTile = pathList[maze.startPoint].Last();
                    NextPathTileType(CurrentTile, maze.startPoint, out isEnd);

                    if (isEnd) break;
                }
            }
        }        
    }

    //다음 타일로 무작위 이동 
    private void NextPathTileType(Vector2Int currentTile, Vector2Int startPoint, out bool isEnd)
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(0,-1),
            new Vector2Int(0,1),
            new Vector2Int(-1,0),
            new Vector2Int(1,0)
        };

        bool tileMoveAble = false;
        directions = directions.OrderBy(d => UnityRandom.value).ToList();

        foreach(var  direction in directions)
        {
            Vector2Int NextTile = currentTile + direction;

            if(!IsValidTile(NextTile)) continue;

            tileMoveAble = true;

            if (tiles[NextTile.y, NextTile.x].Type == TileType.EndPoint) isEnd = true;
            else
            {
                tiles[NextTile.y, NextTile.x].Type = TileType.Path;
                isEnd = false;
            }

            pathList[startPoint].Add(NextTile);
            return;
        }

        if(!tileMoveAble)
        {
            pathList[startPoint].Remove(currentTile);
        }

        isEnd = false;
    }

    // 타일 조건 확인
    private bool IsValidTile(Vector2Int tile)
    {
        return tile.x >= 0 && tile.x < mapSize &&
               tile.y >= 0 && tile.y < mapSize &&
               (tiles[tile.y, tile.x].Type == TileType.None ||
                tiles[tile.y, tile.x].Type == TileType.EndPoint);
    }

    // 디버그 용
    private void DrawMazeWithPaths()
    {
        string mazeRepresentation = "";

        for (int y = 0; y < mapSize; y++)
        {
            string row = "";
            for (int x = 0; x < mapSize; x++)
            {
                if (tiles[y, x].Type == TileType.StartPoint)
                {
                    row += "S ";
                }
                else if (tiles[y, x].Type == TileType.EndPoint)
                {
                    row += "E ";
                }
                else if (tiles[y, x].Type == TileType.Path)
                {
                    row += GetPathNormal(x, y) + " ";
                }
                else
                {
                    row += "# ";
                }
            }
            mazeRepresentation += row + "\n";
        }

        Debug.Log(mazeRepresentation);
    }

    private string GetPathNormal(int x, int y)
    {
        Vector2Int currentTile = new Vector2Int(x, y);

        foreach (var path in pathList.Values)
        {
            int pathIndex = path.IndexOf(currentTile);

            if (pathIndex == -1 || pathIndex == 0 || pathIndex == path.Count - 1)
            {
                continue; // 시작점과 끝점은 무시
            }

            Vector2Int preTile = path[pathIndex - 1];
            Vector2Int nextTile = path[pathIndex + 1];

            Vector2Int preDirection = currentTile - preTile;
            Vector2Int nextDirection = nextTile - currentTile;

            // 각 타일의 연결 방향을 확인하여 스프라이트 결정
            if ((preDirection == Vector2Int.up && nextDirection == Vector2Int.down) ||
                (preDirection == Vector2Int.down && nextDirection == Vector2Int.up))
            {
                return "│"; // 수직 연결
            }
            else if ((preDirection == Vector2Int.left && nextDirection == Vector2Int.right) ||
                     (preDirection == Vector2Int.right && nextDirection == Vector2Int.left))
            {
                return "─"; // 수평 연결
            }
            else if ((preDirection == Vector2Int.up && nextDirection == Vector2Int.right) ||
                     (preDirection == Vector2Int.right && nextDirection == Vector2Int.up))
            {
                return "└"; // 왼쪽 아래 모서리
            }
            else if ((preDirection == Vector2Int.up && nextDirection == Vector2Int.left) ||
                     (preDirection == Vector2Int.left && nextDirection == Vector2Int.up))
            {
                return "┘"; // 오른쪽 아래 모서리
            }
            else if ((preDirection == Vector2Int.down && nextDirection == Vector2Int.right) ||
                     (preDirection == Vector2Int.right && nextDirection == Vector2Int.down))
            {
                return "┌"; // 왼쪽 위 모서리
            }
            else if ((preDirection == Vector2Int.down && nextDirection == Vector2Int.left) ||
                     (preDirection == Vector2Int.left && nextDirection == Vector2Int.down))
            {
                return "┐"; // 오른쪽 위 모서리
            }
        }

        return "."; // 기본값
    }

    private void PrintStack()
    {
        string aa = "";
        foreach ( var Dic in pathList )
        {
            foreach(var List in Dic.Value)
            {
                aa += $"{List} ";
            }
            Debug.Log(aa);
        }
        
    }
}
