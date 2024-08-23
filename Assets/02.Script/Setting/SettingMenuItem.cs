using UnityEngine;
using UnityEngine.UI;

public class SettingMenuItem : MonoBehaviour
{
    [HideInInspector] public Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }
}
