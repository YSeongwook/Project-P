using System;
using System.Data;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;

public class MySQLConnector : MonoBehaviour
{
    private string connectionString;
    [SerializeField] private Text text;
    [SerializeField] private InputField text_Input;

    void Start()
    {
        // 데이터베이스 연결 문자열 설정
        connectionString = "Server=3.38.178.218;Database=ProjectP;User ID=ubuntu;Password=P@ssw0rd!;Pooling=false;SslMode=None;AllowPublicKeyRetrieval=true;";
    }

    public void InsertData(string name)
    {
        name = text_Input.text;
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
                //Debug.Log("Data Inserted Successfully");
                text.text = $"INSERT INTO test (name) VALUES ({name})";
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
                    text.text = $"SELECT * FROM test \n name: {name}";
                    //Debug.Log("Name: " + name + ", Age: " + age);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Failed to read data: " + ex.Message);
            }
        }
    }
}
