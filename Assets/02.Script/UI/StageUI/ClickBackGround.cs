using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickBackGround : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject chapter;
    [SerializeField] private GameObject closeChapterBtn;

    // 챕터 화면이 열려있을때 배경을 클릭시 챕터 화면이 닫힘
    public void OnPointerClick(PointerEventData eventdata)
    {
        chapter.transform.DOLocalMove(new Vector3(1315, 0, 0), 1f).SetEase(Ease.InOutQuad, 0.5f, 0.3f);
        closeChapterBtn.SetActive(false);
    }
}
