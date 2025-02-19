using UnityEngine;
using UnityEngine.UI;

public class CloseGame : MonoBehaviour
{
    [SerializeField] private Button _closeChat;

    void Start()
    {
        _closeChat.onClick.RemoveAllListeners();
        _closeChat.onClick.AddListener(CloseChat);
    }

    private void CloseChat(){
        Application.Quit();
    }
}