using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [FoldoutGroup("UI")][SerializeField] private GameObject _MainMenuUI;
    [FoldoutGroup("UI")][SerializeField] private GameObject _PlayerGoldUI;

    [FoldoutGroup("Tile")][SerializeField] private GameObject _tileNode;
    [FoldoutGroup("Tile")][SerializeField] private float _tileSize = 120;

    [FoldoutGroup("Tile Sprite")]
    [SerializeField] private List<Sprite> RoadList;
    [FoldoutGroup("Tile Sprite")]
    [SerializeField] private List<Sprite> GimmickList;

    private Canvas canvas;
    private List<Tile> tileList = new List<Tile>();
    private RectTransform rectTransform;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();

        EventManager<DataEvents>.StartListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StartListening(DataEvents.CheckAnswer, CheckAnswer);
    }

    private void Start()
    {
        canvas.enabled = false;
    }

    private void OnDestroy()
    {
        EventManager<DataEvents>.StopListening<int, int>(DataEvents.SelectStage, OpenNewStage);
        EventManager<DataEvents>.StopListening(DataEvents.CheckAnswer, CheckAnswer);
    }

    //스테이지 열기
    private void OpenNewStage(int chapter, int stage)
    {
        DestoryAllTile();

        string fileName = $"{chapter}-{stage}";
        tileList = DataManager.Instance.GetPuzzleTileMap(fileName);
        if(tileList == null )
        {
            DebugLogger.Log("생성되어 있는 미로가 존재하지 않습니다.");
            return;
        }

        // UI들 비활성화
        _MainMenuUI.SetActive(false);
        _PlayerGoldUI.SetActive(false);

        // 캔버스 활성화
        canvas.enabled =true;

        // tileList의 제곱근 길이만큼 RectTransform 크기 설정
        float sizeValue = Mathf.Sqrt(tileList.Count) * _tileSize;
        rectTransform.sizeDelta = new Vector2(sizeValue, sizeValue);

        // tileList의 길이만큼 TileNode 생성
        foreach(var tile in tileList)
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


        // 플레이어의 보유 아이템 이벤트로 넘기기
    }

    private void DestoryAllTile()
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

        tileList.Clear();   // 모든 타일이 삭제되면 저장하고 있던 리스트 초기화
    }

    // 정답 확인
    private void CheckAnswer()
    {
        int checking = 1;

        foreach(Transform child in transform)
        {
            var childTileNode = child.GetComponent<TileNode>();
            if (childTileNode == null) continue;
            if (childTileNode.GetTileInfo.Type == TileType.None) 
                continue;

            int check = childTileNode.isCorrect ? 1 : 0;

            if (check == 0)
            {
                Debug.Log($"{childTileNode.GetTileInfo.RoadShape} => {childTileNode.GetTileInfo.RotateValue} : {childTileNode.CorrectTileInfo.RotateValue}");
            }
            checking *= check;
        }

        Check = checking == 1;

    }

    private bool Check;
    private void Update()
    {
        if (Check) 
            Debug.Log("정답");
    }
}
