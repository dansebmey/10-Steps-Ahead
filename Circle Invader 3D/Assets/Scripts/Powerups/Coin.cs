using UnityEngine;

[CreateAssetMenu(menuName = "Item.Coin")]
public class Coin : Item
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.FieldItemManager.CoinsCollected++;
        gm.Hud.UpdateBigHammerInterface();
        gm.AudioManager.Play("CoinCollected", 0.05f);
    }
}