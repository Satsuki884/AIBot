using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RulesPage : MonoBehaviour
{
    private const string FirstTimeKey = "FirstTimeOpen";
    [SerializeField] private TMP_InputField _usersKeyInput;
    [SerializeField] private Button _saveKey;
    

    void Start()
    {
        if (PlayerPrefs.GetInt(FirstTimeKey, 1) == 1)
        {
            gameObject.SetActive(true);
            PlayerPrefs.SetInt(FirstTimeKey, 0);
            PlayerPrefs.Save();
        }
        else
        {
            gameObject.SetActive(false);
        }

        _saveKey.onClick.RemoveAllListeners();
        _saveKey.onClick.AddListener(() =>
            {
                SaveApiKey.Instance.SaveKey(_usersKeyInput.text);
                gameObject.SetActive(false);
            }
        );

    }

    void Update()
    {
        if (_usersKeyInput.text.Length <= 0)
        {
            _saveKey.interactable = false;
        }
        else
        {
            _saveKey.interactable = true;
        }
    }
}