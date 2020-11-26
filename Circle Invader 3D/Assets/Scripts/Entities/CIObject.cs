using System;
using UnityEngine;

public class CIObject : MonoBehaviour
{
    protected GameManager gameManager;
    protected virtual void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
}