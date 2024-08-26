using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class AirShipMove : MonoBehaviour
{
    public Transform AirShipPivot;

    [SerializeField] private Transform[] airShipPos;

    private int currentPosIndex = 0; // 현재 위치 배열
    private float airShipSpeed = 20f; // 비행선 속도
    private float rotationSpeed = 1f; // 회전 속도 (Lerp에서 사용)
    private Quaternion airShipDir; // 비행선 방향

    private void Start()
    {
        if (airShipPos.Length > 0)
        {
            UpdateRotation();
        }
    }

    private void Update()
    {
        if (airShipPos.Length == 0 || currentPosIndex >= airShipPos.Length) return;

        Vector3 direction = airShipPos[currentPosIndex].position - AirShipPivot.position;
        AirShipPivot.position += direction.normalized * airShipSpeed * Time.deltaTime;

        // 방향을 부드럽게 회전 (Slerp 사용)
        AirShipPivot.rotation = Quaternion.Slerp(AirShipPivot.rotation, airShipDir, rotationSpeed * Time.deltaTime);

        // 목표 위치에 거의 도달했는지 확인
        if (Vector3.Distance(AirShipPivot.position, airShipPos[currentPosIndex].position) < 1f)
        {
            // 다음 목표 위치로 전환
            currentPosIndex++;

            if (currentPosIndex < airShipPos.Length)
            {
                UpdateRotation();
            }
        }
    }

    private void UpdateRotation()
    {
        Vector3 direction = airShipPos[currentPosIndex].position - AirShipPivot.position;
        airShipDir = Quaternion.LookRotation(direction);
    }
}
