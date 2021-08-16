using StudioForge.Engine.Integration;
using StudioForge.BlockWorld;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using Microsoft.Xna.Framework;

namespace EntitiesMod
{
    class EntitiesModPlugin : ITMPlugin
    {
        Entity boat;
        ITMGame game;

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
        }

        void ITMPlugin.PlayerLeft(ITMPlayer player)
        {
            game.AddNotification($"Player {player.Gamer.Gamertag} has left");
        }

        void ITMPlugin.InitializeGame(ITMGame game)
        {
            this.game = game;
        }

        void ITMPlugin.Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }

        bool ITMPlugin.HandleInput(ITMPlayer player)
        {
            return false;
        }

        int frame;

        void ITMPlugin.Update(ITMPlayer player)
        {
            if ((++frame % 480) == 0)
                game.AddNotification($"Player {player.Gamer.Gamertag} is updating");

            if (boat != null)
            {
                //boat.Position = player.EyePosition;
                var m = Matrix.Invert(player.ViewMatrix);
                boat.Position.X = m.M41;
                boat.Position.Y = m.M42;
                boat.Position.Z = m.M43;
                boat.ViewDirection = new Vector3(m.M31, m.M32, m.M33);
                boat.ViewDirection = player.ViewDirection;
                boat.DrawRotY = MathHelper.PiOver2;
                boat.DrawOffY = 0;// -3.5f;
            }
        }

        void ITMPlugin.Update()
        {
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
