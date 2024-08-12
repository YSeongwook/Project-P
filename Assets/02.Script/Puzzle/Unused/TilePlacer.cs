using System.Collections.Generic;
using UnityEngine;

public class TilePlacer : MonoBehaviour
{
    public GameObject gridParent;
    public GameObject tilePrefab;
    public Color startColor = Color.green;
    public Color endColor = Color.red;

    private PathGenerator pathGenerator;

    private void Start()
    {
        pathGenerator = GetComponent<PathGenerator>();
        List<Vector2Int> path = pathGenerator.GeneratePath();

        if (path != null)
        {
            DebugLogger.Log("PlaceTiles");
            PlaceTiles(path);
        }
        else
        {
            Debug.LogError("경로 생성에 실패했습니다.");
        }
    }

    private void PlaceTiles(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int pos = path[i];
            GameObject tile = Instantiate(tilePrefab, gridParent.transform);

            if (i == 0)
            {
                ChangeTileColor(tile, startColor); // 시작 타일 색상 변경
            }
            else if (i == path.Count - 1)
            {
                ChangeTileColor(tile, endColor); // 끝 타일 색상 변경
            }
        }
    }

    private void ChangeTileColor(GameObject tile, Color color)
    {
        DebugLogger.Log("ChangeTileColor");
        SpriteRenderer sr = tile.transform.GetChild((0)).GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = color;
        }
    }
}