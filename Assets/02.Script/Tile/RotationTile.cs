using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using EventLibrary;

public class RotationTile : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.5f; // 회전에 걸리는 시간

    private const float RotationAngle = -90f;  // 음수로 설정하여 시계 방향으로 회전
    private bool _isRotating = false; // 현재 회전중인지 확인하는 플래그 변수
    private int _rotateValue;  // 회전값을 각도 단위로 누적

    private Transform _roadImage;
    private Transform _HintImage;

    private readonly Queue<int> _rotationQueue = new Queue<int>();
    private int _currentRotation;

    private void Awake()
    {
        _roadImage = transform.GetChild(1);
        _HintImage = transform.GetChild(3);
    }

    private void Start()
    {
        _rotateValue = 0;  // 초기 회전 값을 0으로 설정
    }

    public void InitRotateTile(int rotateValue)
    {
        _rotateValue = rotateValue;
        float targetAngle = _rotateValue * RotationAngle;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
        _roadImage.rotation = targetRotation;   // 직접적으로 회전값을 반영 - 길
        _HintImage.rotation = targetRotation;   // - 힌트
    }

    public void RotateTile()
    {
        _rotateValue = (_rotateValue + 1) % 4;  // 90도씩 회전

        _rotationQueue.Enqueue(_rotateValue);

        if (_isRotating) return; // 이미 회전 중인 타일의 경우 다시 클릭해도 회전 되지 않는다.
        StartCoroutine(ProcessRotationQueue());
        //StartCoroutine(RotateOverTime(targetRotation, rotationDuration));
    }

    public void RotateTile(int rotateValue)
    {
        _rotateValue = rotateValue % 4;
        //_roadImage.rotation = targetRotation;  // 직접적으로 회전값을 반영
        _rotationQueue.Enqueue(_rotateValue);

        if (_isRotating) return; // 이미 회전 중인 타일의 경우 다시 클릭해도 회전 되지 않는다.
        StartCoroutine(ProcessRotationQueue());
        //StartCoroutine(RotateOverTime(targetRotation, rotationDuration));
    }

    public void RandomRotateTile(int rotateValue)
    {
        _rotateValue = rotateValue % 4;
        float targetAngle = _rotateValue * RotationAngle;  // 음수로 시계 방향 회전
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        _roadImage.rotation = targetRotation;
    }

    // 회전 딜레이 추가
    private IEnumerator ProcessRotationQueue()
    {
        while(_rotationQueue.Count > 0 )
        {
            _isRotating = true;
            _currentRotation = _rotationQueue.Dequeue();
            float targetAngle = _rotateValue * RotationAngle;  // 음수로 시계 방향 회전
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            DebugLogger.Log(targetRotation);
            yield return StartCoroutine(RotateOverTime(targetRotation, rotationDuration));
        }

        _isRotating = false;
        // MapGenerator의 CheckAnswer 이벤트 실행
        EventManager<DataEvents>.TriggerEvent(DataEvents.CheckAnswer);
    }

    // 회전 보간 메서드
    private IEnumerator RotateOverTime(Quaternion endRotation, float duration)
    {
        _isRotating = true;

        Quaternion startRotation = _roadImage.rotation;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            _roadImage.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _roadImage.rotation = endRotation;
        _isRotating = false;
    }
}