using UnityEngine;

public class Fish : DropHandler
{
    private RectTransform _rectTransform;
    private Collider2D _collider;

    public bool IsClearAble {  get; private set; }

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _collider.enabled = true;
        IsClearAble = false;

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
}
