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
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Game;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Blocks;



namespace Tree_Feller_v1
{
    static class Items
    {
        public static Item TreeFall;
    }

    class Class1 : ITMPlugin
    {
        public ITMHand hand;
        public ITMGame game;
        public ITMMap map;

        #region ITMPlugin
        public void WorldSaved(int version)
        {
        }
        public void PlayerJoined(ITMPlayer player)
        {
        }
        public void PlayerLeft(ITMPlayer player)
        {
        }        
        #endregion


        #region Update
        public void Update(ITMPlayer player)
        {
        }
        public void Update()
        {
        }
        #endregion 


        #region Initialization
        public void Initialize(ITMPluginManager manager, string data)
        {
        }
        public void InitializeGame(ITMGame game)
        {
            game.AddEventItemSwing(Items.TreeFall, onAxeSwing);
        }
        #endregion


        void onAxeSwing(Item item, ITMHand hand)
        {
            /* Check for blocks around it XXX  and above and below OXO
                                          XWX                      OWO
                                          XXX                      0XO
            */
            var player = hand.Owner as ITMPlayer;
            var blockID = map.GetBlockID(player.SwingTarget);

            if(blockID == Block.Wood || blockID == Block.BirchWood)
            {
                game.AddNotification("Has Detected Block", NotifyRecipient.Local);
            }
        }

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

    }
}
