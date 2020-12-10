using System;

[Serializable]
public class GameData
{
    public PlayerData playerData;
    public EnemyData enemyData;
    public BarrierManagerData bmData;
    public FieldItemManagerData fimData;

    public GameData(GameManager gm)
    {
        playerData = new PlayerData(gm.player);
        enemyData = new EnemyData(gm.enemy);
        bmData = new BarrierManagerData(gm.BarrierManager);
        fimData = new FieldItemManagerData(gm.FieldItemManager);
    }
}