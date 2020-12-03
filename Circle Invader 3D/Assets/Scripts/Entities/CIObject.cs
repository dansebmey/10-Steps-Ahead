using System;
using UnityEngine;

public abstract class CIObject : MonoBehaviour
{
    protected GameManager Gm;
    [HideInInspector] public Vector3 targetPos;

    protected virtual void Awake()
    {
        Gm = FindObjectOfType<GameManager>();
    }

    protected virtual void Start()
    {
        Gm.RegisterObject(this);
        targetPos = transform.position;
    }

    protected virtual void Update()
    {
        if (transform.position != targetPos)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.05f);
            AngleTowardsSomething();
        }
    }

    protected virtual void AngleTowardsSomething()
    {
        // by default, do nothing
    }
}