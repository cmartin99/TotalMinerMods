using System;
using System.Collections.Generic;
using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;
using VehiclesMod.Screens;

namespace VehiclesMod
{
    static class Items
    {
        public static Item TrainSpawner;
        public static Item VehicleSpawner;
    }

    class VehiclesMod : ITMPlugin
    {
        #region ITMPlugin

        void ITMPlugin.WorldSaved(int version)
        {
        }

        #endregion

        #region Fields

        public static string Path;
        public ITMGame Game;
        public ITMMap Map;

        public VehicleDataXML[] Cars;
        public VehicleDataXML[] Trucks;
        public VehicleDataXML[] TrainEngines;
        public VehicleDataXML[] TrainCars;

        int spawnCount;
        float autoSpawnTimer;
        List<Item> autoSpawnItems = new List<Item>();
        List<GlobalPoint3D> autoSpawnPoint = new List<GlobalPoint3D>();
        List<Vector3> autoSpawnDir = new List<Vector3>();

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
            game.AddItemCustomSetup(Item.Rasta, Permissions.Admin);
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
            UpdateAutoSpawns();
        }

        public void Update(ITMPlayer player)
        {
        }

        void UpdateAutoSpawns()
        {
            autoSpawnTimer += Services.ElapsedTime;
            if (autoSpawnTimer > 8)
            {
                autoSpawnTimer = 0;
                for (int i = 0; i < autoSpawnPoint.Count; ++i)
                {
                    var item = autoSpawnItems[i];
                    var type = item == Items.TrainSpawner ? VehicleType.TrainEngine : Game.Random.Next(2) == 0 ? VehicleType.Car : VehicleType.Truck;
                    SpawnVehicle(type, autoSpawnPoint[i], autoSpawnDir[i]);
                }
            }
        }

        #endregion

        #region Draw

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
            //CoreGlobals.SpriteBatch.Begin();
            //CoreGlobals.SpriteBatch.DrawString(CoreGlobals.GameFont, "Hello", new Vector2(100, 100), Color.White);
            //CoreGlobals.SpriteBatch.End();
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

            if (hand.HandType == InventoryHand.Left)
            {
                AutoSpawnVehicle(type, hand);
            }
            else
            {
                SpawnVehicle(type, owner.SwingTarget, owner.ViewDirection);
            }
        }

        void SpawnVehicle(VehicleType type, GlobalPoint3D p, Vector3 dir)
        {
            var list = GetVehicleTypeArray(type);
            var data = list[Game.Random.Next(list.Length)];

            var vehicle = new Vehicle(this, data);
            vehicle.Position = Map.GetBlockCenter(p);
            vehicle.Position.Y += Map.TileSize * 0.51f;
            vehicle.ViewDirection = ClampDirection(new Vector3(dir.X, 0, dir.Z));
            vehicle.Velocity = vehicle.ViewDirection * data.Speed;
            vehicle.Scale = 0.5f;
            vehicle.DrawOffY = type == VehicleType.TrainCar || type == VehicleType.TrainEngine ? -1.8f : 0;

            Game.EntityManager.AddEntity("Trub's Trains", data.Name, vehicle);
            Game.AddNotification(string.Format("{0}: {1}", data.Name, ++spawnCount), NotifyRecipient.Local);
        }

        void AutoSpawnVehicle(VehicleType type, ITMHand hand)
        {
            var owner = hand.Owner as ITMPlayer;

            for (int i = autoSpawnPoint.Count - 1; i >= 0; --i)
            {
                if (autoSpawnPoint[i] == owner.SwingTarget)
                {
                    autoSpawnPoint.RemoveAt(i);
                    autoSpawnItems.RemoveAt(i);
                    autoSpawnDir.RemoveAt(i);
                    return;
                }
            }

            autoSpawnItems.Add(hand.ItemID);
            autoSpawnPoint.Add(owner.SwingTarget);
            autoSpawnDir.Add(owner.ViewDirection);
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
