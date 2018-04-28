using Microsoft.Xna.Framework;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;

namespace VehiclesMod
{
    public enum VehicleType
    {
        Car,
        Truck,
        TrainEngine,
        TrainCar,
    }

    class Vehicle : Entity
    {
        #region Fields

        public Vector3 Velocity;
        CraigGTAPlugin mod;
        ITMMap map;
        VehicleDataXML data;
        float smokeTimer;
        ParticleData smokeParticle;
        GlobalPoint3D lastDirPoint;

        #endregion

        #region Initialization

        public Vehicle(CraigGTAPlugin mod, VehicleDataXML data)
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

        #endregion

        #region Update

        public override void Update()
        {
            UpdateMovementDirection();

            Position.X += Velocity.X;
            Position.Z += Velocity.Z;

            UpdateSmokeParticles();
        }

        void UpdateMovementDirection()
        {
            if (Velocity.X == 0 && Velocity.Z == 0)
            {
                FindNewDirection();
            }
            else
            {
                ContinueDirection();
            }
        }

        void ContinueDirection()
        {
            float vdx = Velocity.X < 0 ? -0.5f : Velocity.X > 0 ? 0.5f : 0;
            float vdz = Velocity.Z < 0 ? -0.5f : Velocity.Z > 0 ? 0.5f : 0;

            var p = map.GetPoint(Position + new Vector3(vdx, data.GroundOffsetY, vdz));
            if (p != lastDirPoint)
            {
                lastDirPoint = p;
                bool continueDir = true;

                if ((Velocity.X != 0 || Velocity.Z != 0) && mod.Game.Random.Next(2) == 0)
                {
                    if (ChangeDirection(map.GetPoint(Position + new Vector3(0, data.GroundOffsetY, 0)), 0))
                    {
                        UpdateNewMovementDirection();
                        continueDir = false;
                    }
                }

                if (continueDir)
                    ContinueDirection(p, 0);
            }
        }

        void ContinueDirection(GlobalPoint3D p, int yOffset)
        {
            p.Y += yOffset;
            var b = (Block)map.GetBlockID(p);
            p.Y -= yOffset;

            if (b != data.TrackNE && b != data.TrackSW)
            {
                if (yOffset == -1)
                {
                    Velocity.X = 0;
                    Velocity.Z = 0;
                }
                else
                    ContinueDirection(p, yOffset == 0 ? 1 : -1);
            }
            else
            {
                Position.Y += yOffset;
                lastDirPoint = p;
            }
        }

        bool ChangeDirection(GlobalPoint3D p, int yOffset)
        {
            Block b;
            if (Velocity.Z != 0) // north or south
            {
                ++p.X; b = (Block)map.GetBlockID(p); --p.X; // check west
                if (b == data.TrackSW)
                {
                    Velocity.X = 1;
                    Velocity.Z = 0;
                    return true;
                }
                --p.X; b = (Block)map.GetBlockID(p); ++p.X; // check east
                if (b == data.TrackNE)
                {
                    Velocity.X = -1;
                    Velocity.Z = 0;
                    return true;
                }
            }
            else if (Velocity.X != 0) // east or west
            {
                ++p.Z; b = (Block)map.GetBlockID(p); --p.Z; // check north
                if (b == data.TrackNE)
                {
                    Velocity.X = 0;
                    Velocity.Z = 1;
                    return true;
                }
                --p.Z; b = (Block)map.GetBlockID(p); ++p.Z; // check south
                if (b == data.TrackSW)
                {
                    Velocity.X = 0;
                    Velocity.Z = -1;
                    return true;
                }
            }
            return false;
        }
            //--p.Z; // south
            //++p.Z  // north
            //--p.X; // east
            //++p.X  // west

        void FindNewDirection()
        {
            var p = map.GetPoint(Position + new Vector3(0, data.GroundOffsetY, 0));

            ++p.Z; var b = (Block)map.GetBlockID(p); --p.Z;
            if (b == data.TrackNE)
            {
                Velocity.Z = 1;
                Velocity.X = 0;
            }
            else
            {
                --p.X; b = (Block)map.GetBlockID(p); ++p.X;
                if (b == data.TrackNE)
                {
                    Velocity.Z = 0;
                    Velocity.X = -1;
                }
                else
                {
                    --p.Z; b = (Block)map.GetBlockID(p); ++p.Z;
                    if (b == data.TrackSW)
                    {
                        Velocity.Z = -1;
                        Velocity.X = 0;
                    }
                    else
                    {
                        ++p.X; b = (Block)map.GetBlockID(p); --p.X;
                        if (b == data.TrackSW)
                        {
                            Velocity.Z = 0;
                            Velocity.X = 1;
                        }
                    }
                }
            }

            if (Velocity.X != 0 || Velocity.Z != 0)
            {
                UpdateNewMovementDirection();
            }
        }

        void UpdateNewMovementDirection()
        {
            float v1 = (float)(mod.Game.Random.NextDouble() * (data.Speed * 0.2) + data.Speed);
            Velocity.X *= v1;
            Velocity.Z *= v1;
            ViewDirection = Vector3.Normalize(Velocity);
        }

        void UpdateSmokeParticles()
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
        }

        #endregion
    }

    public struct VehicleDataXML
    {
        public string Name;
        public VehicleType Type;
        public float RotY;
        public float Scale;
        public float Speed;
        public Block TrackNE;
        public Block TrackSW;
        public float GroundOffsetY;
        public float SmokeDelay;
        public float SmokeOffsetZ;
    }
}
