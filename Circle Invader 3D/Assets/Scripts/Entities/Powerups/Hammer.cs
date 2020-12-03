using UnityEngine;

public class Hammer : Powerup
{
    private float _offset;
    private float _scaleFactor;

    protected override void Start()
    {
        base.Start();
        _scaleFactor = 1.0f / transform.localScale.x;
    }

    protected override void Update()
    {
        base.Update();

        _offset += 0.01f;
        Vector3 pos = transform.position;
        pos = new Vector3(
            pos.x,
            pos.y + _scaleFactor * 0.33f * Mathf.Sin(_offset),
            pos.z);
        transform.position = pos;

        transform.Rotate(new Vector3(0, 0.5f, 0));
        
        // TODO: Replace this with an Animator
    }

    public override void OnPickup()
    {
        base.OnPickup();
        if (Gm.player.AddToInventory(this))
        {
            Destroy(gameObject);
        }
    }
}