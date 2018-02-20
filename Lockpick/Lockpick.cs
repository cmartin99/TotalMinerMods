///
/// Lockpick Mod - Created by MrMarooca
///
using Microsoft.Xna.Framework.Audio;
using StudioForge.BlockWorld;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace Lockpick
{
    static class Items
    {
        public static Item Lockpick;
    }

    enum LockPickStatus
    {
        None,
        Picking,
        Delay,
        Finish,
        PickBroke,
    }

    class PlayerData
    {
        public LockPickStatus LockPickState;
        public Timer LockPickTimer;
        public GlobalPoint3D LockPickDoorPos;
        public int PickAttemptsRequired;
        public int PickAttemptsCount;
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

        void ITMPlugin.UnloadMod()
        {
        }

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
            pickSounds[0] = game.AudioManager.LoadSoundEffectFromStream(FileSystem.RootPath + ModPath + "357_reload1.wav");
            pickSounds[1] = game.AudioManager.LoadSoundEffectFromStream(FileSystem.RootPath + ModPath + "357_reload2.wav");
            pickSounds[2] = game.AudioManager.LoadSoundEffectFromStream(FileSystem.RootPath + ModPath + "357_reload3.wav");
            pickSounds[3] = game.AudioManager.LoadSoundEffectFromStream(FileSystem.RootPath + ModPath + "smg1_reload.wav");            

            game.AddNotification("Lockpick Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.Lockpick, OnLockpickSwing);
            game.AddConsoleCommand(CmdUnlock, "unlock", "Use lockpick on a Locked Door", "Use lockpick on a Locked Door\n\nunlock\n\nExamples:\nunlock  -- unlock the door.");
        }

        void CmdUnlock(string command, ITMGame game, ITMPlayer caller, ITMPlayer player, IOutputLog log)
        {
            OnLockpickSwing(Items.Lockpick, player.RightHand);
        }

        #endregion

        #region Input

        public bool HandleInput(ITMPlayer player)
        {
            return false;
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
                    case LockPickStatus.Picking:
                        game.AddNotification("Lockpicking..", NotifyRecipient.Local);
                        PlayRandSound();
                        playerData.LockPickTimer.Start(1);
                        ++playerData.LockPickState;
                        break;

                    case LockPickStatus.Delay:
                        playerData.LockPickTimer.Update();
                        if (playerData.LockPickTimer.IsComplete)
                        {
                            if (game.Random.Next(8) == 0)
                                playerData.LockPickState = LockPickStatus.PickBroke;
                            else
                                playerData.LockPickState = 
                                    (++playerData.PickAttemptsCount >= playerData.PickAttemptsRequired) 
                                        ? LockPickStatus.Finish : LockPickStatus.Picking;
                        }
                        break;

                    case LockPickStatus.PickBroke:
                        game.AddNotification("Your lockpick has broken!", NotifyRecipient.Local);
                        PlaySound(3);
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
                    playerData.LockPickState = LockPickStatus.Picking;
                    playerData.LockPickDoorPos = player.SwingTarget;
                    playerData.PickAttemptsCount = 0;
                    var b = map.GetBlockID(player.SwingTarget + GlobalPoint3D.Down);
                    playerData.PickAttemptsRequired = b == Block.Granite ? 3 : b == Block.Rhyolite ? 4 : 2;
                }
            }
        }

        void PlayRandSound()
        {
            PlaySound(game.Random.Next(3));
        }

        void PlaySound(int i)
        {
            if (i >= 0 && i < pickSounds.Length && pickSounds[i] != null)
                pickSounds[i].Play();
        }

        #endregion

        #region Draw

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {            
        }

        #endregion
    }
}
