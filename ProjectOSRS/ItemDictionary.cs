using StudioForge.Engine.Core;
using StudioForge.TotalMiner;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectOSRS
{
    class ItemDictionary
    {
        public static ItemDictionary Instance;
        private Dictionary<string, Item> _dictionary;

        public ItemDictionary(string modPath, Item offset)
        {
            ItemDictionary.Instance = this;
            _dictionary = new Dictionary<string, Item>(StringComparer.OrdinalIgnoreCase);

            var itemDataPath = Path.Combine(new[] {FileSystem.RootPath, modPath, "ItemData.xml"});
            try
            {
                var itemData = Utils.Deserialize1<ModItemDataXML[]>(itemDataPath);
                foreach (ModItemDataXML item in itemData)
                {
                    _dictionary.Add(item.ItemID, offset++);
                }
                Logger.Log(string.Format("Loaded {0} items from {1}", _dictionary.Keys.Count, itemDataPath));
            } catch (Exception e)
            {
                Logger.LogErr(string.Format("Failed to load items from {0}\n{1}", itemDataPath, e));
            }
        }

        public Item Get(string itemID)
        {
            Item item;
            if (_dictionary.TryGetValue(itemID, out item)) {
                return item;
            } else
            {
                return Item.None;
            }
        }
    }
}
