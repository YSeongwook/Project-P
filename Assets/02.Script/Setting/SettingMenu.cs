using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [Header("Space Between Menu Item")]
    [SerializeField] Vector2 Spacing;

    [SerializeField] Image background;
    Button mainBtn;
    SettingMenuItem[] menuItems;

    bool isExpanded = false;

    Vector2 mainButtonPosition;
    int itemsCount;

    // Start is called before the first frame update
    void Start()
    {
        menuItems = transform.GetComponentsInChildren<SettingMenuItem>();
        itemsCount = menuItems.Length;

        mainBtn = transform.GetChild (0).GetComponent<Button>();
        mainBtn.onClick.AddListener(ToggleMenu);
        mainBtn.transform.SetAsLastSibling();

        mainButtonPosition = mainBtn.transform.position;

        ResetPosition();
    }

    private void ResetPosition()
    {
        background.gameObject.SetActive (false);
        foreach (SettingMenuItem item in menuItems)
        {
            item.transform.position = mainButtonPosition;
            item.gameObject.SetActive(false);
        }
    }

    void ToggleMenu()
    {
        isExpanded = !isExpanded;

        if (isExpanded)
        {
            background.gameObject.SetActive(true);
            for (int i =0; i<itemsCount; i++)
            {
                menuItems[i].gameObject.SetActive(true);
                menuItems[i].transform.position = mainButtonPosition + Spacing * (i + 1);
            }
        }
        else
        {
            ResetPosition();
        }
    }

    private void OnDestroy()
    {
        mainBtn.onClick.RemoveListener(ToggleMenu);
    }
}
