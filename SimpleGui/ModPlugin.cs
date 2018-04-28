using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace SimpleGui
{
    public class ModPlugin : ITMPlugin
    {
        #region Fields

        ITMGame game;
        Block lastBlockID;

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
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
        }

        #endregion

        #region Update

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
            var blockID = (Block)game.World.Map.GetBlockID(player.SwingTarget);
            if (blockID != lastBlockID)
            {
                lastBlockID = blockID;
                if (blockID == Block.ColorBlack)
                {
                    var menuScreen = game.OpenPauseMenu(player);
                    var screen = new SimpleScreen(menuScreen, game, player);
                    menuScreen.PushOtherTab(screen);
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        #endregion
    }
}
