using ProjectOSRS.Sets;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectOSRS
{
    class SetRegistry
    {
        public static SetRegistry Instance;
        private Dictionary<Item, Set> _setDictionary;

        public SetRegistry()
        {
            _setDictionary = new Dictionary<Item, Set>();
            SetRegistry.Instance = this;
        }

        public int Count
        {
            get
            {
                return _setDictionary.Count;
            }
        }

        public Dictionary<Item, Set>.KeyCollection Keys
        {
            get
            {
                return _setDictionary.Keys;
            }
        }

        public Dictionary<Item, Set>.ValueCollection Values
        {
            get
            {
                return _setDictionary.Values;
            }
        }

        public void Register(Set set)
        {
            _setDictionary.Add(set.GetSetItem(), set);
        }

        public Set Get(Item item)
        {
            Set set;
            if(_setDictionary.TryGetValue(item, out set))
            {
                return set;
            } else
            {
                return null;
            }
        }

        public void RegisterEvents(ITMGame game)
        {
            foreach(Item item in this.Keys)
            {
                game.AddEventItemSwing(item, ItemSwingEvent);
                Logger.Log(string.Format("Registered {0} swing event", Globals1.ItemData[(int)item].IDString));
            }
        }

        private void ItemSwingEvent(Item item, ITMHand hand)
        {
            Logger.Log(string.Format("{0} swung", item));
            this.Get(item).ConvertToItems(hand.Player, 1);
        }
    }
}
