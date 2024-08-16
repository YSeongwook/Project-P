using UnityEngine;

public class CustomGridLayOut : MonoBehaviour
{
    [SerializeField] private GameObject nullImage;
    [SerializeField] private int columnCount = 4; // 한 줄에 들어가는 칸 수
    private int currentIndex = 0; // 현재 배치할 요소의 인덱스
    private int i = 0;
    public void AddElement()
    {
        // 좌표 계산
        int row = currentIndex / columnCount;//행
        int col = currentIndex % columnCount;//열 
        bool isreverse = (col == row - 2 * i && col != 3);

            if(isreverse)
            {
                for (int j = 0; j < 4; j++)
                {
                Instantiate(nullImage, transform);
                currentIndex++;
                }
            currentIndex++;
            }
            else
            {
                for(int j = 0; j < 2; j++)
                {
                Instantiate(nullImage, transform);
                currentIndex++;
                i++;
                }
            currentIndex++;
            }
       
    }
}
