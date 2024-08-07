using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationPuzzle : MonoBehaviour , IPointerClickHandler
{
    private float rotationAngle = 90f;

    private float rotationDuration = 0.5f;

    private bool isRotating = false;

    private Quaternion targetRotation;

    private void Start()
    {
        targetRotation = transform.rotation;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        targetRotation *= Quaternion.Euler(0, 0, -rotationAngle);
        StartCoroutine(RotateOverTime(targetRotation, rotationDuration));
    }

    private IEnumerator RotateOverTime(Quaternion endRotation, float duration)
    {
        isRotating = true;

        Quaternion startRotation = transform.rotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        isRotating = false;
    }
}
