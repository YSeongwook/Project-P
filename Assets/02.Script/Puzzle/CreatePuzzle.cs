using System.Collections.Generic;
using UnityEngine;

public class CreatePuzzle : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;  // 타일 프리팹
    [SerializeField] private GameObject gridParent;  // 그리드 레이아웃이 적용된 부모 객체
    [SerializeField] private Color startColor = Color.green;  // 시작 타일 색상
    [SerializeField] private Color endColor = Color.red;  // 끝 타일 색상

    [SerializeField] private int maxGenerationAttempts = 30;  // 경로 생성 최대 시도 횟수

    public void OnCreate3X3Puzzle() => GenerateAndPlacePuzzle(3, 3);
    public void OnCreate4X4Puzzle() => GenerateAndPlacePuzzle(4, 4);
    public void OnCreate5X5Puzzle() => GenerateAndPlacePuzzle(5, 5);
    public void OnCreate6X6Puzzle() => GenerateAndPlacePuzzle(6, 6);
    public void OnCreate7X7Puzzle() => GenerateAndPlacePuzzle(7, 7);

    private void GenerateAndPlacePuzzle(int width, int height)
    {
        // 기존 퍼즐 삭제
        DestroyAllChildren();

        // 경로 생성
        List<Vector2Int> path = GeneratePath(width, height);

        if (path != null)
        {
            PlaceTiles(path, width, height);
        }
        else
        {
            Debug.LogError("퍼즐 생성에 실패했습니다.");
        }
    }

    private List<Vector2Int> GeneratePath(int width, int height)
    {
        PathGenerator pathGenerator = new PathGenerator
        {
            width = width,
            height = height,
            minDistance = Mathf.Min(width, height) - 1,
            maxGenerationAttempts = maxGenerationAttempts
        };

        return pathGenerator.GeneratePath();
    }

    private void PlaceTiles(List<Vector2Int> path, int width, int height)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int pos = path[i];
            GameObject tile = Instantiate(tilePrefab, gridParent.transform);

            if (i == 0)
            {
                ChangeTileColor(tile, startColor);  // 시작 타일 색상 변경
            }
            else if (i == path.Count - 1)
            {
                ChangeTileColor(tile, endColor);  // 끝 타일 색상 변경
            }
        }
    }

    private void ChangeTileColor(GameObject tile, Color color)
    {
        SpriteRenderer sr = tile.transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }

    private void DestroyAllChildren()
    {
        int childCount = gridParent.transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Destroy(gridParent.transform.GetChild(i).gameObject);
        }
    }
}
