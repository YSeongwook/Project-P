using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PaymentMessage : MonoBehaviour
{
    [BoxGroup("Payment Message")] [SerializeField] private TMP_Text textMessage;

    private void Awake()
    {
        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void AddEvent()
    {
        EventManager<DataEvents>.StartListening<bool>(DataEvents.OnPaymentSuccessful, PopUpMessage);
        EventManager<UIEvents>.StartListening<string>(UIEvents.GameMessagePopUp, ErrorMessage);
    }
    private void RemoveEvent()
    {
        EventManager<DataEvents>.StopListening<bool>(DataEvents.OnPaymentSuccessful, PopUpMessage);
        EventManager<UIEvents>.StopListening<string>(UIEvents.GameMessagePopUp, ErrorMessage);
    }

    private void PopUpMessage(bool PaymentSuccessful)
    {
        gameObject.SetActive(true);

        if (PaymentSuccessful)
        {
            textMessage.text = "Payment is Complete";
            EventManager<UIEvents>.TriggerEvent(UIEvents.UpdatePlayerResources);
        }
        else
        {
            textMessage.text = "Payment is Fail";
        }
    }

    private void ErrorMessage(string message)
    {
        gameObject.SetActive(true);

        textMessage.text = message;
    }
}
