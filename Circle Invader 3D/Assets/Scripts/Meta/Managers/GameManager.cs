using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BarrierManager BarrierManager { get; private set; }
    public int currentPositionIndex;

    private void Awake()
    {
        BarrierManager = GetComponentInChildren<BarrierManager>();
    }
}