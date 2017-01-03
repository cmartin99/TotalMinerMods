///
/// Lockpick Mod - Created by MrMarooca
/// 
using Craig.BlockWorld;
using Craig.Engine;
using Craig.Engine.Core;
using Craig.TotalMiner;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework.Audio;

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

        public static string ModPath;

        ITMGame game;
        ITMWorld world;
        ITMMap map;
        SoundEffect[] pickSounds;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            var itemOffset = (Item)mgr.Offsets.ItemID;
            Items.Lockpick = itemOffset++;
            ModPath = path;
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;
            this.world = game.World;
            this.map = game.World.Map;

            // Preload sound effects so gameplay is not hindered by load.
            pickSounds = new SoundEffect[3];
            pickSounds[0] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "357_reload1.wav");
            pickSounds[1] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "357_reload2.wav");
            pickSounds[2] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "357_reload3.wav");

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
                        pickSounds[game.Random.Next(3)].Play();
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
            var player = hand.Player;
            if (player == null) return;
            if (player.SwingFace == BlockFace.ProxyDefault) return;

            var playerData = player.Tag as PlayerData;
            if (playerData.LockPickState > 0) return; // Already picking a lock. Cannot pick more than one lock at a time.

            var blockID = map.GetBlockID(player.SwingTarget);
            if (blockID == Block.LockedDoorBottom)
            {
                if (world.IsBlockReceivingPower(player.SwingTarget))
                {
                    world.SetPower(player.SwingTarget, false, player);
                    map.Commit();
                }
                else
                {
                    playerData.LockPickState = 1;
                    playerData.LockPickDoorPos = player.SwingTarget;
                }
            }
        }
    }
}
