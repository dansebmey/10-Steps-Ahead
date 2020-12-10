using System;

[Serializable]
public class FieldItemManagerData
{
    public int stepsSinceLastPowerup;
    public FieldItemData[] fieldItems;

    public FieldItemManagerData(FieldItemManager fim)
    {
        stepsSinceLastPowerup = fim.StepsSinceLastItemSpawn;
        for (int i = 0; i < fim.ItemsInField.Count; i++)
        {
            fieldItems[i] = new FieldItemData(fim.ItemsInField.ToArray()[i]);
        }
    }

    [Serializable]
    public class FieldItemData
    {
        public string name;
        public int posIndex;
        public int remainingDuration;

        public FieldItemData(FieldItem item)
        {
            name = item.name;
            posIndex = item.CurrentPosIndex;
            remainingDuration = item.RemainingDuration;
        }
    }
}