using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KakaoSystem : MonoBehaviour
{
    private AndroidJavaObject _androidJavaObject;

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
}
