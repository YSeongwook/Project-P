using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JsonMapManager : MonoBehaviour
{
    private static JsonMapManager _instance;
    public static JsonMapManager Instance
    {
        get
        {
            // 만약 인스턴스가 null이면, 새로운 GameManager 오브젝트를 생성
            if (_instance == null)
            {
                _instance = FindObjectOfType<JsonMapManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<JsonMapManager>();
                    singletonObject.name = typeof(JsonMapManager).ToString() + " (Singleton)";

                    // GameManager 오브젝트가 씬 전환 시 파괴되지 않도록 설정
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

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
    }

    private void Start()
    {
        StartCoroutine(MakeJsonFile("LimitCountTable.json"));
        StartCoroutine(MakeJsonFile("1-1.json"));
        StartCoroutine(MakeJsonFile("1-2.json"));
    }

    private IEnumerator MakeJsonFile(string fileName)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        string jsonContent = "";

        Debug.Log("JSON 파일 경로: " + path);

        if (path.Contains("://") || path.Contains(":///"))
        {
            using (UnityWebRequest www = UnityWebRequest.Get(path))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("JSON 파일 로드 실패: " + www.error);
                }
                else
                {
                    jsonContent = www.downloadHandler.text;
                    Debug.Log("JSON 파일 로드 성공: " + fileName);
                }
            }
        }
        else
        {
            try
            {
                jsonContent = System.IO.File.ReadAllText(path);
                Debug.Log("JSON 파일 로드 성공: " + fileName);
            }
            catch (Exception e)
            {
                Debug.LogError("JSON 파일 로드 실패: " + e.Message);
            }
        }
    }
}
