///
/// TreeFeller Mod - Created by TM Charles
///
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace Tree_Feller_v1
{
    static class Items
    {
        public static Item TreeFeller;
    }

    class TreeFellerMod : ITMPlugin
    {
        public static string ModPath;
        ITMGame game;
        ITMWorld world;
        ITMMap map;

        #region ITMPlugin

        public void UnloadMod()
        {
        }
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

        #region Initialization
        public void Initialize(ITMPluginManager manager, string path)
        {
            var itemOffset = (Item)manager.Offsets.ItemID;
            Items.TreeFeller = itemOffset++;
            ModPath = path;
        }
        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            world = game.World;
            map = world.Map;
            game.AddEventItemSwing(Items.TreeFeller, onAxeSwing);
        }
        #endregion

        #region Input
        public bool HandleInput(ITMPlayer player)
        {
            return false;
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

        #region Draw
        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }
        #endregion

        void onAxeSwing(Item item, ITMHand hand)
        {
            /* Check for blocks around it XXX  and above and below OXO
                                          XWX                      OWO
                                          XXX                      0XO
            */
            var player = hand.Player;
            var blockID = map.GetBlockID(player.SwingTarget);

            if(blockID == Block.Wood || blockID == Block.BirchWood)
            {
                game.AddNotification("Has Detected Block", NotifyRecipient.Local);
            }
        }
    }
}
