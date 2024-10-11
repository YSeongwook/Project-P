using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBallonSlotUI : MonoBehaviour
{
    [SerializeField] Text Text_Speech;

    private Transform Transform_TargetObject;
    private Camera _cameraLocalPlayer;

    private float _ballonPosY = 50;

    private void Update()
    {
        if(Transform_TargetObject != null)
        {
            if(_cameraLocalPlayer != null)
            {
                var pos = _cameraLocalPlayer.WorldToScreenPoint(Transform_TargetObject.position);
                var rePos = new Vector3(pos.x, (pos.y + _ballonPosY), pos.z);
                this.transform.position = rePos;
            }
        }
    }

    public void SetSpeechText(Transform targetObj, string msg)
    {
        var cameraObj = GameObject.Find("LocalPlayerCamera");
        if(cameraObj != null)
        {
            _cameraLocalPlayer = cameraObj.GetComponent<Camera>();
        }

        Text_Speech.text = msg;
        Transform_TargetObject = targetObj;
        StartCoroutine(CoDelayCloseChatMsg());
    }

    private IEnumerator CoDelayCloseChatMsg()
    {
        yield return new WaitForSeconds(5.0f);
        DestroyImmediate(this.gameObject);
        // this.gameObject.SetActive(false);
    }

}
