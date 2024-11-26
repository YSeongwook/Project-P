using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI ��Ҹ� �ٷ�� ���� �ʿ�

public class AlertFadeOut : MonoBehaviour
{
    [Header ("Display Time")]
    public float displayDuration = 2.0f; // Alert â�� ǥ�õ� �ð�
    public float fadeDuration = 1.0f;    // Fade-out�� �ɸ��� �ð�

    [Header("Message Properties")]
    [SerializeField] private Text Text_Msg;
    private AndroidJavaObject _androidJavaObject;
    private CanvasGroup canvasGroup;
    // �ڷ�ƾ�� ������ ���� ����
    private Coroutine fadeOutCoroutine;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        _androidJavaObject = new AndroidJavaObject("com.unity3d.player.KakaoLogin");
    }

    public void OnAlertMsg(string msg)
    {
        // �����ų �޽��� ����
        Text_Msg.text = msg;

        // SetActive(true)�� �����Ϸ���.
        canvasGroup.alpha = 1;

        // ������ ���� ���̴� FadeOut �ڷ�ƾ�� ������ ����
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }

        // ���ο� �ڷ�ƾ ����
        fadeOutCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        // ���� �ð� ���
        yield return new WaitForSeconds(displayDuration);

        float startAlpha = canvasGroup.alpha;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            // ���İ��� ���������� ���̱�
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }

        // ���������� ���İ��� 0���� �����Ͽ� ������ �����ϰ� ����
        canvasGroup.alpha = 0;

        // �ʿ��� ���, Alert â�� ��Ȱ��ȭ�ϰų� ����
        //gameObject.SetActive(false);
    }
}
