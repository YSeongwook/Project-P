using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;

public class MySQLManager : MonoBehaviour
{
    private AndroidJavaObject _androidJavaObject;
    private string connectionString;
    [SerializeField] private Text textCheck;

    void Start()
    {
        _androidJavaObject = new AndroidJavaObject("com.unity3d.player.KakaoLogin");
        // 데이터베이스 연결 문자열 설정
        connectionString = "Server=3.38.178.218;Database=ProjectP;User ID=ubuntu;Password=P@ssw0rd!;Pooling=false;SslMode=None;AllowPublicKeyRetrieval=true;";
    }

    public void InsertData(string name)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "INSERT INTO test (name) VALUES (@name)";
                //string query = "INSERT INTO your_table_name (Name, Age) VALUES (@name, @age)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                //cmd.Parameters.AddWithValue("@age", age);
                cmd.ExecuteNonQuery();
                Debug.Log("Data Inserted Successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to insert data: " + ex.Message);
            }
        }
    }

    public void ReadData()
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();
                string query = "SELECT * FROM test";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["name"].ToString();
                    //int age = Convert.ToInt32(reader["Age"]);
                    //Debug.Log("name: " + name);
                    //text.text = $"SELECT * FROM test \n name: {name}";
                    //Debug.Log("Name: " + name + ", Age: " + age);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read data: " + ex.Message);
            }
        }
    }

    //public void FirstLoginSuccess(string str)
    //{
    //    _androidJavaObject.Call("GetUserData");
    //}

    // Kotlin에서 넘겨받은 카카오톡 유저 데이터
    public void GetUserData(string userData)
    {
        string[] strArr = userData.Split("||");

        foreach(string str in strArr)
        {
            textCheck.text += $"{str}\n";
        }
    }
}
