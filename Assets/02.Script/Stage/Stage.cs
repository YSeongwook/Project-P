using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    private int _chapter; 
    
    private Button _btnStage;
    private Image _lock;

    private void Awake()
    {
        _btnStage = GetComponent<Button>();
        _lock = transform.GetChild(1).GetComponent<Image>();
    }
    
    public void SetStageNumber(int chapter, int number)
    {
        this._chapter = chapter;
        stageNumber = number;
    }

    public void OnClickStageButton()
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.SelectStage, this._chapter,stageNumber);
    }     

    public void ButtonActivate(bool isEnable)
    {
        _btnStage.interactable = isEnable;
        _lock.enabled = !isEnable;
    }
}
