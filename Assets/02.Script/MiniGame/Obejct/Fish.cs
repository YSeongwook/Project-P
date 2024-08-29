using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector2 SpawnPoint;

    private Collider2D collider;

    public bool isSlices { get; private set; }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        collider.enabled = true;
        isSlices = false;

        RandomSpawn();
    }

    private void RandomSpawn()
    {
        var randomX = Random.Range(-380f, 380f);
        var randomY = Random.Range(-275f, -880f);

        rectTransform.localPosition = new Vector2(randomX, randomY);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

        }
    }
}
