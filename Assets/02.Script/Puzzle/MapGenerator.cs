using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth, mapHeight;
    [SerializeField] private GameObject tileReference;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Sprite straightTile, closedStraightTile, tTile, lTile;
    [SerializeField] private int maxRetryCount = 10; // 경로 생성 시도 횟수
    [SerializeField] private int maxMapGenerationCount = 30; // 맵 생성 시도 횟수

    private List<Vector2Int> startPoints;
    private List<Vector2Int> endPoints;

    private TileData[,] tileData;

    public struct TileData
    {
        public Transform Transform;
        public Image Image;
        public int TileID;
    }

    private void Start()
    {
        tileData = new TileData[mapWidth, mapHeight];
    }

    // 버튼을 누르면 이 메서드가 호출되어 맵을 생성
    public void OnGenerateMapButtonClicked()
    {
        StartCoroutine(GenerateMapWithRetries());
    }

    private IEnumerator GenerateMapWithRetries()
    {
        int mapGenerationCount = 0;

        while (mapGenerationCount < maxMapGenerationCount)
        {
            DebugLogger.Log($"맵 생성 시도: {mapGenerationCount + 1}/{maxMapGenerationCount}");
            GenerateMap();
            
            // 맵 생성이 성공하면 중단
            if (WasMapGeneratedSuccessfully())
            {
                DebugLogger.Log("맵 생성 성공");
                yield break;
            }

            mapGenerationCount++;
            yield return null; // 다음 프레임으로 넘어가기
        }

        DebugLogger.LogError("최대 맵 생성 시도 횟수를 초과하여 맵 생성에 실패했습니다.");
    }

    private void GenerateMap()
    {
        InitializeTiles();
        SetStartAndEndPoints();
        StartCoroutine(GeneratePaths());
    }

    private void InitializeTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                GameObject newTile = Instantiate(tileReference, gridParent);
                tileData[x, y].Transform = newTile.transform;
                tileData[x, y].Image = newTile.transform.GetChild(0).GetComponent<Image>();
                tileData[x, y].TileID = 0;
                tileData[x, y].Image.sprite = null;
            }
        }
    }

    private void SetStartAndEndPoints()
    {
        startPoints = new List<Vector2Int>
        {
            new Vector2Int(0, 0),
            new Vector2Int(4, 0)
        };

        endPoints = new List<Vector2Int>
        {
            new Vector2Int(0, mapHeight - 1),
            new Vector2Int(4, mapHeight - 1)
        };
    }

    private IEnumerator GeneratePaths()
    {
        int retryCount = 0;

        while (retryCount < maxRetryCount)
        {
            try
            {
                ClearTiles();
                bool pathGenerationSuccess = true;

                for (int i = 0; i < startPoints.Count; i++)
                {
                    if (!GeneratePath(startPoints[i], endPoints[i]))
                    {
                        pathGenerationSuccess = false;
                        break; // 한 경로라도 실패하면 나머지 경로 생성 시도를 중단
                    }
                }

                if (pathGenerationSuccess)
                {
                    yield break; // 경로 생성에 성공하면 중단
                }
            }
            catch (System.Exception ex)
            {
                DebugLogger.LogError(ex.Message);
            }

            retryCount++;
            DebugLogger.Log($"경로 생성 재시도: {retryCount}/{maxRetryCount}");
            yield return null; // 다음 프레임으로 넘어가기
        }

        DebugLogger.LogError("최대 경로 생성 시도 횟수를 초과하여 경로 생성에 실패했습니다.");
    }

    private bool GeneratePath(Vector2Int startPoint, Vector2Int endPoint)
    {
        int curX = startPoint.x;
        int curY = startPoint.y;
        UpdateMap(curX, curY, closedStraightTile);

        while (curX != endPoint.x || curY != endPoint.y)
        {
            ChooseTileAndDirection(ref curX, ref curY);

            // 현재 위치가 배열의 범위를 벗어나는지 확인
            if (curX < 0 || curX >= mapWidth || curY < 0 || curY >= mapHeight)
            {
                DebugLogger.LogError($"잘못된 타일 위치: ({curX}, {curY})는 배열의 범위를 벗어났습니다.");
                return false; // 범위를 벗어나면 경로 생성 실패
            }

            UpdateMap(curX, curY, spriteToUse);
        }

        UpdateMap(curX, curY, closedStraightTile);
        return true; // 경로 생성 성공
    }

    private void ClearTiles()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                tileData[x, y].Image.sprite = null;
                tileData[x, y].TileID = 0;
            }
        }
    }

    private void ChooseTileAndDirection(ref int curX, ref int curY)
    {
        // 경로 생성 시 배열 범위를 넘지 않도록 조건을 추가
        if (curY == mapHeight - 1)
        {
            spriteToUse = closedStraightTile;
        }
        else if (curDirection == CurrentDirection.DOWN)
        {
            spriteToUse = straightTile;
            float directionChance = Random.value;

            if (directionChance < 0.33f && curX > 0)
            {
                curDirection = CurrentDirection.LEFT;
                spriteToUse = lTile;
            }
            else if (directionChance < 0.66f && curX < mapWidth - 1)
            {
                curDirection = CurrentDirection.RIGHT;
                spriteToUse = lTile;
            }
            else if (curX > 0 && curX < mapWidth - 1)
            {
                spriteToUse = tTile;
            }
        }
        else if (curDirection == CurrentDirection.LEFT || curDirection == CurrentDirection.RIGHT)
        {
            spriteToUse = tTile;
            curDirection = CurrentDirection.DOWN;
        }
        else
        {
            spriteToUse = lTile;
            curDirection = CurrentDirection.DOWN;
        }

        // 다음 위치를 결정할 때도 경계 체크
        if (curDirection == CurrentDirection.DOWN && curY + 1 < mapHeight)
        {
            curY++;
        }
        else if (curDirection == CurrentDirection.LEFT && curX - 1 >= 0)
        {
            curX--;
        }
        else if (curDirection == CurrentDirection.RIGHT && curX + 1 < mapWidth)
        {
            curX++;
        }
        else if (curDirection == CurrentDirection.UP && curY - 1 >= 0)
        {
            curY--;
        }
    }

    private void UpdateMap(int mapX, int mapY, Sprite spriteToUse)
    {
        if (mapX < 0 || mapX >= mapWidth || mapY < 0 || mapY >= mapHeight)
        {
            DebugLogger.LogError($"잘못된 타일 위치: ({mapX}, {mapY})는 배열의 범위를 벗어났습니다.");
            return;
        }

        tileData[mapX, mapY].TileID = 1;
        tileData[mapX, mapY].Image.sprite = spriteToUse;
        DebugLogger.Log($"타일 배치됨: ({mapX}, {mapY})");
    }

    private bool WasMapGeneratedSuccessfully()
    {
        // 맵 생성이 성공적으로 이루어졌는지 확인하는 로직을 추가합니다.
        // 예를 들어, 모든 경로가 제대로 생성되었는지, 경로가 닫히지 않았는지 확인할 수 있습니다.
        // 여기에 맞는 검증 로직을 구현하세요.

        return true; // 임시로 항상 true를 반환. 실제 검증 로직을 추가하세요.
    }

    private enum CurrentDirection
    {
        LEFT,
        RIGHT,
        DOWN,
        UP
    };

    private CurrentDirection curDirection = CurrentDirection.DOWN;
    private Sprite spriteToUse;
}
