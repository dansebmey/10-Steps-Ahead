using System;

[Serializable]
public class GameData
{
    public bool isPlayerDefeated;
    public int playerScore;

    public PlayerData playerData;
    public EnemyData enemyData;
    public BarrierManagerData bmData;
    public FieldItemManagerData fimData;

    public GameData(GameManager gm)
    {
        isPlayerDefeated = gm.player.isDefeated;
        playerScore = gm.PlayerScore;
        
        playerData = new PlayerData(gm.player);
        enemyData = new EnemyData(gm.enemy);
        bmData = new BarrierManagerData(gm.BarrierManager);
        fimData = new FieldItemManagerData(gm.FieldItemManager);
    }
}