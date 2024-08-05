using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Security.Cryptography;
using System;

public class SaveValues
{
    public int coins;
    public int selectedVehicleIndex;
    public List<VehicleData> ownedVehicles;
}

public class SaveLoadManager : MonoBehaviour
{
    public static string fileName = "Save";
    public static string path;
    private static readonly string key = "3mLpXJvQxvB5oB8Rj6Z0kFiRDbG34zWZh1+Mm0XkxOc=";

    private void Start()
    {
        //Debug.Log(GenerateKey());
        DontDestroyOnLoad(gameObject);
    }

    public string GenerateKey()
    {
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.GenerateKey();
            return Convert.ToBase64String(aes.Key);
        }
    }

    public static void Save(SaveValues data)
    {
        File.WriteAllText($"{path}/{fileName}.json", Encrypt(JsonUtility.ToJson(data)));
        Debug.Log("File Succesfully Saved...");
    }

    public static SaveValues Load()
    {
        if(path == string.Empty) path = Application.persistentDataPath;
        if (!File.Exists($"{path}/{fileName}.json")) return null;
        SaveValues data = JsonUtility.FromJson<SaveValues>(Decrypt(File.ReadAllText($"{path}/{fileName}.json")));
        Debug.Log("File Succesfully Loaded...");
        return data;
    }

    public static string Encrypt(string data)
    {
        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(data);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    public static string Decrypt(string text)
    {
        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(text);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Convert.FromBase64String(key); ;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
}