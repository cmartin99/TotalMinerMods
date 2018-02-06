using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;

namespace TotalDefenderArcade
{
    struct SpriteAnimation
    {
        public Rectangle[] Rect;
        public SpriteEffects Effects;
    }

    class TotalDefenderRenderer : IArcadeMachineRenderer
    {
        #region Fields

        public static TotalDefenderRenderer TotalDefenderRendererInstance;
        public SpriteAnimation[] SpriteAnimations;
        public Texture2D SpriteSheet;

        TotalDefenderGame game;
        SpriteFont font;
        SpriteBatchSafe spriteBatch;
        Rectangle playerDeathRect;

        #endregion

        #region Initialization

        public TotalDefenderRenderer()
        {
            TotalDefenderRendererInstance = this;
        }

        public void LoadContent(InitState state)
        {
            spriteBatch = CoreGlobals.SpriteBatch;
            font = CoreGlobals.GameFont;

            using (var stream = File.OpenRead(FileSystem.RootPath + TotalDefenderMod.Path + "SpriteSheet.png")) SpriteSheet = Texture2D.FromStream(CoreGlobals.GraphicsDevice, stream);

            SpriteAnimations = new SpriteAnimation[10];
            SpriteAnimations[(int)EntityType.Player] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(0, 39, 15, 6), new Rectangle(22, 39, 15, 6) } };
            SpriteAnimations[(int)EntityType.Humaniod] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(82,0,3,8) } };
            SpriteAnimations[(int)EntityType.Lander] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(110, 13, 9, 8), new Rectangle(126, 13, 9, 8), new Rectangle(140, 13, 9, 8), new Rectangle(1, 26, 9, 8), new Rectangle(15, 26, 9, 8), new Rectangle(31, 26, 9, 8) } };
            SpriteAnimations[(int)EntityType.Mutant] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(0, 0, 9, 8), new Rectangle(15, 0, 10, 8) } };
            SpriteAnimations[(int)EntityType.Bomber] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(91, 0, 6, 8), new Rectangle(105, 0, 6, 8), new Rectangle(117, 0, 6, 8), new Rectangle(131, 0, 6, 8), new Rectangle(143, 0, 6, 8), new Rectangle(1, 13, 6, 8), new Rectangle(13, 13, 6, 8), new Rectangle(27, 13, 6, 8) } };
            SpriteAnimations[(int)EntityType.Pod] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(56, 0, 7, 7), new Rectangle(70, 0, 7, 7) } };
            SpriteAnimations[(int)EntityType.Swarmer] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(88, 13, 5, 4), new Rectangle(100, 13, 5, 4) } };
            SpriteAnimations[(int)EntityType.Baiter] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(45, 26, 11, 4), new Rectangle(63, 26, 11, 4), new Rectangle(79, 26, 11, 4), new Rectangle(97, 26, 11, 4), new Rectangle(113, 26, 11, 4), new Rectangle(131, 26, 11, 4) } };
            SpriteAnimations[(int)EntityType.BomberBomb] = new SpriteAnimation() { Rect = new Rectangle[] { new Rectangle(52, 13, 3, 3), new Rectangle(62, 13, 3, 3), new Rectangle(70, 13, 3, 3), new Rectangle(80, 13, 3, 3) } };

            playerDeathRect = new Rectangle(21, 90, 15, 6);
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

            switch (game.State)
            {
                case TotalDefenderGame.GameState.Play:
                    DrawPlay();
                    DrawRadar();
                    DrawHud();
                    break;

                case TotalDefenderGame.GameState.EndOfWave:
                    DrawHud();
                    DrawEndOfWave();
                    break;

                case TotalDefenderGame.GameState.GameOver:
                    DrawGameOver();
                    break;
            }

            spriteBatch.End();
        }

        void DrawPlay()
        {
            float camX = game.PlayerWorldPos.X - game.PlayerScreenPos.X;

            int i;
            int y = game.HUDHeight + 1;
            Vector2 m2 = Vector2.Zero;
            for (i = 0; i < game.Mountains.Count; ++i)
            {
                var m1 = game.Mountains[i];
                m1.X = game.GetScreenX(m1.X);
                m1.Y += y;
                m2 = game.Mountains[i + 1 == game.Mountains.Count ? 0 : i + 1];
                m2.X = game.GetScreenX(m2.X);
                m2.Y += y;

                if (i + 1 == game.Mountains.Count) 
                    spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle((int)m2.X, y, 1, game.ScreenSize.Y), Color.Red);

                if (m2.X > 0 && m1.X < game.ScreenSize.X)
                    spriteBatch.DrawLine(CoreGlobals.BlankTexture, 1, Color.RosyBrown, m1, m2);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, null);

            var p = new Particle();
            Vector2 pos;
            for (i = 0; i < game.Particles.Length; ++i)
            {
                if (game.Particles[i].Age > 0)
                {
                    p = game.Particles[i];
                    pos.X = game.GetScreenX(p.Position.X);
                    pos.Y = p.Position.Y + y;
                    Color c = p.Color;
                    if (p.Age < 0.3f) c.A = (byte)(MathHelper.Lerp(c.A / 255.0f, 0, (0.3f - p.Age) * 3.333f) * 255f);
                    spriteBatch.Draw(CoreGlobals.BlankTexture, pos, null, c, 0, new Vector2(p.Size * 0.5f, p.Size * 0.5f), p.Size, SpriteEffects.None, 0);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, DepthStencilState.None, null);

            var bulletOrigin = new Vector2(1.0f, 1.0f);
            Entity entity;
            for (i = 0; i < game.Entities.Length; ++i)
            {
                entity = game.Entities[i];
                if (entity.Type != EntityType.None)
                {
                    pos.X = game.GetScreenX(entity.Position.X);
                    pos.Y = entity.Position.Y + y;

                    if (entity.Type != EntityType.EnemyBullet)
                    {
                        DrawAnimatedSprite(pos, entity.Rotation, entity.Type);
                    }
                    else
                    {
                        spriteBatch.Draw(CoreGlobals.BlankTexture, pos, null, Color.White, 0, bulletOrigin, 2, SpriteEffects.None, 0);
                    }
                }
            }

            Vector4 bullet;
            var rect = new Rectangle();
            rect.Height = 1;
            for (i = 0; i < game.BulletsAlive.Length; ++i)
            {
                if (game.BulletsAlive[i])
                {
                    bullet = game.Bullets[i];
                    rect.X = (int)bullet.X;
                    rect.Y = (int)bullet.Y + y;
                    rect.Width = (int)bullet.Z;
                    if (bullet.W < 1) rect.X -= rect.Width;
                    spriteBatch.Draw(CoreGlobals.BlankTexture, rect, Color.Green);
                }
            }

            if (game.PlayerState == EntityState.Default)
            {
                SpriteAnimations[(int)EntityType.Player].Effects = game.PlayerDir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                DrawAnimatedSprite(game.PlayerScreenPos + new Vector2(0, y), 0, EntityType.Player);
            }
            else if (game.PlayerState == EntityState.PlayerDeath)
            {
                Color color = ((game.PlayerDeathTimer / 10) % 2) == 0 ? Color.Red : Color.White;
                spriteBatch.Draw(SpriteSheet, game.PlayerScreenPos + new Vector2(0, y), playerDeathRect, color, 0, new Vector2(playerDeathRect.Width * 0.5f, playerDeathRect.Height * 0.5f), 1, game.PlayerDir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            }
        }

        void DrawAnimatedSprite(Vector2 pos, float rot, EntityType type)
        {
            DrawAnimatedSprite(pos, type, rot, 1);
        }

        void DrawAnimatedSprite(Vector2 pos, EntityType type, float rot, float scale)
        {
            var anim = SpriteAnimations[(int)type];
            int frame = (int)((long)(Services.TotalTime * 10) % (long)anim.Rect.Length);
            var animRect = anim.Rect[frame];
            if (type == EntityType.Mutant)
            {
                pos.X += (float)(game.Random.NextDouble() * 2.0 - 1.0);
                pos.Y += (float)(game.Random.NextDouble() * 2.0 - 1.0);
            }
            spriteBatch.Draw(SpriteSheet, pos, animRect, Color.White, rot, new Vector2(animRect.Width * 0.5f, animRect.Height * 0.5f), scale, anim.Effects, 0);
        }

        void DrawEndOfWave()
        {
            float scale = 0.5f;
            Color color = new Color(100, 255, 100, 255);

            int y = game.HUDHeight + 40;
            spriteBatch.DrawStringCentered(font, "Attack Wave " + game.Wave.ToString(), y, color, scale);
            y += 24;
            spriteBatch.DrawStringCentered(font, "Completed", y, color, scale);
            y += 40;
            int points = game.Wave < 5 ? game.Wave * 100 : 500;
            spriteBatch.DrawStringCentered(font, "Bonus X " + points.ToString() , y, color, scale);
            y += 40;
            int x = 112;
            for (int i = 0; i < 10; i++)
            {
                if (game.Entities[i].Type == EntityType.Humaniod)
                {
                    DrawAnimatedSprite(new Vector2(x, y), EntityType.Humaniod, 0, 1.5f);
                    x += 18;
                }
            }
        }

        void DrawHud()
        {
            var srcRect = new Rectangle(84, 39, 10, 4);
            var rect = new Rectangle(2, 2, 10, 4);
            for (int i = 0; i < Math.Min(5, game.PlayerLives); ++i)
            {
                spriteBatch.Draw(SpriteSheet, rect, srcRect, Color.White);
                rect.X += srcRect.Width + 2;
            }
            spriteBatch.DrawString(font, game.ScoreText, new Vector2(2, 5), Color.White, 0f, Vector2.Zero, 0.4f, SpriteEffects.None, 0f);
            srcRect = new Rectangle(99, 39, 6, 3);
            rect = new Rectangle(62, game.HUDHeight - 5, 6, 3);
            for (int i = 0; i < Math.Min(4, game.PlayerSmartBombs); ++i)
            {
                spriteBatch.Draw(SpriteSheet, rect, srcRect, Color.White);
                rect.Y -= srcRect.Height + 1;
            }

            Color color = Color.Blue;
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(70, 0, 2, game.HUDHeight + 1), color);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(251, 0, 2, game.HUDHeight + 1), color);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(0, game.HUDHeight + 1, game.ScreenSize.X, 1), color);

            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(150, 0, 22, 1), Color.White);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(150, 1, 1, 1), Color.White);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(171, 1, 1, 1), Color.White);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(150, game.HUDHeight + 1, 22, 1), Color.White);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(150, game.HUDHeight, 1, 1), Color.White);
            spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle(171, game.HUDHeight, 1, 1), Color.White);            
        }

        void DrawRadar()
        {
            Vector2 m1 = game.GetRadarSpace(game.Mountains[0]) + game.RadarPos;
            Vector2 m2 = Vector2.Zero;
            for (int i = 0; i < game.Mountains.Count - 1; ++i)
            {
                m2 = game.GetRadarSpace(game.Mountains[i + 1]) + game.RadarPos;
                if (m1.X < m2.X && m2.X - m1.X < game.RadarSize.X * 0.5f)
                    spriteBatch.DrawLine(CoreGlobals.BlankTexture, 1, Color.RosyBrown, m1, m2);
                m1 = m2;
            }
            m2 = game.GetRadarSpace(new Vector2(game.WorldSize.X, game.Mountains[0].Y)) + game.RadarPos;
            if (m1.X < m2.X && m2.X - m1.X < game.RadarSize.X * 0.5f)
                spriteBatch.DrawLine(CoreGlobals.BlankTexture, 1, Color.RosyBrown, m2, m1);

            Vector2 pos;
            Entity entity;
            for (int i = 0; i < game.Entities.Length; ++i)
            {
                entity = game.Entities[i];
                if (entity.Type != EntityType.None && entity.Type != EntityType.EnemyBullet && entity.Type != EntityType.BomberBomb)
                {
                    pos = game.GetRadarSpace(entity.Position) + game.RadarPos;
                    spriteBatch.Draw(CoreGlobals.BlankTexture, new Rectangle((int)pos.X, (int)pos.Y, 1, 1), GetEntityColor(entity.Type));
                }
            }

            pos = game.GetRadarSpace(game.PlayerWorldPos) + game.RadarPos;
            var rect = new Rectangle((int)pos.X - 1, (int)pos.Y, 3, 1);
            spriteBatch.Draw(CoreGlobals.BlankTexture, rect, GetEntityColor(EntityType.Player));
            rect.X++;
            rect.Y--;
            rect.Width = 1;
            rect.Height = 3;
            spriteBatch.Draw(CoreGlobals.BlankTexture, rect, GetEntityColor(EntityType.Player));
        }

        Color GetEntityColor(EntityType type)
        {
            switch (type)
            {
                case EntityType.Player:
                    return Color.White;
                case EntityType.Humaniod:
                    return Color.Pink;
                case EntityType.Lander:
                    return Color.Green;
                case EntityType.Mutant:
                    return Color.Gray;
                case EntityType.Bomber:
                    return Color.BlueViolet;
                case EntityType.Pod:
                    return Color.Purple;
                case EntityType.Swarmer:
                    return Color.Orange;
                case EntityType.Baiter:
                    return Color.LightGreen;
                default:
                    return Color.White;
            }
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
