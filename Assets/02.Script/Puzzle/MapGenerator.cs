using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth, mapHeight; // 맵의 너비와 높이 설정
    [SerializeField] private GameObject straightTilePrefab, lTilePrefab, tTilePrefab, crossTilePrefab, closedStraightTilePrefab; // 타일 프리팹들
    [SerializeField] private Transform gridParent; // 타일이 배치될 그리드 레이아웃의 부모 객체 참조

    private int _curX; // 현재 X 좌표
    private int _curY; // 현재 Y 좌표
    private GameObject _spriteToUse; // 현재 사용할 타일 프리팹
    private List<Vector2Int> _tileEntrances; // 현재 사용할 타일의 입구 위치 리스트
    private TileData[,] _tileData; // 타일 데이터를 저장하는 2D 배열
    private Vector2Int _startPoint, _endPoint; // 시작 지점과 도착 지점의 좌표
    private int _minPathLength = 5; // 최소 경로 길이 설정

    private enum CurrentDirection
    {
        Left,  // 왼쪽 방향
        Right, // 오른쪽 방향
        Down,  // 아래 방향
        Up     // 위 방향
    }

    private CurrentDirection _curDirection = CurrentDirection.Down; // 초기 방향은 아래로 설정

    public struct TileData
    {
        public Transform Transform; // 타일의 Transform 참조
        public RotationTile RotationTile; // 타일의 RotationTile 스크립트 참조
        public int TileID; // 타일 ID, 경로 여부를 나타내는 데 사용
    }

    private void Awake()
    {
        _tileData = new TileData[mapWidth, mapHeight]; // 맵 크기에 맞게 타일 데이터 배열 초기화
        GenerateMap(); // 맵 생성
    }

    // 맵을 초기화하고 경로 생성을 시작하는 메서드
    private void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                _tileData[x, y].TileID = 0; // 초기 타일 ID는 0으로 설정 (경로가 아님을 의미)
            }
        }
        StartCoroutine(GeneratePath()); // 경로 생성 시작
    }

    // 스페이스 키를 눌렀을 때 맵을 재생성하는 메서드
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RegenerateMap();
        }
    }

    // 맵을 재생성하는 메서드
    private void RegenerateMap()
    {
        StopAllCoroutines(); // 기존 경로 생성 코루틴 중지
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (_tileData[x, y].Transform != null)
                {
                    Destroy(_tileData[x, y].Transform.gameObject);
                    _tileData[x, y].Transform = null;
                    _tileData[x, y].TileID = 0;
                }
            }
        }
        StartCoroutine(GeneratePath()); // 새로운 경로 생성 시작
    }

    // 경로를 생성하는 코루틴
    private IEnumerator GeneratePath()
    {
        _startPoint = new Vector2Int(Random.Range(0, mapWidth), 0); // 시작 지점 무작위로 설정
        _endPoint = new Vector2Int(Random.Range(0, mapWidth), mapHeight - 1); // 도착 지점 무작위로 설정

        _curX = _startPoint.x;
        _curY = _startPoint.y;

        // 시작 지점에 한쪽이 막힌 일자 타일 배치
        PlaceTile(_curX, _curY, closedStraightTilePrefab, new List<Vector2Int> { new Vector2Int(1, 0) });
        Debug.Log($"경로 시작: 위치 ({_curX}, {_curY})");

        int currentPathLength = 0;

        while (_curX != _endPoint.x || _curY != _endPoint.y)
        {
            DebugLogger.Log($"{_tileData[_curX,_curY]}");
            DebugLogger.Log($"{_tileData[_curX,_curY].RotationTile.Entrances}");
            
            // 다음 타일의 입구 위치를 결정
            List<Vector2Int> previousEntrances = _tileData[_curX, _curY].RotationTile.Entrances;
            Vector2Int nextEntrance = GetNextEntrance(previousEntrances);

            // 다음 타일의 종류와 위치를 결정
            ChooseNextTileAndDirection(nextEntrance);
            currentPathLength++;

            // 최소 경로 길이에 도달하지 않았다면 경로 생성 계속
            if (currentPathLength < _minPathLength && _curX == _endPoint.x && _curY == _endPoint.y)
            {
                continue; // 도착 지점에 도달했지만 최소 경로 길이를 채우지 않았을 경우 경로 연장
            }

            // 경로 생성 중 현재 위치가 시작점이나 끝점이 아닌 경우에만 타일 배치
            if (!(_curX == _startPoint.x && _curY == _startPoint.y) && !(_curX == _endPoint.x && _curY == _endPoint.y))
            {
                PlaceTile(_curX, _curY, _spriteToUse, _tileEntrances);
                Debug.Log($"타일 생성: 위치 ({_curX}, {_curY}), 방향 {_curDirection}");
            }

            // 현재 방향에 따라 위치 업데이트
            UpdatePosition();

            // 도착 지점에 도달했을 때
            if (_curX == _endPoint.x && _curY == _endPoint.y)
            {
                PlaceTile(_curX, _curY, closedStraightTilePrefab, new List<Vector2Int> { new Vector2Int(1, 2) });
                Debug.Log($"도착 지점: 위치 ({_curX}, {_curY})");
                break; // 경로 생성 완료
            }

            yield return new WaitForSeconds(0.05f); // 잠시 대기 후 다음 타일 배치
        }
    }

    // 타일을 배치하는 메서드
    private void PlaceTile(int x, int y, GameObject tilePrefab, List<Vector2Int> entrances)
    {
        GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity, gridParent);
        _tileData[x, y].Transform = newTile.transform;

        // 자식 오브젝트에서 RotationTile 스크립트를 가져옴
        RotationTile rotationTile = newTile.transform.GetChild(0).GetComponent<RotationTile>();
    
        if (rotationTile == null)
        {
            Debug.LogError($"RotationTile 스크립트를 찾을 수 없습니다. 위치: ({x}, {y}), 타일 프리팹: {tilePrefab.name}");
            return;
        }
    
        _tileData[x, y].RotationTile = rotationTile;
        _tileData[x, y].TileID = 1;

        _tileData[x, y].RotationTile.Entrances = entrances;
        _tileData[x, y].RotationTile.UpdateEntrances();
        
        DebugLogger.Log($"{_tileData[x,y].RotationTile}");
    }

    // 다음 타일의 입구 위치를 결정하는 메서드
    private Vector2Int GetNextEntrance(List<Vector2Int> previousEntrances)
    {
        // 이전 타일의 출구가 어디인지 확인하여 다음 타일의 입구로 설정
        foreach (var entrance in previousEntrances)
        {
            if (entrance == new Vector2Int(1, 0)) return new Vector2Int(1, 2);
            if (entrance == new Vector2Int(2, 1)) return new Vector2Int(0, 1);
            if (entrance == new Vector2Int(1, 2)) return new Vector2Int(1, 0);
            if (entrance == new Vector2Int(0, 1)) return new Vector2Int(2, 1);
        }
        return new Vector2Int(1, 0); // 기본값 (예외 처리)
    }

    // 다음 타일과 방향을 결정하는 메서드
    private void ChooseNextTileAndDirection(Vector2Int nextEntrance)
    {
        // 여기에서 다음 타일 종류와 방향을 결정합니다.
        // 예: T타일을 배치해야 하는 경우
        _spriteToUse = tTilePrefab;
        _tileEntrances = new List<Vector2Int> { new Vector2Int(1, 2), new Vector2Int(2, 1), new Vector2Int(1, 0) };

        // 랜덤하게 L타일을 배치할 수도 있습니다.
        if (Random.value < 0.5f)
        {
            _spriteToUse = lTilePrefab;
            _tileEntrances = new List<Vector2Int> { nextEntrance, GetNextEntrance(new List<Vector2Int> { nextEntrance }) };
        }
    }

    // 현재 위치 업데이트
    private void UpdatePosition()
    {
        if (_curDirection == CurrentDirection.Down)
        {
            _curY++;
        }
        else if (_curDirection == CurrentDirection.Left)
        {
            _curX--;
        }
        else if (_curDirection == CurrentDirection.Right)
        {
            _curX++;
        }
        else if (_curDirection == CurrentDirection.Up)
        {
            _curY--;
        }
    }
}
