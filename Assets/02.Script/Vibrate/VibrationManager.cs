using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    // 특정 시간과 강도로 진동 발생
    public void Vibrate(long milliseconds, int amplitude)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

            if (vibrator.Call<bool>("hasVibrator"))
            {
                // Android OS 버전 체크
                AndroidJavaClass androidOsBuildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                int sdkInt = androidOsBuildVersion.GetStatic<int>("SDK_INT");

                if (sdkInt >= 26) // Android API 26 이상
                {
                    AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                    AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude);
                    vibrator.Call("vibrate", vibrationEffect);
                }
                else // Android API 26 이하
                {
                    vibrator.Call("vibrate", milliseconds);
                }
            }
        }
        else
        {
            Handheld.Vibrate(); // 다른 플랫폼에서는 기본 진동
        }
    }
}