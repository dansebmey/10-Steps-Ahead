using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpDisplay : MonoBehaviour
{
    
    private Text
        _scoreShadowLabel,
        _scoreLabel,
        _labelForTesting;

    private GameManager _gmForTesting;

    private BigHammerInterface _bigHammerInterface;

    private void Awake()
    {
        _scoreShadowLabel = GetComponentsInChildren<Text>()[0];
        _scoreLabel = GetComponentsInChildren<Text>()[1];
        _labelForTesting = GetComponentsInChildren<Text>()[2];

        _gmForTesting = FindObjectOfType<GameManager>();

        _bigHammerInterface = GetComponentInChildren<BigHammerInterface>();
    }

    public void UpdateScore(int score)
    {
        _scoreShadowLabel.text = score.ToString();
        _scoreLabel.text = score.ToString();
    }

    public void UpdateBigHammerInterface()
    {
        _bigHammerInterface.UpdateMeter();
    }

    private void Update()
    {
        _labelForTesting.text = _gmForTesting.CurrentState.ToString();
    }
}
