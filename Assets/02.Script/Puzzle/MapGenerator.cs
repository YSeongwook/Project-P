using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [FoldoutGroup("Canvas")][SerializeField] private Canvas _MainMenuUI;
    [FoldoutGroup("Canvas")][SerializeField] private Canvas _PlayerGoldUI;
    [FoldoutGroup("Canvas")][SerializeField] private GameObject _missionSuccess;
    [FoldoutGroup("Canvas")][SerializeField] private GameObject _missionFail;

    [FoldoutGroup("Tile")][SerializeField] private GameObject _tileNode;
    [FoldoutGroup("Tile")][SerializeField] private float _tileSize;

    [FoldoutGroup("Tile Sprite")]
    [SerializeField] private List<Sprite> RoadList;
    [FoldoutGroup("Tile Sprite")]
    [SerializeField] private List<Sprite> GimmickList;

    private Canvas _canvas;
    private List<Tile> _tileList = new List<Tile>();
    private RectTransform _rectTransform;
    private GridLayoutGroup _grid;
    private int _limitCount;
    
    private bool _check; // 정답 체크 변수
    private int currentChapter;
    private int currentStage;

    private Temp2 temp; // 디버거

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _grid = GetComponentInChildren<GridLayoutGroup>();
        temp = new Temp2();

        AddEvents();
    }

    private void Start()
    {
        _canvas.enabled = false;
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
        EventManager<UIEvents>.StartListening(UIEvents.MissionSuccessPopUp, PopUpMissionSuccess);
        EventManager<DataEvents>.StartListening(DataEvents.DecreaseLimitCount, DecreaseLimitCount);
        temp.SetTileGridEvent(true);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StopListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<DataEvents>.StopListening<RectTransform, TileNode>(DataEvents.SetTileGrid, SetTileMapPositionGrid);
        EventManager<UIEvents>.StopListening(UIEvents.MissionSuccessPopUp, PopUpMissionSuccess);
        EventManager<DataEvents>.StopListening(DataEvents.DecreaseLimitCount, DecreaseLimitCount);
        temp.SetTileGridEvent(false);
    }

    //스테이지 열기
    private void OpenNewStage(int chapter, int stage)
    {
        // 입장권 티켓 감소
        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickUseTicket);

        DestroyAllTile();
        _check = false;

        string fileName = $"{chapter}-{stage}";
        _tileList = new List<Tile>(DataManager.Instance.GetPuzzleTileMap(fileName));
        if (_tileList == null )
        {
            DebugLogger.Log("생성되어 있는 미로가 존재하지 않습니다.");
            return;
        }

        currentChapter = chapter;
        currentStage = stage;

        // UI들 비활성화
        _MainMenuUI.enabled = false;
        _PlayerGoldUI.enabled = false;

        // 캔버스 활성화
        _canvas.enabled =true;

        //맵 사이즈 결정
        DetectTileSize(_tileList.Count);
        //셀 사이즈 결정
        _grid.cellSize = new Vector2(_tileSize, _tileSize);

        // tileList의 제곱근 길이만큼 RectTransform 크기 설정
        float sizeValue = Mathf.Sqrt(_tileList.Count) * _tileSize;
        _rectTransform.sizeDelta = new Vector2(sizeValue, sizeValue);

        // 만약 랜덤으로 생성한 오브젝트가 정답과 동일하면 다시 랜덤
        bool isLoop = true;
        while (isLoop)
        {
            int index = 0;
            // tileList의 길이만큼 TileNode 생성
            foreach (var tile in _tileList)
            {
                index++;
                var newTile = Instantiate(_tileNode, _grid.transform);
                newTile.name = $"TileNode{index}";
                var tileNode = newTile.GetComponent<TileNode>();
                if (tileNode == null) continue;
                tileNode.SetTileNodeData(tile);
                int shapeRotation = (int)tile.RoadShape;
                if (shapeRotation <= 0)
                {
                    //shapeRotation = Random.Range(1, 5);
                    continue;
                }
                tileNode.SetTilImage(RoadList[shapeRotation - 1]);

                
            }

            isLoop = IsCorrectAnswer();
        }

        // 제한 횟수 수치 이벤트로 넘기기
        var tileMapTable = DataManager.Instance.GetTileMapTable($"M{chapter}{stage.ToString("000")}");
        _limitCount = tileMapTable.LimitCount;
        EventManager<StageEvent>.TriggerEvent(StageEvent.StartStage, _limitCount);

        // 플레이어의 보유 아이템 이벤트로 넘기기
    }



    // 다시하기 버튼 클릭
    public void OnClickReplay()
    {
        _missionFail.SetActive(false);
        OpenNewStage(currentChapter, currentStage);
    }

    // 메인 UI로 돌아가기
    public void OnClickClose()
    {
        // UI 변화
        _missionFail.SetActive(false);
        _missionSuccess.SetActive(false);
        _canvas.enabled = false;
        _MainMenuUI.enabled = true;
        _PlayerGoldUI.enabled = true;
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

    private bool IsCorrectAnswer()
    {
        int checking = 1;

        foreach (Transform child in transform)
        {
            var childTileNode = child.GetComponent<TileNode>();
            if (childTileNode == null) continue;
            if (childTileNode.GetTileInfo.Type == TileType.None) continue;

            int check = childTileNode.IsCorrect ? 1 : 0;

            checking *= check;
        }

        return checking == 1;
    }

    // 정답 확인
    private void CheckAnswer()
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.ResetTileGrid);

        _check = IsCorrectAnswer();

        // 정답이면
        if (_check)
        {
            // 플레이어 골드 증가
            var playerGold = PlayerInformation.Instance.PlayerViewModel.PlayerGold;
            int plusGold = currentChapter * 50;
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerGoldChanged, playerGold + plusGold);

            // 플레이어 해금 챕터 및 스테이지 증가
            EventManager<DataEvents>.TriggerEvent(DataEvents.UpdateCurrentChapterAndStage, currentChapter, currentStage);

            // 정답 애니메이션 연출 발생

            // 정답 UI Enable
            EventManager<UIEvents>.TriggerEvent(UIEvents.MissionSuccessPopUp);
            DebugLogger.Log("클리어");
            return;
        }
        
        if(_limitCount <= 0)
        {
            // 실패 UI 등장
            _missionFail.SetActive(true);
            DebugLogger.Log("실패");
        }

    }
        
    // 리스트 수에 맞추어 Tile의 크기 설정
    private void DetectTileSize(int listCount)
    {
        switch(Mathf.Sqrt(listCount))
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

    private void SetTileMapPositionGrid(RectTransform transform, TileNode tileNode)
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetTileGrid, transform, tileNode);
    }

    private void PopUpMissionSuccess()
    {
        // 정답 UI 등장
        _missionSuccess.SetActive(true);
    }

    private void DecreaseLimitCount()
    {
        _limitCount -= 1;

        DebugLogger.Log(_limitCount);
    }
}
