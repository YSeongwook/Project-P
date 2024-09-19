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
        ws = new WebSocket("ws://3.38.178.218:8080");

        // 서버로부터 메시지를 받을 때 호출되는 이벤트 핸들러 설정
        ws.OnMessage += (sender, e) =>
        {
            if (e.IsBinary)
            {
                // 바이트 데이터를 UTF-8 문자열로 변환
                string message = Encoding.UTF8.GetString(e.RawData);
                Debug.Log("Received from server (binary): " + message);
                //AddChatMessage(message);

                // AddChatMessage 호출을 메인 스레드에서 실행하도록 큐에 추가
                mainThreadQueue.Enqueue(() => AddChatMessage(message));
            }
            else if (e.IsText)
            {
                // 텍스트 메시지일 경우 (보통 Data에 저장됨)
                Debug.Log("Received from server (text): " + e.Data);
            }
            else
            {
                Debug.Log("Received from server: Unknown data format.");
            }
        };

        // 서버에 연결
        ws.Connect();

        // 연결 상태 출력
        Debug.Log("WebSocket State: " + ws.ReadyState.ToString());

        // 테스트 메시지 전송
        //ws.Send("Hello, Server!");
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
        // 서버로 메시지 전송
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message);
        }
    }

    public void SendMessage()
    {
        //string nickName = DBDataManager.Instance.UserData["Nickname"];
        string nickName = "Name";

        string mess = $"{nickName} : {InputField_Text.text}";

        if (false == mess.IsNullOrEmpty())
        {
            SendMessageToServer(mess);
            //Debug.Log(mess);
            InputField_Text.text = "";
        }
        
    }

    public void AddChatMessage(string message)
    {
        // 내껀지 아닌지 판단
        // 프리팹 인스턴스 생성
        GameObject newMessage;

        if (true) newMessage = Instantiate(chatMessagePrefab_own, chatContent.transform);
        else newMessage = Instantiate(chatMessagePrefab_other, chatContent.transform);

        // 메시지 텍스트 설정
        //Text messageText = newMessage.GetComponentInChildren<Text>();
        //messageText.text = message;
        // "Text_Chat" 오브젝트의 TextMeshPro 컴포넌트 찾기
        TextMeshProUGUI messageText = newMessage.transform.Find("Text_Chat").GetComponent<TextMeshProUGUI>();
        messageText.text = message;

        // 메시지 추가 후 스크롤을 아래로 이동
        Canvas.ForceUpdateCanvases();
        ScrollRect scrollRect = chatContent.GetComponentInParent<ScrollRect>();
        scrollRect.verticalNormalizedPosition = 0f;
    }

}
