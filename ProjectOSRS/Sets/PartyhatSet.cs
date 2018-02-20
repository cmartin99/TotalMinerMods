using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StudioForge.TotalMiner;

namespace ProjectOSRS.Sets
{
    class PartyhatSet : Set
    {
        private Item RedPartyhat;
        private Item YellowPartyhat;
        private Item GreenPartyhat;
        private Item BluePartyhat;
        private Item PurplePartyhat;
        private Item WhitePartyhat;
        private Item PartyhatSetItem;

        public PartyhatSet()
        {
            this.RedPartyhat = ItemDictionary.Instance.Get("RedPartyhat");
            this.YellowPartyhat = ItemDictionary.Instance.Get("YellowPartyhat");
            this.GreenPartyhat = ItemDictionary.Instance.Get("GreenPartyhat");
            this.BluePartyhat = ItemDictionary.Instance.Get("BluePartyhat");
            this.PurplePartyhat = ItemDictionary.Instance.Get("PurplePartyhat");
            this.WhitePartyhat = ItemDictionary.Instance.Get("WhitePartyhat");
            this.PartyhatSetItem = ItemDictionary.Instance.Get("PartyhatSet");
        }

        public override IList<InventoryItem> GetItems()
        {
            return new[] {
                new InventoryItem(this.RedPartyhat, 1),
                new InventoryItem(this.YellowPartyhat, 1),
                new InventoryItem(this.GreenPartyhat, 1),
                new InventoryItem(this.BluePartyhat, 1),
                new InventoryItem(this.PurplePartyhat, 1),
                new InventoryItem(this.WhitePartyhat, 1)
            };
        }

        public override Item GetSetItem()
        {
            return this.PartyhatSetItem;
        }
    }
}
