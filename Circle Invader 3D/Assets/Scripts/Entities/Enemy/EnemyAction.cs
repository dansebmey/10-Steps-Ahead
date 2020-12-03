using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EnemyAction
{
    public string name;
    public TotemLayer layerPrefab;
    public Action action;
    
    [Range(-1, 100)] public float chance = 100;
}