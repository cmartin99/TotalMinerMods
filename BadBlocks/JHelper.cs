using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BadBlocks
{
    class JHelper
    {
        private ITMGame game;

        public JHelper(ITMGame game)
        {
            this.game = game;
        }

        public void NotifyAll(string text)
        {
            game.AddNotification(text, NotifyRecipient.Local | NotifyRecipient.Remote);
        }

        public void NotifyAdmins(string text)
        {
            game.AddNotification(text, NotifyRecipient.Admin);
        }

        public void NotifyLocal(string text)
        {
            game.AddNotification(text, NotifyRecipient.Local);
        }

        public void NotifyRemote(string text)
        {
            game.AddNotification(text, NotifyRecipient.Remote);
        }

        /*public Item GetItem(string name)
        {
            return null;// Array.Find(Globals1.ItemData, i => i.IDString == name).ItemID;
        }*/
    }
}
