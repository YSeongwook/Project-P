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
    [FoldoutGroup("Tile Prefab")] public GameObject tTilePrefab; // ㅗ자 타일 프리팹
    [FoldoutGroup("Tile Prefab")] public GameObject cornerTilePrefab; // ㄱ자 타일 프리팹

    private List<Vector2Int> _path; // 경로 저장
    private HashSet<Vector2Int> _occupied; // 점유된 타일 좌표 저장
    private Dictionary<Vector2Int, GameObject> _tiles; // 위치와 타일 매핑

    private void Start()
    {
        Vector2Int startPoint = GetRandomPoint();
        Vector2Int endPoint = GetDistantPoint(startPoint, minDistance);

        GeneratePath(startPoint, endPoint);
        PlacePathTiles();
        RotateTilesRandomly();
    }

    // 무작위 시작점 반환
    private Vector2Int GetRandomPoint()
    {
        return new Vector2Int(Random.Range(0, width), Random.Range(0, height));
    }

    // 특정 거리 이상 떨어진 무작위 점 반환
    private Vector2Int GetDistantPoint(Vector2Int startPoint, int minDistance)
    {
        Vector2Int endPoint;
        do
        {
            endPoint = GetRandomPoint();
        } while (Vector2Int.Distance(startPoint, endPoint) < minDistance);

        return endPoint;
    }

    // 경로 생성 메서드
    private void GeneratePath(Vector2Int startPoint, Vector2Int endPoint)
    {
        _path = new List<Vector2Int>();
        _occupied = new HashSet<Vector2Int>();
        Vector2Int currentPos = startPoint;
        _path.Add(currentPos);
        _occupied.Add(currentPos);

        while (currentPos != endPoint)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();

            if (currentPos.x < width - 1 && !_occupied.Contains(currentPos + Vector2Int.right))
                possibleMoves.Add(Vector2Int.right);
            if (currentPos.y < height - 1 && !_occupied.Contains(currentPos + Vector2Int.up))
                possibleMoves.Add(Vector2Int.up);
            if (currentPos.x > 0 && !_occupied.Contains(currentPos + Vector2Int.left))
                possibleMoves.Add(Vector2Int.left);
            if (currentPos.y > 0 && !_occupied.Contains(currentPos + Vector2Int.down))
                possibleMoves.Add(Vector2Int.down);

            if (possibleMoves.Count == 0) break; // 더 이상 이동할 곳이 없으면 종료

            Vector2Int move = possibleMoves[Random.Range(0, possibleMoves.Count)];
            currentPos += move;

            _path.Add(currentPos);
            _occupied.Add(currentPos);
        }
    }

    // 타일 배치 메서드
    private void PlacePathTiles()
    {
        _tiles = new Dictionary<Vector2Int, GameObject>();
        for (int i = 0; i < _path.Count; i++)
        {
            Vector2Int pos = _path[i];
            GameObject tile = null;

            if (i == 0 || i == _path.Count - 1)
            {
                // 시작 타일과 끝 타일
                tile = Instantiate(straightTilePrefab, gridParent.transform);
            }
            else
            {
                Vector2Int prevPos = _path[i - 1];
                Vector2Int nextPos = _path[i + 1];

                // 타일 종류 선택
                tile = SelectTile(prevPos, pos, nextPos);
            }

            RectTransform rectTransform = tile.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(pos.x * 100, pos.y * 100); // 각 타일의 위치 설정
            _tiles[pos] = tile;

            // 클릭 이벤트 추가
            tile.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => RotateTile(tile));
        }
    }

    // 타일 선택 메서드
    private GameObject SelectTile(Vector2Int prevPos, Vector2Int currentPos, Vector2Int nextPos)
    {
        if (prevPos.x == currentPos.x && nextPos.x == currentPos.x)
        {
            return Instantiate(straightTilePrefab, gridParent.transform); // 일자 타일
        }
        else if (prevPos.y == currentPos.y && nextPos.y == currentPos.y)
        {
            return Instantiate(straightTilePrefab, gridParent.transform); // 일자 타일
        }
        else if (prevPos.x != currentPos.x && nextPos.x != currentPos.x && prevPos.y != currentPos.y && nextPos.y != currentPos.y)
        {
            return Instantiate(crossTilePrefab, gridParent.transform); // 십자 타일
        }
        else if ((prevPos.x == currentPos.x && nextPos.y == currentPos.y) || (prevPos.y == currentPos.y && nextPos.x == currentPos.x))
        {
            return Instantiate(tTilePrefab, gridParent.transform); // ㅗ자 타일
        }
        else
        {
            return Instantiate(cornerTilePrefab, gridParent.transform); // ㄱ자 타일
        }
    }

    // 타일 회전 메서드
    private void RotateTile(GameObject tile)
    {
        tile.transform.rotation *= Quaternion.Euler(0, 0, 90); // 90도 회전
    }

    // 타일 랜덤 회전 메서드
    private void RotateTilesRandomly()
    {
        foreach (KeyValuePair<Vector2Int, GameObject> entry in _tiles)
        {
            int randomRotation = Random.Range(0, 4) * 90; // 0, 90, 180, 270 중 하나 선택
            entry.Value.transform.rotation = Quaternion.Euler(0, 0, randomRotation);
        }
    }
}
