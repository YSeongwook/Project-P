using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KakaoLogin : MonoBehaviour
{
    private AndroidJavaObject _androidJavaObject;
    [SerializeField] private GameObject LoginBtn;

    // Start is called before the first frame update
    void Start()
    {
        _androidJavaObject = new AndroidJavaObject("com.unity3d.player.KakaoLogin");
        _androidJavaObject.Call("checkKakaoLoginStatus");
    }

    public void Login()
    {
        _androidJavaObject.Call("KakaoLogin");
    }

    // 로그인이 필요할때 (토큰이 없을때) 로그인 버튼이 켜짐
    public void OnLoginBtn(string str)
    {
        LoginBtn.SetActive(true);
    }

    // 첫 로그인이 완료되면 켜져있던 로그인 버튼이 꺼짐
    public void OffLoginBtn(string str)
    {
        LoginBtn.SetActive(false);
    }
}
