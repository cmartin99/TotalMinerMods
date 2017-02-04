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

namespace TestWeapons
{
    public class TestWeapons : ITMPlugin
    {
        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        public void Initialize(ITMPluginManager mgr, string path)
        {
        }

        public void InitializeGame(ITMGame game)
        {
        }

        public void PlayerJoined(ITMPlayer player)
        {
        }

        public void PlayerLeft(ITMPlayer player)
        {
        }

        public bool HandleInput(ITMPlayer player)
        {
            return false;
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
    }
}
