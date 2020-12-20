using UnityEngine;

public class FirewallField : FieldItem
{
    public override void OnPickup()
    {
        item.OnConsume();
    }
}