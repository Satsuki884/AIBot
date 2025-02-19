using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class ChatAI : MonoBehaviour, IChat
{

    private TimeSpan _requestTimeout;
    private HttpClient _httpClient;
    private string _apiUrl;

    public ChatAI(TimeSpan requestTimeout, HttpClient httpClient, string apiUrl)
    {
        _requestTimeout = requestTimeout;
        _httpClient = httpClient;
        _apiUrl = apiUrl;
    }


    public async Task<string> SendMessageToAI(string userMessage)
    {
        Debug.Log("SendMessage To Ai from ChatAI");

        var requestData = new { inputs = userMessage };
        string jsonData = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        try
        {
            using (var cts = new CancellationTokenSource(_requestTimeout))
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_apiUrl, content, cts.Token);
                string responseText = await response.Content.ReadAsStringAsync();
                Debug.Log(responseText);

                string generatedText = null;
                // JObject jsonObject = JObject.Parse(responseText);

                if (responseText.TrimStart().StartsWith("{")) // JSON-объект
                {
                    JObject jsonObject = JObject.Parse(responseText);
                    generatedText = jsonObject["generated_text"]?.ToString();
                }
                else if (responseText.TrimStart().StartsWith("[")) // JSON-массив
                {
                    JArray jsonArray = JArray.Parse(responseText);
                    if (jsonArray.Count > 0)
                    {
                        JObject firstObject = (JObject)jsonArray[0];
                        generatedText = firstObject["generated_text"]?.ToString();
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    Debug.LogError($"[Error]: {response.ReasonPhrase}");
                    return $"<color=red> [Error]: {response.ReasonPhrase}</color>";
                }
                else if (!string.IsNullOrEmpty(generatedText))
                {
                    return $"<color=#0bad61> {generatedText}</color>";
                }
                else
                {
                    Debug.LogError("[Error]: No response from AI.");
                    return "<color=red> [Error]: No response from AI.</color>";
                }
            }
        }
        catch (TaskCanceledException)
        {
            Debug.LogError(" [Error]: The request has exceeded the waiting time.");
            return "<color=red> [Error]: The request has exceeded the waiting time.</color>";
        }
        catch (HttpRequestException e)
        {
            Debug.LogError($" [Error]: {e.Message}");
            return $"<color=red> [Error]: {e.Message}</color>";
        }
        catch (Exception e)
        {
            Debug.LogError($" [Error]: Unexpected error: {e.Message}");
            return $"<color=red> [Error]: Unexpected error: {e.Message}</color>";
        }
    }
}