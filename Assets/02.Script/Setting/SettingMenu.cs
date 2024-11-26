using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [Header("Space Between Menu Item")]
    [SerializeField] private Vector2 spacing;
    [SerializeField] private Image background;
    
    private Button _mainBtn;
    private SettingMenuItem[] _menuItems;

    private bool _isExpanded;
    private Vector2 _mainButtonPosition;
    private int _itemsCount;

    private void Start()
    {
        _menuItems = transform.GetComponentsInChildren<SettingMenuItem>();
        _itemsCount = _menuItems.Length;

        _mainBtn = transform.GetChild (0).GetComponent<Button>();
        _mainBtn.onClick.AddListener(ToggleMenu);
        _mainBtn.transform.SetAsLastSibling();

        _mainButtonPosition = _mainBtn.transform.position;

        ResetPosition();
    }
    
    private void OnDestroy()
    {
        _mainBtn.onClick.RemoveListener(ToggleMenu);
    }

    private void ResetPosition()
    {
        background.gameObject.SetActive (false);
        foreach (SettingMenuItem item in _menuItems)
        {
            item.transform.position = _mainButtonPosition;
            item.gameObject.SetActive(false);
        }
    }

    public void ToggleMenu()
    {
        _isExpanded = !_isExpanded;

        if (_isExpanded)
        {
            background.gameObject.SetActive(true);
            for (int i =0; i<_itemsCount; i++)
            {
                _menuItems[i].gameObject.SetActive(true);
            }
        }
        else
        {
            ResetPosition();
        }
    }
}
