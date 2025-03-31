using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




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
    public InputField NameInputField;
    public Button StartButton;

    private bool gameStarted = false;
    private int score = 0;
    private bool isGameOver = false;
    private int remainingBricks;
    private string playerName = "";
    private List<HighScoreEntry> highScores = new List<HighScoreEntry>();

    void Start()
    {
        Time.timeScale = 0f; // El juego comienza pausado
        LoadHighScores();
        UpdateHighScoreText();
        InitializeBricks();
        StartButton.onClick.AddListener(StartGame);
    }

    private void InitializeBricks()
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
        if (string.IsNullOrEmpty(NameInputField.text))
            return;

        playerName = NameInputField.text;
        NameInputField.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
        Time.timeScale = 1f;

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
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        GameOverText.SetActive(true);
        EndGame();
    }

    private void EndGame()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        SaveHighScore();
    }

    #region High Scores

    [System.Serializable]
    private class HighScoreEntry
    {
        public string Name;
        public int Score;
    }

    private void SaveHighScore()
    {
        LoadHighScores();
        highScores.Add(new HighScoreEntry { Name = playerName, Score = score });
        highScores = highScores.OrderByDescending(entry => entry.Score).Take(5).ToList();

        PlayerPrefs.SetString("HighScores", JsonUtility.ToJson(new HighScoreList { Scores = highScores }));
        PlayerPrefs.Save();

        UpdateHighScoreText();
    }

    private void LoadHighScores()
    {
        if (PlayerPrefs.HasKey("HighScores"))
        {
            string json = PlayerPrefs.GetString("HighScores");
            var loadedScores = JsonUtility.FromJson<HighScoreList>(json);
            if (loadedScores != null)
                highScores = loadedScores.Scores;
        }
    }

    private void UpdateHighScoreText()
    {
        HighScoreText.text = "High Scores:\n";
        foreach (var entry in highScores)
        {
            HighScoreText.text += $"{entry.Name}: {entry.Score}\n";
        }
    }

    [System.Serializable]
    private class HighScoreList
    {
        public List<HighScoreEntry> Scores;
    }

    #endregion
}
