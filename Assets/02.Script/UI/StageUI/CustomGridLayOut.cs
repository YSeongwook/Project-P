using UnityEngine;

public class CustomGridLayOut : MonoBehaviour
{
    public float spacing = 100f;
    public Vector2 startPosition = new Vector2(10f, 10f);
    public Vector2 elementSize = new Vector2(100f, 100f);
    private int _currentIndex = 0;

    public void AddElement(GameObject element)
    {
        Vector2 position = CalculatePosition(_currentIndex);
        element.transform.localPosition = position;
        element.transform.SetParent(transform, false);
        _currentIndex++;
    }

    private Vector2 CalculatePosition(int index)
    {
        int groupIndex = index / 4; // 그룹 인덱스: 0은 첫 번째 그룹, 1은 두 번째 그룹 등
        int inGroupIndex = index % 4; // 그룹 내에서 요소의 위치

        float x = 0f, y = 0f;

        if (index < 4)
        {
            // 첫 번째 그룹: 오른쪽 대각선으로 나열
            x = startPosition.x + (elementSize.x + spacing) * inGroupIndex;
            y = startPosition.y + (elementSize.y + spacing) * inGroupIndex;
        }
        else
        {
            // 이후 그룹들: 3개씩 번갈아가며 대각선으로 나열
            int adjustedIndex = index - 4;
            int inAdjustedGroupIndex = adjustedIndex % 3;

            float offsetY;

            if (index < 8)
            {

                // index가 4보다 크고 8보다 작은 경우
                offsetY = (elementSize.y + spacing) * 4 * groupIndex;
            }
            else
            {
                // index가 8보다 큰 경우
                offsetY = (elementSize.y + spacing) * 3 * groupIndex;
            }

            if (groupIndex % 2 == 0)
            {
                // 오른쪽 대각선
                x = startPosition.x + (elementSize.x + spacing) * (1 +inAdjustedGroupIndex);
                y = startPosition.y + offsetY + (elementSize.y + spacing) * inAdjustedGroupIndex;
            }
            else
            {
                // 왼쪽 대각선
                x = startPosition.x + (elementSize.x + spacing) * (2 - inAdjustedGroupIndex);
                y = startPosition.y + offsetY + (elementSize.y + spacing) * inAdjustedGroupIndex;
            }
        }

        return new Vector2(x, y);
    }
}
