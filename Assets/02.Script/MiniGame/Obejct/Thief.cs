using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Thief : MonoBehaviour, IPointerClickHandler
{
    private Image image;
    public bool isAppear {  get; private set; }

    private float Timer;

    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
    }

    private void Start()
    {
        Caught();
    }

    IEnumerator Appering()
    {
        while (isAppear)
        {
            yield return null;

            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                Caught();
                continue;
            }
        }

        yield break;
    }

    public void AppearThief()
    {
        image.enabled = true;
        isAppear = true;

        Timer = Random.Range(0.5f, 2f);

        StartCoroutine(Appering());
    }

    public void Caught()
    {
        image.enabled = false;
        isAppear = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(isAppear)
        {
            Caught();
            EventManager<MiniGame>.TriggerEvent(MiniGame.Catch);
        }
        else
        {
            EventManager<MiniGame>.TriggerEvent(MiniGame.PoliceGameOver);
        }
    }
}
