using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntroManger : MonoBehaviour
{
    public TextMeshProUGUI Text_ClickToStart;

    void Start()
    {

    }

    private void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            gameObject.SetActive(false);
        }

        BlinkText();
    }

    private void BlinkText()
    {
        float alphaValue = Mathf.Lerp(1f, 0.3f, Mathf.PingPong(Time.time, 1));
        Color color = Text_ClickToStart.color;
        color.a = alphaValue;
        Text_ClickToStart.color = color;
    }
}
