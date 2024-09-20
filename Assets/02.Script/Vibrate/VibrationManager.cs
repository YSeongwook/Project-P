using EnumTypes;
using EventLibrary;
using VibrationUtility;
// 진동 유틸리티 네임스페이스 추가

public class VibrationManager : Singleton<VibrationManager>
{
    protected override void Awake()
    {
        base.Awake();
        VibrationUtil.Init(); // 진동 유틸리티 초기화
        RegisterEvents(); // 이벤트 등록
    }
    
    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void RegisterEvents()
    {
        // 이벤트 리스너 등록
        EventManager<VibrateEvents>.StartListening(VibrateEvents.ShortWeak, VibrateShortWeak);
        EventManager<VibrateEvents>.StartListening(VibrateEvents.ShortStrong, VibrateShortStrong);
        EventManager<VibrateEvents>.StartListening(VibrateEvents.LongWeak, VibrateLongWeak);
        EventManager<VibrateEvents>.StartListening(VibrateEvents.LongStrong, VibrateLongStrong);
    }

    private void RemoveEvents()
    {
        // 이벤트 리스너 해제
        EventManager<VibrateEvents>.StopListening(VibrateEvents.ShortWeak, VibrateShortWeak);
        EventManager<VibrateEvents>.StopListening(VibrateEvents.ShortStrong, VibrateShortStrong);
        EventManager<VibrateEvents>.StopListening(VibrateEvents.LongWeak, VibrateLongWeak);
        EventManager<VibrateEvents>.StopListening(VibrateEvents.LongStrong, VibrateLongStrong); 
    }

    // 짧고 약한 진동
    private void VibrateShortWeak()
    {
        VibrationUtil.Vibrate(VibrationType.Light, 0.5f); // 0.5f 진동 강도
    }

    // 짧고 강한 진동
    private void VibrateShortStrong()
    {
        VibrationUtil.Vibrate(VibrationType.Heavy, 1.0f); // 최대 강도
    }

    // 길고 약한 진동
    private void VibrateLongWeak()
    {
        VibrationUtil.VibrateCustomized(new long[] { 0, 500 }, new int[] { 0, 50 }); // 0.5초 진동, 약한 강도
    }

    // 길고 강한 진동
    private void VibrateLongStrong()
    {
        VibrationUtil.VibrateCustomized(new long[] { 0, 500 }, new int[] { 0, 200 }); // 0.5초 진동, 강한 강도
    }
}
