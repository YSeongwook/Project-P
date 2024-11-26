using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Camera particleCamera;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector3 currentPos = particleCamera.ScreenToWorldPoint(eventData.position);
        transform.position = currentPos;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget=true;
    }


}
