using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class BigHammerInterface : GmAwareObject
{
    private Image _circularMeter;
    private float _targetFillAmount;

    protected override void Awake()
    {
        base.Awake();
        _circularMeter = GetComponentsInChildren<Image>()[1];
    }

    public void UpdateMeter()
    {
        _targetFillAmount = Gm.FieldItemManager.CoinsCollected / 4f;
    }

    private void Update()
    {
        if (Math.Abs(_targetFillAmount - _circularMeter.fillAmount) > 0.001f)
        {
            _circularMeter.fillAmount = Mathf.Lerp(_circularMeter.fillAmount, _targetFillAmount, 0.05f);
        }
    }
}
