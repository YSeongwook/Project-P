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

    // �α����� �ʿ��Ҷ� (��ū�� ������) �α��� ��ư�� ����
    public void OnLoginBtn(string str)
    {
        LoginBtn.SetActive(true);
    }

    // ù �α����� �Ϸ�Ǹ� �����ִ� �α��� ��ư�� ����
    public void OffLoginBtn(string str)
    {
        LoginBtn.SetActive(false);
    }
}
