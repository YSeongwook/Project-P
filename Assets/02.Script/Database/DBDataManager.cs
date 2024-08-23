using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBDataManager : Singleton<DBDataManager>
{
    [Header ("User Data")]
    public Dictionary<string, string> UserData;

    private void Awake()
    {
        UserData = new Dictionary<string, string>();
    }
}
