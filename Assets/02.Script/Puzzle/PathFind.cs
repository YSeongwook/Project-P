using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class PathFind
{
    private Dictionary<Vector2, TileNode> _tileGrid = new Dictionary<Vector2, TileNode>();
    private Dictionary<int, Dictionary<Vector2, TileNode>> _pathTileList = new Dictionary<int, Dictionary<Vector2, TileNode>>();
    private Dictionary<Vector2, TileNode> _startPoint = new Dictionary<Vector2, TileNode>();
    private Dictionary<Vector2, TileNode> _endPoint = new Dictionary<Vector2, TileNode>();
    private Dictionary<Vector2, Vector2> _warpPoints = new Dictionary<Vector2, Vector2>();
    private List<TileNode> _linkTiles = new List<TileNode>();

    private float _cellSize;
    private bool _isMiniGameStage;
    
    // 각 RoadShape와 RotateValue에 따른 방향 설정 사전
    private static readonly Dictionary<RoadShape, Vector2Int[][]> ShapeDirectionMappings = new Dictionary<RoadShape, Vector2Int[][]>
    {
        { RoadShape.Straight, new Vector2Int[][] {
            new Vector2Int[] { Vector2Int.up, Vector2Int.down },  // 0도, 180도
            new Vector2Int[] { Vector2Int.left, Vector2Int.right } // 90도, 270도
        }},
        { RoadShape.L, new Vector2Int[][] {
            new Vector2Int[] { Vector2Int.up, Vector2Int.left },   // 0도
            new Vector2Int[] { Vector2Int.right, Vector2Int.up },  // 90도
            new Vector2Int[] { Vector2Int.right, Vector2Int.down }, // 180도
            new Vector2Int[] { Vector2Int.down, Vector2Int.left }   // 270도
        }},
        { RoadShape.T, new Vector2Int[][] {
            new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.right }, // 0도
            new Vector2Int[] { Vector2Int.right, Vector2Int.left, Vector2Int.down }, // 90도
            new Vector2Int[] { Vector2Int.down, Vector2Int.left, Vector2Int.up }, // 180도
            new Vector2Int[] { Vector2Int.left, Vector2Int.up, Vector2Int.right } // 270도
        }},
        { RoadShape.Cross, new Vector2Int[][] {
            new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right } // 모든 각도 동일
        }}
    };

    public void SetTileGridEvent(bool isRegister)
    {
        if (isRegister) AddEvents();
        else RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<StageEvent>.StartListening(StageEvent.ResetTileGrid, ResetTileGrid);
        EventManager<StageEvent>.StartListening<Vector2, TileNode>(StageEvent.SetPathTileGridAdd, AddTileGrid);
        EventManager<StageEvent>.StartListening<float>(StageEvent.SetPathEndPoint, SetStartAndEndPoint);
        EventManager<StageEvent>.StartListening(StageEvent.SortPathTileGrid, CheckTilePath);
        EventManager<PuzzleEvent>.StartListening<TileNode, bool>(PuzzleEvent.Rotation, LinkTileRotate);
        EventManager<StageEvent>.StartListening<int>(StageEvent.SetRandomRotateLinkTile, SetLinkTileRandomRotate);
        EventManager<UIEvents>.StartListening<bool>(UIEvents.ActiveMiniGameUI, SetMiniGameStage);
    }

    private void RemoveEvents()
    {
        EventManager<StageEvent>.StopListening(StageEvent.ResetTileGrid, ResetTileGrid);
        EventManager<StageEvent>.StopListening<Vector2, TileNode>(StageEvent.SetPathTileGridAdd, AddTileGrid);
        EventManager<StageEvent>.StopListening<float>(StageEvent.SetPathEndPoint, SetStartAndEndPoint);
        EventManager<StageEvent>.StopListening(StageEvent.SortPathTileGrid, CheckTilePath);
        EventManager<PuzzleEvent>.StopListening<TileNode, bool>(PuzzleEvent.Rotation, LinkTileRotate);
        EventManager<StageEvent>.StopListening<int>(StageEvent.SetRandomRotateLinkTile, SetLinkTileRandomRotate);
        EventManager<UIEvents>.StopListening<bool>(UIEvents.ActiveMiniGameUI, SetMiniGameStage);
    }

    private void ResetTileGrid()
    {
        _startPoint.Clear();
        _endPoint.Clear();
        _tileGrid.Clear();
        _pathTileList.Clear();
        _warpPoints.Clear();
        _linkTiles.Clear();
    }

    private void AddTileGrid(Vector2 tilePosition, TileNode tileNode)
    {
        if (_tileGrid.ContainsKey(tilePosition)) _tileGrid[tilePosition] = tileNode;
        else _tileGrid.Add(tilePosition, tileNode);
    }

    private void SetStartAndEndPoint(float cellSize)
    {
        _cellSize = cellSize;

        List<Vector2> unpairedWarps = new List<Vector2>();

        foreach (var tile in _tileGrid)
        {
            if (tile.Value.GetTileInfo.RoadShape == RoadShape.Start)
            {
                _startPoint.Add(tile.Key, tile.Value);
                continue;
            }

            if (tile.Value.GetTileInfo.RoadShape == RoadShape.End)
            {
                _endPoint.Add(tile.Key, tile.Value);
                continue;
            }

            if (tile.Value.GetTileInfo.GimmickShape == GimmickShape.Warp)
            {
                unpairedWarps.Add(tile.Key);
                continue;
            }

            if(tile.Value.GetTileInfo.GimmickShape == GimmickShape.Link)
            {
                _linkTiles.Add(tile.Value);
            }
        }

        for (int i = 0; i < unpairedWarps.Count; i += 2)
        {
            if (i + 1 < unpairedWarps.Count)
            {
                _warpPoints.Add(unpairedWarps[i], unpairedWarps[i + 1]);
                _warpPoints.Add(unpairedWarps[i + 1], unpairedWarps[i]); // 양방향 워프
            }
        }
    }

    private void CheckTilePath()
    {
        // 모바일 환경에서만 진동 발생
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // 길고 약한 진동 발생
            EventManager<VibrateEvents>.TriggerEvent(VibrateEvents.LongWeak);
        }
        
        if (TilePathFind())
        {
            EventManager<StageEvent>.TriggerEvent(StageEvent.GameEnd, true);

            Sequence animationSequence = DOTween.Sequence();

            foreach (var path in _pathTileList.Values)
            {

                foreach (var item in path.Reverse())
                {
                    // 애니메이션을 시퀀스에 추가
                    animationSequence.AppendCallback(() => item.Value.StartPathAnimation());
                    // 각 타일의 애니메이션 시간만큼 지연을 추가 (예: 1초)
                    animationSequence.AppendInterval(0.2f);
                }
            }

            animationSequence.OnComplete(() =>
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    if (_isMiniGameStage)
                    {
                        // 로비와 Stage UI Disable
                        EventManager<StageEvent>.TriggerEvent(StageEvent.SetMiniGame, false);
                        //미니 게임 화면 등장
                        EventManager<MiniGame>.TriggerEvent(MiniGame.StartMiniGame);
                    }
                    else
                    {
                        // 하나 이상의 startPoint가 모든 endPoint와 연결된 경우
                        EventManager<StageEvent>.TriggerEvent(StageEvent.MissionSuccess);
                    }
                });

            });

            animationSequence.Restart();
        }
        else
        {
            // 모든 startPoint에서 하나 이상의 endPoint가 연결되지 않은 경우
            EventManager<StageEvent>.TriggerEvent(StageEvent.CheckMissionFail);
        }
    }

    public bool TilePathFind()
    {
        bool missionSuccess = false;
        var _connectedPoints = new List<TileNode>();

        var successfulPaths = new Dictionary<int, Dictionary<Vector2, TileNode>>();

        foreach (var startPoint in _startPoint)
        {
            foreach (var endPoint in _endPoint)
            {
                var path = FindPath(startPoint.Key, endPoint.Key);
                if (path != null)
                {
                    // 경로가 존재할 경우, successfulPaths에 저장
                    int pathIndex = successfulPaths.Count + 1;
                    successfulPaths.Add(pathIndex, path);

                    if(!_connectedPoints.Contains(startPoint.Value))
                        _connectedPoints.Add(startPoint.Value);

                    if (!_connectedPoints.Contains(endPoint.Value))
                        _connectedPoints.Add(endPoint.Value);                    
                }
            }

            // 완성된 경로가 시작 포인트보다 많으면 종료.
            if (_connectedPoints.Count >=( _startPoint.Count + _endPoint.Count))
            {
                // 하나의 startPoint에서 모든 endPoint들이 연결된 경우
                _pathTileList = successfulPaths;
                missionSuccess = true;
                break;
            }
        }

        return missionSuccess;
    }

    private Dictionary<Vector2, TileNode> FindPath(Vector2 start, Vector2 end)
    {
        var openSet = new List<Vector2> { start };
        var cameFrom = new Dictionary<Vector2, Vector2>();
        var gScore = new Dictionary<Vector2, float> { [start] = 0 };
        var fScore = new Dictionary<Vector2, float> { [start] = Heuristic(start, end) };

        while (openSet.Count > 0)
        {
            var current = openSet.OrderBy(n => fScore.ContainsKey(n) ? fScore[n] : float.MaxValue).First();

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);

            foreach (var neighbor in GetNeighbors(current))
            {
                float tentativeGScore = gScore[current] + Vector2.Distance(current, neighbor);

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null; // 경로가 없는 경우
    }

    private Dictionary<Vector2, TileNode> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        var totalPath = new Dictionary<Vector2, TileNode>();
        while (cameFrom.ContainsKey(current))
        {
            totalPath[current] = _tileGrid[current];
            current = cameFrom[current];
        }
        totalPath[current] = _tileGrid[current]; // Add the start node
        return totalPath;
    }

    private List<Vector2> GetNeighbors(Vector2 current)
    {
        List<Vector2> neighbors = new List<Vector2>();

        if (_tileGrid[current].GetTileInfo.GimmickShape == GimmickShape.Warp)
        {
            neighbors.Add(_warpPoints[current]);
        }

        foreach (var direction in new[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left })
        {
            Vector2 neighborPos = current + direction * _cellSize;
            bool a = _tileGrid.ContainsKey(neighborPos);
            if (a)
            {
                bool b = IsConnected(current, neighborPos);

                if (b)
                    neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }

    private float Heuristic(Vector2 a, Vector2 b)
    {
        return Vector2.Distance(a, b);
    }

    private bool IsConnected(Vector2 current, Vector2 neighbor)
    {
        TileNode currentNode = _tileGrid[current];
        TileNode neighborNode = _tileGrid[neighbor];

        // 현재 타일과 이웃 타일의 연결 방향을 가져옴
        List<Vector2Int> currentConnections = GetConnectedDirections(currentNode);
        List<Vector2Int> neighborConnections = GetConnectedDirections(neighborNode);

        // 현재 타일에서 이웃 타일로의 방향
        Vector2Int directionToNeighbor = Vector2Int.RoundToInt((neighbor - current)/_cellSize);
        Vector2Int directionFromNeighbor = Vector2Int.RoundToInt((current - neighbor)/_cellSize);

        return currentConnections.Contains(directionToNeighbor) && neighborConnections.Contains(directionFromNeighbor);
    }

    private List<Vector2Int> GetConnectedDirections(TileNode tileNode)
    {
        List<Vector2Int> connections = new List<Vector2Int>();

        // 각 RoadShape와 RotateValue에 따라 타일의 연결 방향을 계산합니다.
        switch (tileNode.GetTileInfo.RoadShape)
        {
            case RoadShape.Straight:
            case RoadShape.L:
            case RoadShape.T:
            case RoadShape.Cross:
                connections.AddRange(GetConnectionsForShape(tileNode.GetTileInfo.RoadShape, tileNode.GetTileInfo.RotateValue));
                break;
            // Start와 End 타일의 경우, 특정 연결 상태만을 가질 수 있도록 처리
            case RoadShape.Start:
                connections.Add(GetDirectionForRotation(tileNode.GetTileInfo.RotateValue, true));
                break;
            case RoadShape.End:
                connections.Add(GetDirectionForRotation(tileNode.GetTileInfo.RotateValue, false));
                break;
        }

        return connections;
    }
    
    // 연결 방향을 반환하는 메서드
    private List<Vector2Int> GetConnectionsForShape(RoadShape shape, int rotateValue)
    {
        List<Vector2Int> connections = new List<Vector2Int>();

        if (ShapeDirectionMappings.TryGetValue(shape, out Vector2Int[][] directions))
        {
            // rotateValue를 0~3 사이로 제한하고 해당 방향 추가
            int rotationIndex = rotateValue % directions.Length;
            connections.AddRange(directions[rotationIndex]);
        }

        return connections;
    }
    
    // 방향 설정 메서드 분리
    private Vector2Int GetDirectionForRotation(int rotateValue, bool isStart)
    {
        // isStart가 true이면 Start 타일의 방향을 반환, false이면 End 타일의 방향을 반환
        return rotateValue switch
        {
            0 => isStart ? Vector2Int.down : Vector2Int.up,
            1 => isStart ? Vector2Int.left : Vector2Int.right,
            2 => isStart ? Vector2Int.up : Vector2Int.down,
            3 => isStart ? Vector2Int.right : Vector2Int.left,
            _ => Vector2Int.zero // rotateValue가 0~3 외의 값일 경우를 대비
        };
    }

    private void LinkTileRotate(TileNode tile, bool isReverse)
    {
        RotationTile rotationTile = tile.transform.GetComponent<RotationTile>();
        
        if (!_linkTiles.Contains(tile))
        {
            rotationTile.RotateTile(tile.GetTileInfo.RotateValue);
            return;
        }

        foreach(var linktile in _linkTiles)
        {
            linktile.SetLinkTileRotate(true);
        }

        if (isReverse)
        {
            // 사용한 아이템의 수 감소 
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.DecreaseItemCount, nameof(ItemID.I1002));
            // 모든 타일들의 ReverseRotate 값 변화
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, false);
        }

    }

    private void SetLinkTileRandomRotate(int rotateValue)
    {
        foreach (var linkTile in _linkTiles)
        {
            for (var i = 0; i < rotateValue; i++)
            {
                linkTile.SetLinkTileRotate(false);
            }
        }
    }

    public bool GetRotationConditionSuccess(int minCount, List<TileNode> _pathTileList)
    {
        int RotateCountValue = 0;
        for(int i=0; i< _pathTileList.Count; i++)
        {
            var correctRotateValue = _pathTileList[i].CorrectTileInfo.RotateValue;
            var checkTargetRotateValue = _pathTileList[i].GetTileInfo.RotateValue;

            var disValue = 0;
            switch (_pathTileList[i].GetTileInfo.RoadShape)
            {
                case RoadShape.Straight:
                    disValue = (correctRotateValue - checkTargetRotateValue) % 2;
                    break;
                case RoadShape.Cross:
                    disValue = 0;
                    break;
                default:
                    disValue = Mathf.Abs(checkTargetRotateValue - correctRotateValue);
                    break;
            }

            RotateCountValue += disValue;
        }  
        DebugLogger.Log(RotateCountValue);

        return RotateCountValue > minCount;
    }

    private void SetMiniGameStage(bool isMiniGameStage)
    {
        if (this._isMiniGameStage == isMiniGameStage) return;

        this._isMiniGameStage = isMiniGameStage;

        DebugLogger.Log($"{isMiniGameStage}");
    }
}
