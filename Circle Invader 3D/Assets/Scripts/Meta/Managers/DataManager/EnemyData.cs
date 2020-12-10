using System;
using System.Linq;

[Serializable]
public class EnemyData
{
    public string[] actionQueue;
    public MissileData[] missiles;

    public EnemyData(MechaTotem enemy)
    {
        actionQueue = new string[enemy.queuedActions.Count];
        for (int i = 0; i < enemy.queuedActions.Count; i++)
        {
            actionQueue[i] = enemy.queuedActions.ToArray()[i].name;
        }

        missiles = new MissileData[enemy.MissilesInField.Count];
        for (int i = 0; i < enemy.MissilesInField.Count; i++)
        {
            missiles[i] = new MissileData(enemy.MissilesInField.ToArray()[i]);
        }
    }

    [Serializable]
    public class MissileData
    {
        public int posIndex;
        public int stepsTaken;
        
        public MissileData(Missile missile)
        {
            posIndex = missile.CurrentPosIndex;
            stepsTaken = missile.StepsTaken;
        }
    }
}