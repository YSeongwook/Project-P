using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 요소를 다루기 위해 필요

public class AlertFadeOut : MonoBehaviour
{
    [Header ("Display Time")]
    public float displayDuration = 2.0f; // Alert 창이 표시될 시간
    public float fadeDuration = 1.0f;    // Fade-out이 걸리는 시간

    [Header("Message Properties")]
    [SerializeField] private Text Text_Msg;
    private AndroidJavaObject _androidJavaObject;
    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        _androidJavaObject = new AndroidJavaObject("com.unity3d.player.KakaoLogin");
    }

    public void OnAlertMsg(string msg)
    {
        // 노출시킬 메시지 삽입
        Text_Msg.text = msg;

        // SetActive(true)로 시작하려고.
        canvasGroup.alpha = 1;

        // Alert 창을 일정 시간 후에 서서히 사라지게 하는 코루틴 시작
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // 일정 시간 대기
        yield return new WaitForSeconds(displayDuration);

        float startAlpha = canvasGroup.alpha;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // 알파값을 점차적으로 줄이기
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }

        // 최종적으로 알파값을 0으로 설정하여 완전히 투명하게 만듦
        canvasGroup.alpha = 0;

        // 필요한 경우, Alert 창을 비활성화하거나 제거
        gameObject.SetActive(false);
    }
}
