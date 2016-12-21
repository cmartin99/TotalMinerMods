using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.Graphics;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;

namespace TrainsMod
{
    class Train : Entity
    {
        TrainsMod mod;
        float smokeTimer;
        Vector3 velocity;

        static ParticleData smokeParticle;
        
        public Train(TrainsMod mod)
        {
            this.mod = mod;

            if (smokeParticle.Duration == 0)
            {
                smokeParticle = new ParticleData() 
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
            }
        }

        public override void Update()
        {
            smokeTimer += Services.ElapsedTime;
            if (smokeTimer > 0.05f)
            {
                var partPos = Position;
                partPos.Y += 1.7f;
                partPos += velocity * 10;
                mod.Game.AddParticle(partPos, ref smokeParticle);
                smokeTimer = 0;
            }

            var p = mod.Game.Map.GetPoint(Position);

            if (velocity.X == 0 && velocity.Z == 0)
            {
                velocity = mod.GetNextTrackDirection(p);
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

                if ((Block)mod.Map.GetBlockID(p) != Blocks.TrainTrackStraight)
                {
                    velocity.X = 0;
                    velocity.Z = 0;
                }
            }

            Position.X += velocity.X;
            Position.Z += velocity.Z;
        }
    }
}
