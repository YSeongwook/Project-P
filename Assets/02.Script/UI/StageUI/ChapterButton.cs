using DG.Tweening;
using UnityEngine;

public class ChapterButton : MonoBehaviour
{
    [SerializeField] private GameObject closeChapterBtn;
    [SerializeField] private GameObject chapter1Stage;
    [SerializeField] private GameObject chapter2Stage;
    [SerializeField] private GameObject chapter3Stage;
    [SerializeField] private GameObject chapter4Stage;
    
    public void OnClickOpenChapterButton()
    {
        gameObject.transform.DOLocalMove(new Vector3(540,0,0), 1f).SetEase(Ease.InOutQuad, 0.01f, 0.2f);
        closeChapterBtn.SetActive(true);
    }

    public void OnClickCloseChapterButton()
    {
        gameObject.transform.DOLocalMove(new Vector3(1315, 0, 0), 1f)
            .SetEase(Ease.InOutQuad, 0.5f, 0.3f)
            .OnComplete(() => closeChapterBtn.SetActive(false));  // 애니메이션 완료 후 closeChapterBtn 비활성화
    }

    public void OnClickOpenChapter1()
    {
        chapter1Stage.SetActive(true);
    }
}
