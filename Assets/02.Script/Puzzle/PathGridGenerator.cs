using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathGridGenerator : MonoBehaviour
{
    public GameObject gridParent; // 그리드 레이아웃이 적용된 부모 객체

    public int width = 5; // 그리드의 너비
    public int height = 5; // 그리드의 높이
    [PropertySpace(0f, 5f)] public int minDistance = 5; // 스타트 포인트와 엔드 포인트 간 최소 거리

    [FoldoutGroup("Tile Prefab")] public GameObject straightTilePrefab; // 일자 타일 프리팹
    [FoldoutGroup("Tile Prefab")] public GameObject crossTilePrefab; // 십자 타일 프리팹
    [FoldoutGroup("Tile Prefab")] public GameObject tTilePrefab; // T자 타일 프리팹
    [FoldoutGroup("Tile Prefab")] public GameObject cornerTilePrefab; // ㄱ자 타일 프리팹
    [FoldoutGroup("Tile Prefab")] public GameObject endTilePrefab; // 끝 부분 타일 프리팹

    private List<Vector2Int> _path; // 경로 저장
    private HashSet<Vector2Int> _occupied; // 점유된 타일 좌표 저장
    private Dictionary<Vector2Int, GameObject> _tiles; // 위치와 타일 매핑

    private void Start()
    {
        GenerateAndPlacePath();
    }

    public void GenerateAndPlacePath()
    {
        bool success = GenerateFullGridPath();
        if (success)
        {
            PlacePathTiles();
            RotateTilesRandomly();
        }
        else
        {
            Debug.LogError("Failed to generate a valid path.");
        }
    }

    // 무작위 시작점 반환
    private Vector2Int GetRandomPoint()
    {
        return new Vector2Int(Random.Range(0, width), Random.Range(0, height));
    }

    // 경로 생성 메서드
    private bool GenerateFullGridPath()
    {
        _path = new List<Vector2Int>();
        _occupied = new HashSet<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Vector2Int startPos = GetRandomPoint();
        queue.Enqueue(startPos);
        _path.Add(startPos);
        _occupied.Add(startPos);

        while (queue.Count > 0)
        {
            Vector2Int currentPos = queue.Dequeue();
            List<Vector2Int> possibleMoves = new List<Vector2Int>();

            if (currentPos.x < width - 1 && !_occupied.Contains(currentPos + Vector2Int.right))
                possibleMoves.Add(Vector2Int.right);
            if (currentPos.y < height - 1 && !_occupied.Contains(currentPos + Vector2Int.up))
                possibleMoves.Add(Vector2Int.up);
            if (currentPos.x > 0 && !_occupied.Contains(currentPos + Vector2Int.left))
                possibleMoves.Add(Vector2Int.left);
            if (currentPos.y > 0 && !_occupied.Contains(currentPos + Vector2Int.down))
                possibleMoves.Add(Vector2Int.down);

            foreach (Vector2Int move in possibleMoves)
            {
                Vector2Int newPos = currentPos + move;
                if (!_occupied.Contains(newPos))
                {
                    queue.Enqueue(newPos);
                    _path.Add(newPos);
                    _occupied.Add(newPos);
                }
            }
        }

        return _path.Count == width * height;
    }

    // 타일 배치 메서드
    private void PlacePathTiles()
    {
        _tiles = new Dictionary<Vector2Int, GameObject>();
        for (int i = 0; i < _path.Count; i++)
        {
            Vector2Int pos = _path[i];
            GameObject tile = null;
            int rotation = 0;

            if (i == 0)
            {
                // 시작 타일
                tile = Instantiate(endTilePrefab, gridParent.transform);
                rotation = 0; // 하단 입구 설정
                var rotationTile = tile.GetComponentInChildren<RotationTile>();
                if (rotationTile != null)
                {
                    rotationTile.Entrances = new List<Vector2Int> { Vector2Int.up }; // 시작 타일 입구 설정
                }
            }
            else if (i == _path.Count - 1)
            {
                // 끝 타일
                tile = Instantiate(endTilePrefab, gridParent.transform);
                rotation = 180; // 하단 입구 설정
                var rotationTile = tile.GetComponentInChildren<RotationTile>();
                if (rotationTile != null)
                {
                    rotationTile.Entrances = new List<Vector2Int> { Vector2Int.down }; // 끝 타일 입구 설정
                }
            }
            else
            {
                Vector2Int prevPos = _path[i - 1];
                Vector2Int nextPos = (i < _path.Count - 1) ? _path[i + 1] : prevPos;

                // 타일 종류 선택 및 배치
                tile = SelectAndPlaceTile(prevPos, pos, nextPos);
            }

            if (tile != null)
            {
                tile.transform.rotation = Quaternion.Euler(0, 0, rotation);

                RectTransform rectTransform = tile.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(pos.x * 100, pos.y * 100); // 각 타일의 위치 설정
                _tiles[pos] = tile;

                Debug.Log($"Placed tile at position {pos}"); // 디버그 메시지 추가
            }
            else
            {
                Debug.LogError($"Failed to place tile at position {pos}");
            }
        }
    }


    // 타일 선택 및 배치 메서드
    private GameObject SelectAndPlaceTile(Vector2Int prevPos, Vector2Int currentPos, Vector2Int nextPos)
    {
        GameObject tile = null;
        int rotation = 0;

        // 일자 타일 조건 추가
        if ((prevPos.x == currentPos.x && nextPos.x == currentPos.x) || (prevPos.y == currentPos.y && nextPos.y == currentPos.y))
        {
            tile = Instantiate(straightTilePrefab, gridParent.transform); // 일자 타일
            rotation = (prevPos.x == currentPos.x) ? 90 : 0; // 수직 또는 수평 방향
        }
        // 십자 타일 조건 수정
        else if (IsCross(prevPos, currentPos))
        {
            tile = Instantiate(crossTilePrefab, gridParent.transform); // 십자 타일
            rotation = 0; // 모든 방향
        }
        // ㅗ자 타일 조건 추가
        else if (IsTShape(prevPos, currentPos, nextPos, out rotation))
        {
            tile = Instantiate(tTilePrefab, gridParent.transform); // T자 타일
        }
        // ㄱ자 타일 조건 추가
        else if (IsCorner(prevPos, currentPos, nextPos, out rotation))
        {
            tile = Instantiate(cornerTilePrefab, gridParent.transform); // ㄱ자 타일
        }

        if (tile != null)
        {
            tile.transform.rotation = Quaternion.Euler(0, 0, rotation);
        
            // 입구 정보 업데이트
            RotationTile rotationTile = tile.GetComponentInChildren<RotationTile>();
            if (rotationTile != null)
            {
                rotationTile.UpdateEntrances();
            }
        }
        else
        {
            Debug.LogError("Tile instantiation failed.");
        }
        
        return tile;
    }

    // 십자 타일인지 확인하는 메서드 수정
    private bool IsCross(Vector2Int prevPos, Vector2Int currentPos)
    {
        return (prevPos.x != currentPos.x && prevPos.y != currentPos.y);
    }

    // ㅗ자 타일인지 확인하는 메서드 및 회전값 설정
    private bool IsTShape(Vector2Int prevPos, Vector2Int currentPos, Vector2Int nextPos, out int rotation)
    {
        rotation = 0;

        if ((prevPos.y == currentPos.y && currentPos.x != nextPos.x) || (prevPos.x == currentPos.x && currentPos.y != nextPos.y))
        {
            if (currentPos.y > nextPos.y)
            {
                rotation = 0; // 아래 방향
            }
            else if (currentPos.y < nextPos.y)
            {
                rotation = 180; // 위 방향
            }
            else if (currentPos.x > nextPos.x)
            {
                rotation = 90; // 오른쪽 방향
            }
            else if (currentPos.x < nextPos.x)
            {
                rotation = 270; // 왼쪽 방향
            }
            return true;
        }

        return false;
    }

    // ㄱ자 타일인지 확인하는 메서드 및 회전값 설정
    private bool IsCorner(Vector2Int prevPos, Vector2Int currentPos, Vector2Int nextPos, out int rotation)
    {
        rotation = 0;

        if ((prevPos.x != currentPos.x && currentPos.y != nextPos.y) || (prevPos.y != currentPos.y && currentPos.x != nextPos.y))
        {
            if (currentPos.x < nextPos.x && currentPos.y < prevPos.y)
            {
                rotation = 0; // 오른쪽 아래
            }
            else if (currentPos.x < nextPos.x && currentPos.y > prevPos.y)
            {
                rotation = 270; // 오른쪽 위
            }
            else if (currentPos.x > nextPos.x && currentPos.y < prevPos.y)
            {
                rotation = 90; // 왼쪽 아래
            }
            else if (currentPos.x > nextPos.x && currentPos.y > prevPos.y)
            {
                rotation = 180; // 왼쪽 위
            }
            return true;
        }

        return false;
    }

    // 타일 랜덤 회전 메서드
    private void RotateTilesRandomly()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> entry in _tiles)
        {
            int randomRotation = Random.Range(0, 4) * 90; // 0, 90, 180, 270 중 하나 선택
            entry.Value.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
            
            // 회전 후 입구 정보 업데이트
            RotationTile rotationTile = entry.Value.GetComponentInChildren<RotationTile>();
            if (rotationTile != null)
            {
                rotationTile.UpdateEntrances();
            }
        }
    }
}
