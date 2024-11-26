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

    // Kotlin���� ��ȯ�� ģ�� ����� ���� �޼���
    public void OnGetFriendsListResult(string friendsList)
    {
        //Debug.Log("Received friends list from Kotlin: " + friendsList);
        text.text = $"Received friends list from Kotlin: \n{friendsList}";
        // �̰����� ���� friendsList�� ���ϴ� ��� ó���ϸ� �˴ϴ�.
    }
}
