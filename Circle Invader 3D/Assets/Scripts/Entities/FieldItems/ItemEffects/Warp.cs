using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Warp")]
public class Warp : Powerup
 {
     public override void OnConsume()
     {
         GameManager gm = FindObjectOfType<GameManager>();
         gm.CurrentPosIndex += gm.BarrierManager.amountOfBarriers / 2;
         
         Barrier b = gm.BarrierManager.Barriers[gm.CurrentPosIndex];
         b.IsCollapsed = false;
         if (b.Health < 1)
         {
             b.RestoreHealth(1);
             FindObjectOfType<AudioManager>().Play("PerfectPerf");
             
             EventManager<AchievementManager.AchievementType, int>
                 .Invoke(EventType.IncrementAchievementProgress, AchievementManager.AchievementType.OptimalWarpUse, 1);
         }
         else
         {
             EventManager<AchievementManager.AchievementType, int>
                 .Invoke(EventType.ResetAchievementProgress, AchievementManager.AchievementType.OptimalWarpUse, 0);
         }
     }
 }