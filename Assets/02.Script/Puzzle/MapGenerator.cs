using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트를 사용하기 위해 추가

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth, mapHeight; // 맵의 너비와 높이 설정
    [SerializeField] private GameObject tileReference; // 타일 프리팹 참조
    [SerializeField] private Transform gridParent; // 타일이 배치될 그리드 레이아웃의 부모 객체 참조
    [SerializeField] private Sprite straightTile, closedStraightTile, tTile, lTile; // 다양한 타일 스프라이트들

    private int curX; // 현재 X 좌표
    private int curY; // 현재 Y 좌표
    private Sprite spriteToUse; // 현재 타일에 사용할 스프라이트

    private enum CurrentDirection
    {
        LEFT,  // 왼쪽 방향
        RIGHT, // 오른쪽 방향
        DOWN,  // 아래 방향
        UP     // 위 방향
    };
    private CurrentDirection curDirection = CurrentDirection.DOWN; // 현재 진행 방향 (초기 값은 아래)

    public struct TileData
    {
        public Transform transform; // 타일의 Transform 참조
        public Image image; // 타일의 자식 Image 컴포넌트 참조 (타일 모양 이미지)
        public int tileID; // 타일 ID, 경로 여부를 나타내는 데 사용
    }

    private TileData[,] tileData; // 타일 데이터를 저장하는 2D 배열
    private Vector2Int startPoint, endPoint; // 시작 지점과 도착 지점의 좌표

    private void Awake()
    {
        tileData = new TileData[mapWidth, mapHeight]; // 맵 크기에 맞게 타일 데이터 배열 초기화
        GenerateMap(); // 맵 생성
    }

    // 맵을 초기화하고 경로 생성을 시작하는 메서드
    private void GenerateMap()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 타일 생성 및 초기화, gridParent를 부모로 설정
                GameObject newTile = Instantiate(tileReference, gridParent);
                tileData[x, y].transform = newTile.transform; // 타일의 Transform 저장

                // 자식 오브젝트의 Image 컴포넌트를 가져옴
                tileData[x, y].image = newTile.transform.GetChild(0).GetComponent<Image>();
                tileData[x, y].tileID = 0; // 초기 타일 ID는 0으로 설정 (경로가 아님을 의미)
                tileData[x, y].image.sprite = null; // 초기에는 스프라이트가 없음
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
                // 타일을 초기 상태로 되돌림
                tileData[x, y].image.sprite = null;
                tileData[x, y].tileID = 0;
            }
        }
        StartCoroutine(GeneratePath()); // 새로운 경로 생성 시작
    }

    // 경로를 생성하는 코루틴
    private IEnumerator GeneratePath()
    {
        startPoint = new Vector2Int(Random.Range(0, mapWidth), 0); // 시작 지점 무작위로 설정
        endPoint = new Vector2Int(Random.Range(0, mapWidth), mapHeight - 1); // 도착 지점 무작위로 설정

        curX = startPoint.x;
        curY = startPoint.y;

        // 시작 지점에 한쪽이 막힌 일자 타일 배치
        UpdateMap(curX, curY, closedStraightTile);

        while (curX != endPoint.x || curY != endPoint.y)
        {
            ChooseTileAndDirection(); // 타일 선택 및 방향 결정
            UpdateMap(curX, curY, spriteToUse); // 맵 업데이트 (타일 배치)

            // 현재 방향에 따라 위치 업데이트
            if (curDirection == CurrentDirection.DOWN)
            {
                curY++;
            }
            else if (curDirection == CurrentDirection.LEFT)
            {
                curX--;
            }
            else if (curDirection == CurrentDirection.RIGHT)
            {
                curX++;
            }
            else if (curDirection == CurrentDirection.UP)
            {
                curY--;
            }

            yield return new WaitForSeconds(0.05f); // 잠시 대기 후 다음 타일 배치
        }

        // 도착 지점에 한쪽이 막힌 일자 타일 배치
        UpdateMap(curX, curY, closedStraightTile);
    }

    // 현재 위치와 방향에 따라 사용할 타일과 다음 방향을 선택하는 메서드
    private void ChooseTileAndDirection()
    {
        if (curX == startPoint.x && curY == startPoint.y)
        {
            spriteToUse = closedStraightTile; // 시작 지점에서는 한쪽이 막힌 일자 타일 사용
        }
        else if (curY == mapHeight - 1)
        {
            spriteToUse = closedStraightTile; // 도착 지점에 도달한 경우, 한쪽이 막힌 일자 타일 사용
        }
        else if (curDirection == CurrentDirection.DOWN)
        {
            spriteToUse = straightTile; // 아래로 진행 중인 경우 일자 타일 사용

            // 랜덤으로 방향 변경: 좌우 또는 직진
            float directionChance = Random.value;

            if (directionChance < 0.33f && curX > 0)
            {
                curDirection = CurrentDirection.LEFT;
                spriteToUse = lTile; // L자 타일로 방향 변경
            }
            else if (directionChance < 0.66f && curX < mapWidth - 1)
            {
                curDirection = CurrentDirection.RIGHT;
                spriteToUse = lTile; // L자 타일로 방향 변경
            }
            else
            {
                curDirection = CurrentDirection.DOWN;
            }
        }
        else
        {
            // T자 타일 또는 L자 타일 사용
            spriteToUse = (curDirection == CurrentDirection.LEFT || curDirection == CurrentDirection.RIGHT) ? lTile : tTile;

            // 다시 아래로 이동
            curDirection = CurrentDirection.DOWN;
        }
    }

    // 맵을 업데이트하여 타일을 배치하는 메서드
    private void UpdateMap(int mapX, int mapY, Sprite spriteToUse)
    {
        tileData[mapX, mapY].tileID = 1; // 경로로 설정
        tileData[mapX, mapY].image.sprite = spriteToUse; // 자식 오브젝트의 Image 컴포넌트에 스프라이트 할당
    }
}
