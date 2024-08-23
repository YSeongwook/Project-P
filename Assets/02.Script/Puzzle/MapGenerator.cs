using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    [FoldoutGroup("Canvas")][SerializeField] private GameObject _MainMenuUI;
    [FoldoutGroup("Canvas")][SerializeField] private GameObject _PlayerGoldUI;

    [FoldoutGroup("UI")]
    [SerializeField] private TMP_Text _limitCountTextUI;

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

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _grid = GetComponent<GridLayoutGroup>();

        AddEvents();
    }

    private void Start()
    {
        _canvas.enabled = false;
    }
    
    private void Update()
    {
        if (_check) DebugLogger.Log("정답");
    }

    private void OnDestroy()
    {
        RemoveEvents();
    }

    private void AddEvents()
    {
        EventManager<DataEvents>.StartListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StartListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<StageEvent>.StartListening<int>(StageEvent.UseTurn, UpdateChangedLimitCoutUI);
    }

    private void RemoveEvents()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StopListening(DataEvents.CheckAnswer, CheckAnswer);
        EventManager<StageEvent>.StopListening<int>(StageEvent.UseTurn, UpdateChangedLimitCoutUI);
    }

    //스테이지 열기
    private void OpenNewStage(int chapter, int stage)
    {
        DestroyAllTile();

        string fileName = $"{chapter}-{stage}";
        _tileList = new List<Tile>(DataManager.Instance.GetPuzzleTileMap(fileName));
        if (_tileList == null )
        {
            DebugLogger.Log("생성되어 있는 미로가 존재하지 않습니다.");
            return;
        }

        // UI들 비활성화
        _MainMenuUI.SetActive(false);
        _PlayerGoldUI.SetActive(false);

        // 캔버스 활성화
        _canvas.enabled =true;

        //맵 사이즈 결정
        DetectTileSize(_tileList.Count);
        //셀 사이즈 결정
        _grid.cellSize = new Vector2(_tileSize, _tileSize);

        // tileList의 제곱근 길이만큼 RectTransform 크기 설정
        float sizeValue = Mathf.Sqrt(_tileList.Count) * _tileSize;
        _rectTransform.sizeDelta = new Vector2(sizeValue, sizeValue);

        // tileList의 길이만큼 TileNode 생성
        foreach (var tile in _tileList)
        {
            var newTile = Instantiate(_tileNode, transform);
            var tileNode = newTile.GetComponent<TileNode>();
            if(tileNode == null ) continue;
            tileNode.SetTileNodeData(tile);
            int shapeRotation = (int)tile.RoadShape;
            if(shapeRotation <= 0)
            {
                shapeRotation = Random.Range(1, 5);
            }
            tileNode.SetTilImage(RoadList[shapeRotation - 1]);
        }

        // 제한 횟수 수치 이벤트로 넘기기
        var tileMapTable = DataManager.Instance.GetTileMapTable($"M{chapter}{stage.ToString("000")}");
        _limitCount = tileMapTable.LimitCount;
        UpdateChangedLimitCoutUI(0);

        // 플레이어의 보유 아이템 이벤트로 넘기기
    }

    // 제한 횟수 UI 동기화
    private void UpdateChangedLimitCoutUI(int changeLimitCount)
    {
        _limitCount = Mathf.Clamp(_limitCount - changeLimitCount, 0, 999);
        _limitCountTextUI.text = $"{_limitCount}";
    }

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

    // 정답 확인
    private void CheckAnswer()
    {
        int checking = 1;

        foreach(Transform child in transform)
        {
            var childTileNode = child.GetComponent<TileNode>();
            if (childTileNode == null) continue;
            if (childTileNode.GetTileInfo.Type == TileType.None) continue;

            int check = childTileNode.isCorrect ? 1 : 0;

            checking *= check;
        }

        _check = checking == 1;

        // 정답이면
        if(_check) DebugLogger.Log("클리어");
        else DebugLogger.Log("실패");

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
}
