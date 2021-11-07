using TMPro;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerup.Hammer")]
public class Hammer : Powerup
{
    public int range;
    public int healValue;

    public override void OnConsume()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        gm.AudioManager.Play("HammerFix");
        
        int hpRestored = gm.BarrierManager.RepairBarriers(range, healValue);
        if (hpRestored == 9)
        {
            FindObjectOfType<AudioManager>().Play("PerfectPerf");
            EventManager<AchievementManager.AchievementType, int>
                .Invoke(EventType.IncrementAchievementProgress, AchievementManager.AchievementType.OptimalHammerUse, 1);
        }
        else
        {
            EventManager<AchievementManager.AchievementType, int>
                .Invoke(EventType.ResetAchievementProgress, AchievementManager.AchievementType.OptimalHammerUse, 0);
        }
    }
}