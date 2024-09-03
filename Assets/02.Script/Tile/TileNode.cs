using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;

// 빈 타일, 길 타일, 기믹 타일(길을 항상 포함)
public enum TileType
{
    None,
    Road,
    Gimmick
}

// 타일 모양(빈 타일의 경우 None, 순서대로 선, L자, T자, 십자, 출발점, 종료점)
public enum RoadShape
{
    None,
    Straight,
    L,
    T,
    Cross,
    Start,
    End
}

// 기믹 모양(기믹 없는 타일의 경우 None
public enum GimmickShape
{
    None,
    Warp,
    Link,
}

public struct Tile
{
    public TileType Type; // 빈 타일, 길 타일, 기믹 타일
    public RoadShape RoadShape;
    public GimmickShape GimmickShape;
    public int RotateValue;
}

public class TileNode : MonoBehaviour
{
    public Tile CorrectTileInfo { get; private set; }   // 정답 확인용 Tile
    private Tile _tile;             // Player에게 조작되는 Tile

    public Tile GetTileInfo {  get { return _tile; } }

    public GimmickAnimation _gimmick { get; private set; }

    private Image _background;
    private Image _imageRoad;
    private Image _imageGimmick;
    private RectTransform _rectTransform;
    private RectTransform _imageRoadRectTransform;
    private RectTransform _imageGimmickRectTransform;
    private Outline _backgroundOutline;

    public bool IsCorrect { get; private set; }

    private bool IsReverseRotate;

    private void Awake()
    {
        _gimmick = GetComponentInChildren<GimmickAnimation>();

        _background = transform.GetChild(0).GetComponent<Image>();
        _imageRoad = transform.GetChild(1).GetComponent<Image>();
        _imageGimmick = transform.GetChild(2).GetComponent<Image>();

        _backgroundOutline = transform.GetChild(0).GetComponent<Outline>();
        _rectTransform = GetComponent<RectTransform>();

        _imageRoadRectTransform = _imageRoad.GetComponent<RectTransform>();
        _imageGimmickRectTransform = _imageGimmick.GetComponent<RectTransform>();

        EventManager<InventoryItemEvent>.StartListening<bool>(InventoryItemEvent.SetReverseRotate, SetReverse);
    }

    private void OnDestroy()
    {
        EventManager<InventoryItemEvent>.StopListening<bool>(InventoryItemEvent.SetReverseRotate, SetReverse);
    }

    private void OnEnable()
    {
        IsReverseRotate = false;
    }

    private void Start()
    {
        _backgroundOutline.enabled = false;

        if(_imageGimmick.sprite == default)
            _imageGimmick.enabled = false;

        if (_tile.Type == TileType.Road)
            EventManager<DataEvents>.TriggerEvent(DataEvents.SetTileGrid, this);
    }

    // 타일 정보 삽입
    public void SetTileNodeData(Tile tile)
    {
        _tile = tile;
        CorrectTileInfo = tile;

        IsCorrect = false;

        _gimmick.GetGimmickShape(_tile.GimmickShape);
    }

    // Road 타일 이미지 변경
    public void SetTileRoadImage(Sprite Road)
    {
        _imageRoad.sprite = Road;

        // 임시 테스트용
        RotationTile(_tile.RotateValue, false);

        //RandomTileRotate();
    }

    // Gimmick 타일 이미지 변경
    public void SetTileGimmickImage(Sprite Gimmick)
    {
        _imageGimmick.enabled = true;
        _imageGimmick.sprite = Gimmick;

        _imageGimmickRectTransform.rotation = Quaternion.identity;

        // 기믹 애니메이션 실행
        _gimmick.StartGimmickAnimation();
    }

    // 타일 회전값 랜덤 설정
    //private void RandomTileRotate()
    //{
    //    int randomRotateValue = Random.Range(0, 4);
    //    _tile.RotateValue = randomRotateValue;

    //    RotationTile(randomRotateValue, false);
    //}

    // 회전 명령 실행
    public void OnClickRotationTile()
    {
        if (IsReverseRotate)
        {
            _tile.RotateValue = (_tile.RotateValue + 3) % 4;

            // 모든 타일들의 ReverseRotate 값 변화
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, false);
        }
        else
        {
            _tile.RotateValue = (_tile.RotateValue + 1) % 4;
        }        

        EventManager<StageEvent>.TriggerEvent(StageEvent.UseTurn);

        EventManager<PuzzleEvent>.TriggerEvent(PuzzleEvent.Rotation, this);
        //RotationTile(_tile.RotateValue, true);
    }

    // 타일 회전
    public void RotationTile(int rotateValue, bool isCheckAble)
    {
        float rotationAngle = rotateValue * -90f;

        _imageRoadRectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        //_imageGimmickRectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        //정답 rotation과 비교
        CheckAnswer(isCheckAble);
    }

    private void CheckAnswer(bool isCheckAble)
    {
        int calculatedValue = 1;
        switch (_tile.RoadShape)
        {
            case RoadShape.Straight:
                calculatedValue = 2;
                break;
            case RoadShape.Cross:
                calculatedValue = 1;
                break;
            default:
                calculatedValue = 4;
                break;
        }

        IsCorrect = (_tile.RotateValue % calculatedValue) == (CorrectTileInfo.RotateValue % calculatedValue);

        _background.enabled = !IsCorrect;

        if (isCheckAble)
        {
            // MapGenerator의 CheckAnswer 이벤트 실행
            EventManager<DataEvents>.TriggerEvent(DataEvents.CheckAnswer);
        }
    }

    private void SetReverse(bool isReverse)
    {
        IsReverseRotate = isReverse;
    }
}
