using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;

namespace TrainsMod
{
    static class Items
    {
        public static Item TrainSpawner;
    }

    class TrainsMod : ITMPlugIn
    {
        #region Fields

        public ITMGame Game;
        public ITMMap Map;

        static string[] TrainNames = new string[]
        {
            "Locomotives_Alco C-630M",
            "Locomotives_Alco RS-3",
            "Locomotives_Alco S2 DCC",
            "Locomotives_BLW DT6-6-2000",
            //"Locomotives_Class 17",
            "Locomotives_Class 56",
            "Locomotives_EMC TA",
            "Locomotives_EMD GP40",
            "Locomotives_EMD GP50",
            "Locomotives_EMD SD50F",
            "Locomotives_F7 A DCC",
            "Locomotives_GP 38-2",
            "Locomotives_GTA V Engine",
            "Locomotives_IC C40-8W",
            "Locomotives_Milwaukee Road",
            //"Locomotives_NSE Class 159",
            "Locomotives_RENFE 272",
            "Locomotives_SW Class 159",
        };

        #endregion

        #region Initialize

        public void Initialize(ITMPluginManager mgr)
        {
            Items.TrainSpawner = (Item)mgr.Offsets.ItemID;
        }

        public void InitializeGame(ITMGame game)
        {
            Game = game;
            Map = game.Map;
            game.AddNotification("Trains Mod: Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.TrainSpawner, OnTrainSpawnerSwing);
        }

        #endregion

        #region Update

        public void Update()
        {
        }

        public void Update(ITMPlayer player)
        {
        }

        #endregion

        #region Draw

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        #endregion

        #region Event Handlers

        void OnTrainSpawnerSwing(Item itemID, ITMHand hand)
        {
            var owner = hand.Owner as ITMPlayer;
            if (owner == null) return;
            if (owner.SwingFace == BlockFace.ProxyDefault) return;

            var blockID = (Block)Map.GetBlockID(owner.SwingTarget);
            if (blockID != Blocks.TrainTrackStraight)
            {
                Game.AddNotification("Must place Train on straight track", NotifyRecipient.Local);
                return;
            }

            var engine = new Train(this);
            engine.Position = Map.GetBlockCenter(owner.SwingTarget);
            engine.Position.Y -= Map.TileSize * 0.4f;
            engine.ViewDirection = GetNextTrackDirection(owner.SwingTarget);
            engine.Scale = 0.3f;

            var comName = TrainNames[Game.Random.Next(TrainNames.Length)];
            Game.EntityManager.AddEntity("Trub's Trains", comName, engine);
            Game.AddNotification(comName, NotifyRecipient.Local);
        }

        public Vector3 GetNextTrackDirection(GlobalPoint3D p)
        {
            --p.Z;
            if ((Block)Map.GetBlockID(p) == Blocks.TrainTrackStraight) return Vector3.Forward;
            p.Z += 2;
            if ((Block)Map.GetBlockID(p) == Blocks.TrainTrackStraight) return Vector3.Backward;
            --p.Z;
            --p.X;
            if ((Block)Map.GetBlockID(p) == Blocks.TrainTrackStraight) return Vector3.Left;
            
            return Vector3.Right;
        }

        #endregion
    }
}
