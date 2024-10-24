using System.Collections;
using DG.Tweening;
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
    Fog,
    Rock,
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
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private float animationDuration = 1f;

    public Tile CorrectTileInfo { get; private set; }   // 정답 확인용 Tile
    private Tile _tile;             // Player에게 조작되는 Tile

    public Tile GetTileInfo {  get { return _tile; } }

    public GimmickAnimation Gimmick { get; private set; }

    private Image _background;
    private Image _imageRoad;
    private Image _imageGimmick;
    private Image _imageHint;
    private RectTransform _rectTransform;
    private RectTransform _imageRoadRectTransform;
    private RectTransform _imageGimmickRectTransform;
    private RectTransform _imageHintRectTransform;
    private Outline _backgroundOutline;
    private DOTweenAnimation tweenAnimation;

    private RotationTile _rotationTile; // 타일 회전 스크립트

    public bool IsCorrect { get; private set; }

    private bool _isReverseRotate;
    private bool _isHint;
    private bool _isEnd;

    private void Awake()
    {
        Initialize();
    }
    
    private void OnEnable()
    {
        _isReverseRotate = false;
        _isEnd = false;
        
        EventManager<InventoryItemEvent>.StartListening<bool>(InventoryItemEvent.SetReverseRotate, SetReverse);
        EventManager<InventoryItemEvent>.StartListening<bool>(InventoryItemEvent.SetHint, UseHintItem);
        EventManager<StageEvent>.StartListening<bool>(StageEvent.GameEnd, SetGameEnd);
    }

    private void OnDisable()
    {
        EventManager<InventoryItemEvent>.StopListening<bool>(InventoryItemEvent.SetReverseRotate, SetReverse);
        EventManager<InventoryItemEvent>.StopListening<bool>(InventoryItemEvent.SetHint, UseHintItem);
        EventManager<StageEvent>.StopListening<bool>(StageEvent.GameEnd, SetGameEnd);
    }

    private void Start()
    {
        _backgroundOutline.enabled = false;

        if(_imageGimmick.sprite == default)
            _imageGimmick.enabled = false;

        if (_tile.Type == TileType.Road)
            EventManager<DataEvents>.TriggerEvent(DataEvents.SetTileGrid, this);        
    }

    private void Initialize()
    {
        Gimmick = GetComponentInChildren<GimmickAnimation>();

        _background = transform.GetChild(0).GetComponent<Image>();
        _imageRoad = transform.GetChild(1).GetComponent<Image>();
        _imageGimmick = transform.GetChild(2).GetComponent<Image>();
        _imageHint = transform.GetChild(3).GetComponent<Image>();

        var newColor = _imageHint.color;
        newColor.a = 0.45f;
        _imageHint.color = newColor;

        _backgroundOutline = transform.GetChild(0).GetComponent<Outline>();
        _rectTransform = GetComponent<RectTransform>();

        _imageRoadRectTransform = _imageRoad.GetComponent<RectTransform>();
        _imageGimmickRectTransform = _imageGimmick.GetComponent<RectTransform>();
        _imageHintRectTransform = _imageHint.GetComponent<RectTransform>();

        _rotationTile = GetComponent<RotationTile>();
        tweenAnimation = GetComponent<DOTweenAnimation>();
    }

    // 타일 정보 삽입
    public void SetTileNodeData(Tile tile)
    {
        _tile = tile;
        CorrectTileInfo = tile;

        IsCorrect = false;

        Gimmick.GetGimmickShape(_tile.GimmickShape);
    }

    // Road 타일 이미지 변경
    public void SetTileRoadImage(Sprite Road)
    {
        _imageRoad.sprite = Road;
        _imageHint.sprite = Road;

        _imageHint.enabled = false;

        if (_rotationTile != null)
        {
            _rotationTile.InitRotateTile(_tile.RotateValue);  // 회전 로직 RotationTile에 위임
            //CheckAnswer(false);
        }

        //RandomTileRotate();
    }

    // Gimmick 타일 이미지 변경
    public void SetTileGimmickImage(Sprite Gimmick)
    {
        _imageGimmick.enabled = true;
        _imageGimmick.sprite = Gimmick;

        _imageGimmickRectTransform.rotation = Quaternion.identity;

        // 기믹 애니메이션 실행
        this.Gimmick.StartGimmickAnimation();
    }

    //타일 회전값 랜덤 설정
    public void SetRandomTileRotate(int rotateValue)
    {
        _tile.RotateValue = rotateValue;

        if (_rotationTile != null)
        {
            _rotationTile.RandomRotateTile(rotateValue);  // 회전 로직 RotationTile에 위임
            //CheckAnswer(false);
        }
    }

    public void SetLinkTileRotate(bool isChecking)
    {
        if (_isReverseRotate)
        {
            _tile.RotateValue = (_tile.RotateValue + 3) % 4;

            // 모든 타일들의 ReverseRotate 값 변화
            //EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, false);
        }
        else
        {
            _tile.RotateValue = (_tile.RotateValue + 1) % 4;
        }
        
        if (_rotationTile != null)
        {
            _rotationTile.RotateTile(_tile.RotateValue);  // 회전 로직 RotationTile에 위임
            //CheckAnswer(false);
        }
    }

    // 회전 명령 실행
    public void OnClickRotationTile()
    {
        if (_isEnd) return;
        if (_rotationTile.IsRotating) return; // 이미 타일이 회전 중이면 더 회전하지 못하게 방지

        // 짧은 진동 발생
        EventManager<VibrateEvents>.TriggerEvent(VibrateEvents.ShortWeak);

        if (_tile.GimmickShape == GimmickShape.Link && !_isHint)
        {
            EventManager<PuzzleEvent>.TriggerEvent(PuzzleEvent.Rotation, this, _isReverseRotate);
            EventManager<StageEvent>.TriggerEvent(StageEvent.UseTurn); // 제한 횟수 감소 이벤트 발생
            return;
        }

        if (_isReverseRotate && !_isHint)
        {
            _tile.RotateValue = (_tile.RotateValue + 3) % 4;

            // 사용한 아이템의 수 감소 
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.DecreaseItemCount, nameof(ItemID.I1002));
            // 모든 타일들의 ReverseRotate 값 변화
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, false);
        }
        else if(!_isReverseRotate && _isHint)
        {
            TriggerHint();
        }
        else
        {
            _tile.RotateValue = (_tile.RotateValue + 1) % 4;
            EventManager<StageEvent>.TriggerEvent(StageEvent.UseTurn); // 제한 횟수 감소 이벤트 발생
        }

        if (_rotationTile != null && !_isHint)
        {
            _rotationTile.RotateTile(_tile.RotateValue);  // 회전 로직 RotationTile에 위임
        }
    }

    private void SetReverse(bool isReverse)
    {
        _isReverseRotate = isReverse;
    }

    private void UseHintItem(bool isHint)
    {
        _isHint = isHint;
    }

    // 힌트 실행
    private void TriggerHint()
    {
        // 사용한 아이템의 수 감소 
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.DecreaseItemCount, nameof(ItemID.I1003));

        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetHint, false);
        StartCoroutine(ShowHintTile());
    }

    private IEnumerator ShowHintTile()
    {
        _imageHint.enabled = true;

        yield return new WaitForSeconds(5f);

        _imageHint.enabled = false;
    }

    // Scale 애니메이션 실행
    public void StartPathAnimation()
    {
        //tweenAnimation.DORestart();

        // 기본 크기 저장
        var initScale = transform.localScale;

        // 시퀀스 생성
        Sequence scaleSequence = DOTween.Sequence();

        scaleSequence.Append(transform.DOScale(Vector3.one * 1.5f, animationDuration).SetEase(scaleCurve));

        scaleSequence.Append(transform.DOScale(initScale, animationDuration).SetEase(scaleCurve));
    }

    private void SetGameEnd(bool isEnd)
    {
        this._isEnd = isEnd;
    }

    public void SetStartEndSize()
    {
        // 좌우, 상하 여백을 40씩 줄이기
        _imageGimmickRectTransform.offsetMin = new Vector2(40, 40); // 좌, 하단에서 40만큼 안쪽으로
        _imageGimmickRectTransform.offsetMax = new Vector2(-40, -40); // 우, 상단에서 40만큼 안쪽으로
    }
}
