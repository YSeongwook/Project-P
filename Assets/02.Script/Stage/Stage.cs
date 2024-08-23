using EnumTypes;
using EventLibrary;
using UnityEngine;

public class Stage : MonoBehaviour
{
    [SerializeField] private int stageNumber;
    private int _chapter; 

    public void SetStageNumber(int chapter, int number)
    {
        this._chapter = chapter;
        stageNumber = number;
    }

    public void OnClickStageButton()
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.SelectStage, this._chapter,stageNumber);
    }        
}
