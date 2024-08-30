using Org.BouncyCastle.X509;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Chapter2_MiniGame : MonoBehaviour
{
    [SerializeField] private Transform Rice;
    private List<GameObject> RiceObjects = new List<GameObject>();
    [SerializeField] private TMP_Text Text_timer;

    [SerializeField] private float SetTimer;
    private float timer;

    private bool isGameClear;

    private void OnEnable()
    {
        isGameClear = false;
        timer = SetTimer;
        RiceObjects.Clear();

        SetRiceObejct();
    }

    private void Start()
    {
        
        StartCoroutine(StartTimer());
    }

    private void SetRiceObejct()
    {
        foreach(Transform child in Rice)
        {
            RiceObjects.Add(child.gameObject);
        }
    }

    IEnumerator StartTimer()
    {
        while(true)
        {
            yield return null;
            timer -= Time.deltaTime;
            Text_timer.text = timer.ToString("0");

            isGameClear = CheckGameClear();

            if (isGameClear)
            {
                GameClear();
                yield break;
            }

            if (timer <= 0)
            {
                GameOver();
                yield break;
            }
        }
    }

    private void GameOver()
    {
        DebugLogger.Log($"Clear 실패");
    }

    private void GameClear()
    {
        // 디버그 용
        DebugLogger.Log($"게임 클리어 : {isGameClear}");
    }

    private bool CheckGameClear()
    {
        bool isClear = true;
        foreach(var rice in RiceObjects)
        {
            var Rice = rice.GetComponent<Rice>();
            if (Rice == null) continue;
            isClear = Rice.isSlices;
            if (!isClear) break;
        }

        return isClear;
    }
}
