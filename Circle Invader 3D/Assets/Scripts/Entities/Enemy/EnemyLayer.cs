using UnityEngine;

public class EnemyLayer : MovableObject
{
    [HideInInspector] public Quaternion targetRot;
    [HideInInspector] public Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.05f);
    }
}