using Craig.Engine;
using Craig.BlockWorld;
using Craig.TotalMiner;
using Craig.TotalMiner.Blocks;
using Craig.TotalMiner.Graphics;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;

namespace CraigMod1
{
    class CraigMod1 : ITMPlugIn
    {
        public static string Path;
        ITMGame game;
        float notifyElapsed;
        float particleElapsed;

        public void Initialize(ITMPluginManager mgr, string path)
        {
            Path = path;
        }

        public void InitializeGame(ITMGame game)
        {
            this.game = game;

            //if (game.CurrentBiome == BiomeType.Grasslands && game.IsEasyDifficulty)
            //{
            //    OreProperties.AddOre(new OreProperty() { BlockID = Block.Diamond, DepositFrequency = 1000, DepositSize = 60, MaxDepth = 0.2f, MinDepth = 0 });
            //    OreProperties.AddOre(new OreProperty() { BlockID = Block.Ruby, DepositFrequency = 1000, DepositSize = 60, MaxDepth = 0.2f, MinDepth = 0 });
            //    OreProperties.AddOre(new OreProperty() { BlockID = Block.None, DepositFrequency = 1000000, DepositSize = 600, MaxDepth = 0.99f, MinDepth = 0 });
            //}

            game.AddNotification("Craig Mod 1: Activated", NotifyRecipient.Local);
        }

        public void Update()
        {
            notifyElapsed += Services.ElapsedTime;
            if (notifyElapsed > 25)
            {
                game.AddNotification("Craig Mod 1: Active", NotifyRecipient.Local);
                notifyElapsed = 0;
            }
        }

        public void Update(ITMPlayer player)
        {
            particleElapsed += Services.ElapsedTime;
            if (particleElapsed > 1)
            {
                //var data = new ParticleData()
                //{
                //    Duration = 8000,
                //    Gravity = 1,
                //    Size = new Vector4(0.2f, 0.2f, 0.2f, 1f),
                //    StartColor = Color.White,
                //    EndColor = Color.White,
                //    VelocityType = ScriptCoordType.Absolute,
                //    Velocity = player.ViewDirection * 10
                //};
                //game.AddParticle(player.EyePosition, ref data);
                particleElapsed = 0;
            }
        }

        public void Draw(ITMPlayer player, ITMPlayer virtualPlayer)
        {
        }
    }
}
