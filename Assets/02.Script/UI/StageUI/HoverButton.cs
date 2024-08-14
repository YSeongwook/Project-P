using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private RectTransform imageRectTransform; //UI 이미지의 RectTransform
    private float scaleMultiplier = 1.2f; //오브젝트를 얼마나 크게 만드는지 설정
    private float duration = 0.2f; // 애니메이션이 얼마나 걸릴지 설정

    private Vector3 originalScale; // 원래 크기를 저장할 변수

    private void Start()
    {
        if (imageRectTransform == null)
        {
            imageRectTransform = GetComponent<RectTransform>();
        }
            originalScale = imageRectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //마우스가 오브젝트 위에 올라가면 크기를 확대
        imageRectTransform.DOScale(originalScale * scaleMultiplier, duration).SetEase(Ease.OutQuad);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(originalScale, duration)
            .SetEase(Ease.OutQuad);
    }

    public void OnDrag(PointerEventData eventData)
    {

    }
}
