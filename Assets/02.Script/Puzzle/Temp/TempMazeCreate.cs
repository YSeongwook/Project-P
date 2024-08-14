using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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

public class TempMazeCreate : MonoBehaviour
{
    [SerializeField] private int mapSize;
    [SerializeField] private GameObject tileReference;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Sprite straightTile, closedStraightTile, tTile, lTile, crossTile;

    [FoldoutGroup("Maze Setting")]
    [SerializeField] List<MazeSetting> mazeSettings;

    public struct Tile
    {
        public Transform Transform;
        public Image Image;
        public int TileID;
        public TileType Type;
        public int TileShap;    // 0. ㅡ, 1. ㄱ, 2. T,  3. +, 4. closeTile
        public int TileRotate;  // 0. ↑,  1. →,  2. ↓,  3. ←
    }

    [Serializable]
    public class MazeSetting
    {
        public Vector2Int startPoint;
        public List<Vector2Int> EndPoints;
    }

    private Tile[,] tiles;
    //public Dictionary<int, List<Vector2Int>> pathList { get; private set; } = new Dictionary<int, List<Vector2Int>>();
    public Dictionary<int, Dictionary<Vector2Int, List<Vector2Int>>> pathLists { get; private set; } = new Dictionary<int, Dictionary<Vector2Int, List<Vector2Int>>>();
    private const float RotationAngle = 90f;

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
                tiles[i,j].Image = tile.transform.GetChild(0).GetComponent<Image>();
                tiles[i, j].TileID = i/ mapSize + j+1;
                tiles[i, j].Type = TileType.None;
                tiles[i, j].TileShap = UnityRandom.Range(0, 4);
                tiles[i, j].TileRotate = UnityRandom.Range(0, 4);
            }
        }
    }

    // 시작 및 끝 지점 랜덤 지정
    private void SetRandomPoint()
    {
        int index = 0;
        foreach(var setting in mazeSettings)
        {
            setting.startPoint = new Vector2Int(UnityRandom.Range(0, mapSize), UnityRandom.Range(0, mapSize));
            tiles[setting.startPoint.y, setting.startPoint.x].Type = TileType.StartPoint;

            //pathList.Add(index, new List<Vector2Int> { setting.startPoint });
            Dictionary<Vector2Int, List<Vector2Int>> tempDic = new Dictionary<Vector2Int, List<Vector2Int>>();
            tempDic.Add(setting.startPoint, new List<Vector2Int> { setting.startPoint });
            pathLists.Add(index, tempDic);
            AddRandomPoints(setting);

            index++;
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
        pathLists.Clear();
        SetRandomPoint();

        int index = 0;
        foreach(var maze in mazeSettings)
        {
            var pathList = pathLists[index];

            int EndPointCount = 0;
            foreach (var endPoint in maze.EndPoints)
            {
                EndPointCount++;

                if(EndPointCount <= 1)
                {
                    // 첫번째 
                    bool isEnd = false;
                    while (true)
                    {
                        Vector2Int CurrentTile = pathList[maze.startPoint].Last();
                        NextPathTileType(CurrentTile, maze.startPoint, index, out isEnd);

                        if (isEnd) break;
                    }
                }
                else
                {
                    // 두번째 이상
                    bool isEnd = false;

                    var list = pathList[maze.startPoint];
                    var StartPoint = list[UnityRandom.Range(1, list.Count)];
                    Vector2Int CurrentTile = Vector2Int.zero;

                    while (true)
                    {
                        if (!pathList.ContainsKey(StartPoint))
                        {
                            pathLists[index].Add(StartPoint, new List<Vector2Int> { StartPoint } );
                        }

                        CurrentTile = pathList[StartPoint].Last();

                        NextPathTileType(CurrentTile, StartPoint, index, out isEnd);

                        if (isEnd) break;
                    }
                }                
            }

            index++;
        }        
    }

    //다음 타일로 무작위 이동 
    private void NextPathTileType(Vector2Int currentTile, Vector2Int startPoint, int index, out bool isEnd)
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

            if(!IsValidTile(NextTile, startPoint, index)) continue;

            tileMoveAble = true;

            if (tiles[NextTile.y, NextTile.x].Type == TileType.EndPoint) isEnd = true;
            else
            {
                tiles[NextTile.y, NextTile.x].Type = TileType.Path;
                isEnd = false;
            }

            //pathLists[index].Add(NextTile);
            pathLists[index][startPoint].Add(NextTile);
            return;
        }

        if(!tileMoveAble)
        {
            //pathLists[index].Remove(currentTile);
            pathLists[index][startPoint].Remove(currentTile);
        }

        isEnd = false;
    }

    // 타일 조건 확인
    private bool IsValidTile(Vector2Int tile, Vector2Int startPoint, int index)
    {
        return tile.x >= 0 && tile.x < mapSize &&
               tile.y >= 0 && tile.y < mapSize &&
               (!pathLists[index][startPoint].Contains(tile) ||
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
                    tiles[y, x].TileShap = 4;
                    SetEndPointRotateValue(y, x);
                    row += "S ";
                }
                else if (tiles[y, x].Type == TileType.EndPoint)
                {
                    tiles[y, x].TileShap = 4;
                    SetEndPointRotateValue(y, x);
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

                ChangedTileSprite(y, x);
                RotationTileSprite(y, x);
            }
            mazeRepresentation += row + "\n";
        }

        
        Debug.Log(mazeRepresentation);
    }

    // 마지막 지점 스프라이트 회전
    private void SetEndPointRotateValue(int y, int x)
    {
        foreach(var Dic in pathLists)
        {
            int key = Dic.Key;
            Dictionary<Vector2Int, List<Vector2Int>> innerDictionary = Dic.Value;

            Vector2Int rotationValue = Vector2Int.zero;

            foreach(var List in innerDictionary)
            {
                if (List.Value.Contains(new Vector2Int(x, y)))
                {
                    switch (tiles[y, x].Type)
                    {
                        case TileType.StartPoint:
                            rotationValue = List.Value[1] - List.Value[0];
                            break;
                        case TileType.EndPoint:
                            int lastIndex = List.Value.Count - 1;
                            rotationValue = List.Value[lastIndex - 1] - List.Value[lastIndex];
                            break;
                        default:
                            DebugLogger.Log("잘못된 사용입니다.");
                            return;
                    }

                    if (rotationValue == Vector2Int.up)
                    {
                        tiles[y, x].TileRotate = 0;
                    }
                    else if (rotationValue == Vector2Int.right)
                    {
                        tiles[y, x].TileRotate = 3;
                    }
                    else if (rotationValue == Vector2Int.down)
                    {
                        tiles[y, x].TileRotate = 2;
                    }
                    else if (rotationValue == Vector2Int.left)
                    {
                        tiles[y, x].TileRotate = 1;
                    }
                }
            }
            //if (Dic.Contains(new Vector2Int(x,y)))
            //{
            //    switch(tiles[y, x].Type)
            //    {
            //        case TileType.StartPoint:
            //            rotationValue = Dic.Value[1] - Dic.Value[0];                        
            //            break; 
            //        case TileType.EndPoint:
            //            int lastIndex = Dic.Value.Count - 1;
            //            rotationValue = Dic.Value[lastIndex-1] - Dic.Value[lastIndex];
            //            break;
            //        default:
            //            DebugLogger.Log("잘못된 사용입니다.");
            //            return;
            //    }

            //    if (rotationValue == Vector2Int.up)
            //    {
            //        tiles[y, x].TileRotate = 0;
            //    }
            //    else if (rotationValue == Vector2Int.right)
            //    {
            //        tiles[y, x].TileRotate = 3;
            //    }
            //    else if (rotationValue == Vector2Int.down)
            //    {
            //        tiles[y, x].TileRotate = 2;
            //    }
            //    else if (rotationValue == Vector2Int.left)
            //    {
            //        tiles[y, x].TileRotate = 1;
            //    }
            //}
        }
    }

    // 스프라이트 이미지 변경
    private void ChangedTileSprite(int y, int x)
    {
        switch (tiles[y, x].TileShap)
        {
            case 0:
                tiles[y, x].Image.sprite = straightTile;
                break;
            case 1:
                tiles[y, x].Image.sprite = lTile;
                break;
            case 2:
                tiles[y, x].Image.sprite = tTile;
                break;
            case 3:
                tiles[y, x].Image.sprite = crossTile;
                break;
            case 4:
                tiles[y, x].Image.sprite = closedStraightTile;
                break;
        }
    }

    // 스프라이트 이미지 회전
    private void RotationTileSprite(int y, int x)
    {
        switch (tiles[y, x].TileRotate)
        {
            case 0:
                tiles[y, x].Image.transform.rotation = Quaternion.identity;
                break;
            case 1:
                tiles[y, x].Image.transform.rotation = Quaternion.Euler(0, 0, -RotationAngle);
                break;
            case 2:
                tiles[y, x].Image.transform.rotation = Quaternion.Euler(0, 0, -RotationAngle*2);
                break;
            case 3:
                tiles[y, x].Image.transform.rotation = Quaternion.Euler(0, 0, -RotationAngle*3);
                break;
        }
    }

    private string GetPathNormal(int x, int y)
    {
        Vector2Int currentTile = new Vector2Int(x, y);

        foreach (var path in pathLists.Values)
        {

            foreach (var list in path.Values)
            {
                int pathIndex = list.IndexOf(currentTile);

                if (pathIndex == -1 || pathIndex == 0 || pathIndex == path.Count - 1)
                {
                    continue; // 시작점과 끝점은 무시
                }

                Vector2Int preTile = list[pathIndex - 1];
                Vector2Int nextTile = list[pathIndex + 1];

                Vector2Int preDirection = preTile - currentTile;
                Vector2Int nextDirection = nextTile - currentTile;

                // 각 타일의 연결 방향을 확인하여 스프라이트 결정
                if ((preDirection == Vector2Int.up && nextDirection == Vector2Int.down) ||
                    (preDirection == Vector2Int.down && nextDirection == Vector2Int.up))
                {
                    tiles[y, x].TileShap = 0;
                    tiles[y, x].TileRotate = 0;

                    tiles[y, x].Image.sprite = straightTile;
                    return "│"; // 수직 연결
                }
                else if ((preDirection == Vector2Int.left && nextDirection == Vector2Int.right) ||
                         (preDirection == Vector2Int.right && nextDirection == Vector2Int.left))
                {
                    tiles[y, x].TileShap = 0;
                    tiles[y, x].TileRotate = 1;

                    tiles[y, x].Image.sprite = straightTile;
                    return "─"; // 수평 연결
                }
                else if ((preDirection == Vector2Int.down && nextDirection == Vector2Int.left) ||
                         (preDirection == Vector2Int.left && nextDirection == Vector2Int.down))
                {
                    tiles[y, x].TileShap = 1;
                    tiles[y, x].TileRotate = 0;
                    return "┘"; // 오른쪽 아래 모서리
                }
                else if ((preDirection == Vector2Int.down && nextDirection == Vector2Int.right) ||
                         (preDirection == Vector2Int.right && nextDirection == Vector2Int.down))
                {
                    tiles[y, x].TileShap = 1;
                    tiles[y, x].TileRotate = 1;
                    return "└"; // 왼쪽 아래 모서리
                }
                else if ((preDirection == Vector2Int.up && nextDirection == Vector2Int.right) ||
                         (preDirection == Vector2Int.right && nextDirection == Vector2Int.up))
                {
                    tiles[y, x].TileShap = 1;
                    tiles[y, x].TileRotate = 2;
                    return "┌"; // 왼쪽 위 모서리
                }
                else if ((preDirection == Vector2Int.up && nextDirection == Vector2Int.left) ||
                         (preDirection == Vector2Int.left && nextDirection == Vector2Int.up))
                {
                    tiles[y, x].TileShap = 1;
                    tiles[y, x].TileRotate = 3;
                    return "┐"; // 오른쪽 위 모서리
                }
            }
           
            
        }

        return "."; // 기본값
    }

    private void PrintStack()
    {
       
        foreach ( var Dic in pathLists )
        {

            foreach (var Dic2 in Dic.Value)
            {
                string aa = "";
                foreach (var list in Dic2.Value)
                {
                    aa += $"{list} ";
                }
                Debug.Log(aa);
            }

        }
        
    }
}
