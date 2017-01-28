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

namespace Dice
{
    static class Items
    {
        public static Item Dice;
    }

    public class Dice : ITMPlugin
    {
        #region Fields

        private ITMGame game;
        private TMHelper helper;
        private Random random;

        #endregion

        #region ITMPlugin

        public void PlayerJoined(ITMPlayer player)
        {
        }

        public void PlayerLeft(ITMPlayer player)
        {
        }

        public void WorldSaved(int version)
        {
        }

        #endregion

        #region Initialization

        public void Initialize(ITMPluginManager mgr, string path)
        {
            var itemOffset = (Item)mgr.Offsets.ItemID;
            Items.Dice = itemOffset++;
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            this.helper = new TMHelper(game);
            this.random = new Random();

            game.AddEventItemSwing(Items.Dice, EventSwingItem);
        }

        #endregion

        #region Update

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
        }

        public void EventSwingItem(Item item, ITMHand hand)
        {
            helper.NotifyAll($"{hand.Owner.ToString() /* PLACEHOLDER, REPLACE WITH NAME FIELD LATER */} rolled a [{random.Next(6) + 1}]");
        }

        #endregion

        #region Draw

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        #endregion
    }
}
