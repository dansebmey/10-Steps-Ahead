using System;

[Serializable]
public class PlayerData
{
    public int posIndex;
    public InventoryData inventory;

    public PlayerData(Player player)
    {
        posIndex = player.CurrentPosIndex;
        inventory = new InventoryData(player.Inventory);
    }

    [Serializable]
    public class InventoryData
    {
        public int highlightedItemIndex;
        public string[] carriedPowerupNames;

        public InventoryData(Inventory inventory)
        {
            highlightedItemIndex = inventory.HighlightedItemIndex;
            
            carriedPowerupNames = new string[inventory.carriedPowerups.Count];
            for (int i = 0; i < inventory.carriedPowerups.Count; i++)
            {
                carriedPowerupNames[i] = inventory.carriedPowerups[i].powerupName;
            }
        }
    }
}