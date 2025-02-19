using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChatEcho : IChat
{
    public Task<string> SendMessageToAI(string userMessage)
    {
        // Debug.Log("SendMessageToEcho from ChatEcho");
        return Task.FromResult(userMessage);
    }
}