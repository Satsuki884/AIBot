using System.Threading.Tasks;
public interface IChat
{
    Task<string> SendMessageToAI(string chatHistory);

}