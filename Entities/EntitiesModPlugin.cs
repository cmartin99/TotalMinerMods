using StudioForge.Engine.Integration;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using StudioForge.TotalMiner.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EntitiesMod
{
    class EntitiesModPlugin : ITMPlugin
    {
        Entity boat;
        ITMGame game;
        ParticleData particleData;
        Entity[] dust;
        int dustCounter;

        #region ITMPlugin

        void ITMPlugin.UnloadMod()
        {
        }

        void ITMPlugin.WorldSaved(int version)
        {
        }

        void ITMPlugin.PlayerJoined(ITMPlayer player)
        {
            var b = new Entity();
            if (game.World.EntityManager.AddEntity("entities", "spaceshipcockpit", b) == ErrorCode.Success)
                boat = b;

            particleData = new ParticleData()
            {
                Duration = 8,
                StartColor = Color.White,
                EndColor = Color.White,
                Size = new Vector4(0.1f, 0.1f, 0.1f, 1f),
                WindFactor = 0,
            };

            particleData.Size = new Vector4(1f, 1f, 1f, 1f);
            particleData.StartColor = particleData.EndColor = Color.Red;

            dust = new Entity[100];
            for (int i = 0; i < dust.Length; ++i)
            {
                var d = new Entity() { Scale = (float)(game.Random.NextDouble() * 0.1 + 0.05), Position = new Vector3(1, 1000, 1) };
                dust[i] = d;
                d.ViewDirection = Vector3.Forward;
                game.World.EntityManager.AddEntity("space", "dust", d);
                //game.World.EntityManager.AddEntity("entities", "spaceshipcockpit", d);
            }
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
        }

        void ITMPlugin.InitializeGame(ITMGame game)
        {
            this.game = game;
        }

        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer, Viewport vp)
        {
        }

        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            return false;
        }

        int frame;

        void ITMPlugin.Update(ITMPlayer player)
        {
            if (boat != null)
            {
                boat.Position = player.Position;
                boat.Position += player.EyeOffset;
                boat.ViewDirection = player.ViewDirection;
                boat.DrawRotY = MathHelper.PiOver2;
                boat.DrawOffY = -3.5f;

                if ((frame % 4) == 0 && player.Velocity.Length() > 0.2f)
                {
                    int i = dustCounter++;
                    if (dustCounter >= dust.Length) dustCounter = 0;
                    Vector3 pos = boat.Position + player.ViewDirection * 35f;
                    pos.X += (float)(game.Random.NextDouble() * 40 - 20);
                    pos.Y += (float)(game.Random.NextDouble() * 40 - 20);
                    pos.Z += (float)(game.Random.NextDouble() * 40 - 20);
                    dust[i].Position = pos;
                    game.World.AddParticle(pos, ref particleData);
                }
            }
            ++frame;
        }

        void ITMPlugin.Update()
        {
        }

        void ITMPlugin.Callback(string data, GlobalPoint3D? p, ITMActor actor, ITMActor contextActor)
        {
            game.AddNotification($"I was called back with {data}");
        }

        #endregion

        public static string Path;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
            var typeCounts = new EnumTypeOffsets();
            mgr.RegisterEnumCounts(typeCounts);
        }
    }
}
