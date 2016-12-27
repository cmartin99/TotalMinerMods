using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.API;

namespace ArcadeGames
{
    class ArcadeGamesMod : ITMPlugin
    {
        #region ITMPlugin

        void ITMPlugin.WorldSaved(int version)
        {
        }

        #endregion

        public static string Path;
        ITMGame game;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var typeCounts = new EnumTypeOffsets();
            typeCounts.ArcadeMachine = 1;
            mgr.RegisterEnumCounts(typeCounts);
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            game.AddNotification("Arcade Games: Activated", NotifyRecipient.Local);
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
