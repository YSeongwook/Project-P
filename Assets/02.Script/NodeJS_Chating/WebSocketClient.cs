using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

public class WebSocketClient : MonoBehaviour
{
    [Header("Websocket")] public WebSocket ws;
    [SerializeField] private InputField InputField_Text;

    [Header("MalPongsun")]
    public GameObject chatContent; // Content 오브젝트
    public GameObject chatMessagePrefab_own; // 채팅 메시지 프리팹
    public GameObject chatMessagePrefab_other; // 채팅 메시지 프리팹

    private Queue<Action> mainThreadQueue = new Queue<Action>();

    private void Start()
    {
        // WebSocket 서버에 연결
        ws = new WebSocket("ws://192.168.0.174:8080/ws");

        ws.OnError += (sender, e) =>
        {
            DebugLogger.Log("WebSocket Error: " + e.Message);
            CheckLog("WebSocket Error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            DebugLogger.Log($"WebSocket Closed: Code={e.Code}, Reason={e.Reason}");
            CheckLog($"WebSocket Closed: Code={e.Code}, Reason={e.Reason}");
        };

        // 서버로부터 메시지를 받을 때 호출되는 이벤트 핸들러 설정
        ws.OnMessage += (sender, e) =>
        {
            if (e.IsBinary)
            {
                // 바이트 데이터를 UTF-8 문자열로 변환
                string message = Encoding.UTF8.GetString(e.RawData);
                DebugLogger.Log("Received from server (binary): " + message);
                //AddChatMessage(message);

                // AddChatMessage 호출을 메인 스레드에서 실행하도록 큐에 추가
                mainThreadQueue.Enqueue(() => AddChatMessage(message));
            }
            else if (e.IsText)
            {
                // 텍스트 메시지일 경우 (보통 Data에 저장됨)
                DebugLogger.Log("Received from server (text): " + e.Data);
                string message = e.Data;
                mainThreadQueue.Enqueue(() => AddChatMessage(message));
            }
            else
            {
                DebugLogger.Log("Received from server: Unknown data format.");
            }
        };

        // 서버에 연결
        ws.Connect();

        // 연결 상태 출력
        DebugLogger.Log("WebSocket State: " + ws.ReadyState.ToString());
        //CheckLog("WebSocket State: " + ws.ReadyState.ToString());
    }

    private void LateUpdate()
    {
        // 큐에 저장된 작업을 메인 스레드에서 처리
        while (mainThreadQueue.Count > 0)
        {
            var action = mainThreadQueue.Dequeue();
            action?.Invoke();
        }
    }

    private void OnApplicationQuit()
    {
        // 응용 프로그램 종료 시 WebSocket 연결 닫기
        if (ws != null)
        {
            ws.Close();
        }
    }

    public void SendMessageToServer(string message)
    {
        //CheckLog($"SendMessageToServer : {ws != null && ws.ReadyState == WebSocketState.Open}");
        // 서버로 메시지 전송
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message);
        }
    }

    public void SendMessage()
    {
        //CheckLog("Test");
        try
        {
            string nickName = DBDataManager.Instance.UserData["Nickname"];

            // 한국 시간대(KST) 설정
            //TimeZoneInfo kstZone = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            //DateTime koreanTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, kstZone);

            // UTC 기준의 현재 시간
            DateTime utcNow = DateTime.UtcNow;

            // 한국 시간대는 UTC+9 오프셋
            TimeSpan koreanOffset = TimeSpan.FromHours(9);

            // 한국 시간대로 변환
            DateTime koreanTime = utcNow + koreanOffset;

            // 현재 한국 시간 (예: 2024-09-19 14:30:00)
            string currentTime = koreanTime.ToString("yyyy-MM-dd HH:mm:ss");

            string mess = $"{nickName}/|||/{InputField_Text.text}/|||/{currentTime}";

            if (false == mess.IsNullOrEmpty())
            {
                SendMessageToServer(mess);
                InputField_Text.text = "";
            }
            else
            {
                CheckLog($"mess.IsNullOrEmpty() : {mess.IsNullOrEmpty()}");
            }
        }
        catch (Exception e) 
        {
            CheckLog($"{e.Message}");
            // 오류 메시지와 스택 트레이스 출력
            string errorMessage = $"Exception: {e.Message}\nStackTrace: {e.StackTrace}";
            CheckLog(errorMessage);
            DebugLogger.LogError(errorMessage);
        }
    }

    public void AddChatMessage(string message)
    {
        string[] strArr = message.Split("/|||/");

        string nickName = strArr[0];
        string context = strArr[1];
        string time = strArr[2];

        // 내껀지 아닌지 판단
        // 프리팹 인스턴스 생성
        GameObject newMessage;

        //if (nickName.Equals("Name")) newMessage = Instantiate(chatMessagePrefab_own, chatContent.transform);
        if (nickName.Equals(DBDataManager.Instance.UserData["Nickname"])) newMessage = Instantiate(chatMessagePrefab_own, chatContent.transform);
        else newMessage = Instantiate(chatMessagePrefab_other, chatContent.transform);


        // "Text_Chat" 오브젝트의 TextMeshPro 컴포넌트 찾기
        TextMeshProUGUI nickNameText = newMessage.transform.Find("Speech_Bubble/Group_UserID_Tme/Text_UserID").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = newMessage.transform.Find("Speech_Bubble/Text_Chat").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeText = newMessage.transform.Find("Speech_Bubble/Group_UserID_Tme/Text_Time").GetComponent<TextMeshProUGUI>();

        nickNameText.text = nickName;
        messageText.text = context;
        timeText.text = time;

        // 메시지 추가 후 스크롤을 아래로 이동
        Canvas.ForceUpdateCanvases();
        
        ScrollRect scrollRect = chatContent.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    public void CheckLog(string message)
    {
        //string[] strArr = message.Split("/|||/");

        string nickName = "Test";
        string context = message;
        string time = "Test";

        // 내껀지 아닌지 판단
        // 프리팹 인스턴스 생성
        GameObject newMessage = Instantiate(chatMessagePrefab_own, chatContent.transform);
        
        // "Text_Chat" 오브젝트의 TextMeshPro 컴포넌트 찾기
        TextMeshProUGUI nickNameText = newMessage.transform.Find("Speech_Bubble/Group_UserID_Tme/Text_UserID").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI messageText = newMessage.transform.Find("Speech_Bubble/Text_Chat").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI timeText = newMessage.transform.Find("Speech_Bubble/Group_UserID_Tme/Text_Time").GetComponent<TextMeshProUGUI>();

        nickNameText.text = nickName;
        messageText.text = context;
        timeText.text = time;

        // 메시지 추가 후 스크롤을 아래로 이동
        Canvas.ForceUpdateCanvases();

        ScrollRect scrollRect = chatContent.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}
