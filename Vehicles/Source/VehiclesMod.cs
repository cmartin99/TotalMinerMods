using System;
using System.Collections.Generic;
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

        public static string Path;
        public ITMGame Game;
        public ITMMap Map;

        public VehicleDataXML[] Cars;
        public VehicleDataXML[] Trucks;
        public VehicleDataXML[] TrainEngines;
        public VehicleDataXML[] TrainCars;

        #endregion

        #region Initialize

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var itemOffset = (Item)mgr.Offsets.ItemID;
            Items.TrainSpawner = itemOffset++;
            Items.VehicleSpawner = itemOffset++;

            var data = Craig.Engine.Core.Utils.Deserialize1<VehicleDataXML[]>(path + "VehicleData.XML");
            var cars = new List<VehicleDataXML>();
            var trucks = new List<VehicleDataXML>();
            var trains = new List<VehicleDataXML>();
            var traincars = new List<VehicleDataXML>();
            foreach (var d in data)
            {
                if (d.Type == VehicleType.Car) cars.Add(d);
                else if (d.Type == VehicleType.Truck) trucks.Add(d);
                else if (d.Type == VehicleType.TrainEngine) trains.Add(d);
                else if (d.Type == VehicleType.TrainCar) traincars.Add(d);
            }
            Cars = cars.ToArray();
            Trucks = trucks.ToArray();
            TrainEngines = trains.ToArray();
            TrainCars = traincars.ToArray();
        }

        public void InitializeGame(ITMGame game)
        {
            Game = game;
            Map = game.Map;
            game.AddNotification("Trains Mod: Activated", NotifyRecipient.Local);
            game.AddEventItemSwing(Items.TrainSpawner, OnTrainSpawnerSwing);
            game.AddEventItemSwing(Items.VehicleSpawner, OnVehicleSpawnerSwing);
        }

        VehicleDataXML[] GetVehicleTypeArray(VehicleType type)
        {
            switch (type)
            {
                case VehicleType.Car:
                    return Cars;
                case VehicleType.Truck:
                    return Trucks;
                case VehicleType.TrainEngine:
                    return TrainEngines;
                case VehicleType.TrainCar:
                    return TrainCars;
                default:
                    return null;
            }
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
            SpawnVehicle(VehicleType.TrainEngine, hand);
        }

        void OnVehicleSpawnerSwing(Item itemID, ITMHand hand)
        {
            SpawnVehicle(Game.Random.Next(2) == 0 ? VehicleType.Car : VehicleType.Truck, hand);
        }

        void SpawnVehicle(VehicleType type, ITMHand hand)
        {
            var owner = hand.Owner as ITMPlayer;
            if (owner == null) return;
            if (owner.SwingFace == BlockFace.ProxyDefault) return;

            var list = GetVehicleTypeArray(type);
            var data = list[Game.Random.Next(list.Length)];

            var blockID = (Block)Map.GetBlockID(owner.SwingTarget);
            if (blockID != data.Track)
            {
                Game.AddNotification("Must place Vehicle on " + Globals1.ItemData[(int)data.Track].Name, NotifyRecipient.Local);
                return;
            }

            var vehicle = new Vehicle(this, data);
            vehicle.Position = Map.GetBlockCenter(owner.SwingTarget);
            vehicle.Position.Y += Map.TileSize * 0.51f;
            vehicle.ViewDirection = vehicle.Velocity = ClampDirection(new Vector3(owner.ViewDirection.X, 0, owner.ViewDirection.Z)) * data.Speed;
            vehicle.Scale = 0.5f;

            Game.EntityManager.AddEntity("Trub's Trains", data.Name, vehicle);
            Game.AddNotification(data.Name, NotifyRecipient.Local);
        }

        Vector3 ClampDirection(Vector3 dir)
        {
            if (Math.Abs(dir.X) > Math.Abs(dir.Z))
            {
                if (dir.X < 0) return Vector3.Left; else return Vector3.Right;
            }
            else
            {
                if (dir.Z < 0) return Vector3.Forward; else return Vector3.Backward;
            }
        }

        #endregion
    }
}
