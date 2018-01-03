using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;

namespace TotalDefender
{
    class TotalDefenderRenderer : IArcadeMachineRenderer
    {
        #region Fields

        TotalDefenderGame game;
        SpriteFont font;
        SpriteBatchSafe spriteBatch;

        #endregion

        #region Initialization

        public void LoadContent(InitState state)
        {
            spriteBatch = CoreGlobals.SpriteBatch;
            font = CoreGlobals.GameFont;
        }

        public void UnloadContent()
        {
        }

        void IArcadeMachineRenderer.LoadTexturePack()
        {
        }

        #endregion

        #region Draw

        public void Draw(ArcadeMachine baseGame)
        {
            game = baseGame as TotalDefenderGame;
            if (game == null) return;

            CoreGlobals.GraphicsDevice.SetRenderTarget(game.RenderTarget);
            CoreGlobals.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, DepthStencilState.None, null);

            if (game.State == TotalDefenderGame.GameState.GameOver)
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
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, game.GameScreenY, game.ScreenSize.X, 1), Color.White);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, game.ScreenSize.Y - 1, game.ScreenSize.X, 1), Color.White);

            for (int i = 0; i < game.Mountains.Length; ++i)
            {
                var m2 = game.Mountains[i + 1];
                spriteBatch.DrawLine(CoreGlobals.BlankTexture, 1, Color.RosyBrown, game.Mountains[i], m2);
                if (m2.X >= game.ScreenSize.X) break;
            }

            spriteBatch.Draw(CoreGlobals.BlankTexture, game.PlayerRect, Color.White);
        }

        void DrawHud()
        {
            spriteBatch.DrawString(font, game.ScoreText, new Vector2(4f, 0f), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
        }

        void DrawGameOver()
        {
            spriteBatch.DrawStringCentered(font, "Total", 40, Color.White, 1.2f);
            spriteBatch.DrawStringCentered(font, "Defender", 70, Color.White, 1.2f);
            spriteBatch.DrawStringCentered(font, "Game Over", 140, Color.White, 0.8f);
            spriteBatch.DrawStringCentered(font, game.CreditText, 180, Color.White, 0.6f);
        }

        #endregion
    }
}
