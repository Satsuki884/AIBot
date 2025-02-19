using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveApiKey : MonoBehaviour
{
    private string apiKeyFilePath;

    public static SaveApiKey _instance;
    public static SaveApiKey Instance => _instance;

    public async Task Initialize(params object[] param)
    {
        if (_instance != null)
        {
            return;
        }

        _instance = this;


        await Task.Delay(100);
    }

    void Start()
    {
        apiKeyFilePath = Path.Combine(Application.persistentDataPath, "apikey.txt");
    }

    public void SaveKey(string apiKey)
    {
        File.WriteAllText(apiKeyFilePath, apiKey);
        Debug.Log("API Key saved successfully.");
    }

    public string LoadKey()
    {
        if (File.Exists(apiKeyFilePath))
        {
            string apiKey = File.ReadAllText(apiKeyFilePath);
            Debug.Log("API Key loaded successfully.");
            return apiKey;
        }
        else
        {
            Debug.LogWarning("API Key file not found.");
            return null;
        }
    }
}