using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationPuzzle : MonoBehaviour , IPointerClickHandler
{
    private float rotationAngle = 90f;
    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.transform.Rotate(0, 0, -rotationAngle);
    }
}
