using DG.Tweening;
using UnityEngine;

public class DotweenEffect : MonoBehaviour
{
    private DOTweenAnimation tween;

    private void Awake()
    {
        tween = GetComponent<DOTweenAnimation>();
    }

    private void OnEnable()
    {
        tween.DORestart();

        tween.tween.SetAutoKill(false);

        tween.tween.OnComplete(OnScaleComplete);
    }

    private void OnScaleComplete()
    {
        transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
}
