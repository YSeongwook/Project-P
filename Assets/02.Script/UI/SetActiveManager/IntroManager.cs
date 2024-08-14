using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;

public class IntroManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public GameObject ChapterStageCanvas;
    public void OnPointerClick(PointerEventData eventdata)
    {
        ChapterStageCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
}
