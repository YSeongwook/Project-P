using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    private List<Tile> tileList;

    private void Awake()
    {
        EventManager<DataEvents>.StartListening<int, int>(DataEvents.SelectStage, OpenNewStage);
    }

    private void OnDestroy()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
    }

    private void OpenNewStage(int chapter, int stage)
    {
        string fileName = $"{chapter}-{stage}";
        tileList = DataManager.Instance.GetPuzzleTileMap(fileName);
        if(tileList == null )
        {
            DebugLogger.Log("생성되어 있는 미로가 존재하지 않습니다.");
            return;
        }


    }
}
