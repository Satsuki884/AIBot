using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private SaveApiKey _saveApiKey;

    private async void Awake()
    {
        if (_saveApiKey == null)
        {
            _saveApiKey = FindObjectOfType<SaveApiKey>();
            if (_saveApiKey == null)
            {
                Debug.LogError("SaveApiKey not found in the scene!");
                return;
            }
        }

        await _saveApiKey.Initialize();
    }
}
