///
/// Lockpick Mod - Created by MrMarooca
/// 
using Craig.BlockWorld;
using Craig.Engine;
using Craig.Engine.Core;
using Craig.TotalMiner;
using Craig.TotalMiner.API;

namespace Lockpick
{
    static class Items
    {
        public static Item Lockpick;
    }

    class PlayerData
    {
        public int LockPickState;
        public Timer LockPickTimer;
        public GlobalPoint3D LockPickDoorPos;
    }

    class Lockpick : ITMPlugin
    {
        #region ITMPlugin

        void ITMPlugin.WorldSaved(int version)
        {
        }

        void ITMPlugin.PlayerJoined(ITMPlayer player)
        {
            player.Tag = new PlayerData();
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
        }

        #endregion

        ITMGame game;
        ITMWorld world;
        ITMMap map;
        public static string modPath;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            var itemOffset = (Item)mgr.Offsets.ItemID;
            Items.Lockpick = itemOffset++;
            modPath = path;
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            this.world = game.World;
            this.map = game.World.Map;
            game.AddNotification("Lockpick Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.Lockpick, OnLockpickSwing);
        }

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
            var playerData = player.Tag as PlayerData;

            if (playerData.LockPickState > 0)
            {
                // Simple FSM to manage the different lock pick states.
                switch (playerData.LockPickState)
                {
                    case 1:
                    case 3:
                        game.AudioManager.PlaySoundFromStream(modPath + "357_reload" + game.Random.Next(1, 4) + ".wav");
                        game.AddNotification("Lockpicking..", NotifyRecipient.Local);
                        playerData.LockPickTimer.Start(1);
                        ++playerData.LockPickState;
                        break;

                    case 2:
                    case 4:
                        playerData.LockPickTimer.Update();
                        if (playerData.LockPickTimer.IsComplete) ++playerData.LockPickState;
                        break;

                    case 5:
                        world.SetPower(playerData.LockPickDoorPos, true, player);
                        map.Commit();
                        playerData.LockPickState = 0;
                        break;
                }
            }
        }

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {            
        }

        void OnLockpickSwing(Item itemID, ITMHand hand)
        {
            var player = hand.Owner as ITMPlayer;
            if (player == null) return;
            if (player.SwingFace == BlockFace.ProxyDefault) return;

            var playerData = player.Tag as PlayerData;
            if (playerData.LockPickState > 0) return;

            var blockID = (Block)map.GetBlockID(player.SwingTarget);
            if (blockID == Block.LockedDoorBottom)
            {
                var p = player.SwingTarget;
                if (world.IsBlockReceivingPower(p))
                {
                    world.SetPower(p, false, player);
                    map.Commit();
                }
                else
                {
                    playerData.LockPickState = 1;
                    playerData.LockPickDoorPos = p;
                }
            }
        }
    }
}
