using System.Collections;
using UnityEngine;

public class RotationTile : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.5f; // 회전에 걸리는 시간

    private const float RotationAngle = -90f;  // 음수로 설정하여 시계 방향으로 회전
    private bool _isRotating = false; // 현재 회전중인지 확인하는 플래그 변수
    private int _rotateValue = 0;  // 회전값을 각도 단위로 누적

    private void Start()
    {
        _rotateValue = 0;  // 초기 회전 값을 0으로 설정
    }

    public void RotateTile()
    {
        if (_isRotating) return; // 이미 회전 중인 타일의 경우 다시 클릭해도 회전 되지 않는다.

        _rotateValue = (_rotateValue + 1) % 4;  // 90도씩 회전
        float targetAngle = _rotateValue * RotationAngle;  // 음수로 시계 방향 회전
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        StartCoroutine(RotateOverTime(targetRotation, rotationDuration));
    }

    public void RotateTile(int rotateValue)
    {
        _rotateValue = rotateValue % 4;
        float targetAngle = _rotateValue * RotationAngle;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        transform.rotation = targetRotation;  // 직접적으로 회전값을 반영
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