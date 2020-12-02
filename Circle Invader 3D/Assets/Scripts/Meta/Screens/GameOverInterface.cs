using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverInterface : MonoBehaviour
{
    private Text _gameOverLabel, _scoreLabel, _pressToRetryLabel;

    private void Awake()
    {
        _gameOverLabel = GetComponentsInChildren<Text>()[1];
        _scoreLabel = GetComponentsInChildren<Text>()[2];
        _pressToRetryLabel = GetComponentsInChildren<Text>()[3];

        gameObject.SetActive(false);
    }

    public void OnGameOver(int score)
    {
        gameObject.SetActive(true);

        _scoreLabel.text = "You scored " + score + " points!";
    }
}
