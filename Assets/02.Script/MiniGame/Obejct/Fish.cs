using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditorInternal.ReorderableList;

public class Fish : DropHandler
{
    private RectTransform rectTransform;

    private Collider2D collider;

    public bool isClearAble {  get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        collider.enabled = true;
        isClearAble = false;

        RandomSpawn();
    }

    private void RandomSpawn()
    {
        var randomX = Random.Range(-380f, 380f);
        var randomY = Random.Range(-275f, -880f);

        rectTransform.localPosition = new Vector2(randomX, randomY);
    }

    public void SetClearAble(bool SetAble)
    {
        isClearAble = SetAble;

        DebugLogger.Log($"{gameObject.name} : {isClearAble}");
    }
}
