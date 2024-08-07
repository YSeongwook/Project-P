using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] private GameObject puzzlePrefab;
    
    private const int Puzzle3X3 = 9;
    private const int Puzzle4X4 = 16;
    private const int Puzzle5X5 = 25;
    private const int Puzzle6X6 = 36;
    private const int Puzzle7X7 = 49;

    public void OnCreate3X3Puzzle()
    {
        DestroyAllChildren();
        CreatePuzzle(Puzzle3X3);
    }

    public void OnCreate4X4Puzzle()
    {
        DestroyAllChildren();
        CreatePuzzle(Puzzle4X4);
    }

    public void OnCreate5X5Puzzle()
    {
        DestroyAllChildren();
        CreatePuzzle(Puzzle5X5);
    }

    public void OnCreate6X6Puzzle()
    {
        DestroyAllChildren();
        CreatePuzzle(Puzzle6X6);
    }

    public void OnCreate7X7Puzzle()
    {
        DestroyAllChildren();
        CreatePuzzle(Puzzle7X7);
    }

    private void DestroyAllChildren()
    {
        int childCount = transform.childCount;

        if (childCount > 0)
        {
            for (int i = childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
            }
        }
    }

    private void CreatePuzzle(int puzzleSize)
    {
        for (int i = 0; i < puzzleSize; i++)
        {
            GameObject puzzlePiece = Instantiate(puzzlePrefab, transform.position, transform.rotation);
            puzzlePiece.transform.SetParent(gameObject.transform);
        }
    }
}
