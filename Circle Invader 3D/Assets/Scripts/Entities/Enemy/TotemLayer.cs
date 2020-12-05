using UnityEngine;

public class TotemLayer : MovableObject
{
    public Quaternion targetRot;

    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 0.05f);
        // transform.rotation = new Quaternion(0, transform.rotation.y, 0, 0);
    }
}