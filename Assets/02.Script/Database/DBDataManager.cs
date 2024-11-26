using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class DBDataManager : MonoBehaviour
{
    /*
        "UserData" Key Value.
        DBDataManager.Instance.UserData.Add("MemberID",strArr[0]);
        DBDataManager.Instance.UserData.Add("Email", strArr[1]);
        DBDataManager.Instance.UserData.Add("Nickname", strArr[2]);
        DBDataManager.Instance.UserData.Add("ProfileURL", strArr[3]);

        "Assets" Key Value.
        DBDataManager.Instance.UserAssetsData.Add("MemberID", strArr[0]);
        DBDataManager.Instance.UserAssetsData.Add("Gold", strArr[1]);
        DBDataManager.Instance.UserAssetsData.Add("HeartTime", strArr[2]);
        DBDataManager.Instance.UserAssetsData.Add("ItemCount", strArr[3]);
        
        "MapData" Key Value.
        DBDataManager.Instance.MapData.Add("Chapter", strArr[0]);
        DBDataManager.Instance.MapData.Add("Stage", strArr[1]);
        DBDataManager.Instance.MapData.Add("MapID", strArr[2]);
        DBDataManager.Instance.MapData.Add("TileValue", strArr[3]);
        DBDataManager.Instance.MapData.Add("LimitCount", strArr[4]);
        DBDataManager.Instance.MapData.Add("CreateTime", strArr[5]);
     */

    private static DBDataManager _instance;
    public static DBDataManager Instance
    {
        get
        {
            // 만약 인스턴스가 null이면, 새로운 GameManager 오브젝트를 생성
            if (_instance == null)
            {
                _instance = FindObjectOfType<DBDataManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<DBDataManager>();
                    singletonObject.name = typeof(DBDataManager).ToString() + " (Singleton)";

                    // GameManager 오브젝트가 씬 전환 시 파괴되지 않도록 설정
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    [SerializeField] public Dictionary<string, string> UserData;
    [SerializeField] public Dictionary<string, string> UserAssetsData;
    [SerializeField] public Dictionary<string, string> MapData;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        UserData = new Dictionary<string, string>();
        UserAssetsData = new Dictionary<string, string>();
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
            case "Assets":
                foreach (var str in UserAssetsData)
                {
                    TextCheck.text += $"{str.Key} / {str.Value}\n";
                }
                break;
            case "MapData":
                foreach (var str in MapData)
                {
                    TextCheck.text += $"{str.Key} / {str.Value}\n";
                }
                break;
            default :
                break;
        }
    }
}
