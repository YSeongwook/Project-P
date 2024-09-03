using EnumTypes;
using EventLibrary;
using UnityEngine;
using DG.Tweening;

public class GimmickAnimation : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 2f;
    [SerializeField] private Vector3 rotationAngle = new Vector3 (0, 0, 360);
    [SerializeField] private float scaleDuration = 1f;
    [SerializeField] private Vector3 minScale = Vector3.one * 0.7f;

    private GimmickShape shape;
    private RectTransform rectTransform;
    private Vector2 Size;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetScale(float scale)
    {
        Size = new Vector2(scale, scale);
    }

    public void GetGimmickShape(GimmickShape shape)
    {
        this.shape = shape;
    }

    public void StartGimmickAnimation()
    {
        switch(shape)
        {
            case GimmickShape.Warp:
                WarpAnimation();
                break;
            case GimmickShape.Link:
                LinkAnimation(); 
                break;
        }
    }

    private void WarpAnimation()
    {
        // 무한 루프 돌면서 Z축으로 회전 (2초 동안 360도 회전)
        rectTransform.DORotate(rotationAngle, rotationDuration, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        // 무한 루프 돌면서 크기 변경 (최소 크기 -> 최대 크기 -> 최소 크기)
        rectTransform.DOScale(minScale, scaleDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo); // 무한 반복
    }

    private void LinkAnimation()
    {
        rectTransform.anchorMin = new Vector2(1, 0);
        rectTransform.anchorMax = new Vector2(1, 0);

        rectTransform.pivot = new Vector2(1, 0);
        rectTransform.sizeDelta = Size / 4;
    }
}
