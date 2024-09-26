using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fish : DropHandler
{
    private RectTransform _rectTransform;
    private Collider2D _collider;

    public bool IsClearAble {  get; private set; }
    private bool _isGameStart;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _collider = GetComponent<Collider2D>();

        EventManager<MiniGame>.StartListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnDestroy()
    {
        EventManager<MiniGame>.StopListening<bool>(MiniGame.SetStartTrigger, SetGameStart);
    }

    private void OnEnable()
    {
        _collider.enabled = true;
        IsClearAble = false;
        _isGameStart = false;

        RandomSpawn();
    }

    private void RandomSpawn()
    {
        var randomX = Random.Range(-380f, 380f);
        var randomY = Random.Range(-275f, -880f);

        _rectTransform.localPosition = new Vector2(randomX, randomY);
    }

    public void SetClearAble(bool setAble)
    {
        IsClearAble = setAble;

        DebugLogger.Log($"{gameObject.name} : {IsClearAble}");
    }

    private void SetGameStart(bool isGameStart)
    {
        if (!isGameStart) return;

        this._isGameStart = isGameStart;
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isGameStart) return;

        //base.OnBeginDrag(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!_isGameStart) return;
        
        _rectTransform.position = eventData.position;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!_isGameStart) return;
        
        //base.OnEndDrag(eventData);
    }
}
