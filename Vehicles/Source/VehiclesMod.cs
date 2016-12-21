using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;

namespace VehiclesMod
{
    static class Items
    {
        public static Item TrainSpawner;
        public static Item VehicleSpawner;
    }

    class VehiclesMod : ITMPlugIn
    {
        #region Fields

        public ITMGame Game;
        public ITMMap Map;
        int count;

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

        static string[] TrainCarNames = new string[]
        {
            "Cars_3-Dome Tank 1",
            "Cars_3-Dome Tank 2",
            "Cars_3-Dome Tank 3",
            "Cars_Commuter Car",
            "Cars_DEV VIC-Y Passenger",
            "Cars_Flat Car",
            "Cars_Flatcar 2",
            "Cars_Flatcar W Containers",
            "Cars_Flatcar W Trailer 1",
            "Cars_Flatcar W Trailer 2",
            "Cars_Gondola 1",
            "Cars_Gondola 2",
            "Cars_Log Car",
            "Cars_Milwaukee Passenger",
            "Cars_NSE Class 159",
            "Cars_Quad Hopper",
            "Cars_Santa Fe Slug",
            "Cars_Stock Car 1",
            "Cars_Stock Car 2",
            "Cars_Stock Car 3",
            "Cars_SW Class 159",
            "Cars_Tank Car 1",
            "Cars_Tank Car 2",
            "Cars_Tank Car 3",
            "Cars_Tank Car 4",
        };

        static string[] VehicleNames = new string[]
        {
            "Vehicles_Car Carrier 1",
            "Vehicles_Delivery Truck 1",
            "Vehicles_Delivery Truck 2",
            "Vehicles_Red Muscle Car",
            "Vehicles_White Muscle Car",
        };

        #endregion

        #region Initialize

        public void Initialize(ITMPluginManager mgr)
        {
            var itemOffset = (Item)mgr.Offsets.ItemID;
            Items.TrainSpawner = itemOffset++;
            Items.VehicleSpawner = itemOffset++;
        }

        public void InitializeGame(ITMGame game)
        {
            Game = game;
            Map = game.Map;
            game.AddNotification("Trains Mod: Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.TrainSpawner, OnTrainSpawnerSwing);
            game.AddEventItemSwing(Items.VehicleSpawner, OnVehicleSpawnerSwing);
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
            engine.Scale = 0.3f;

            var comName = TrainNames[Game.Random.Next(TrainNames.Length)];
            Game.EntityManager.AddEntity("Trub's Trains", comName, engine);
            Game.AddNotification(comName, NotifyRecipient.Local);
        }

        void OnVehicleSpawnerSwing(Item itemID, ITMHand hand)
        {
            var owner = hand.Owner as ITMPlayer;
            if (owner == null) return;
            if (owner.SwingFace == BlockFace.ProxyDefault) return;

            var blockID = (Block)Map.GetBlockID(owner.SwingTarget);
            if (blockID != Block.ColorBlack)
            {
                Game.AddNotification("Must place Vehicle on black road", NotifyRecipient.Local);
                return;
            }

            var engine = new Vehicle(this);
            engine.Position = Map.GetBlockCenter(owner.SwingTarget);
            engine.Position.Y += Map.TileSize * 0.51f;
            engine.Scale = 0.3f;

            var comName = VehicleNames[Game.Random.Next(VehicleNames.Length)];
            Game.EntityManager.AddEntity("Trub's Trains", comName, engine);

            ++count;
            Game.AddNotification(comName + ": " + count.ToString(), NotifyRecipient.Local);
        }

        #endregion
    }
}
