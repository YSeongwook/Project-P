using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.EventSystems;

public class Chapter2Drag : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    private Camera ParticleCamera;
    public Collider2D bladeCollider;

    public float minSliceVelocity = 0.01f;
    public GameObject lineObject;
    private TrailRenderer trailRenderer;
    private Canvas canvas;

    public Vector3 direction { get; private set; }

    private bool isGameStart;

    private void Awake()
    {
        ParticleCamera = Camera.main;
        canvas = GetComponentInParent<Canvas>();
        trailRenderer = lineObject.GetComponent<TrailRenderer>();

        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnDestroy()
    {
        EventManager<MiniGame>.StopListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnDisable()
    {
        StopSlicing();
    }

    private void Start()
    {
        //ParticleCamera.GetComponent<RectTransform>().position = canvas.GetComponent<RectTransform>().position + new Vector3(0,0,-30f);
        ParticleCamera.transform.position = canvas.transform.position + new Vector3(0,0,-30f);
        //var mainCamera = Camera.main.GetComponent<UniversalAdditionalCameraData>();
        //if(mainCamera != null )
        //{
        //    mainCamera.cameraStack.Add(ParticleCamera);
        //}

        AdjustCameraToCanvas();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isGameStart) return;

        Drag(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isGameStart) return;

        StartSlicing(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isGameStart) return;

        StopSlicing();
    }

    private void StartSlicing(PointerEventData eventData)
    {
        UpdateBladePosition(eventData);

        bladeCollider.enabled = true;
        trailRenderer.enabled = true;
        trailRenderer.Clear();
    }

    private void StopSlicing()
    {
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

    void SetGameStart(bool isGameStart)
    {   
        this.isGameStart = isGameStart;
    }
}
