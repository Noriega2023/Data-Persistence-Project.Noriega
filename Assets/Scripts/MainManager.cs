using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    public GameObject VictoryText; // Nuevo objeto para mostrar "Victoria"

    private bool m_Started = false;
    private int m_Points;
    private bool m_GameOver = false;
    private int remainingBricks; // Contador de bloques restantes

    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        remainingBricks = LineCount * perLine; // Inicializamos el contador de bloques

        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
                brick.onDestroyed.AddListener(CheckVictory); // Comprobamos si ganamos
            }
        }
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Restaurar velocidad del juego y recargar la escena
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }


    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    void CheckVictory(int points)
    {
        remainingBricks--; // Reducimos la cantidad de bloques restantes
        if (remainingBricks <= 0) // Si ya no quedan bloques
        {
            VictoryText.SetActive(true); // Mostramos el texto de victoria
            m_GameOver = true; // Habilitamos la opci�n de reinicio

            // Pausar el tiempo del juego
            Time.timeScale = 0f;
        }
    }


    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}