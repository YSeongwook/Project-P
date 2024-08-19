using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Text;

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

    // 지갑을 생성하는 메서드
    public void CreateWallet()
    {
        StartCoroutine(CreateWalletRequest());
    }

    // 토큰을 생성하는 메서드
    public void CreateToken()
    {
        StartCoroutine(CreateTokenRequest());
    }

    // 토큰 잔액을 조회하는 메서드
    public void GetTokenBalance()
    {
        StartCoroutine(GetTokenBalanceRequest());
    }

    // 토큰을 삭제하는 메서드
    public void DeleteToken()
    {
        StartCoroutine(DeleteTokenRequest());
    }

    // 지갑 생성 요청을 처리하는 코루틴
    private IEnumerator CreateWalletRequest()
    {
        // JSON 데이터 생성 (Newtonsoft.Json 사용)
        string jsonData = JsonConvert.SerializeObject(new { uid });

        Debug.Log($"전송된 JSON 데이터: {jsonData}");  // JSON 데이터 확인

        using (UnityWebRequest www = new UnityWebRequest($"{apiUrl}{_createWallet}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            Debug.Log($"HTTP 상태 코드: {www.responseCode}");  // 상태 코드 확인

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"지갑 생성 실패: {www.error}");
            }
            else
            {
                Debug.Log("지갑 생성 성공!");
                Debug.Log($"서버 응답: {www.downloadHandler.text}");
            }
        }
    }

    // 토큰 생성 요청을 처리하는 코루틴
    private IEnumerator CreateTokenRequest()
    {
        // JSON 데이터 생성 (Newtonsoft.Json 사용)
        string jsonData = JsonConvert.SerializeObject(new { uid, value });

        Debug.Log($"전송된 JSON 데이터: {jsonData}");  // JSON 데이터 확인

        using (UnityWebRequest www = new UnityWebRequest($"{apiUrl}{_createToken}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            Debug.Log($"HTTP 상태 코드: {www.responseCode}");  // 상태 코드 확인

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"토큰 생성 실패: {www.error}");
            }
            else
            {
                Debug.Log("토큰 생성 성공!");
                Debug.Log($"서버 응답: {www.downloadHandler.text}");
            }
        }
    }

    // 토큰 삭제(사용) 요청을 처리하는 코루틴
    private IEnumerator DeleteTokenRequest()
    {
        // JSON 데이터 생성 (Newtonsoft.Json 사용)
        string jsonData = JsonConvert.SerializeObject(new { uid, value });

        Debug.Log($"전송된 JSON 데이터: {jsonData}");  // JSON 데이터 확인

        using (UnityWebRequest www = new UnityWebRequest($"{apiUrl}{_deleteToken}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            Debug.Log($"HTTP 상태 코드: {www.responseCode}");  // 상태 코드 확인

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"토큰 삭제 실패: {www.error}");
            }
            else
            {
                Debug.Log("토큰 삭제 성공!");
                Debug.Log($"서버 응답: {www.downloadHandler.text}");
            }
        }
    }

    // 토큰 잔액 조회 요청을 처리하는 코루틴
    private IEnumerator GetTokenBalanceRequest()
    {
        using (UnityWebRequest www = UnityWebRequest.Get($"{apiUrl}{_getWallet}"))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log($"토큰 잔액 조회 실패: {www.error}");
            }
            else
            {
                Debug.Log($"현재 토큰 잔액: {www.downloadHandler.text}");
            }
        }
    }
}
