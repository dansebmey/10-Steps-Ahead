using System;
using UnityEngine;

public class LowPassFilterManager : GmAwareObject, IPlayerCommandListener
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

    public void OnPlayerCommandPerformed(KeyCode keyCode)
    {
        int newFreq = (maxFreq / _totalBarrierHealth) * Gm.BarrierManager.DetermineRemainingBarrierHealth();
        _lowPassFilter.cutoffFrequency = Mathf.Clamp(newFreq, minFreq, maxFreq);
    }
}