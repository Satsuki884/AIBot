using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    private HttpClient httpClient;
    private static readonly TimeSpan requestTimeout = TimeSpan.FromSeconds(100);

    private string lastMessage;

    void Start()
    {
        httpClient = new HttpClient { Timeout = requestTimeout };
        httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + SaveApiKey.Instance.LoadKey());
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
        _chat = new ChatAI(requestTimeout, httpClient, apiUrl);

        _refreshButtonPrefab.gameObject.SetActive(false);
        _refreshButtonPrefab.onClick.RemoveAllListeners();
        _refreshButtonPrefab.onClick.AddListener(RefreshChat);

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
        var aiMessage = AppendMessageToAIChat();

        try
        {
            string response = await _chat.SendMessageToAI(userMessage);
            if (response.Contains("[Error]"))
            {
                _refreshButtonPrefab.gameObject.SetActive(true);
                aiMessage.text = "AI: " + $"<color=red>Something is wrong :/\nReload answer or send new message)</color>";
                StartCoroutine(ScrollToBottomWithDelay());
            }
            else
            {
                aiMessage.text = "AI: " + $"<color=#0bad61> {response}</color>";
                StartCoroutine(ScrollToBottomWithDelay());
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

        sms.rectTransform.sizeDelta = new Vector2(sms.rectTransform.sizeDelta.x, sms.preferredHeight);

        StartCoroutine(ScrollToBottomWithDelay());
    }

    TMP_Text AppendMessageToAIChat()
    {
        TMP_Text sms = Instantiate(_chatOutput, _content);
        sms.text = "AI: " + $"<color=#0bad61>Texting . . .</color>";

        sms.rectTransform.sizeDelta = new Vector2(sms.rectTransform.sizeDelta.x, sms.preferredHeight);

        StartCoroutine(ScrollToBottomWithDelay());
        return sms;
    }

    private IEnumerator ScrollToBottomWithDelay()
    {
        yield return new WaitForEndOfFrame();
        _scrollView.verticalNormalizedPosition = 0f;
    }
}
