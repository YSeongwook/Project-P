using System.Collections;
using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public Canvas stageCanvas;
    public GameObject mapGridLayout;
    
    [FoldoutGroup("Tile")][SerializeField] private GameObject _tileNode;
    [FoldoutGroup("Tile")][SerializeField] private float _tileSize;

    [FoldoutGroup("Tile Sprite")] [SerializeField] private List<Sprite> roadList;
    [FoldoutGroup("Tile Sprite")] [SerializeField] private List<Sprite> gimmickList;

    private List<Tile> _tileList = new List<Tile>();
    private RectTransform _rectTransform;
    private GridLayoutGroup _grid;
    private int _limitCount;
    
    private int _currentChapter;
    private int _currentStage;

    private Temp2 temp; // 디버거

    private void Awake()
    {
        _rectTransform = mapGridLayout.GetComponent<RectTransform>();
        _grid = mapGridLayout.GetComponent<GridLayoutGroup>();
        temp = new Temp2();

        AddEvents();
    }

    private void Start()
    {
        stageCanvas.enabled = false;
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<DataEvents>.StartListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StartListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<DataEvents>.StartListening<RectTransform, TileNode>(DataEvents.SetTileGrid, SetTileMapPositionGrid);
        EventManager<DataEvents>.StartListening(DataEvents.DecreaseLimitCount, DecreaseLimitCount);
        EventManager<StageEvent>.StartListening(StageEvent.MissionSuccess, HandleCorrectAnswer);
        EventManager<StageEvent>.StartListening(StageEvent.CheckMissionFail, CheckMissionFail);
        temp.SetTileGridEvent(true);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StopListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<DataEvents>.StopListening<RectTransform, TileNode>(DataEvents.SetTileGrid, SetTileMapPositionGrid);
        EventManager<DataEvents>.StopListening(DataEvents.DecreaseLimitCount, DecreaseLimitCount);
        EventManager<StageEvent>.StopListening(StageEvent.MissionSuccess, HandleCorrectAnswer);
        EventManager<StageEvent>.StopListening(StageEvent.CheckMissionFail, CheckMissionFail);
        temp.SetTileGridEvent(false);
    }

    // 스테이지 열기
    private void OpenNewStage(int chapter, int stage)
    {
        DestroyAllTiles();
        InitializeStage(chapter, stage);

        bool isLoop = true;
        while (isLoop)
        {
            EventManager<StageEvent>.TriggerEvent(StageEvent.ResetTileGrid);
            GenerateTiles();
            isLoop = IsCorrectAnswer();
        }        
    }

    // 스테이지 초기화
    private void InitializeStage(int chapter, int stage)
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickUseTicket);
        EventManager<StageEvent>.TriggerEvent(StageEvent.EnterStage);
        
        string fileName = $"{chapter}-{stage}";
        var newTileList = DataManager.Instance.GetPuzzleTileMap(fileName);

        if (newTileList == default)
        {
            HandleError("업데이트 예정입니다.");
            return;
        }

        _tileList = new List<Tile>(newTileList);

        _currentChapter = chapter;
        _currentStage = stage;
        
        SetupGridSize();
    }

    // 에러 처리
    private void HandleError(string errorMessage)
    {
        EventManager<UIEvents>.TriggerEvent(UIEvents.GameMessagePopUp, errorMessage);
        DebugLogger.Log(errorMessage);
    }

    // 그리드 사이즈 설정
    private void SetupGridSize()
    {
        DetectTileSize(_tileList.Count);
        _grid.cellSize = new Vector2(_tileSize, _tileSize);

        float sizeValue = Mathf.Sqrt(_tileList.Count) * _tileSize;
        _rectTransform.sizeDelta = new Vector2(sizeValue, sizeValue);
    }

    // 타일 생성
    private void GenerateTiles()
    {
        int index = 0;
        foreach (var tile in _tileList)
        {
            index++;
            var newTile = Instantiate(_tileNode, _grid.transform);
            newTile.name = $"TileNode{index}";

            var tileNode = newTile.GetComponent<TileNode>();
            if (tileNode == null) continue;

            tileNode.SetTileNodeData(tile);

            int tileShape = (int)tile.RoadShape;
            if (tileShape <= 0)
            {
                tileShape = Random.Range(1, 5);
                var newTileInfo = new Tile { Type = TileType.Road, RoadShape = (RoadShape)tileShape, GimmickShape = GimmickShape.None };
                tileNode.SetTileNodeData(newTileInfo);
            }

            tileNode.SetTilImage(roadList[tileShape - 1]);
        }

        StartCoroutine(Dummy());
    }

    private IEnumerator Dummy()
    {
        yield return new WaitForEndOfFrame();

        var tileList = _grid.GetComponentsInChildren<TileNode>();

        foreach(var tile in tileList)
        {
            var tilePos = tile.GetComponent<RectTransform>().anchoredPosition;
            EventManager<StageEvent>.TriggerEvent(StageEvent.SetPathTileGridAdd, tilePos, tile);
        }

        FinalizeStage();
    }

    // 스테이지 생성 완료 후 처리
    private void FinalizeStage()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetPathEndPoint, _tileSize);

        var tileMapTable = DataManager.Instance.GetTileMapTable($"M{_currentChapter}{_currentStage.ToString("000")}");
        _limitCount = tileMapTable.LimitCount;
        EventManager<StageEvent>.TriggerEvent(StageEvent.StartStage, _limitCount);
    }

    // 타일 리셋
    private void DestroyAllTiles()
    {
        foreach (Transform child in _grid.GetComponent<Transform>())
        {
            Destroy(child.gameObject);
        }

        _tileList.Clear();
    }

    // 정답 여부 확인
    private bool IsCorrectAnswer()
    {
        foreach (Transform child in mapGridLayout.transform)
        {
            var childTileNode = child.GetComponent<TileNode>();
            if (childTileNode == null || childTileNode.GetTileInfo.Type == TileType.None) continue;
            if (!childTileNode.IsCorrect) return false;
        }
        return true;
    }

    // 정답 확인 처리
    private void CheckAnswer()
    {
        // 정답 확인 시 바로 클리어 되는 것이 아니라 연출 이후에 스테이지 클리어
        EventManager<StageEvent>.TriggerEvent(StageEvent.SortPathTileGrid);
    }

    // 미션 성공
    private void HandleCorrectAnswer()
    {
        float playerGold = PlayerInformation.Instance.PlayerViewModel.PlayerGold;
        float plusGold = _currentChapter * 50;
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerGoldChanged, playerGold + plusGold);

        EventManager<DataEvents>.TriggerEvent(DataEvents.UpdateCurrentChapterAndStage, _currentChapter, _currentStage);
        EventManager<StageEvent>.TriggerEvent(StageEvent.StageClear);

        DebugLogger.Log("클리어");
    }

    // 미션 실패
    private void CheckMissionFail()
    {
        if (_limitCount <= 0)
        {
            EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail);
            DebugLogger.Log("실패");
        }
    }

    // 제한 횟수 감소
    private void DecreaseLimitCount()
    {
        _limitCount -= 1;
        DebugLogger.Log(_limitCount);
    }

    // 타일 크기 설정
    private void DetectTileSize(int listCount)
    {
        switch (Mathf.Sqrt(listCount))
        {
            case 3:
                _tileSize = 320;
                break;
            case 4:
                _tileSize = 270;
                break;
            case 5:
                _tileSize = 220;
                break;
            case 6:
                _tileSize = 170;
                break;
            case 7:
                _tileSize = 120;
                break;
        }
    }

    // 타일 맵 위치 그리드 설정
    private void SetTileMapPositionGrid(RectTransform rectTransform, TileNode tileNode)
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetPathTileGridAdd, rectTransform, tileNode);
    }

    // 타일 리셋
    private void DestroyAllTile()
    {
        int childCount = transform.childCount;
        if (childCount == 0)
        {
            DebugLogger.Log("삭제할 타일이 없습니다.");
            return;
        }

        for(int i = childCount-1; i>=0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        _tileList.Clear();   // 모든 타일이 삭제되면 저장하고 있던 리스트 초기화
    }
}
