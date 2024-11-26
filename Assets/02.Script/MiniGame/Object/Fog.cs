using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Fog : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private Transform fogTransform;  // 안개 스프라이트 오브젝트의 Transform
    [SerializeField] private float shrinkAmount = 0.01f;  // 드래그할 때마다 줄어드는 scale 양
    [SerializeField] private float resetTime = 5.0f;  // 일정 시간 후 리셋될 시간
    [SerializeField] private float resetAfterReleaseTime = 3.0f;  // 손을 떼고 난 후 리셋될 시간

    private Vector3 originalScale;  // 초기 scale을 저장
    private Coroutine resetCoroutine;  // 리셋을 관리할 코루틴
    private Coroutine releaseResetCoroutine;  // 손을 떼고 나서 리셋을 관리할 코루틴

    private void Start()
    {
        // 안개의 초기 scale 저장
        originalScale = fogTransform.localScale;
        // 일정 시간 후 자동 리셋을 위한 코루틴 시작
        resetCoroutine = StartCoroutine(ResetFogAfterTime(resetTime));
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 손을 떼고 나서의 리셋 코루틴이 작동 중이면 중지
        if (releaseResetCoroutine != null)
        {
            StopCoroutine(releaseResetCoroutine);
            releaseResetCoroutine = null;
        }

        // 안개의 scale을 줄임
        Vector3 newScale = fogTransform.localScale - new Vector3(shrinkAmount, shrinkAmount, 0f);

        // scale이 0 이하가 되지 않도록 제한하고, 비활성화 처리
        if (newScale.x <= 0f || newScale.y <= 0f)
        {
            newScale = Vector3.zero;
            fogTransform.gameObject.SetActive(false);
            StopCoroutine(resetCoroutine);  // 안개가 다 사라졌으므로 자동 리셋 코루틴 중지
        }

        fogTransform.localScale = newScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 손을 떼면 3초 후 리셋하는 코루틴 시작
        releaseResetCoroutine = StartCoroutine(ResetFogAfterTime(resetAfterReleaseTime));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 손을 다시 터치하면 리셋 코루틴 중지
        if (releaseResetCoroutine != null)
        {
            StopCoroutine(releaseResetCoroutine);
            releaseResetCoroutine = null;
        }

        // 손을 다시 터치했을 때 자동 리셋 코루틴이 멈췄다면 다시 시작
        if (resetCoroutine == null)
        {
            resetCoroutine = StartCoroutine(ResetFogAfterTime(resetTime));
        }
    }

    private IEnumerator ResetFogAfterTime(float time)
    {
        // 주어진 시간 대기
        yield return new WaitForSeconds(time);

        // 안개 리셋
        ResetFog();
    }

    // 안개를 초기 상태로 되돌리는 메서드
    private void ResetFog()
    {
        fogTransform.localScale = originalScale;
        fogTransform.gameObject.SetActive(true);
        // 일정 시간 후 자동 리셋 코루틴을 재시작
        resetCoroutine = StartCoroutine(ResetFogAfterTime(resetTime));
    }
}
