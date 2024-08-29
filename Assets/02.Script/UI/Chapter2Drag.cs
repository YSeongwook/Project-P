using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chapter2Drag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public Camera ParticleCamera;
    public Collider2D bladeCollider;
    private bool slicing;
    public float minSliceVelocity = 0.01f;
    public GameObject lineObject;
    private TrailRenderer trailRenderer;

    private Canvas canvas;

    public Vector3 direction { get; private set; }

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        trailRenderer = lineObject.GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        StopSlicing();
    }

    private void Start()
    {
        ParticleCamera.GetComponent<RectTransform>().position = canvas.GetComponent<RectTransform>().position + new Vector3(0,0,-30f);
        AdjustCameraToCanvas();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Drag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartSlicing(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopSlicing();
    }

    private void StartSlicing(PointerEventData eventData)
    {
        UpdateBladePosition(eventData);

        slicing = true;
        bladeCollider.enabled = true;
        trailRenderer.enabled = true;
        trailRenderer.Clear();
    }

    private void StopSlicing()
    {
        slicing = false;
        bladeCollider.enabled = false;
        trailRenderer.enabled = false;
    }

    private void Drag(PointerEventData eventData)
    {
        UpdateBladePosition(eventData);

        direction = bladeCollider.transform.position - transform.position;

        float Velocity = direction.magnitude / Time.deltaTime;
        bladeCollider.enabled = Velocity > minSliceVelocity;
    }

    private void UpdateBladePosition(PointerEventData eventData)
    {
        bladeCollider.transform.position = eventData.position;
        //lineObject.transform.position = bladeCollider.transform.position + new Vector3(0,0,-0.1f);
        lineObject.transform.position = bladeCollider.transform.position + new Vector3(0,0,-10f);
    }
    void AdjustCameraToCanvas()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float aspectRatio = (float)Screen.width / (float)Screen.height;

        // 적절한 Orthographic Size 설정
        ParticleCamera.orthographicSize = canvasRect.rect.height / 2f * canvasRect.localScale.x;

        // 카메라의 너비 조정 (선택 사항)
        ParticleCamera.aspect = aspectRatio;
    }

}
