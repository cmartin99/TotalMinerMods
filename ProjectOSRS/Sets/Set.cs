using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectOSRS.Sets
{
    abstract class Set
    {
        public abstract Item GetSetItem();
        public abstract IList<InventoryItem> GetItems();


        public void GiveSet(ITMPlayer player)
        {
            player.AddToInventory(new InventoryItem(this.GetSetItem(), 1));
        }

        public void GiveItems(ITMPlayer player)
        {
            foreach (InventoryItem item in this.GetItems()) {
                player.AddToInventory(item);
            }
        }

        public void ConsumeSet(ITMPlayer player)
        {
            player.Inventory.DecrementItem(this.GetSetItem(), 1);
        }

        public void ConsumeItems(ITMPlayer player)
        {
            foreach(InventoryItem item in this.GetItems())
            {
                player.Inventory.DecrementItem(item.ItemID, item.Count);
            }
        }

        public void ConvertToItems(ITMPlayer player, int setCount)
        {
            Logger.Log(string.Format("Converting {0} to items {1}", this.GetSetItem(), player.Inventory.ItemCount(this.GetSetItem())));
            if (player.Inventory.ItemCount(this.GetSetItem()) >= setCount)
            {
                for (int i = 0; i < setCount; i++)
                {
                    ConsumeSet(player);
                    GiveItems(player);
                }
            }
        }

        public void ConvertToSets(ITMPlayer player, int setCount)
        {
            if (this.GetMaxSetCount(player) >= setCount)
            {
                for (int i = 0; i < setCount; i++)
                {
                    ConsumeItems(player);
                    GiveSet(player);
                }
            }
        }

        public int GetMaxSetCount(ITMPlayer player)
        {
            List<int> itemCounts = new List<int>();

            foreach(InventoryItem item in this.GetItems())
            {
                itemCounts.Add((int)Math.Floor((double)player.Inventory.ItemCount(item.ItemID) / item.Count));
            }

            return itemCounts.Min();
        }

        public int GetItemCount()
        {
            return GetItems().Count;
        }
    }
}
