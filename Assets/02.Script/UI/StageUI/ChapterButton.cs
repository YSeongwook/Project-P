using DG.Tweening;
using UnityEngine;

public class ChapterButton : MonoBehaviour
{
    [SerializeField] private GameObject closeChapterBtn;
    public void OnClickOpenChapterButton()
    {
        gameObject.transform.DOLocalMove(new Vector3(540,0,0), 1f).SetEase(Ease.InOutQuad, 0.01f, 0.2f);
        closeChapterBtn.SetActive(true);
    }

    public void OnClickCloseChapterButton()
    {
        gameObject.transform.DOLocalMove(new Vector3(1341, 0, 0), 1f).SetEase(Ease.InOutQuad, 0.5f, 0.3f);
        closeChapterBtn.SetActive(false);
    }

}
