using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationPuzzle : MonoBehaviour , IPointerClickHandler
{ 
    [SerializeField] private float rotationDuration = 0.5f; // 회전에 걸리는 시간
    
    private const float RotationAngle = 90f;
   
    private bool _isRotating = false; // 현재 회전중인지 확인하는 플래그 변수
    private Quaternion _targetRotation;

    private void Start()
    {
        _targetRotation = transform.rotation;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        _targetRotation *= Quaternion.Euler(0, 0, -RotationAngle);
        
        if (_isRotating) return;
        StartCoroutine(RotateOverTime(_targetRotation, rotationDuration));
    }

    // 회전 보간 메서드
    private IEnumerator RotateOverTime(Quaternion endRotation, float duration)
    {
        _isRotating = true;

        Quaternion startRotation = transform.rotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRotation;
        _isRotating = false;
    }
}
