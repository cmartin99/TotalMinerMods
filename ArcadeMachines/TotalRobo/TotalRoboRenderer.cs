using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;

namespace ArcadeMachines.TotalRobo
{
    class AnimatedSprite
    {
        public Rectangle[] SrcRect;
    }

    class TotalRoboRenderer : IArcadeMachineRenderer
    {
        #region Fields

        TotalRoboGame game;
        SpriteFont font;
        SpriteBatchSafe spriteBatch;
        AnimatedSprite[] animatedSprites;
        Texture2D spriteSheet;
        int entitiesAlive;

        #endregion

        #region Initialization

        public void LoadContent(InitState state)
        {
            spriteBatch = CoreGlobals.SpriteBatch;
            font = CoreGlobals.Content.Load<SpriteFont>(@"Fonts\Arcade");

            using (var stream = FileSystem.OpenFile(ArcadeMachinesModPlugin.Path + "RoboSpriteSheet.png", System.IO.FileMode.Open, System.IO.FileAccess.Read))
                spriteSheet = Texture2D.FromStream(CoreGlobals.GraphicsDevice, stream);

            animatedSprites = new AnimatedSprite[(int)EntityType.zLast];
            for (int i = 0; i < animatedSprites.Length; ++i) animatedSprites[i] = new AnimatedSprite();
            animatedSprites[1].SrcRect = new Rectangle[] { new Rectangle(173, 82, 7, 11) };
            animatedSprites[3].SrcRect = new Rectangle[] { new Rectangle(75, 117, 9, 13) };
        }

        public void UnloadContent()
        {
        }

        void IArcadeMachineRenderer.LoadTexturePack()
        {
        }

        #endregion

        #region Helpers

        public Rectangle GetSpriteSrcRect(Entity entity)
        {
            return animatedSprites[(int)entity.Type].SrcRect[0];
        }

        #endregion

        #region Draw

        public void Draw(ArcadeMachine baseGame)
        {
            game = baseGame as TotalRoboGame;
            if (game == null) return;

            CoreGlobals.GraphicsDevice.SetRenderTarget(game.RenderTarget);
            CoreGlobals.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, DepthStencilState.None, null);

            if (game.State == TotalRoboGame.GameState.GameOver)
            {
                DrawGameOver();
                DrawHud();
            }
            else
            {
                DrawPlay();
                DrawHud();
            }

            spriteBatch.End();
        }

        void DrawPlay()
        {
            entitiesAlive = 0;

            foreach (var entity in game.Entities)
            {
                switch (entity.Type)
                {
                    case EntityType.PlayerBullet:
                        DrawPlayerBullet(entity.Position);
                        ++entitiesAlive;
                        break;

                    case EntityType.None:
                        break;

                    default:
                        DrawAnimatedSprite(entity.Type, entity.Position);
                        ++entitiesAlive;
                        break;
                }
            }

            spriteBatch.DrawBox(new Rectangle(0, 20, game.ScreenSize.X, game.ScreenSize.Y - 20), game.BorderSize, Color.Purple, 0);
            spriteBatch.DrawString(font, entitiesAlive.ToString(), Vector2.Zero, entitiesAlive < game.Entities.Length ? Color.White : Color.Red, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0);
        }

        void DrawAnimatedSprite(EntityType type, Vector2 pos)
        {
            if (type != EntityType.PlayerBullet)
            {
                var srcRect = animatedSprites[(int)type].SrcRect[0];
                spriteBatch.Draw(spriteSheet, pos, srcRect, Color.White, 0, new Vector2(srcRect.Width * 0.5f, srcRect.Height * 0.5f), 1, SpriteEffects.None, 0);
            }
        }

        void DrawPlayerBullet(Vector2 pos)
        {
            spriteBatch.Draw(CoreGlobals.BlankTexture, pos, null, Color.Green, 0, new Vector2(1.5f), 3, SpriteEffects.None, 0);
        }

        void DrawHud()
        {
        }

        void DrawGameOver()
        {
            spriteBatch.DrawStringCentered(font, "Total", 40, Color.White, 1.2f);
            spriteBatch.DrawStringCentered(font, "Robo", 70, Color.White, 1.2f);
            spriteBatch.DrawStringCentered(font, "Game Over", 140, Color.White, 0.8f);
            spriteBatch.DrawStringCentered(font, game.CreditText, 180, Color.White, 0.6f);
        }

        #endregion
    }
}
