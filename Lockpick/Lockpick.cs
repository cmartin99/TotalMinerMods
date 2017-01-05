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
        public LockPickStatus LockPickState;
        public Timer LockPickTimer;
        public GlobalPoint3D LockPickDoorPos;
        public int pickRan = 0;

        public enum LockPickStatus
        {
            Standby,
            Begin,
            Lockpicking,
            Finish
        }
    }

    class Lockpick : ITMPlugin
    {
        ITMGame game;
        ITMMap map;
        ITMWorld world;
        public static string modPath;
        public const int pickRuns = 2;


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
            this.map = world.Map;

            game.AddNotification("Lockpick Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.Lockpick, OnLockpickSwing);
        }

        public void Update()
        {

        }

        public void Update(ITMPlayer player)
        {
            var playerData = player.Tag as PlayerData;

            if (playerData.LockPickState != PlayerData.LockPickStatus.Standby)
            {
                switch (playerData.LockPickState)
                {
                    case PlayerData.LockPickStatus.Begin:
                        int breakPick = game.Random.Next(1, 8);
                        if (breakPick != game.Random.Next(1, 8))
                        {
                            game.AudioManager.PlaySoundFromStream(modPath + "357_reload" + game.Random.Next(1, 4) + ".wav");
                            game.AddNotification("Lockpicking..", NotifyRecipient.Local);
                            playerData.LockPickTimer.Start(1);
                            playerData.LockPickState = PlayerData.LockPickStatus.Lockpicking;
                            break;
                        }
                        else
                        {
                            foreach (InventoryItem itm in player.Inventory.Items)
                            {
                                if (itm.ItemID == Items.Lockpick)
                                {
                                    player.Inventory.DecrementItem(Items.Lockpick, 1);
                                    game.AudioManager.PlaySoundFromStream(modPath + "smg1_reload.wav");
                                    playerData.LockPickState = PlayerData.LockPickStatus.Standby;
                                    game.AddNotification("Your lockpick has broken!", NotifyRecipient.Local);
                                    break;
                                }
                            }
                            break;
                        }

                    case PlayerData.LockPickStatus.Lockpicking:
                        playerData.LockPickTimer.Update();
                        if (playerData.LockPickTimer.IsComplete)
                        {
                            if (playerData.pickRan == pickRuns - 1)
                            {
                                playerData.LockPickState = PlayerData.LockPickStatus.Finish;
                            }
                            else
                            {
                                ++playerData.pickRan;
                                playerData.LockPickState = PlayerData.LockPickStatus.Begin;
                            }
                        }
                        break;

                    case PlayerData.LockPickStatus.Finish:
                        world.SetPower(playerData.LockPickDoorPos, true, player);
                        map.Commit();
                        playerData.pickRan = 0;
                        playerData.LockPickState = PlayerData.LockPickStatus.Standby;
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
            if (playerData.LockPickState != PlayerData.LockPickStatus.Standby) return;

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
                    playerData.LockPickState = PlayerData.LockPickStatus.Begin;
                    playerData.LockPickDoorPos = p;
                }
            }
        }
    }
}
