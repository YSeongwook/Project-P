using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class GuestLogin : MonoBehaviour
{
    [Header("Guest Login Properties")]
    [SerializeField] private Button BGBtn;
    [SerializeField] private InputField IDInputField;
    [SerializeField] private AlertFadeOut AlertFadeOut;
    //[SerializeField] private Button GuestLoginBtn;

    // 메인 로그인 화면에있는 로그인버튼
    public void OnMainGuestLoginBtn()
    {
        BGBtn.gameObject.SetActive(true);
    }

    // 패널 화면에 있는 로그인 버튼
    public void GusetLoginStart()
    {
        string GuestID = IDInputField.text;

        if (GuestID.IsNullOrEmpty() == true)
        {
            AlertFadeOut.OnAlertMsg($"Please enter ID.");
            return;
        }

        switch (GuestID)
        {
            case "Guest1":
                BGBtn.gameObject.SetActive(false);
                AlertFadeOut.OnAlertMsg($"'{GuestID}' Login.");
                MySQLManager.Instance.ReadData("MemberInfo||1");
                break;
            case "Guest2":
                BGBtn.gameObject.SetActive(false);
                AlertFadeOut.OnAlertMsg($"'{GuestID}' Login.");
                MySQLManager.Instance.ReadData("MemberInfo||2");
                break;
            case "Guest3":
                BGBtn.gameObject.SetActive(false);
                AlertFadeOut.OnAlertMsg($"'{GuestID}' Login.");
                MySQLManager.Instance.ReadData("MemberInfo||3");
                break;
            case "Guest4":
                BGBtn.gameObject.SetActive(false);
                AlertFadeOut.OnAlertMsg($"'{GuestID}' Login.");
                MySQLManager.Instance.ReadData("MemberInfo||4");
                break;
            case "Guest5":
                BGBtn.gameObject.SetActive(false);
                AlertFadeOut.OnAlertMsg($"'{GuestID}' Login.");
                MySQLManager.Instance.ReadData("MemberInfo||5");
                break;
            default:
                AlertFadeOut.OnAlertMsg($"{GuestID} is Invalid,\nPlease Check ID again.");
                IDInputField.text = "";
                return;
        }

        //BGBtn.gameObject.SetActive(false);
        //AlertFadeOut.OnAlertMsg($"'{GuestID}' Login.");
    }
}
