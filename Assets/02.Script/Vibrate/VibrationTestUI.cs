using UnityEngine;
using UnityEngine.UI;
using VibrationUtility;  // 진동 유틸리티 네임스페이스 추가

public class VibrationTestUI : MonoBehaviour
{
    public Button buttonShortWeak;
    public Button buttonShortStrong;
    public Button buttonLongWeak;
    public Button buttonLongStrong;

    private void Start()
    {
        // 진동 유틸리티 초기화
        VibrationUtil.Init();

        // 각 버튼에 다른 진동을 연결
        buttonShortWeak.onClick.AddListener(VibrateShortWeak);
        buttonShortStrong.onClick.AddListener(VibrateShortStrong);
        buttonLongWeak.onClick.AddListener(VibrateLongWeak);
        buttonLongStrong.onClick.AddListener(VibrateLongStrong);
    }

    private void VibrateShortWeak()
    {
        // 짧고 약한 진동 (Weak 진동 타입과 짧은 시간)
        VibrationUtil.Vibrate(VibrationType.Light, 0.5f);  // 0.5f는 진동 강도
        Debug.Log("짧고 약한 진동 발생!");
    }

    private void VibrateShortStrong()
    {
        // 짧고 강한 진동 (Heavy 진동 타입과 짧은 시간)
        VibrationUtil.Vibrate(VibrationType.Heavy, 1.0f);  // 1.0f는 최대 강도
        Debug.Log("짧고 강한 진동 발생!");
    }

    private void VibrateLongWeak()
    {
        // 길고 약한 진동 (Light 진동 타입과 긴 시간)
        VibrationUtil.VibrateCustomized(new long[] { 0, 500 }, new int[] { 0, 50 });  // 0.5초 진동, 약한 강도
        Debug.Log("길고 약한 진동 발생!");
    }

    private void VibrateLongStrong()
    {
        // 길고 강한 진동 (Heavy 진동 타입과 긴 시간)
        VibrationUtil.VibrateCustomized(new long[] { 0, 500 }, new int[] { 0, 200 });  // 0.5초 진동, 강한 강도
        Debug.Log("길고 강한 진동 발생!");
    }
}