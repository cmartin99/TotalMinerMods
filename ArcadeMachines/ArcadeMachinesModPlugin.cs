using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace ArcadeMachines
{
    class ArcadeMachinesModPlugin : ITMPlugin
    {
        #region ITMPlugin

        void ITMPlugin.UnloadMod()
        {
        }

        void ITMPlugin.WorldSaved(int version)
        {
        }

        void ITMPlugin.PlayerJoined(ITMPlayer player)
        {
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
        }
        
        void ITMPlugin.InitializeGame(ITMGame game)
        {
        }

        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            return false;
        }

        void ITMPlugin.Update(ITMPlayer player)
        {
        }

        void ITMPlugin.Update()
        {
        }

        #endregion

        public static string Path;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var typeCounts = new EnumTypeOffsets();
            typeCounts.ArcadeMachine = 3;
            mgr.RegisterEnumCounts(typeCounts);
        }
    }
}
