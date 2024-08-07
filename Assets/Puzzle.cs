using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private int puzzle3X3 = 9;
    private int puzzle4X4 = 16;
    private int puzzle5X5 = 25;
    private int puzzle6X6 = 36;
    private int puzzle7X7 = 49;

    [SerializeField]
    private GameObject puzzleprefab;

    public void OnCreate3X3Puzzle()
    {
        DestroyAllChildren();
        for (int i = 0; i < puzzle3X3; i++)
        {
            GameObject puzzlePiece = Instantiate(puzzleprefab, transform.position, transform.rotation);
            puzzlePiece.transform.SetParent(gameObject.transform);
        }
    }

    public void OnCreate4X4Puzzle()
    {
        DestroyAllChildren();
        for (int i = 0; i < puzzle4X4; i++)
        {
            GameObject puzzlePiece = Instantiate(puzzleprefab, transform.position, transform.rotation);
            puzzlePiece.transform.SetParent(gameObject.transform);
        }
    }

    public void OnCreate5X5Puzzle()
    {
        DestroyAllChildren();
        for (int i = 0; i < puzzle5X5; i++)
        {
            GameObject puzzlePiece = Instantiate(puzzleprefab, transform.position, transform.rotation);
            puzzlePiece.transform.SetParent(gameObject.transform);
        }
    }

    public void OnCreate6X6Puzzle()
    {
        DestroyAllChildren();
        for (int i = 0; i < puzzle6X6; i++)
        {
            GameObject puzzlePiece = Instantiate(puzzleprefab, transform.position, transform.rotation);
            puzzlePiece.transform.SetParent(gameObject.transform);
        }
    }

    public void OnCreate7X7Puzzle()
    {
        DestroyAllChildren();
        for (int i = 0; i < puzzle7X7; i++)
        {
            GameObject puzzlePiece = Instantiate(puzzleprefab, transform.position, transform.rotation);
            puzzlePiece.transform.SetParent(gameObject.transform);
        }
    }

    public void DestroyAllChildren()
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


}
