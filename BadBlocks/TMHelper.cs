using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner;

namespace BadBlocks
{
    public class TMHelper
    {
        private ITMGame game;

        public TMHelper(ITMGame game)
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

        public Item GetItem(string name)
        {
            return Array.Find(Globals1.ItemData, i => i.IDString == name).ItemID;
        }
    }
}
