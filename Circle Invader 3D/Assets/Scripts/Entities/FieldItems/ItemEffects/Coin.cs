using UnityEngine;

[CreateAssetMenu(menuName = "Item.Coin")]
public class Coin : Item
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.FieldItemManager.CoinsCollected++;
        gm.AudioManager.Play("CoinCollected");
    }
}