using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.Graphics;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;

namespace VehiclesMod
{
    class Vehicle : Entity
    {
        protected Vector3 velocity;
        protected VehiclesMod mod;
        protected ITMMap map;
        protected float smokeDelay;
        protected Block groundBlock;
        protected float groundBlockOffsetY;
        float smokeTimer;
        ParticleData smokeParticle;

        public Vehicle(VehiclesMod mod)
        {
            this.mod = mod;
            map = mod.Map;
            groundBlock = Block.ColorBlack;
            groundBlockOffsetY = -0.1f;
            smokeDelay = 0;// 0.1f;
            smokeParticle = NewSmokeParticleData();
        }

        protected virtual ParticleData NewSmokeParticleData()
        {
            var data = new ParticleData()
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
            return data;
        }

        public override void Update()
        {
            if (smokeDelay > 0)
            {
                smokeTimer += Services.ElapsedTime;
                if (smokeTimer >= smokeDelay)
                {
                    var partPos = Position;
                    partPos.Y += 1.7f;
                    partPos += velocity * 10;
                    mod.Game.AddParticle(partPos, ref smokeParticle);
                    smokeTimer = 0;
                }
            }

            var p = mod.Game.Map.GetPoint(Position + new Vector3(0, groundBlockOffsetY, 0));

            if (velocity.X == 0 && velocity.Z == 0)
            {
                ViewDirection = velocity = GetNextDirection(p);
                float v = (float)(mod.Game.Random.NextDouble() * 0.02 + 0.05);
                velocity.X *= v;
                velocity.Z *= v;
            }
            else
            {
                if (velocity.Z < 0)
                {
                    --p.Z;
                }
                else if (velocity.Z > 0)
                {
                    ++p.Z;
                }
                else if (velocity.X < 0)
                {
                    --p.X;
                }
                else if (velocity.X > 0)
                {
                    ++p.X;
                }

                if ((Block)mod.Map.GetBlockID(p) != groundBlock)
                {
                    velocity.X = 0;
                    velocity.Z = 0;
                }
            }

            Position.X += velocity.X;
            Position.Z += velocity.Z;
        }

        protected virtual Vector3 GetNextDirection(GlobalPoint3D p)
        {
            --p.Z;
            if ((Block)map.GetBlockID(p) == groundBlock) return Vector3.Forward;
            p.Z += 2;
            if ((Block)map.GetBlockID(p) == groundBlock) return Vector3.Backward;
            --p.Z;
            --p.X;
            if ((Block)map.GetBlockID(p) == groundBlock) return Vector3.Left;

            return Vector3.Right;
        }
    }
}
