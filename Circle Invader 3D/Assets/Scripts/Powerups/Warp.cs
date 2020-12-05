using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Warp")]
public class Warp : Powerup
 {
     public override void OnConsume()
     {
         GameManager gm = FindObjectOfType<GameManager>();
         gm.CurrentPosIndex += gm.BarrierManager.amountOfBarriers / 2;
     }
 }