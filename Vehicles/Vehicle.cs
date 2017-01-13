using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;

namespace VehiclesMod
{
    class Vehicle : Entity
    {
        public Vector3 Velocity;
        protected VehiclesMod mod;
        protected ITMMap map;
        protected VehicleDataXML data;
        float smokeTimer;
        ParticleData smokeParticle;

        public Vehicle(VehiclesMod mod, VehicleDataXML data)
        {
            this.mod = mod;
            map = mod.Map;
            this.data = data;
            DrawRotY = data.RotY / 180f * MathHelper.Pi;
            smokeParticle = NewSmokeParticleData();
        }

        protected virtual ParticleData NewSmokeParticleData()
        {
            switch (data.Type)
            {
                case VehicleType.TrainEngine:
                    return new ParticleData()
                    {
                        Duration = 8000,
                        Gravity = 150,
                        EmitPosVariance = new Vector3(0.1f, 0.1f, 0.1f),
                        Size = new Vector4(0.3f, 0.3f, 0.3f, 6f),
                        StartColor = new Color(0.1f, 0.1f, 0.1f, 0.8f),
                        EndColor = new Color(0.8f, 0.8f, 0.8f, 0.1f),
                        Velocity = new Vector3(0, 2f, 0),
                        VelocityVariance = new Vector3(0.2f, 0.2f, 0.2f),
                        VelocityType = ScriptCoordType.Absolute,
                        WindFactor = 0.1f,
                    };
                case VehicleType.Car:
                case VehicleType.Truck:
                    return new ParticleData()
                    {
                        Duration = 200,
                        Gravity = -10,
                        EmitPosVariance = new Vector3(0.02f, 0.02f, 0.02f),
                        Size = new Vector4(0.1f, 0.1f, 0.1f, 2f),
                        StartColor = new Color(0.3f, 0.3f, 0.3f, 0.8f),
                        EndColor = new Color(0.8f, 0.8f, 0.8f, 0.1f),
                        VelocityVariance = new Vector3(0.05f, 0.05f, 0.05f),
                        VelocityType = ScriptCoordType.Absolute,
                        WindFactor = 0.1f,
                    };

                default:
                    data.SmokeDelay = 0;
                    return new ParticleData();
            }
        }

        public override void Update()
        {
            if (data.SmokeDelay > 0)
            {
                smokeTimer += Services.ElapsedTime;
                if (smokeTimer >= data.SmokeDelay)
                {
                    var partPos = Position;
                    partPos.Y += 0.5f;
                    partPos += ViewDirection * -data.SmokeOffsetZ;
                    mod.World.AddParticle(partPos, ref smokeParticle);
                    smokeTimer = 0;
                }
            }

            var p = map.GetPoint(Position + new Vector3(0, data.GroundOffsetY, 0));

            if (Velocity.X == 0 && Velocity.Z == 0)
            {
                ViewDirection = Velocity = GetNextDirection(p);
                float v = (float)(mod.Game.Random.NextDouble() * (data.Speed * 0.2) + data.Speed);
                Velocity.X *= v;
                Velocity.Z *= v;
            }
            else
            {
                if (Velocity.Z < 0)
                {
                    --p.Z;
                }
                else if (Velocity.Z > 0)
                {
                    ++p.Z;
                }
                else if (Velocity.X < 0)
                {
                    --p.X;
                }
                else if (Velocity.X > 0)
                {
                    ++p.X;
                }

                if ((Block)map.GetBlockID(p) != data.Track)
                {
                    Velocity.X = 0;
                    Velocity.Z = 0;
                }
            }

            Position.X += Velocity.X;
            Position.Z += Velocity.Z;
        }

        protected virtual Vector3 GetNextDirection(GlobalPoint3D p)
        {
            if (ViewDirection.X != 0)
            {
                --p.Z;
                if ((Block)map.GetBlockID(p) == data.Track) return Vector3.Forward;
                p.Z += 2;
                if ((Block)map.GetBlockID(p) == data.Track) return Vector3.Backward;
                --p.Z;
                --p.X;
                if ((Block)map.GetBlockID(p) == data.Track) return Vector3.Left;
                return Vector3.Right;
            }
            else
            {
                --p.X;
                if ((Block)map.GetBlockID(p) == data.Track) return Vector3.Left;
                p.X += 2;
                if ((Block)map.GetBlockID(p) == data.Track) return Vector3.Right;
                --p.X;
                --p.Z;
                if ((Block)map.GetBlockID(p) == data.Track) return Vector3.Forward;
                return Vector3.Backward;
            }
        }
    }

    public enum VehicleType
    {
        Car,
        Truck,
        TrainEngine,
        TrainCar,
    }

    public struct VehicleDataXML
    {
        public string Name;
        public VehicleType Type;
        public float RotY;
        public Block Track;
        public float Speed;
        public float GroundOffsetY;
        public float SmokeDelay;
        public float SmokeOffsetZ;
    }
}
