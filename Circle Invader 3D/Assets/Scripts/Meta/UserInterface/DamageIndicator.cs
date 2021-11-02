using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    private Text textObject;
    private Animator animator;

    public Color goodColour;
    public Color neutralColour;
    public Color badColour;

    private int lastDamageValue;
    private float _timeSinceLastTrigger;

    private void Awake()
    {
        textObject = GetComponent<Text>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _timeSinceLastTrigger += Time.deltaTime;
    }

    public void Trigger(int value)
    {
        if (_timeSinceLastTrigger < 0.2f)
        {
            value += lastDamageValue;
        }
        
        textObject.text = value >= 0 ? "+" + value : value.ToString();
        textObject.color = value < 0 ? badColour : value == 0 ? neutralColour : goodColour;
        
        animator.Play("barrier-damage-indicator-triggered", -1, 0);
        
        _timeSinceLastTrigger = 0;
        lastDamageValue = value;
    }
}
