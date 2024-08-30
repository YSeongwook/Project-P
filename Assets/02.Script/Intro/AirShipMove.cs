using UnityEngine;
using UnityEngine.Serialization;

public class AirShipMove : MonoBehaviour
{
    public Transform airShipPivot;

    [SerializeField] private Transform[] airShipPos;

    private int _currentPosIndex = 0; // 현재 위치 배열
    private float _airShipSpeed = 20f; // 비행선 속도
    private float _rotationSpeed = 1f; // 회전 속도 (Lerp에서 사용)
    private Quaternion _airShipDir; // 비행선 방향

    private void Start()
    {
        if (airShipPos.Length > 0)
        {
            UpdateRotation();
        }
    }

private void Update()
{
    if (airShipPos.Length == 0 || _currentPosIndex >= airShipPos.Length) return;

    // 목표 위치에 거의 도달했는지 먼저 확인하여 불필요한 계산 방지
    if ((airShipPivot.position - airShipPos[_currentPosIndex].position).sqrMagnitude < 1f * 1f)
    {
        _currentPosIndex++;
        if (_currentPosIndex < airShipPos.Length)
        {
            UpdateRotation();
        }
        return;
    }

    // 방향 및 위치 계산
    Vector3 direction = airShipPos[_currentPosIndex].position - airShipPivot.position;
    airShipPivot.position += direction.normalized * (_airShipSpeed * Time.deltaTime);

    // 방향을 부드럽게 회전
    airShipPivot.rotation = Quaternion.Slerp(airShipPivot.rotation, _airShipDir, _rotationSpeed * Time.deltaTime);
}

    private void UpdateRotation()
    {
        Vector3 direction = airShipPos[_currentPosIndex].position - airShipPivot.position;
        _airShipDir = Quaternion.LookRotation(direction);
    }
}
