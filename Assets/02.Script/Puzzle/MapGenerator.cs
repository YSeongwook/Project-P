using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [FoldoutGroup("Tile")] [SerializeField] private GameObject _tileNode;
    [FoldoutGroup("Tile")] [SerializeField] private float _tileSize;

    [FoldoutGroup("Tile Sprite")] [SerializeField] private List<Sprite> roadList;
    [FoldoutGroup("Tile Sprite")] [SerializeField] private List<Sprite> gimmickList;

    private List<Tile> _tileList = new List<Tile>();
    private RectTransform _rectTransform;
    private GridLayoutGroup _grid;
    private int _limitCount;

    private bool _check; // 정답 체크 변수
    private int _currentChapter;
    private int _currentStage;

    private Temp2 temp; // 디버거

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _grid = GetComponent<GridLayoutGroup>();
        temp = new Temp2();

        AddEvents();
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<DataEvents>.StartListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StartListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<DataEvents>.StartListening<RectTransform, TileNode>(DataEvents.SetTileGrid,
            SetTileMapPositionGrid);
        temp.SetTileGridEvent(true);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StopListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<DataEvents>.StopListening<RectTransform, TileNode>(DataEvents.SetTileGrid, SetTileMapPositionGrid);
        temp.SetTileGridEvent(false);
    }

    //스테이지 열기
    private void OpenNewStage(int chapter, int stage)
    {
        string fileName = $"{chapter}-{stage}";
        _tileList = new List<Tile>(DataManager.Instance.GetPuzzleTileMap(fileName));
        if (_tileList == null)
        {
            DebugLogger.Log("생성되어 있는 미로가 존재하지 않습니다.");
            return;
        }

        DestroyAllTile();
        _check = false;

        _currentChapter = chapter;
        _currentStage = stage;

        EventManager<UIEvents>.TriggerEvent(UIEvents.OnClickUseTicket); // 입장권 티켓 감소
        EventManager<StageEvent>.TriggerEvent(StageEvent.EnterStage);

        DetectTileSize(_tileList.Count); // 맵 사이즈 결정

        _grid.cellSize = new Vector2(_tileSize, _tileSize); // 셀 사이즈 결정

        // tileList의 제곱근 길이만큼 RectTransform 크기 설정
        float sizeValue = Mathf.Sqrt(_tileList.Count) * _tileSize;
        _rectTransform.sizeDelta = new Vector2(sizeValue, sizeValue);

        // 만약 랜덤으로 생성한 오브젝트가 정답과 동일하면 다시 랜덤
        bool isLoop = true;
        while (isLoop)
        {
            // tileList의 길이만큼 TileNode 생성
            foreach (var tile in _tileList)
            {
                var newTile = Instantiate(_tileNode, transform);
                var tileNode = newTile.GetComponent<TileNode>();
                if (tileNode == null) continue;
                tileNode.SetTileNodeData(tile);
                int shapeRotation = (int)tile.RoadShape;
                if (shapeRotation <= 0)
                {
                    //shapeRotation = Random.Range(1, 5);
                    continue;
                }

                tileNode.SetTilImage(roadList[shapeRotation - 1]);

                EventManager<StageEvent>.TriggerEvent(StageEvent.SetTileGrid, tileNode);
            }

            isLoop = IsCorrectAnswer();
        }

        // 제한 횟수 수치 이벤트로 넘기기
        var tileMapTable = DataManager.Instance.GetTileMapTable($"M{chapter}{stage.ToString("000")}");
        _limitCount = tileMapTable.LimitCount;
        EventManager<StageEvent>.TriggerEvent(StageEvent.StartStage, _limitCount);

        // 플레이어의 보유 아이템 이벤트로 넘기기
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

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            Destroy(child.gameObject);
        }

        _tileList.Clear(); // 모든 타일이 삭제되면 저장하고 있던 리스트 초기화
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

        // Todo: 골드 상승량 테이블을 만들고 StageClaer 이벤트 발생 시 플레이어 재화 관련 스크립트에서 테이블의 데이터를 읽어서 재화를 추가하는 방식으로 수정해야함
        // 정답이면
        if (_check)
        {
            EventManager<StageEvent>.TriggerEvent(StageEvent.StageClear);
            DebugLogger.Log("스테이지 클리어");
            
            // 플레이어 골드 증가
            var playerGold = PlayerInformation.Instance.PlayerViewModel.PlayerGold;
            int plusGold = _currentChapter * 50;
            
            EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerGoldChanged, playerGold + plusGold);
            
            // 플레이어 해금 챕터 및 스테이지 증가
            EventManager<DataEvents>.TriggerEvent(DataEvents.UpdateCurrentChapterAndStage, _currentChapter, _currentStage);
            
            return;
        }

        if (_limitCount <= 0)
        {
            // 실패 UI 등장
            EventManager<StageEvent>.TriggerEvent(StageEvent.StageFail);
            DebugLogger.Log("스테이지 실패");
        }
    }

    // 리스트 수에 맞추어 Tile의 크기 설정
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

    private void SetTileMapPositionGrid(RectTransform transform, TileNode tileNode)
    {
        EventManager<StageEvent>.TriggerEvent(StageEvent.SetTileGrid, transform, tileNode);
    }
}