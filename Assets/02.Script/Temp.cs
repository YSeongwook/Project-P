using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp
{
    private Dictionary<Vector2, TileNode> _tileGrid = new Dictionary<Vector2, TileNode>();

    public void SetTileGrid(bool isRegister)
    {
        if(isRegister) EventManager<StageEvent>.StartListening<RectTransform, TileNode>(StageEvent.SetTileGrid, SetTileGrid);
        else EventManager<StageEvent>.StopListening<RectTransform, TileNode>(StageEvent.SetTileGrid, SetTileGrid);
    }

    private void SetTileGrid(RectTransform rectTransform, TileNode tileNode)
    {
        _tileGrid.Clear();

        Vector2 pos = rectTransform.anchoredPosition;
        _tileGrid.Add(pos, tileNode);

        DebugLogger.Log($"{pos} : {tileNode.GetTileInfo.RoadShape}");
    }

    private bool IsConnected(Vector2Int current, Vector2Int neighbor)
    {
        TileNode currentNode = _tileGrid[current];
        TileNode neighborNode = _tileGrid[neighbor];

        // 현재 타일의 연결 상태를 구합니다.
        List<Vector2Int> currentConnections = GetConnectedDirections(currentNode);

        // 이웃 타일의 연결 상태를 구합니다.
        List<Vector2Int> neighborConnections = GetConnectedDirections(neighborNode);

        // 두 타일이 서로 연결되는지 확인
        Vector2Int directionToNeighbor = neighbor - current;
        Vector2Int directionFromNeighbor = current - neighbor;

        return currentConnections.Contains(directionToNeighbor) && neighborConnections.Contains(directionFromNeighbor);
    }

    private List<Vector2Int> GetConnectedDirections(TileNode tileNode)
    {
        List<Vector2Int> connections = new List<Vector2Int>();

        // 각 RoadShape와 RotateValue에 따라 타일의 연결 방향을 계산합니다.
        switch (tileNode.GetTileInfo.RoadShape)
        {
            case RoadShape.Straight:
                if (tileNode.GetTileInfo.RotateValue % 2 == 0) // 0도, 180도
                {
                    connections.Add(Vector2Int.up);
                    connections.Add(Vector2Int.down);
                }
                else // 90도, 270도
                {
                    connections.Add(Vector2Int.left);
                    connections.Add(Vector2Int.right);
                }
                break;
            case RoadShape.L:
                switch (tileNode.GetTileInfo.RotateValue)
                {
                    case 0:
                        connections.Add(Vector2Int.up);
                        connections.Add(Vector2Int.right);
                        break;
                    case 1:
                        connections.Add(Vector2Int.right);
                        connections.Add(Vector2Int.down);
                        break;
                    case 2:
                        connections.Add(Vector2Int.down);
                        connections.Add(Vector2Int.left);
                        break;
                    case 3:
                        connections.Add(Vector2Int.left);
                        connections.Add(Vector2Int.up);
                        break;
                }
                break;
            case RoadShape.T:
                switch (tileNode.GetTileInfo.RotateValue)
                {
                    case 0:
                        connections.Add(Vector2Int.up);
                        connections.Add(Vector2Int.left);
                        connections.Add(Vector2Int.right);
                        break;
                    case 1:
                        connections.Add(Vector2Int.right);
                        connections.Add(Vector2Int.up);
                        connections.Add(Vector2Int.down);
                        break;
                    case 2:
                        connections.Add(Vector2Int.down);
                        connections.Add(Vector2Int.left);
                        connections.Add(Vector2Int.right);
                        break;
                    case 3:
                        connections.Add(Vector2Int.left);
                        connections.Add(Vector2Int.up);
                        connections.Add(Vector2Int.down);
                        break;
                }
                break;
            case RoadShape.Cross:
                connections.Add(Vector2Int.up);
                connections.Add(Vector2Int.down);
                connections.Add(Vector2Int.left);
                connections.Add(Vector2Int.right);
                break;
            // Start와 End 타일의 경우, 특정 연결 상태만을 가질 수 있도록 처리
            case RoadShape.Start:
            case RoadShape.End:
                if (tileNode.GetTileInfo.RotateValue == 0)
                {
                    connections.Add(Vector2Int.up);
                }
                else if (tileNode.GetTileInfo.RotateValue == 1)
                {
                    connections.Add(Vector2Int.right);
                }
                else if (tileNode.GetTileInfo.RotateValue == 2)
                {
                    connections.Add(Vector2Int.down);
                }
                else if (tileNode.GetTileInfo.RotateValue == 3)
                {
                    connections.Add(Vector2Int.left);
                }
                break;
        }

        return connections;
    }

}
