using System;
using System.Collections.Generic;
using StudioForge.Engine.GamerServices;
using StudioForge.Engine.Net;
using StudioForge.TotalMiner.API;

namespace TestNet
{
    class TestNetPlugin : ITMPlugin
    {
        #region ITMPlugin

        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            return false;
        }

        void ITMPlugin.Initialize(ITMPluginManager mgr, string path)
        {
        }

        void ITMPlugin.InitializeGame(ITMGame game)
        {
        }

        void ITMPlugin.UnloadMod()
        {
        }

        void ITMPlugin.PlayerJoined(ITMPlayer player)
        {
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
        }

        void ITMPlugin.Update(ITMPlayer player)
        {
        }

        void ITMPlugin.Update()
        {
        }

        void ITMPlugin.WorldSaved(int version)
        {
        }

        #endregion
    }

    class TestNetPluginNet : ITMPluginNet
    {
        ITMNetworkManager ITMPluginNet.GetNetworkManager() 
        { 
            return new NetworkManager();
        }
    }
}
