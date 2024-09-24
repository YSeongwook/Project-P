using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropHandler : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Camera ParticleCamera;

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        Vector3 currentPos = ParticleCamera.ScreenToWorldPoint(eventData.position);
        this.transform.position = currentPos;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<Image>().raycastTarget=true;
    }


}
