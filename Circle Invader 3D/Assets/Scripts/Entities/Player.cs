using System;
using UnityEngine;

public class Player : CIObject
{
    private Transform _targetPos;

    protected override void Awake()
    {
        base.Awake();
        _targetPos = GetComponentInChildren<Transform>();
    }

    private void Start()
    {
        _targetPos.position = transform.position;
    }
}