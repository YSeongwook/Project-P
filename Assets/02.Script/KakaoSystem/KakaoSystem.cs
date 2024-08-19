using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KakaoSystem : MonoBehaviour
{
    private AndroidJavaObject _androidJavaObject;
    [SerializeField] private Text text;

    void Start()
    {
        _androidJavaObject = new AndroidJavaObject("com.unity3d.player.UKakao");
    }

    public void Login()
    {
        _androidJavaObject.Call("KakaoLogin");
    }

    public void GetUserData()
    {
        _androidJavaObject.Call("GetUserData");
    }

    public void GetFriendsList()
    {
        _androidJavaObject.Call("GetFriendsList");
    }

    // Kotlin에서 반환한 친구 목록을 받을 메서드
    public void OnGetFriendsListResult(string friendsList)
    {
        //Debug.Log("Received friends list from Kotlin: " + friendsList);
        text.text = $"Received friends list from Kotlin: \n{friendsList}";
        // 이곳에서 받은 friendsList를 원하는 대로 처리하면 됩니다.
    }
}
