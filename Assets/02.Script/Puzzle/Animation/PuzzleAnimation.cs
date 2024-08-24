using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Vector3 targetScale = new Vector3(1.5f, 1.5f, 1.5f);
    [SerializeField] private float delayBetweenAnimations = 0.3f; // 각 애니메이션 사이의 딜레이 추가

    private void Awake()
    {        
        EventManager<PuzzleEvent>.StartListening<List<TileNode>>(PuzzleEvent.StartClearAnimation, PuzzleClearAnimation);
    }

    private void OnDestroy()
    {
        EventManager<PuzzleEvent>.StopListening<List<TileNode>>(PuzzleEvent.StartClearAnimation, PuzzleClearAnimation);
    }

    private void PuzzleClearAnimation(List<TileNode> tileList)
    {
        StartCoroutine(PlayPuzzleClearAnimations(tileList));
    }

    private IEnumerator PlayPuzzleClearAnimations(List<TileNode> tileList)
    {
        foreach (var child in tileList)
        {
            var childRectTransform = child.transform.GetComponent<RectTransform>();
            StartCoroutine(PopTileNode(childRectTransform)); // 각 코루틴이 순차적으로 실행되도록 변경
            yield return new WaitForSeconds(delayBetweenAnimations); // 각 타일 애니메이션 사이에 딜레이 추가
        }
    }

    IEnumerator PopTileNode(RectTransform rectTransform)
    {
        float time = 0;

        Vector3 originalScale = rectTransform.localScale;

        while (time < duration)
        {
            rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        while (time < duration)
        {
            rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
