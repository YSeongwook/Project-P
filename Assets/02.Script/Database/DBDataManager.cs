using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DBDataManager : Singleton<DBDataManager>
{
    [SerializeField] public Dictionary<string, string> UserData;

    private void Awake()
    {
        UserData = new Dictionary<string, string>();
    }

    public void ShowDicDataCheck(string dicName)
    {
        Text TextCheck = GameObject.Find("TextCheck").GetComponent<Text>();
        switch (dicName) 
        {
            case "UserData" :
                foreach(var str in UserData)
                {
                    TextCheck.text += $"{str.Key} / {str.Value}\n";
                }
                break;
            default :
                break;
        }
    }
}
