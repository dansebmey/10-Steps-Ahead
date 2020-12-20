using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyAction
{
    public string actionName;
    public int scoreReq;
    [Range(0, 50)] public int cooldownInTurns;
    [HideInInspector] public int remainingCooldown;
    public EnemyLayer layerPrefab;
    public Action action;
    
    [Range(-1, 100)] public int chance = 100;
}