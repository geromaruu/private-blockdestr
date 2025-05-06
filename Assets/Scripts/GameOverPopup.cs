using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPopup;

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver(bool isGameOver)
    {
        if (isGameOver)
        {
            Debug.Log("Game Over detected. Showing popup.");
            gameOverPopup.SetActive(true);
        }
    }
}
