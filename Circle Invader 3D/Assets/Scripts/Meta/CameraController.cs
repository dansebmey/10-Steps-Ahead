using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }
}