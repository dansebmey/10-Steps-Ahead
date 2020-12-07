using UnityEngine;

public class TotemLayer : MovableObject
{
    [HideInInspector] public Quaternion targetRot;

    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.05f);
    }
}