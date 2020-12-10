using System;

[Serializable]
public class BarrierManagerData
{
    public BarrierData[] barriers;

    public BarrierManagerData(BarrierManager bm)
    {
        barriers = new BarrierData[bm.Barriers.Length];
        for (int i = 0; i < bm.Barriers.Length; i++)
        {
            barriers[i] = new BarrierData(bm.Barriers[i]);
        }
    }
    
    [Serializable]
    public class BarrierData
    {
        public int posIndex;
        public int health;

        public BarrierData(Barrier barrier)
        {
            posIndex = barrier.CurrentPosIndex;
            health = barrier.Health;
        }
    }
}