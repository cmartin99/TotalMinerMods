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
    public class Dice : ITMPlugin
    {
        static class Items
        {
            public static Item Dice;
        }

        private ITMGame game;
        private JHelper helper;
        private Random random;

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        public void Initialize(ITMPluginManager mgr, string path)
        {
            var itemOffset = (Item)mgr.Offsets.ItemID;
            Items.Dice = itemOffset++;
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            this.helper = new JHelper(game);
            this.random = new Random();

            game.AddEventItemSwing(Items.Dice, onSwingItem);
        }

        public void PlayerJoined(ITMPlayer player)
        {
        }

        public void PlayerLeft(ITMPlayer player)
        {
        }

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
        }

        public void WorldSaved(int version)
        {
        }

        public void onSwingItem(Item item, ITMHand hand)
        {
            helper.NotifyAll($"{hand.Owner.ToString() /* PLACEHOLDER, REPLACE WITH NAME FIELD LATER */} rolled a [{random.Next(6) + 1}]");
        }
    }
}
