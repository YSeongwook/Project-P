using UnityEngine;

public class CustomGridLayOut : MonoBehaviour
{
    public int columnCount = 3;
    public float spacing = 100f;
    public Vector2 startPosition = new Vector2(10f, 10f);
    public Vector2 elementSize = new Vector2(100f, 100f);
    private int _currentIndex = 0;

    public void AddElement(GameObject element)
    {
        int row = _currentIndex / columnCount;//행
        int column = _currentIndex % columnCount;//열

        float x, y;
        bool isRightDiagonal = ShouldBeRightDiagonal(_currentIndex);

        if (isRightDiagonal)
        {
            // 오른쪽 대각선 방향 (startPoint + x값은 start포인트)에서 추가될때마다 열이 하나씩 추가
            x = startPosition.x + (elementSize.x + spacing) * column;//스타트 포인트에서 열의 위치
            y = startPosition.y - (elementSize.y + spacing) * row + (elementSize.y + spacing) * column / 2f;
        }
        else
        {
            // 왼쪽 대각선 방향  (StartPoint + x값은 최대열)에서 추가될때마다 열이 하나씩 감소
            x = startPosition.x + (elementSize.x + spacing) * (columnCount - column - 2); 
            y = startPosition.y + (elementSize.y + spacing) * row + (elementSize.y + spacing) * (columnCount + column - 1) / 2f;
        }

        element.transform.localPosition = new Vector2(x, y);
        element.transform.SetParent(transform, false);

        _currentIndex++;
    }

    private bool ShouldBeRightDiagonal(int index)
    {
        if (index < 4) // 1번째부터 4번째까지
        {
            return true;
        }
        else
        {
            int groupNumber = (index - 4) / 3; // 5번째부터 3개씩 그룹화
            return groupNumber % 2 == 1; // 홀수 그룹은 오른쪽, 짝수 그룹은 왼쪽
        }
    }
}
