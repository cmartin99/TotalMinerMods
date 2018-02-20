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
using StudioForge.BlockWorld;

namespace BadBlocks
{
    public struct BadBlockXML
    {
        public String Name;
    }

    public class BadBlocks : ITMPlugin
    {
        #region Fields

        public static List<Block> badBlocks;
        public static string Path;

        private ITMGame game;
        private TMHelper helper;

        #endregion

        #region ITMPlugin

        void ITMPlugin.UnloadMod()
        {
        }

        public void PlayerJoined(ITMPlayer player)
        {
        }

        public void PlayerLeft(ITMPlayer player)
        {
        }

        public void WorldSaved(int version)
        {
        }

        public bool HandleInput(ITMPlayer player)
        {
            return false;
        }

        #endregion

        #region Initialization

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            BadBlockXML[] data = StudioForge.Engine.Core.Utils.Deserialize1<BadBlockXML[]>(path + "BadBlocksData.xml");
            badBlocks = new List<Block>();

            foreach (BadBlockXML b in data)
            {
                badBlocks.Add((Block)Enum.Parse(typeof(Block), b.Name)); // UNSAFE, Possible better solution?
            }
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            this.helper = new TMHelper(game);

            foreach (Block b in badBlocks)
            {
                game.AddEventBlockMined(b, this.EventBlockMined);
                game.AddEventBlockPlaced(b, this.EventBlockPlaced);
            }
        }

        #endregion

        #region Update

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
        }

        public void EventBlockPlaced(Block block, GlobalPoint3D point, ITMHand hand)
        {
            helper.NotifyAdmins("[" + point.X + ", " + point.Y + ", " + point.Z + "] " + hand.Player.Name + " placed a " + block.ToString() + "! Watch out!");
        }

        public void EventBlockMined(Block block, byte b, GlobalPoint3D point, ITMHand hand)
        {
            helper.NotifyAdmins("[" + point.X + ", " + point.Y + ", " + point.Z + "] " + hand.Player.Name + " mined a " + block.ToString() + "! Watch out!");
        }

        #endregion

        #region Draw

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        #endregion
    }
}
