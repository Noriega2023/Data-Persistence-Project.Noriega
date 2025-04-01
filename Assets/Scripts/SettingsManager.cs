using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private InputField nameInput;
    [SerializeField] private Dropdown difficultyDropdown;
    [SerializeField] private Button startButton;

    private const string PlayerNameKey = "PlayerName";
    private const string DifficultyKey = "Difficulty";

    void Start()
    {
        LoadSettings();
        startButton.onClick.AddListener(StartGame); // Al hacer click en Start, inicia el juego
    }

    private void StartGame()
    {
        // Si el nombre no está vacío, guarda la configuración
        if (!string.IsNullOrEmpty(nameInput.text))
        {
            SaveSettings();
            // Cargar la escena del juego (GameScene)
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("Por favor ingresa un nombre antes de comenzar.");
        }
    }

    private void LoadSettings()
    {
        // Si existen configuraciones guardadas, cargarlas
        if (PlayerPrefs.HasKey(PlayerNameKey))
        {
            nameInput.text = PlayerPrefs.GetString(PlayerNameKey);
        }

        if (PlayerPrefs.HasKey(DifficultyKey))
        {
            difficultyDropdown.value = PlayerPrefs.GetInt(DifficultyKey);
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetString(PlayerNameKey, nameInput.text);
        PlayerPrefs.SetInt(DifficultyKey, difficultyDropdown.value);
        PlayerPrefs.Save();
    }
}
