using UnityEngine;

[CreateAssetMenu(menuName = "Item.Coin")]
public class Coin : Item
{
    public override void OnConsume()
    {
        FindObjectOfType<GameManager>().FieldItemManager.CoinsCollected++;
    }
}