using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private CustomGridLayOut customGridLayOut;
    [SerializeField] private GameObject stagePrefab;
    [SerializeField] private Transform contentTransform;

    private GameObject[] stages;

    private void Start()
    {
        SetUpStages(1);
    }

   public void SetUpStages(int chapter)
    {
        int stageCount = GetStageCountForChapter(chapter);

        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }

        stages = new GameObject[stageCount];

        for (int i = 0; i < stageCount; i++)
        {
            stages[i] = Instantiate(stagePrefab, contentTransform);
            customGridLayOut.AddElement();
            Transform childTransform = stages[i].transform.Find("Text_StageCountTitle");
            TextMeshProUGUI stageText = childTransform.GetComponent<TextMeshProUGUI>();
            if (stageText != null)
            {
                stageText.text = (i + 1).ToString();
            }
        }
    }

    private int GetStageCountForChapter(int chapter)
    {
        switch (chapter)
        {
            case 1:
                return 20;
            case 2:
                return 7;
            case 3:
                return 7;
            default:
                return 7;
        }
    }
}
