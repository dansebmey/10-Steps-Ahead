using System;
using UnityEngine;

public class CameraController : MovableObject
{
    private Quaternion _targetRot;

    protected override void Start()
    {
        base.Start();
        ResetZoom();
    }

    public void ZoomOut()
    {
        targetPos = new Vector3(0, 11.33f, -5);
        // _targetRot = new Quaternion(40, 0, 0, 0);
    }

    public void ResetZoom()
    {
        targetPos = new Vector3(0, 2, -2.13f);
        // _targetRot = new Quaternion(15, 0, 0, 0);
    }

    protected override void Update()
    {
        if (transform.localPosition != targetPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 0.05f);
        }

        if (transform.rotation != _targetRot)
        {
            transform.localRotation = Quaternion.Lerp(transform.localRotation, _targetRot, 0.05f);
        }
    }
}