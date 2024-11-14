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
    private GameObject _effect;

    public bool isAppear {  get; private set; }

    private float Timer;

    private void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        _effect = transform.GetChild(2).gameObject;
    }

    private void Start()
    {
        Caught();
        _effect.gameObject.SetActive(false);
    }

    IEnumerator Appering()
    {
        while (isAppear)
        {
            yield return null;

            Timer -= Time.deltaTime;
            if(Timer <= 0)
            {
                image.enabled = false;
                isAppear = false;

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
        _effect.SetActive(true);
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
