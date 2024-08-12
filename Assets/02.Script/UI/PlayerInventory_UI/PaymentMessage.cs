using EnumTypes;
using EventLibrary;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaymentMessage : MonoBehaviour
{
    [FoldoutGroup("Payment Message")]
    [SerializeField] private TMP_Text Text_Message;

    private void Awake()
    {
        AddEvenet();
    }

    private void OnDestroy()
    {
        RemoveEvenet();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void AddEvenet()
    {
        EventManager<DataEvents>.StartListening<bool>(DataEvents.OnPaymentSuccessful, PopUpMessage);
    }
    private void RemoveEvenet()
    {
        EventManager<DataEvents>.StopListening<bool>(DataEvents.OnPaymentSuccessful, PopUpMessage);
    }

    private void PopUpMessage(bool PaymentSuccessful)
    {
        gameObject.SetActive(true);

        if (PaymentSuccessful)
        {
            Text_Message.text = "Payment is Complete";
        }
        else
        {
            Text_Message.text = "Payment is Fail";
        }
    }
}
