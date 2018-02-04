using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace TotalDefenderArcade
{
    class TotalDefenderMod : ITMPlugin
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

        #endregion

        public static string Path;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var typeCounts = new EnumTypeOffsets();
            typeCounts.ArcadeMachine = 1;
            mgr.RegisterEnumCounts(typeCounts);
        }

        public void InitializeGame(ITMGame game)
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

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }
    }
}
