using System;
using UnityEngine;

public abstract class CIObject : MonoBehaviour
{
    protected GameManager Gm;
    [HideInInspector] public Vector3 targetPos;

    protected virtual void Awake()
    {
        if (Gm == null)
        {
            Gm = FindObjectOfType<GameManager>();
            if (Gm == null)
            {
                Debug.Log("Object ["+name+"] initialised with Gm [null]!");
            }
            else
            {
                Debug.Log("Object ["+name+"] found game manager with id ["+Gm.GetInstanceID()+"]");
            }
        }
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
            transform.position = Vector3.Lerp(transform.position, targetPos, 0.25f);
            AngleTowardsSomething();
        }
    }

    protected virtual void AngleTowardsSomething()
    {
        // do nothing
    }
}