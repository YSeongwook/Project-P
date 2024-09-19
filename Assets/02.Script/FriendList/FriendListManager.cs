using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendListManager : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject FriendPrefab;

    [Header("Content")]
    public GameObject friendsContent; // Content 오브젝트

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            //GameObject newMessage = Instantiate(FriendPrefab, friendsContent.transform);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject newMessage = Instantiate(FriendPrefab, friendsContent.transform);
        }
    }
}
