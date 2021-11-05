using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Item.AntiHammer")]
public class Mine : Item
{
    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.AudioManager.Play("MineTriggered");
        gm.EndGame();
    }
}