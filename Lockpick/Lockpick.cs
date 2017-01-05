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

    enum LockPickStatus
    {
        None,
        PickBroke,
        StartPicking,
        Delay1,
        KeepPicking,
        Delay2,
        Finish
    }

    class PlayerData
    {
        public LockPickStatus LockPickState;
        public Timer LockPickTimer;
        public GlobalPoint3D LockPickDoorPos;
    }

    class Lockpick : ITMPlugin
    {
        #region Fields

        public static string ModPath;

        ITMGame game;
        ITMMap map;
        ITMWorld world;
        SoundEffect[] pickSounds;

        #endregion

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

        #region Initialization

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
            this.map = world.Map;

            // Preload sound effects so gameplay is not stuttered by load.
            pickSounds = new SoundEffect[4];
            pickSounds[0] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "357_reload1.wav");
            pickSounds[1] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "357_reload2.wav");
            pickSounds[2] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "357_reload3.wav");
            pickSounds[3] = game.AudioManager.LoadSoundEffectFromStream(ModPath + "smg1_reload.wav");            

            game.AddNotification("Lockpick Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.Lockpick, OnLockpickSwing);
        }

        #endregion

        #region Update

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
            var playerData = player.Tag as PlayerData;

            if (playerData.LockPickState > LockPickStatus.None)
            {
                switch (playerData.LockPickState)
                {
                    case LockPickStatus.StartPicking:
                    case LockPickStatus.KeepPicking:
                        game.AddNotification("Lockpicking..", NotifyRecipient.Local);
                        pickSounds[game.Random.Next(3)].Play();
                        playerData.LockPickTimer.Start(1);
                        ++playerData.LockPickState;
                        break;

                    case LockPickStatus.Delay1:
                    case LockPickStatus.Delay2:
                        playerData.LockPickTimer.Update();
                        if (playerData.LockPickTimer.IsComplete)
                        {
                            playerData.LockPickState = (game.Random.Next(8) == 0)
                                ? LockPickStatus.PickBroke
                                : playerData.LockPickState + 1;
                        }
                        break;

                    case LockPickStatus.PickBroke:
                        game.AddNotification("Your lockpick has broken!", NotifyRecipient.Local);
                        pickSounds[3].Play();
                        player.Inventory.DecrementItem(Items.Lockpick, 1);
                        playerData.LockPickState = LockPickStatus.None;
                        break;

                    case LockPickStatus.Finish:
                        world.SetPower(playerData.LockPickDoorPos, true, player);
                        map.Commit();
                        playerData.LockPickState = LockPickStatus.None;
                        break;
                }
            }
        }

        void OnLockpickSwing(Item itemID, ITMHand hand)
        {
            var player = hand.Owner as ITMPlayer;
            if (player == null) return;
            if (player.SwingFace == BlockFace.ProxyDefault) return;

            var playerData = player.Tag as PlayerData;
            if (playerData.LockPickState > LockPickStatus.None) return;

            var blockID = (Block)map.GetBlockID(player.SwingTarget);
            if (blockID == Block.LockedDoorBottom)
            {
                if (world.IsBlockReceivingPower(player.SwingTarget))
                {
                    world.SetPower(player.SwingTarget, false, player);
                    map.Commit();
                }
                else
                {
                    playerData.LockPickState = LockPickStatus.StartPicking;
                    playerData.LockPickDoorPos = player.SwingTarget;
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
