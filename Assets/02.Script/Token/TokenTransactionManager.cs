using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using EnumTypes;
using EventLibrary;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class TokenTransactionManager : MonoBehaviour
{
    [SerializeField] private string apiUrl = "https://devops1.store";
    [SerializeField] private string uid; // 인스펙터에서 설정할 수 있는 uid
    [SerializeField] private int value;  // 인스펙터에서 설정할 수 있는 value

    // 엔드포인트 정의
    private readonly string _createWallet = "/meta-transction/createWallet";
    private readonly string _createToken = "/meta-transction/createToken";
    private readonly string _deleteToken = "/meta-transction";
    private string _getWallet => $"/meta-transction/{uid}"; // uid를 사용하여 동적으로 경로 생성

    private void OnEnable()
    {
        EventManager<StageEvent>.StartListening(StageEvent.GameClear, CreateToken);
        EventManager<StageEvent>.StartListening<int>(StageEvent.CreateToken, CreateToken);
        EventManager<StageEvent>.StartListening<int>(StageEvent.DeleteToken, DeleteToken);
    }

    private void OnDisable()
    {
        EventManager<StageEvent>.StopListening(StageEvent.GameClear, CreateToken);
        EventManager<StageEvent>.StopListening<int>(StageEvent.CreateToken, CreateToken);
        EventManager<StageEvent>.StopListening<int>(StageEvent.DeleteToken, DeleteToken);
    }

    // 지갑을 생성하는 메서드
    public void CreateWallet()
    {
        StartCoroutine(SendRequest(_createWallet, "POST", new { uid }, OnWalletCreated));
    }

    // 토큰을 생성하는 메서드
    public void CreateToken()
    {
        StartCoroutine(SendRequest(_createToken, "POST", new { uid, value }, OnTokenCreated));
    }
    
    public void CreateToken(int token)
    {
        StartCoroutine(SendRequest(_createToken, "POST", new { uid, value = token }, OnTokenCreated));
    }

    // 토큰 잔액을 조회하는 메서드
    public void GetTokenBalance()
    {
        StartCoroutine(SendRequest(_getWallet, "GET", null, OnTokenBalanceRetrieved));
    }

    // 토큰을 삭제하는 메서드
    public void DeleteToken()
    {
        StartCoroutine(SendRequest(_deleteToken, "Delete", new { uid, value }, OnTokenDeleted));
    }
    
    public void DeleteToken(int token)
    {
        StartCoroutine(SendRequest(_deleteToken, "Delete", new { uid, value = token }, OnTokenDeleted));
    }

    // 서버 요청을 처리하는 코루틴
    private IEnumerator SendRequest(string endpoint, string method, object jsonData, Action<string> onSuccess)
    {
        string url = $"{apiUrl}{endpoint}";
        UnityWebRequest www;

        if (method == "GET")
        {
            www = UnityWebRequest.Get(url);
        }
        else
        {
            string json = jsonData != null ? JsonConvert.SerializeObject(jsonData) : "";
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            www = new UnityWebRequest(url, method)
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };
            www.SetRequestHeader("Content-Type", "application/json");
        }

        DebugLogger.Log($"Sending {method} request to {url} with data: {jsonData}");

        yield return www.SendWebRequest();

        DebugLogger.Log($"HTTP 상태 코드: {www.responseCode}");  // 상태 코드 확인

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            DebugLogger.LogError($"요청 실패: {www.error}. 응답 본문: {www.downloadHandler.text}");
        }
        else
        {
            DebugLogger.Log($"서버 응답: {www.downloadHandler.text}");
            onSuccess?.Invoke(www.downloadHandler.text);
        }
    }

    // 성공 시 콜백 메서드들
    private void OnWalletCreated(string response)
    {
        DebugLogger.Log("지갑 생성 성공!");
    }

    private void OnTokenCreated(string response)
    {
        DebugLogger.Log("토큰 생성 성공!");
        GetTokenBalance();
    }

    private void OnTokenBalanceRetrieved(string response)
    {
        DebugLogger.Log($"현재 토큰 잔액: {response}");
        try
        {
            // JSON 문자열을 파싱하여 token 값을 추출
            var jsonResponse = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            if (jsonResponse.ContainsKey("token") && int.TryParse(jsonResponse["token"], out int tokenBalance))
            {
                // 토큰 잔액을 이벤트로 전달
                EventManager<DataEvents>.TriggerEvent(DataEvents.PlayerERCChanged, tokenBalance);
            }
            else
            {
                DebugLogger.LogError("토큰 잔액 변환 실패: 'token' 키가 없거나 형식이 잘못되었습니다.");
            }
        }
        catch (JsonException ex)
        {
            DebugLogger.LogError($"JSON 파싱 오류: {ex.Message}");
        }

        EventManager<InventoryItemEvent>.TriggerEvent(InventoryItemEvent.CallbackPlayerResourceUI);
    }

    private void OnTokenDeleted(string response)
    {
        DebugLogger.Log("토큰 삭제 성공!");
        GetTokenBalance();
    }
}
