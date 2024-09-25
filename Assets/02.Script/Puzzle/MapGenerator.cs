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
    private List<TileNode> _pathTileList = new List<TileNode>();
    private RectTransform _rectTransform;
    private GridLayoutGroup _grid;
    private int _limitCount;
    
    private int _currentChapter;
    private int _currentStage;

    private bool _isTutorial;

    private PathFind checkPath; // 디버거

    private void Awake()
    {
        _rectTransform = mapGridLayout.GetComponent<RectTransform>();
        _grid = mapGridLayout.GetComponent<GridLayoutGroup>();
        checkPath = new PathFind();

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
        EventManager<DataEvents>.StartListening<int>(DataEvents.DecreaseLimitCount, LimitCountUpdate);
        EventManager<StageEvent>.StartListening(StageEvent.MissionSuccess, CheckMissionClear);
        EventManager<StageEvent>.StartListening(StageEvent.CheckMissionFail, CheckMissionFail);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickNextButton, StartNextStage);
        EventManager<UIEvents>.StartListening(UIEvents.OnClickRestartButton, ReStartCurrentStage);
        EventManager<StageEvent>.StartListening<bool>(StageEvent.TutorialStage, SetTutotialStage);
        EventManager<MiniGame>.StartListening(MiniGame.StartMiniGame, OpenMiniGame);
        checkPath.SetTileGridEvent(true);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StopListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<DataEvents>.StopListening<RectTransform, TileNode>(DataEvents.SetTileGrid, SetTileMapPositionGrid);
        EventManager<DataEvents>.StopListening<int>(DataEvents.DecreaseLimitCount, LimitCountUpdate);
        EventManager<StageEvent>.StopListening(StageEvent.MissionSuccess, CheckMissionClear);
        EventManager<StageEvent>.StopListening(StageEvent.CheckMissionFail, CheckMissionFail);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickNextButton, StartNextStage);
        EventManager<UIEvents>.StopListening(UIEvents.OnClickRestartButton, ReStartCurrentStage);
        EventManager<StageEvent>.StopListening<bool>(StageEvent.TutorialStage, SetTutotialStage);
        EventManager<MiniGame>.StopListening(MiniGame.StartMiniGame, OpenMiniGame);
        checkPath.SetTileGridEvent(false);
    }

    // 스테이지 열기
    private void OpenNewStage(int chapter, int stage)
    {
        DestroyAllTiles();
        InitializeStage(chapter, stage);
        LimitCountSet();

        bool isLoop = true;
        while (isLoop)
        {
            EventManager<StageEvent>.TriggerEvent(StageEvent.ResetTileGrid);
            GenerateTiles(); // 로비로 나간 후 동일한 스테이지 다시 선택 시 이 구문에서 에러 발생
            isLoop = IsCorrectAnswer();
        }        
    }

    // 스테이지 초기화
    private void InitializeStage(int chapter, int stage)
    {        
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

    // Todo: 4x4이면 타일 크기 줄이고, 7x7이면 타일 크기 키우기
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
        _pathTileList.Clear();

        foreach (var tile in _tileList)
        {
            index++;
            var newTile = Instantiate(_tileNode, _grid.transform);
            newTile.name = $"TileNode{index}";

            var tileNode = newTile.GetComponent<TileNode>();
            if (tileNode == null) continue;

            tileNode.Gimmick.SetScale(_tileSize);
            tileNode.SetTileNodeData(tile);

            int tileShape = (int)tile.RoadShape;
            if (tileShape > 0)
            {
                //tileShape = Random.Range(1, 5);
                //var newTileInfo = new Tile { Type = TileType.Road, RoadShape = (RoadShape)tileShape, GimmickShape = GimmickShape.None };
                //tileNode.SetTileNodeData(newTileInfo);

                tileNode.SetTileRoadImage(roadList[tileShape - 1]);
                _pathTileList.Add(tileNode);
            }

            int GimmickShape = (int)tile.GimmickShape;
            if (GimmickShape > 0)
            {
                tileNode.SetTileGimmickImage(gimmickList[GimmickShape - 1]);
            }
            
            // 시작 지점이거나 종료 지점인 경우 
            if (tileShape == 5)
            {
                tileNode.SetTileGimmickImage(gimmickList[gimmickList.Count - 2]);
                tileNode.SetStartEndSize();
                // todo: 크기 조절
            }
            else if (tileShape == 6)
            {
                tileNode.SetTileGimmickImage(gimmickList[gimmickList.Count - 1]);
                tileNode.SetStartEndSize();
            }
                
        }

        StartCoroutine(Dummy());
    }

    // 타일을 무작위로 회전시키되, 총 회전 수를 만족하도록 구현
    private void RandomlyRotateTilesForTotalRotations(int totalRotations, int currentRotationCount, List<TileNode> allTiles)
    {
        bool isLoop = true;
        while(isLoop)
        {
            int _currentRotationCount = currentRotationCount;
            while (_currentRotationCount < totalRotations)
            {
                // 모든 타일 중에서 랜덤한 타일을 선택
                var randomTile = allTiles[Random.Range(0, allTiles.Count)];

                var rotateValue = randomTile.GetTileInfo.RotateValue;
                rotateValue = (rotateValue + 1) % 4;

                if (randomTile.GetTileInfo.GimmickShape == GimmickShape.Link)
                {
                    EventManager<StageEvent>.TriggerEvent(StageEvent.SetRandomRotateLinkTile, rotateValue);
                }
                else
                {
                    // 90도씩 무작위 회전
                    randomTile.SetRandomTileRotate(rotateValue);
                }

                if (randomTile.GetTileInfo.RotateValue == 0)
                {
                    _currentRotationCount--;
                }
                else
                {
                    _currentRotationCount++;
                }
            }

            // Test
            isLoop = IsCorrectAnswer() && checkPath.GetRotationConditionSuccess(5, allTiles);
        }
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

        if(!_isTutorial)
            RandomlyRotateTilesForTotalRotations(_limitCount, 0, _pathTileList);
    }

    // 스테이지 생성 완료 후 처리
    private void FinalizeStage()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetPathEndPoint, _tileSize);
        EventManager<StageEvent>.TriggerEvent(StageEvent.StartStage, _limitCount);
    }

    // 회전 제한 수 설정
    private void LimitCountSet()
    {
        var tileMapTable = DataManager.Instance.GetTileMapTable($"M{_currentChapter}{_currentStage.ToString("000")}");
        _limitCount = tileMapTable.LimitCount;
    }

    // 타일 리셋
    private void DestroyAllTiles()
    {
        foreach (Transform child in _grid.transform)
        {
            Destroy(child.gameObject);
        }

        _tileList.Clear();
    }

    // 정답 여부 확인
    private bool IsCorrectAnswer()
    {
        bool isLoop = checkPath.TilePathFind();
        return isLoop;
    }

    // 정답 확인 처리
    private void CheckAnswer()
    {
        // 정답 확인 시 바로 클리어 되는 것이 아니라 연출 이후에 스테이지 클리어
        EventManager<StageEvent>.TriggerEvent(StageEvent.SortPathTileGrid);
    }

    // 미션 성공
    private void CheckMissionClear()
    {
        float playerGold = PlayerInformation.Instance.PlayerViewModel.PlayerGold;
        float plusGold = _currentChapter * 50;
        EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerGoldChanged, playerGold + plusGold);

        EventManager<DataEvents>.TriggerEvent(DataEvents.UpdateCurrentChapterAndStage, _currentChapter, _currentStage);
        EventManager<StageEvent>.TriggerEvent(StageEvent.StageClear, true);

        // 디버깅용
        // 클리어하면 입장권 1개 제공
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.RecoveryTicketCountAfterGameClear);

        DebugLogger.Log("클리어");
    }

    // 미션 실패
    private void CheckMissionFail()
    {
        if (_limitCount <= 0)
        {
            EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail, true);
            DebugLogger.Log("실패");
        }
    }

    // 제한 횟수 업데이트
    private void LimitCountUpdate(int ChangedCount)
    {
        _limitCount = ChangedCount;
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
                _tileSize = 250;
                break;
            case 5:
                _tileSize = 220;
                break;
            case 6:
                _tileSize = 170;
                break;
            case 7:
                _tileSize = 150;
                break;
        }
    }

    // 타일 맵 위치 그리드 설정
    private void SetTileMapPositionGrid(RectTransform rectTransform, TileNode tileNode)
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetPathTileGridAdd, rectTransform, tileNode);
    }
    
    public void StartNextStage()
    {
        if (PlayerInformation.Instance.PlayerViewModel.GameTickets <= 0) return;

        // 티켓 사용
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickUseTicket);

        EventManager<StageEvent>.TriggerEvent(StageEvent.StageClear, false);
        EventManager<StageEvent>.TriggerEvent(StageEvent.NextStage, _currentChapter, _currentStage);
    }

    public void ReStartCurrentStage()
    {
        if (PlayerInformation.Instance.PlayerViewModel.GameTickets <= 0) return;

        // 티켓 사용
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickUseTicket);

        EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail, false);
        OpenNewStage(_currentChapter, _currentStage);
    }

    private void SetTutotialStage(bool isTutorialStage)
    {
        _isTutorial = isTutorialStage;
    }

    private void OpenMiniGame()
    {
        EventManager<MiniGame>.TriggerEvent(MiniGame.ActiveMiniGame, _currentChapter, _currentStage);

        StartCoroutine(StartMiniGame());    
    }

    IEnumerator StartMiniGame()
    {
        yield return new WaitForSeconds(3f);

        EventManager<MiniGame>.TriggerEvent(MiniGame.SetStartTrigger, true);
    }
}
