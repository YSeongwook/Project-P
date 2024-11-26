using System;
using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.EventSystems;

public class Feed : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform cowRectTransform; // 소의 RectTransform
    
    public bool IsClearAble {  get; private set; }
    
    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _originalPosition;
    private bool _isGameStart;

    private void Awake()
    {
        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _originalPosition = _rectTransform.anchoredPosition;
    }
    
    private void OnEnable()
    {
        IsClearAble = false;
        _isGameStart = false;
    }

    private void OnDestroy()
    {
        EventManager<MiniGame>.StopListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }
    
    public void SetClearAble(bool setAble)
    {
        IsClearAble = setAble;

        DebugLogger.Log($"{gameObject.name} : {IsClearAble}");
    }

    private void SetGameStart(bool isGameStart)
    {
        if (!isGameStart) return;

        this._isGameStart = isGameStart;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시
        
        if (!_isGameStart) return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isGameStart) return;
        
        // 드래그 중
        if (_canvas == null) return;

        _rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 드래그 종료 시 소와 겹치는지 확인
        if (RectTransformUtility.RectangleContainsScreenPoint(cowRectTransform, eventData.position, _canvas.worldCamera))
        {
            // 소에게 여물을 준 것으로 처리
            EventManager<MiniGame>.TriggerEvent(MiniGame.FeedCountChanged);
            DebugLogger.Log("소에게 여물을 주었습니다!");
            // 여물 오브젝트를 비활성화하거나 제거
            gameObject.SetActive(false);          
        }
        else
        {
            // 원래 위치로 되돌림
            _rectTransform.anchoredPosition = _originalPosition;
        }
    }
}