using System.Threading.Tasks;

public class ChatEcho : IChat
{
    public Task<string> SendMessageToAI(string userMessage)
    {
        return Task.FromResult(userMessage);
    }
}