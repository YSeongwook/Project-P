using System;
using System.Data;
using DG.Tweening.Core.Easing;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MySQLManager : MonoBehaviour
{
    private static MySQLManager _instance;
    public static MySQLManager Instance
    {
        get
        {
            // 만약 인스턴스가 null이면, 새로운 GameManager 오브젝트를 생성
            if (_instance == null)
            {
                _instance = FindObjectOfType<MySQLManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<MySQLManager>();
                    singletonObject.name = typeof(MySQLManager).ToString() + " (Singleton)";

                    // GameManager 오브젝트가 씬 전환 시 파괴되지 않도록 설정
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private AndroidJavaObject _androidJavaObject;
    private string connectionString;
    [SerializeField] private Text textCheck;
    //[SerializeField] private GameObject DBDataManager_;

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

    void Start()
    {
        _androidJavaObject = new AndroidJavaObject("com.unity3d.player.KakaoLogin");
        // 데이터베이스 연결 문자열 설정
        connectionString = "Server=3.38.178.218;Database=ProjectP;User ID=ubuntu;Password=P@ssw0rd!;Pooling=false;SslMode=None;AllowPublicKeyRetrieval=true;";
    }

    // type = Table 이름 str = 넘겨받은 || 로 구분된 정보
    public void InsertData(string type, string str)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                // check용
                //textCheck.text += $"{str}\n";

                conn.Open();

                string[] strArr = str.Split("||");

                string query = $"INSERT INTO {type} (";

                string values = "VALUES (";

                switch (type)
                {
                    case "MemberInfo": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                        if (!string.IsNullOrEmpty(strArr[0]))
                        {
                            query += "MemberID";
                            values += "@MemberID";
                        }
                        if (!string.IsNullOrEmpty(strArr[1]))
                        {
                            query += ", Email";
                            values += ", @Email";
                        }
                        if (!string.IsNullOrEmpty(strArr[2]))
                        {
                            query += ", Nickname";
                            values += ", @Nickname";
                        }
                        if (!string.IsNullOrEmpty(strArr[3]))
                        {
                            query += ", ProfileURL";
                            values += ", @ProfileURL";
                        }
                        //if (!string.IsNullOrEmpty(strArr[4]))
                        //{
                        //    query += ", GuestPassword";
                        //    values += ", @GuestPassword";
                        //}
                        break;
                    case "Assets": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                        if (!string.IsNullOrEmpty(strArr[0]))
                        {
                            query += "MemberID";
                            values += "@MemberID";
                        }
                        if (!string.IsNullOrEmpty(strArr[1]))
                        {
                            query += ", Gold";
                            values += ", @Gold";
                        }
                        if (!string.IsNullOrEmpty(strArr[2]))
                        {
                            query += ", HeartTime";
                            values += ", @HeartTime";
                        }
                        if (!string.IsNullOrEmpty(strArr[3]))
                        {
                            query += ", ItemCount";
                            values += ", @ItemCount";
                        }
                        //if (!string.IsNullOrEmpty(strArr[4]))
                        //{
                        //    query += ", GuestPassword";
                        //    values += ", @GuestPassword";
                        //}
                        break;
                    default:
                        Debug.LogError("Invalid query type provided.");
                        return;
                }

                query += ") ";
                values += ")";

                query += values;

                // check용
                //textCheck.text += $"{query}\n";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                switch (type)
                {
                    case "MemberInfo":
                        if (!string.IsNullOrEmpty(strArr[0])) cmd.Parameters.AddWithValue("@MemberID", Int64.Parse(strArr[0]));
                        if (!string.IsNullOrEmpty(strArr[1])) cmd.Parameters.AddWithValue("@Email", strArr[1]);
                        if (!string.IsNullOrEmpty(strArr[2])) cmd.Parameters.AddWithValue("@Nickname", strArr[2]);
                        if (!string.IsNullOrEmpty(strArr[3])) cmd.Parameters.AddWithValue("@ProfileURL", strArr[3]);
                        //if (!string.IsNullOrEmpty(strArr[4])) cmd.Parameters.AddWithValue("@GuestPassword", "");
                        break;
                    case "Assets":
                        if (!string.IsNullOrEmpty(strArr[0])) cmd.Parameters.AddWithValue("@MemberID", Int64.Parse(strArr[0]));
                        if (!string.IsNullOrEmpty(strArr[1])) cmd.Parameters.AddWithValue("@Gold", strArr[1]);
                        if (!string.IsNullOrEmpty(strArr[2])) cmd.Parameters.AddWithValue("@HeartTime", strArr[2]);
                        if (!string.IsNullOrEmpty(strArr[3])) cmd.Parameters.AddWithValue("@ItemCount", strArr[3]);
                        //if (!string.IsNullOrEmpty(strArr[4])) cmd.Parameters.AddWithValue("@GuestPassword", "");
                        break;
                    default:
                        Debug.LogError("Invalid query type provided.");
                        return;
                }

                cmd.ExecuteNonQuery();

                // 최초 로그인시 DBDataManager에 Dictionary 데이터 저장
                InputDataAtDictionary(type,str);

                Debug.Log("Data Inserted Successfully");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("MySqlPoolManager"))
                {
                    // 오류 무시하고 로깅만
                    Debug.LogWarning("MySqlPoolManager 관련 오류가 발생했지만 무시합니다: " + ex.Message);
                }
                else
                {
                    // 다른 예외는 다시 던지거나 처리
                    Debug.LogError("Failed to insert data: " + ex.Message);
                    //throw;
                }
            }
        }
    }

    public void ReadData(string str)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                //str = "MemberInfo||3666640951";

                // 체크용
                //textCheck.text += $"{str}\n";

                // strArr[0]은 테이블명 strArr[1]은 회원번호 고정
                string[] strArr = str.Split("||");

                string query = string.Empty;

                conn.Open();

                switch (strArr[0])
                {
                    case "MemberInfo": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                        query = $"SELECT * FROM {strArr[0]} WHERE MemberID = {strArr[1]}";
                        // 체크용
                        // textCheck.text += $"{query}\n";
                        break;
                    case "Assets": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                        query = $"SELECT * FROM {strArr[0]} WHERE MemberID = {strArr[1]}";
                        break;
                    case "MapData": // 새로운 MapData 로직 추가
                        query = $"SELECT * FROM MapData";
                        break;
                    default:
                        Debug.LogError("Invalid query type provided.");
                        return;
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    switch (strArr[0])
                    {
                        case "MemberInfo": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                            string MemberID = reader["MemberID"].ToString();
                            string Email = reader["Email"].ToString();
                            string Nickname = reader["Nickname"].ToString();
                            string ProfileURL = reader["ProfileURL"].ToString();

                            // 체크용
                            //textCheck.text += $"{MemberID}\n{Email}\n{Nickname}\n{ProfileURL}";

                            InputDataAtDictionary("MemberInfo", $"{MemberID}||{Email}||{Nickname}||{ProfileURL}");
                            break;
                        case "Assets": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                            string ID = reader["MemberID"].ToString();
                            string Gold = reader["Gold"].ToString();
                            string HeartTime = reader["HeartTime"].ToString();
                            string ItemCount = reader["ItemCount"].ToString();

                            // 체크용
                            //textCheck.text += $"{MemberID}\n{Email}\n{Nickname}\n{ProfileURL}";

                            InputDataAtDictionary("Assets", $"{ID}||{Gold}||{HeartTime}||{ItemCount}");
                            break;
                        case "MapData": // 새로운 MapData 로직 추가
                            string chapter = reader["Chapter"].ToString();
                            string stage = reader["Stage"].ToString();
                            string mapID = reader["MapID"].ToString();
                            string tileValue = reader["TileValue"].ToString();
                            string limitCount = reader["LimitCount"].ToString();
                            string createTime = reader["CreateTime"].ToString();

                            // 데이터를 적절한 포맷으로 저장
                            InputDataAtDictionary("MapData", $"{chapter}||{stage}||{mapID}||{tileValue}||{limitCount}||{createTime}");
                            break;

                        default:
                            Debug.LogError("Invalid query type provided.");
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("MySqlPoolManager"))
                {
                    // 오류 무시하고 로깅만
                    Debug.LogWarning("MySqlPoolManager 관련 오류가 발생했지만 무시합니다: " + ex.Message);
                }
                else
                {
                    // 다른 예외는 다시 던지거나 처리
                    Debug.LogError("Failed to Read data: " + ex.Message);
                    //throw;
                }
            }
        }
    }

    public void UpdateDB(string str)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                // str = "Assets||3666640951||NewNickname||NewProfilePictureURL";

                // strArr[0]은 테이블명, strArr[1]은 회원번호, strArr[2]은 Gold, strArr[3]은 HeartTime, strArr[4]는 ItemCount
                string[] strArr = str.Split("||");

                string query = string.Empty;

                conn.Open();

                switch (strArr[0])
                {
                    case "Assets": // Kakao = 회원번호||이메일||닉네임||프로필사진URL
                        query = $"UPDATE Assets SET Gold = @Gold, HeartTime = @HeartTime, ItemCount = @ItemCount WHERE MemberID = @MemberID";
                        break;
                    default:
                        Debug.LogError("Invalid query type provided.");
                        return;
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);

                // Update문에 필요한 파라미터 바인딩
                cmd.Parameters.AddWithValue("@MemberID", strArr[1]);
                cmd.Parameters.AddWithValue("@Gold", strArr[2]);
                cmd.Parameters.AddWithValue("@HeartTime", strArr[3]);
                cmd.Parameters.AddWithValue("@ItemCount", strArr[4]);

                int rowsAffected = cmd.ExecuteNonQuery();
                Debug.Log($"{rowsAffected} rows updated.");

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("MySqlPoolManager"))
                {
                    // 오류 무시하고 로깅만
                    Debug.LogWarning("MySqlPoolManager 관련 오류가 발생했지만 무시합니다: " + ex.Message);
                }
                else
                {
                    // 다른 예외는 다시 던지거나 처리
                    Debug.LogError("Failed to Read data: " + ex.Message);
                    //throw;
                }
            }
        }
    }

    // Kotlin에서 넘겨받은 카카오톡 유저 데이터
    public void GetAndSetUserData(string userData)
    {
        //userData = "3666640951||dls625@hanmail.net||.||https://img1.kakaocdn.net/thumb/R110x110.q70/?fname=https://t1.kakaocdn.net/account_images/default_profile.jpeg";

        InsertData("MemberInfo", userData);

        // 만들어야 할거!
        //InsertData("Assets", userData);
    }

    public void InputDataAtDictionary(string type, string str)
    {
        string[] strArr = str.Split("||");

        switch (type)
        {
            case "MemberInfo":
                DBDataManager.Instance.UserData.Add("MemberID",strArr[0]);
                DBDataManager.Instance.UserData.Add("Email", strArr[1]);
                DBDataManager.Instance.UserData.Add("Nickname", strArr[2]);
                DBDataManager.Instance.UserData.Add("ProfileURL", strArr[3]);
                DBDataManager.Instance.ShowDicDataCheck("UserData");
                // GuestLogin에서 안불러와져서, 여기서 Assets 데이터 Call.
                ReadData($"Assets||{strArr[0]}");
                break;
            case "Assets":
                DBDataManager.Instance.UserAssetsData.Add("MemberID", strArr[0]);
                DBDataManager.Instance.UserAssetsData.Add("Gold", strArr[1]);
                DBDataManager.Instance.UserAssetsData.Add("HeartTime", strArr[2]);
                DBDataManager.Instance.UserAssetsData.Add("ItemCount", strArr[3]);
                DBDataManager.Instance.ShowDicDataCheck("Assets");
                // 다 불러오면 Scene Change

                // 맵 다운로드 체크 후 SceneChanger
                MapDownloadManager.Instance.CheckMapDownload();

                break;
            case "MapData":
                // MapData의 각 항목을 적절한 키로 Dictionary에 저장
                DBDataManager.Instance.MapData.Add("Chapter", strArr[0]);
                DBDataManager.Instance.MapData.Add("Stage", strArr[1]);
                DBDataManager.Instance.MapData.Add("MapID", strArr[2]);
                DBDataManager.Instance.MapData.Add("TileValue", strArr[3]);
                DBDataManager.Instance.MapData.Add("LimitCount", strArr[4]);
                DBDataManager.Instance.MapData.Add("CreateTime", strArr[5]);
                DBDataManager.Instance.ShowDicDataCheck("MapData");
                break;
            default:
                Debug.LogError("Invalid query type provided.");
                return;
        }

    }
}
