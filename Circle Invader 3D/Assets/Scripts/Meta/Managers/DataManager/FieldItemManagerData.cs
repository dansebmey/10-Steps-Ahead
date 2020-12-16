using System;

[Serializable]
public class FieldItemManagerData
{
    public int stepsSinceLastPowerup;
    public int coinsCollected;
    public FieldItemData[] fieldItems;

    public FieldItemManagerData(FieldItemManager fim)
    {
        stepsSinceLastPowerup = fim.StepsSinceLastItemSpawn;
        coinsCollected = fim.CoinsCollected;
        
        fieldItems = new FieldItemData[fim.ItemsInField.Count];
        for (int i = 0; i < fim.ItemsInField.Count; i++)
        {
            fieldItems[i] = new FieldItemData(fim.ItemsInField.ToArray()[i]);
        }
    }

    [Serializable]
    public class FieldItemData
    {
        public string itemName;
        public int posIndex;
        public int remainingDuration;

        public FieldItemData(FieldItem fieldItem)
        {
            itemName = fieldItem.item.GetType().ToString();
            posIndex = fieldItem.CurrentPosIndex;
            remainingDuration = fieldItem.RemainingDuration;
        }
    }
}