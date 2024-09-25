using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapDownloadManager : MonoBehaviour
{
    private static MapDownloadManager _instance;
    public static MapDownloadManager Instance
    {
        get
        {
            // 만약 인스턴스가 null이면, 새로운 GameManager 오브젝트를 생성
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapDownloadManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<MapDownloadManager>();
                    singletonObject.name = typeof(MapDownloadManager).ToString() + " (Singleton)";

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

    // 맵 다운로드 체크 로직
    public void CheckMapDownload()
    {
        // 있음

        // 없음

        SceneManager.LoadScene("Game");
    }

}
