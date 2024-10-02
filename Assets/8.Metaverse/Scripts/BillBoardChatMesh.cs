using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardChatMesh : MonoBehaviour
{
    [SerializeField] TextMesh TextMesh_Chat;

    private Transform _transformLocalPlayerCamera = null;

    public void ShowChatMsg(string msg)
    {
        var gObj = GameObject.Find("LocalPlayerCamera");
        if (gObj != null)
        {
            _transformLocalPlayerCamera = gObj.transform;
        }

        TextMesh_Chat.text = msg;
        this.gameObject.SetActive(true);
        StartCoroutine(CoDelayCloseChatMsg());
    }

    private IEnumerator CoDelayCloseChatMsg()
    {
        yield return new WaitForSeconds(5.0f);
        this.gameObject.SetActive(false);
    }


    private void LateUpdate()
    {
        if(_transformLocalPlayerCamera != null)
        {
            this.transform.LookAt(this.transform.position + (_transformLocalPlayerCamera.rotation * Vector3.forward));
        }
    }
}
