using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotationTile : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private float rotationDuration = 0.5f; // 회전에 걸리는 시간
    
    private const float RotationAngle = 90f;
    private bool _isRotating = false; // 현재 회전중인지 확인하는 플래그 변수
    private Quaternion _targetRotation;

    [SerializeField]
    public List<Vector2Int> Entrances = new List<Vector2Int>(); // 타일의 입구 정보

    private void Start()
    {
        _targetRotation = transform.rotation;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isRotating) return; // 이미 회전 중인 타일의 경우 다시 클릭해도 회전 되지 않는다.

        Debug.Log("Tile clicked"); // 디버그 로그 추가
        _targetRotation *= Quaternion.Euler(0, 0, -RotationAngle);

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

    // 현재 회전 상태에 따라 입구 업데이트
    public void UpdateEntrances()
    {
        List<Vector2Int> updatedEntrances = new List<Vector2Int>();
        foreach (var entrance in Entrances)
        {
            Vector2Int updatedEntrance = RotateEntrance(entrance);
            updatedEntrances.Add(updatedEntrance);
        }
        Entrances = updatedEntrances;
    }

    private Vector2Int RotateEntrance(Vector2Int entrance)
    {
        switch (entrance)
        {
            case Vector2Int v when v == Vector2Int.up:
                return Vector2Int.right;
            case Vector2Int v when v == Vector2Int.right:
                return Vector2Int.down;
            case Vector2Int v when v == Vector2Int.down:
                return Vector2Int.left;
            case Vector2Int v when v == Vector2Int.left:
                return Vector2Int.up;
            default:
                return entrance;
        }
    }
}
