﻿using System;
using UnityEngine;

public class LowPassFilterManager : GmAwareObject
{
    private AudioLowPassFilter _lowPassFilter;
    private int _totalBarrierHealth;
    [SerializeField] private int minFreq = 300;
    [SerializeField] private int maxFreq = 15000;

    protected override void Awake()
    {
        base.Awake();
        _lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    private void Start()
    {
        _totalBarrierHealth = Gm.BarrierManager.amountOfBarriers * Gm.BarrierManager.initBarrierHealth;
    }

    public void UpdateLPFilter()
    {
        int newFreq = (maxFreq / _totalBarrierHealth) * Gm.BarrierManager.remainingBarrierHealth;
        _lowPassFilter.cutoffFrequency = Mathf.Clamp(newFreq, minFreq, maxFreq);
    }
}