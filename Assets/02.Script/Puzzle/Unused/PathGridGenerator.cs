using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathGridGenerator : MonoBehaviour
{
    public GameObject gridParent; // 그리드 레이아웃이 적용된 부모 객체

    public int width = 5; // 그리드의 너비
    public int height = 5; // 그리드의 높이
    [PropertySpace(0f, 5f)] public int minDistance = 5; // 스타트 포인트와 엔드 포인트 간 최소 거리
    public float maxTimeToGeneratePath = 5f; // 경로 생성에 허용되는 최대 시간(초)

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
        StartCoroutine(GeneratePuzzleCoroutine());
    }

    private IEnumerator GeneratePuzzleCoroutine()
    {
        int maxAttempts = 10;
        int attempts = 0;
        while (!GenerateAndPlacePath() && attempts < maxAttempts)
        {
            ClearGrid();
            attempts++;
            yield return null;
        }

        if (attempts >= maxAttempts)
        {
            DebugLogger.Log("퍼즐 생성 실패: 최대 시도 횟수 초과");
        }
        else
        {
            DebugLogger.Log("퍼즐 생성 완료");
        }
    }

    public bool GenerateAndPlacePath()
    {
        bool success = GenerateFullGridPath(); // 경로 생성 시도
        if (success)
        {
            PlacePathTiles(); // 타일 배치
            RotateTilesRandomly(); // 타일 랜덤 회전
            success = AreAllTilesConnected(); // 모든 타일이 올바르게 연결되었는지 검증
        }
        return success;
    }

    // 그리드를 초기화하는 메서드
    private void ClearGrid()
    {
        foreach (Transform child in gridParent.transform)
        {
            Destroy(child.gameObject); // 그리드의 모든 타일 제거
        }
        _path = null;
        _occupied = null;
        _tiles = null;
    }

    // 무작위 시작점을 반환하는 메서드
    private Vector2Int GetRandomPoint()
    {
        return new Vector2Int(Random.Range(0, width), Random.Range(0, height));
    }

    // 경로 생성 메서드
    private bool GenerateFullGridPath()
    {
        _path = new List<Vector2Int>();
        _occupied = new HashSet<Vector2Int>();

        Vector2Int startPoint = GetRandomPoint();
        Vector2Int endPoint;
        do
        {
            endPoint = GetRandomPoint();
        } while (Vector2Int.Distance(startPoint, endPoint) < minDistance);

        _path.Add(startPoint);
        _occupied.Add(startPoint);

        Vector2Int currentPos = startPoint;
        int maxAttempts = width * height; // 최대 시도 횟수 설정
        int attempts = 0;

        while (currentPos != endPoint && attempts < maxAttempts)
        {
            List<Vector2Int> possibleMoves = GetPossibleMoves(currentPos);

            if (possibleMoves.Count > 0)
            {
                Vector2Int move = possibleMoves[Random.Range(0, possibleMoves.Count)];
                currentPos += move;

                _path.Add(currentPos);
                _occupied.Add(currentPos);

                if (currentPos == endPoint)
                {
                    return true; // 경로 생성 성공
                }
            }
            else
            {
                // 더 이상 이동할 수 없으면 현재 경로 생성 시도를 종료
                return false;
            }

            attempts++;
        }

        return false; // 최대 시도 횟수 초과
    }
    
    private List<Vector2Int> GetPossibleMoves(Vector2Int pos)
    {
        List<Vector2Int> moves = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int newPos = pos + dir;
            if (IsValidMove(newPos) && !_occupied.Contains(newPos))
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

    // 타일을 배치하는 메서드
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
                // 시작 타일 배치
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
                // 끝 타일 배치
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
            }
            else
            {
                DebugLogger.Log($"위치 {pos}에 타일 배치 실패"); // 타일 배치 실패 시 에러 로그
            }
        }
    }

    // 타일을 선택하고 배치하는 메서드
    private GameObject SelectAndPlaceTile(Vector2Int prevPos, Vector2Int currentPos, Vector2Int nextPos)
    {
        GameObject tile = null;
        int rotation = 0;

        // 일자 타일 조건
        if ((prevPos.x == currentPos.x && nextPos.x == currentPos.x) || (prevPos.y == currentPos.y && nextPos.y == currentPos.y))
        {
            tile = Instantiate(straightTilePrefab, gridParent.transform); // 일자 타일
            rotation = (prevPos.x == currentPos.x) ? 90 : 0; // 수직 또는 수평 방향
        }
        // 십자 타일 조건
        else if (IsCross(prevPos, currentPos, nextPos))
        {
            tile = Instantiate(crossTilePrefab, gridParent.transform); // 십자 타일
            rotation = 0; // 모든 방향
        }
        // T자 타일 조건
        else if (IsTShape(prevPos, currentPos, nextPos, out rotation))
        {
            tile = Instantiate(tTilePrefab, gridParent.transform); // T자 타일
        }
        // ㄱ자 타일 조건
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
            DebugLogger.Log("타일 생성 실패."); // 타일 생성 실패 시 에러 로그
        }

        return tile;
    }

    // 모든 타일이 올바르게 연결되었는지 확인
    private bool AreAllTilesConnected()
    {
        foreach (var entry in _tiles)
        {
            Vector2Int pos = entry.Key;
            GameObject tile = entry.Value;
            RotationTile rotationTile = tile.GetComponentInChildren<RotationTile>();
            if (rotationTile != null && !AreAllEntrancesConnected(rotationTile, pos))
            {
                return false;
            }
        }
        return true;
    }

    // 특정 타일의 모든 입구가 연결되었는지 확인
    private bool AreAllEntrancesConnected(RotationTile rotationTile, Vector2Int pos)
    {
        foreach (Vector2Int entrance in rotationTile.Entrances)
        {
            Vector2Int neighborPos = pos + entrance;
            if (!_tiles.ContainsKey(neighborPos)) return false;

            GameObject neighborTile = _tiles[neighborPos];
            RotationTile neighborRotationTile = neighborTile.GetComponentInChildren<RotationTile>();
            if (neighborRotationTile == null || !neighborRotationTile.Entrances.Contains(-entrance))
            {
                return false; // 이웃 타일이 입구와 맞지 않음
            }
        }
        return true;
    }

    // 십자 타일인지 확인하는 메서드
    private bool IsCross(Vector2Int prevPos, Vector2Int currentPos, Vector2Int nextPos)
    {
        bool isCross = (prevPos.x != currentPos.x && prevPos.y != currentPos.y) || (nextPos.x != currentPos.x && nextPos.y != currentPos.y);
        return isCross;
    }

    // T자 타일인지 확인하는 메서드 및 회전값 설정
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

        DebugLogger.Log($"T자 타일 확인 실패: {prevPos}, {currentPos}, {nextPos}");
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

        DebugLogger.Log($"ㄱ자 타일 확인 실패: {prevPos}, {currentPos}, {nextPos}");
        return false;
    }

    // 타일을 랜덤으로 회전시키는 메서드
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
