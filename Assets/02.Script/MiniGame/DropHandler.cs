using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Camera particleCamera;

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 currentPos = particleCamera.ScreenToWorldPoint(eventData.position);
        transform.position = currentPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget=true;
    }
}
