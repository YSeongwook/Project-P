using UnityEngine;
using UnityEngine.UI;

public class InvisibleExitButton : MonoBehaviour
{
   public GameObject target;

   private Button _button;

   private void Awake()
   {
      _button = GetComponent<Button>();
      
      AddButtonEvents();
   }

   private void OnDestroy()
   {
      RemoveButtonEvents();
   }

   private void AddButtonEvents()
   {
      _button.onClick.AddListener(OnClickInvisibleExitButton);
   }

   private void RemoveButtonEvents()
   {
      _button.onClick.RemoveListener(OnClickInvisibleExitButton);
   }

   public void OnClickInvisibleExitButton()
   {
      target.SetActive(false);
      gameObject.SetActive(false);
   }
}
