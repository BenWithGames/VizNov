using UnityEngine;
using TMPro;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    private const string PlayerNameKey = "PlayerName";

    // Called by the confirm button
    public void SavePlayerName()
    {
        string playerName = nameInputField.text;

        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerPrefs.SetString(PlayerNameKey, playerName);
            PlayerPrefs.Save();
            Debug.Log("Saved player name: " + playerName);
        }
    }

    // Call this later to load the saved name
    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString(PlayerNameKey, "Player"); // "Player" = default if none saved
    }
}
