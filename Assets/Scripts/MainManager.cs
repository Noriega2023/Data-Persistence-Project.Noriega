using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [Header("Game Elements")]
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    [Header("UI Elements")]
    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    public GameObject VictoryText;
    public HighScoreManager highScoreManager;

    private string playerName;
    private bool gameStarted = false;
    private int score = 0;
    private bool isGameOver = false;
    private int remainingBricks;

    void Start()
    {
        // Cargar el nombre y la dificultad guardados desde PlayerPrefs
        playerName = PlayerPrefs.GetString("PlayerName", "Jugador");
        int difficulty = PlayerPrefs.GetInt("Difficulty", 0);

        LoadHighScores(); // Cargar las puntuaciones altas

        // Mostrar las puntuaciones altas
        UpdateHighScoreText();

        // Iniciar el juego con la configuración de dificultad
        InitializeBricks(difficulty);
    }

    private void InitializeBricks(int difficulty)
    {
        const float step = 0.6f;
        int bricksPerLine = Mathf.FloorToInt(4.0f / step);
        int[] pointValues = { 1, 1, 2, 2, 5, 5 };
        remainingBricks = LineCount * bricksPerLine;

        for (int i = 0; i < LineCount; i++)
        {
            for (int x = 0; x < bricksPerLine; x++)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointValues[i];
                brick.onDestroyed.AddListener(AddPoints);
                brick.onDestroyed.AddListener(CheckVictoryCondition);
            }
        }
    }

    public void StartGame()
    {
        if (gameStarted) return;

        gameStarted = true;
        Ball.transform.SetParent(null);
        Ball.velocity = Vector3.zero;

        float randomDirection = Random.Range(-1.0f, 1.0f);
        Vector3 forceDirection = new Vector3(randomDirection, 1, 0).normalized;
        Ball.AddForce(forceDirection * 2.0f, ForceMode.VelocityChange);
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.Space))
        {
            // Al presionar espacio, volvemos a la escena de configuración
            SceneManager.LoadScene("SettingsScene");
        }
    }

    private void AddPoints(int points)
    {
        score += points;
        ScoreText.text = $"Score: {score}";
    }

    private void CheckVictoryCondition(int _)
    {
        remainingBricks--;
        if (remainingBricks <= 0)
        {
            VictoryText.SetActive(true);
            EndGame();
        }
    }

    public void GameOver()
    {
        isGameOver = true;
        GameOverText.SetActive(true);
        highScoreManager.SaveNewScore(playerName, score);
    }

    private void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        highScoreManager.SaveNewScore(playerName, score);
    }

    // Cargar puntuaciones altas
    private void LoadHighScores()
    {
        highScoreManager.LoadHighScores();
    }

    private void UpdateHighScoreText()
    {
        HighScoreText.text = "High Scores:\n";
        foreach (var entry in highScoreManager.GetTopScores())
        {
            HighScoreText.text += $"{entry.Name}: {entry.Score}\n";
        }
    }
}
