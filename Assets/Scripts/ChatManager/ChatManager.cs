using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _userInput;
    [SerializeField] private Button _sendButton;
    [SerializeField] private TMP_Text _chatOutput;
    [SerializeField] private Transform _content;
    [SerializeField] private ScrollRect _scrollView;
    [SerializeField] private Toggle _chatAI;
    [SerializeField] private Button _refreshButtonPrefab;
    private IChat _chat;
    private string apiUrl = "https://api-inference.huggingface.co/models/facebook/blenderbot-3B";
    private string apiKey;
    private HttpClient httpClient;
    private static readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(100);

    private string lastMessage;

    void Start()
    {
        httpClient = new HttpClient { Timeout = requestTimeout };
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
        _sendButton.onClick.RemoveAllListeners();
        _sendButton.onClick.AddListener(async () =>
        {
            _refreshButtonPrefab.gameObject.SetActive(false);
            string userMessage = _userInput.text;
            _userInput.text = "";
            AppendMessageToChat("You: " + $"<color=#3358ff> {userMessage}</color>");
            await SendMessageToAI(userMessage);
        });

        _chatAI.onValueChanged.RemoveAllListeners();
        // _chatAI.onValueChanged.AddListener((bool isOn) =>
        // {
        // if (isOn)
        // {
        _chat = new ChatAI(requestTimeout, httpClient);
        // }
        // else
        // {
        //     _chat = new ChatEcho();
        // }
        // });

        _refreshButtonPrefab.gameObject.SetActive(false);
        _refreshButtonPrefab.onClick.RemoveAllListeners();
        _refreshButtonPrefab.onClick.AddListener(RefreshChat);

        // _chatAI.isOn = true;
        _chatAI.gameObject.SetActive(false);

        AppendMessageToChat("AI: " + $"<color=#0bad61> Hello! How can I help you today?</color>");
    }

    private async void RefreshChat()
    {
        _refreshButtonPrefab.gameObject.SetActive(false);
        await SendMessageToAI(lastMessage);
    }

    private async Task SendMessageToAI(string userMessage)
    {

        lastMessage = userMessage;

        try
        {
            string response = await _chat.SendMessageToAI(userMessage);
            if (response.Contains("[Error]"))
            {
                _refreshButtonPrefab.gameObject.SetActive(true);
            }
            else
            {
                AppendMessageToChat("AI: " + $"<color=#0bad61> {response}</color>");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            _refreshButtonPrefab.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (_userInput.text.Length <= 0)
        {
            _sendButton.interactable = false;
        }
        else
        {
            _sendButton.interactable = true;
        }
    }


    void AppendMessageToChat(string message)
    {
        TMP_Text sms = Instantiate(_chatOutput, _content);
        sms.text = message;
        // _chatOutput.text += "\n" + message;

        sms.rectTransform.sizeDelta = new Vector2(sms.rectTransform.sizeDelta.x, sms.preferredHeight);

        StartCoroutine(ScrollToBottomWithDelay());
    }

    private IEnumerator ScrollToBottomWithDelay()
    {
        yield return new WaitForEndOfFrame();
        _scrollView.verticalNormalizedPosition = 0f;
    }
}
