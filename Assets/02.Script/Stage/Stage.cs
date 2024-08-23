using EnumTypes;
using EventLibrary;
using Org.BouncyCastle.Asn1.BC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    private int chapter;
    [SerializeField] private int StageNumber;


    private Button Btn_Stage;
    private Image Lock;

    private void Awake()
    {
        Btn_Stage = GetComponent<Button>();
        Lock = transform.GetChild(0).GetComponent<Image>();
    }

    public void ButtonActivate(bool isEnable)
    {
        Btn_Stage.interactable = isEnable;
        Lock.enabled = !isEnable;
    }

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
