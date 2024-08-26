using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TreeEditor;
using System;
using EventLibrary;
using EnumTypes;

public class DynamicObjectSelector : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float selectionThreshold = 100f; // 중앙으로부터 얼마나 가까워야 선택되는지 결정

    private List<RectTransform> itemRects = new List<RectTransform>();
    private RectTransform lastSelectedObject;

    private Transform Panel;

    private void Awake()
    {
        Panel = transform.parent;

        EventManager<UIEvents>.StartListening<int>(UIEvents.ChangeScrollViewCenter, ScrollToStage);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening<int>(UIEvents.ChangeScrollViewCenter, ScrollToStage);
    }

    private void Start()
    {
        Panel.gameObject.SetActive(false);
    }

    public void SetUpItems(List<RectTransform> items)
    {
        itemRects = items;
    }

    void Update()
    {
        CheckForSelectedObject();
    }

    private void CheckForSelectedObject()
    {
        // 스크롤뷰의 중앙 위치 계산
        Vector2 scrollViewCenter = scrollRect.viewport.rect.center;

        float closestDistance = float.MaxValue;
        RectTransform closestItem = null;

        foreach (var item in itemRects)
        {
            // 오브젝트의 위치를 월드 좌표에서 뷰포트 좌표로 변환
            Vector2 viewportPosition = scrollRect.viewport.InverseTransformPoint(item.position);

            // 스크롤뷰의 중앙과 현재 오브젝트의 거리 계산
            float distance = Vector2.Distance(viewportPosition, scrollViewCenter);

            // 가장 가까운 오브젝트 찾기
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestItem = item;
            }
        }

        // 선택된 오브젝트 처리
        if (closestItem != null && closestDistance < selectionThreshold)
        {
            HandleSelectedObject(closestItem);
        }
    }

    private void HandleSelectedObject(RectTransform selectedObject)
    {
        if (lastSelectedObject != null && lastSelectedObject == selectedObject)
        {
            return;
        }

        if (lastSelectedObject != null)
        {
            lastSelectedObject.DOScale(1f, 0.3f); // 이전 오브젝트 크기를 원래대로
        }

        lastSelectedObject = selectedObject;

        // 선택된 오브젝트 크기 1.5배로 애니메이션 처리
        selectedObject.DOScale(1.5f, 0.3f).SetEase(Ease.OutBounce);
    }

    private void ScrollToStage(int stage)
    {
        var Y_pos = (stage - 1) * 400 * -1;
        Vector2 newContentPosition = new Vector2(0, Y_pos);

        // 콘텐츠 위치 업데이트
        scrollRect.content.anchoredPosition = newContentPosition;
    } 
}
