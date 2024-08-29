using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Drawers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rice : MonoBehaviour
{
    [FoldoutGroup("Sliced")][SerializeField] private GameObject Sliced_Bottom;
    [FoldoutGroup("Sliced")][SerializeField] private GameObject Sliced_Top;

    private Vector2 originPos_Bottom;
    private Vector2 originPos_Top;

    private Collider2D collider;

    public bool isSlices { get; private set; }

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        originPos_Bottom = Sliced_Bottom.transform.position;
        originPos_Top = Sliced_Top.transform.position;
    }

    private void OnEnable()
    {
        Sliced_Bottom.transform.position = originPos_Bottom;
        Sliced_Top.transform.position = originPos_Top;
        Sliced_Top.transform.rotation = Quaternion.identity;
        collider.enabled = true;
        isSlices = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 diretion = (collision.transform.position - transform.position).normalized;
            var roatateDir = diretion.x > 0 ? 1 : -1;
            Sliced_Top.transform.position += new Vector3(0, 15f);
            Sliced_Top.transform.rotation = Quaternion.Euler(0,0, roatateDir * 15f);
            collider.enabled = false;
            isSlices = true;
        }
    }
}
