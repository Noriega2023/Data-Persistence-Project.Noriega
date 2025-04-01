using System.Collections.Generic;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private List<HighScoreEntry> highScores = new List<HighScoreEntry>();

    [System.Serializable]
    public class HighScoreEntry
    {
        public string Name;
        public int Score;
    }

    public void SaveNewScore(string playerName, int score)
    {
        HighScoreEntry newEntry = new HighScoreEntry { Name = playerName, Score = score };
        highScores.Add(newEntry);

        // Ordenar por puntuación y mantener solo los 5 mejores
        highScores.Sort((a, b) => b.Score.CompareTo(a.Score));
        if (highScores.Count > 5)
        {
            highScores.RemoveAt(5);
        }

        string json = JsonUtility.ToJson(new { Scores = highScores });
        PlayerPrefs.SetString("HighScores", json);
        PlayerPrefs.Save();
    }

    public void LoadHighScores()
    {
        if (PlayerPrefs.HasKey("HighScores"))
        {
            string json = PlayerPrefs.GetString("HighScores");
            var loadedScores = JsonUtility.FromJson<HighScoreList>(json);
            highScores = loadedScores.Scores;
        }
    }

    public List<HighScoreEntry> GetTopScores()
    {
        return highScores;
    }

    [System.Serializable]
    public class HighScoreList
    {
        public List<HighScoreEntry> Scores;
    }
}
