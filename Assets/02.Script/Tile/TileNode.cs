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

    private Image _imageRoad;
    private Image _imageGimmick;
    private Image _imageHint;
    private RectTransform _imageGimmickRectTransform;
    private Outline _backgroundOutline;
    private RotationTile _rotationTile; // 타일 회전 스크립트
    private Tile _tile; // Player에게 조작되는 Tile
    
    private bool _isReverseRotate;
    private bool _isHint;
    private bool _isEnd;

    public GimmickAnimation Gimmick { get; private set; }
    public Tile CorrectTileInfo { get; private set; } // 정답 확인용 Tile
    public Tile GetTileInfo {  get { return _tile; } }
    public bool IsCorrect { get; private set; }

    private void Awake()
    {
        Initialize();
    }
    
    private void OnEnable()
    {
        _isReverseRotate = false;
        _isEnd = false;

        AddEvents();
    }

    private void OnDisable()
    {
        RemoveEvents();
    }

    private void Start()
    {
        _backgroundOutline.enabled = false;

        if(_imageGimmick.sprite == default)
            _imageGimmick.enabled = false;

        if (_tile.Type == TileType.Road)
            EventManager<DataEvents>.TriggerEvent(DataEvents.SetTileGrid, this);        
    }

    private void AddEvents()
    {
        EventManager<InventoryItemEvent>.StartListening<bool>(InventoryItemEvent.SetReverseRotate, SetReverse);
        EventManager<InventoryItemEvent>.StartListening<bool>(InventoryItemEvent.SetHint, UseHintItem);
        EventManager<StageEvent>.StartListening<bool>(StageEvent.GameEnd, SetGameEnd);
    }

    private void RemoveEvents()
    {
        EventManager<InventoryItemEvent>.StopListening<bool>(InventoryItemEvent.SetReverseRotate, SetReverse);
        EventManager<InventoryItemEvent>.StopListening<bool>(InventoryItemEvent.SetHint, UseHintItem);
        EventManager<StageEvent>.StopListening<bool>(StageEvent.GameEnd, SetGameEnd);
    }

    private void Initialize()
    {
        Gimmick = GetComponentInChildren<GimmickAnimation>();

        _imageRoad = transform.GetChild(1).GetComponent<Image>();
        _imageGimmick = transform.GetChild(2).GetComponent<Image>();
        _imageHint = transform.GetChild(3).GetComponent<Image>();

        var newColor = _imageHint.color;
        newColor.a = 0.45f;
        _imageHint.color = newColor;

        _backgroundOutline = transform.GetChild(0).GetComponent<Outline>();
        _imageGimmickRectTransform = _imageGimmick.GetComponent<RectTransform>();

        _rotationTile = GetComponent<RotationTile>();
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
        }
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
        }
    }

    public void SetLinkTileRotate(bool isChecking)
    {
        if (_isReverseRotate)
        {
            _tile.RotateValue = (_tile.RotateValue + 3) % 4;
        }
        else
        {
            _tile.RotateValue = (_tile.RotateValue + 1) % 4;
        }
        
        if (_rotationTile != null)
        {
            _rotationTile.RotateTile(_tile.RotateValue);  // 회전 로직 RotationTile에 위임
        }
    }

    // 회전 명령 실행, 현재 링크 타일이 같이 회전하지 않는 이슈가 있음
    public void OnClickRotationTile()
    {
        if (_isEnd || _rotationTile.IsRotating) return;

        // 짧은 진동 발생
        EventManager<VibrateEvents>.TriggerEvent(VibrateEvents.ShortWeak);

        // 힌트 사용 중이면 힌트 실행
        if (_isHint)
        {
            TriggerHint();
            DebugLogger.Log("힌트 아이템 사용");
        }
        // 역회전 아이템 사용 중이면 역회전 실행
        else if (_isReverseRotate)
        {
            _tile.RotateValue = (_tile.RotateValue + 3) % 4;

            // 역회전 아이템 사용 후 상태 초기화
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.DecreaseItemCount, nameof(ItemID.I1002));
            EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.SetReverseRotate, false);
            _isReverseRotate = false;
        }
        // 아무런 아이템 사용 중이 아니면 일반 회전
        else if(_tile.GimmickShape == GimmickShape.Link)
        {
            EventManager<PuzzleEvent>.TriggerEvent(PuzzleEvent.Rotation, this, _isReverseRotate);
            EventManager<StageEvent>.TriggerEvent(StageEvent.UseTurn);

            return;
        }
        else
        {
            _tile.RotateValue = (_tile.RotateValue + 1) % 4;
            EventManager<StageEvent>.TriggerEvent(StageEvent.UseTurn);
        }

        // 회전 후 힌트 타일과 일치 여부 확인
        if (_rotationTile != null)
        {
            _rotationTile.RotateTile(_tile.RotateValue);
            CheckHintMatch(); // 회전 후 힌트와 일치 여부 확인
        }
    
        // 아이템 비활성화 이벤트 호출
        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.CallbackPlayerResourceUI);
    }
    
    // 힌트와 타일이 일치하는지 확인하여 힌트 이미지 비활성화
    private void CheckHintMatch()
    {
        if (_tile.RotateValue == CorrectTileInfo.RotateValue)
        {
            _imageHint.enabled = false; // 힌트 타일과 일치 시 힌트 이미지 비활성화
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
        
        // 힌트 타일 표시 코루틴 시작
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
        // 기본 크기 저장
        var initScale = transform.localScale;

        // 시퀀스 생성
        Sequence scaleSequence = DOTween.Sequence();
        scaleSequence.Append(transform.DOScale(Vector3.one * 1.5f, animationDuration).SetEase(scaleCurve));
        scaleSequence.Append(transform.DOScale(initScale, animationDuration).SetEase(scaleCurve));
    }

    private void SetGameEnd(bool isEnd)
    {
        _isEnd = isEnd;
    }

    public void SetStartEndSize()
    {
        // 좌우, 상하 여백을 40씩 줄이기
        _imageGimmickRectTransform.offsetMin = new Vector2(40, 40); // 좌, 하단에서 40만큼 안쪽으로
        _imageGimmickRectTransform.offsetMax = new Vector2(-40, -40); // 우, 상단에서 40만큼 안쪽으로
    }
}
