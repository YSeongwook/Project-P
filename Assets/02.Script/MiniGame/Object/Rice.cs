using Sirenix.OdinInspector;
using UnityEngine;

public class Rice : MonoBehaviour
{
    [FoldoutGroup("Sliced")][SerializeField] private GameObject Sliced_Bottom;
    [FoldoutGroup("Sliced")][SerializeField] private GameObject Sliced_Top;

    private Vector2 _originPosBottom;
    private Vector2 _originPosTop;

    private Collider2D _collider;

    public bool isSlices { get; private set; }

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _originPosBottom = Sliced_Bottom.transform.position;
        _originPosTop = Sliced_Top.transform.position;
    }

    private void OnEnable()
    {
        Sliced_Bottom.transform.position = _originPosBottom;
        Sliced_Top.transform.position = _originPosTop;
        Sliced_Top.transform.rotation = Quaternion.identity;
        _collider.enabled = true;
        isSlices = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            var rotateDir = direction.x > 0 ? 1 : -1;
            Sliced_Top.transform.position += new Vector3(0, 15f);
            Sliced_Top.transform.rotation = Quaternion.Euler(0,0, rotateDir * 15f);
            _collider.enabled = false;
            isSlices = true;
        }
    }
}
