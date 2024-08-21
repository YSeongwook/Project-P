using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private int chapter;
    [SerializeField] private int StageNumber;

    public void SetStageNumber(int chapter, int number)
    {
        this.chapter = chapter;
        StageNumber = number;
    }

    public void OnClickStageButton()
    {
        EventManager<DataEvents>.TriggerEvent(DataEvents.SelectStage, this.chapter,StageNumber);
    }        
}
