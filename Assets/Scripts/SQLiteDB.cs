using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SQLiteDB : MonoBehaviour
{
    public string dbName;

    private void Awake()
    {
        dbName = "Data Source=" + Application.persistentDataPath + "/DownloadedFiles.db;Version=3";
    }

    void Start()
    {
        CreateDB();
    }

    public void CreateDB()
    {
        using SqliteConnection connection = new(dbName);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE IF NOT EXISTS files (name VARCHAR(20), type VARCHAR(20), description VARCHAR(20));";
        command.ExecuteNonQuery();

        connection.Close();
    }

    /// <summary>
    /// type 0 = Image, 1 = Video, 2 = PDF, 3 = Others
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="type"></param>
    /// <param name="description"></param>
    public void AddFile(string fileName, int type, string description)
    {
        string fileType = "IMAGE";

        if (type == 0)
        {
            fileType = "IMAGE";
        }
        else if (type == 1)
        {
            fileType = "VIDEO";
        }
        else if (type == 2)
        {
            fileType = "PDF";
        }
        else if (type == 3)
        {
            fileType = "OTHERS";
        }

        using SqliteConnection connection = new(dbName);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "INSERT INTO files (name, type, description) VALUES ('" + fileName + "','" + fileType + "', '" + description + "');";
        command.ExecuteNonQuery();

        connection.Close();
    }

    public List<string> RetrieveAllFiles()
    {
        List<string> data = new();

        using SqliteConnection connection = new(dbName);
        connection.Open();

        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM files;";

        using (IDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                data.Add(reader["name"].ToString());
                data.Add(reader["type"].ToString());
                data.Add(reader["description"].ToString());
            }

            reader.Close();
        }

        connection.Close();

        return data;
    }
}
