using System;
using System.Collections.Generic;
using StudioForge.Engine.Integration;
using StudioForge.Engine;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntitiesMod
{
    class TrafficModPlugin : ITMPlugin
    {
        #region Structs

        struct CarData
        {
            public string ComName;
            public float Speed;
        }

        struct VehicleData
        {
            public string ComName;
            public float Speed;
            public float DrawOffY;
            public float DrawRotY;
        }

        string[] trains = { "Alco C-630M", "Alco RS-3", "Alco S2 DCC", "BLW DT6-6-2000", "Class 17", "Class 56", "EMC TA", "EMD GP40", "EMD GP50",
            "EMD SD50F", "F7 A DCC", "GP 38-2", "GTA V Engine", "IC C40-8W", "Milwaukee Road", "NSE Class 159", "RENFE 272", "SW Class 159" };

        CarData[] cars = {
            new CarData() { ComName = "3-Dome Tank 1", Speed = 1f },
            new CarData() { ComName = "3-Dome Tank 2", Speed = 1f },
            new CarData() { ComName = "3-Dome Tank 3", Speed = 1f },
            new CarData() { ComName = "Commuter Car", Speed = 1f },
            new CarData() { ComName = "DEV VIC-Y Passenger", Speed = 1f },
            new CarData() { ComName = "Flat Car", Speed = 1f },
            new CarData() { ComName = "Flatcar 2", Speed = 1f },
            new CarData() { ComName = "Flatcar W Containers", Speed = 1f },
            new CarData() { ComName = "Flatcar W Trailer 1", Speed = 1f },
            new CarData() { ComName = "Flatcar W Trailer 2", Speed = 1f },
            new CarData() { ComName = "Gondola 1", Speed = 1f },
            new CarData() { ComName = "Gondola 2", Speed = 1f },
            new CarData() { ComName = "Log Car", Speed = 1f },
            new CarData() { ComName = "Milwaukee Passenger", Speed = 1f },
            new CarData() { ComName = "NSE Class 159", Speed = 1f },
            new CarData() { ComName = "Quad Hopper", Speed = 1f },
            new CarData() { ComName = "Santa Fe Slug", Speed = 1f },
            new CarData() { ComName = "Stock Car 1", Speed = 1f },
            new CarData() { ComName = "Stock Car 2", Speed = 1f },
            new CarData() { ComName = "Stock Car 3", Speed = 1f },
            new CarData() { ComName = "SW Class 159", Speed = 1f },
            new CarData() { ComName = "Tank Car 1", Speed = 1f },
            new CarData() { ComName = "Tank Car 2", Speed = 1f },
            new CarData() { ComName = "Tank Car 3", Speed = 1f },
            new CarData() { ComName = "Tank Car 4", Speed = 1f } };

        VehicleData[] vehicles = {
            //new VehicleData() { ComName = "Car Carrier 1", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = 0f },
            new VehicleData() { ComName = "Delivery Truck 1", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = 0 },
            new VehicleData() { ComName = "Delivery Truck 2", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = MathHelper.Pi },
            new VehicleData() { ComName = "Red Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = -MathHelper.PiOver2 },
            new VehicleData() { ComName = "White Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = MathHelper.Pi },
            new VehicleData() { ComName = "Red Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = -MathHelper.PiOver2 },
            new VehicleData() { ComName = "White Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = MathHelper.Pi },
            new VehicleData() { ComName = "Red Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = -MathHelper.PiOver2 },
            new VehicleData() { ComName = "White Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = MathHelper.Pi },
            new VehicleData() { ComName = "Red Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = -MathHelper.PiOver2 },
            new VehicleData() { ComName = "White Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = MathHelper.Pi },
            new VehicleData() { ComName = "Red Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = -MathHelper.PiOver2 },
            new VehicleData() { ComName = "White Muscle Car", Speed = 0.15f, DrawOffY = 0.5f, DrawRotY = MathHelper.Pi },
        };

        struct Spawn
        {
            public Vector3 Pos;
            public Vector3 Dir;
            public float Timer;
        }

        struct EntityData
        {
            public int VehicleID;
            public float DirChangeCheckTimer;
        }

        #endregion

        #region Fields

        int frame;
        ITMGame game;
        int activeEntityCount;
        int entityCount;
        Entity[] entities;
        EntityData[] entityData;
        bool[] entityActive;
        bool[] dirc = new bool[4];
        List<Spawn> spawnPoints;
        Vector3[] dir = { Vector3.Forward, Vector3.Right, Vector3.Backward, Vector3.Left };
        public static string Path;

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
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
        }

        #endregion

        #region Initialization

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var typeCounts = new EnumTypeOffsets();
            mgr.RegisterEnumCounts(typeCounts);
        }

        void ITMPlugin.InitializeGame(ITMGame game)
        {
            this.game = game;
            entities = new Entity[500];
            entityData = new EntityData[500];
            entityActive = new bool[500];
            spawnPoints = new List<Spawn>(100);
            entityCount = 0;

            for (int i = 0; i < entities.Length; ++i)
            {
                entities[i] = new Entity();
                entityData[i] = new EntityData();
                entityActive[i] = false;
            }
        }

        #endregion

        #region Input

        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            return false;
        }

        #endregion

        #region Update

        void ITMPlugin.Update()
        {
            UpdateEntities();
            ++frame;
        }

        void ITMPlugin.Update(ITMPlayer player)
        {
            UpdateSpawnPoints(player);
        }

        void UpdateSpawnPoints(ITMPlayer player)
        {
            float d1 = 30 * 30;
            float d2 = 1000 * 1000;
            Spawn s;
            for (int i = 0; i < spawnPoints.Count; ++i)
            {
                s = spawnPoints[i];
                if (s.Timer < Services.TotalTime)
                {
                    s.Timer = Services.TotalTime + 5;
                    spawnPoints[i] = s;
                    float d = Vector3.DistanceSquared(s.Pos, player.EyePosition);
                    if (d >= d1 && d < d2) SpawnCar(s.Pos, s.Dir);
                }
            }
        }

        void UpdateEntities()
        {
            Vector3 po = Vector3.Zero;
            GlobalPoint3D p;
            Block blockID;
            activeEntityCount = 0;
            Entity e;
            EntityData d;
            for (int i = 0; i < entityCount; ++i)
            {
                if (entityActive[i])
                {
                    ++activeEntityCount;
                    e = entities[i];
                    d = entityData[i];
                    e.Position += e.ViewDirection * vehicles[d.VehicleID].Speed;
                    po.Y = e.DrawOffY - 0.5f;
                    p = game.World.Map.GetPoint(e.Position + po);
                    blockID = game.World.Map.GetBlockID(p);
                    if (blockID == Block.MultiTextureBlock2)
                    {
                        if (d.DirChangeCheckTimer < Services.TotalTime)
                        {
                            Vector3? dir = GetRandomDirection(p);
                            if (dir.HasValue) e.ViewDirection = dir.Value;
                            d.DirChangeCheckTimer = Services.TotalTime + 1f;
                            entityData[i] = d;
                        }
                    }
                    else if (blockID == Block.Grass)
                    {
                        game.World.EntityManager.RemoveEntity(e);
                        entityActive[i] = false;
                        if (i == entityCount - 1) --entityCount;
                    }
                }
            }
        }

        void SpawnCar(Vector3 pos, Vector3 dir)
        {
            int vehicleID = game.Random.Next(vehicles.Length);
            int i = -1;
            for (int j = 0; j < entityCount; ++j)
            {
                if (!entityActive[j])
                {
                    i = j;
                    break;
                }
            }
            if (i < 0)
            {
                if (entityCount >= entities.Length)
                    return;
                i = entityCount++;
            }

            var e = entities[i];
            e.Scale = 1f;
            e.Position = pos;
            e.ViewDirection = dir;
            e.DrawOffY = vehicles[vehicleID].DrawOffY;
            e.DrawRotY = vehicles[vehicleID].DrawRotY;
            if (game.World.EntityManager.AddEntity("trub's trains", "vehicles_" + vehicles[vehicleID].ComName, e) == ErrorCode.Success)
            {
                entityData[i].VehicleID = vehicleID;
                entityData[i].DirChangeCheckTimer = Services.TotalTime + 1f;
                entityActive[i] = true;
            }
        }

        Vector3? GetRandomDirection(GlobalPoint3D p)
        {
            bool found = false;
            dirc[0] = dirc[1] = dirc[2] = dirc[3] = false;

            var blockID = game.World.Map.GetBlockID(p + GlobalPoint3D.Forward);
            if (blockID == Block.MultiTextureBlock2)
            {
                dirc[0] = true;
                found = true;
            }

            blockID = game.World.Map.GetBlockID(p + GlobalPoint3D.Right);
            if (blockID == Block.MultiTextureBlock2)
            {
                dirc[1] = true;
                found = true;
            }

            blockID = game.World.Map.GetBlockID(p + GlobalPoint3D.Backward);
            if (blockID == Block.MultiTextureBlock2)
            {
                dirc[2] = true;
                found = true;
            }

            blockID = game.World.Map.GetBlockID(p + GlobalPoint3D.Left);
            if (blockID == Block.MultiTextureBlock2)
            {
                dirc[3] = true;
                found = true;
            }

            while (found)
            {
                int i = game.Random.Next(4);
                if (dirc[i]) return dir[i];
            }

            return null;
        }

        void ITMPlugin.Callback(string data, GlobalPoint3D? p, ITMActor actor, ITMActor contextActor)
        {
            if (p.HasValue && data.Equals("spawncar", StringComparison.OrdinalIgnoreCase))
            {
                AddSpawnPoint(p.Value);
            }
        }

        void AddSpawnPoint(GlobalPoint3D p)
        {
            Vector3 pos = game.World.Map.GetBlockCenter(p);
            foreach (var s in spawnPoints)
                if (s.Pos == pos) return;

            Vector3 dir = GetRandomDirection(p).Value;
            spawnPoints.Add(new Spawn() { Pos = pos, Dir = dir, Timer = Services.TotalTime });
        }

        #endregion

        #region Draw

        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
            CoreGlobals.SpriteBatch.Begin();
            CoreGlobals.SpriteBatch.DrawString(CoreGlobals.GameFont12, $"Vehicles:{activeEntityCount}", new Vector2(vp.Width - 100, 4), Color.White);
            CoreGlobals.SpriteBatch.End();
        }

        #endregion
    }
}
