using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace TotalDefenderArcade
{
    enum EntityType
    {
        None,
        Player,
        Humaniod,
        Lander,
        Mutant,
        Bomber,
        Pod,
        Swarmer,
        Baiter,
        BomberBomb,
        EnemyBullet,
    }

    enum EntityState
    {
        Default,
        LanderPickupHumanoid,
        LanderAbductHumanoid,
        HumanoidBeingAbducted,
        HumanoidFalling,
        PlayerDeath,
        PlayerExplosion,
        Spawning,
    }

    struct Entity
    {
        public EntityType Type;
        public Vector2 Position;
        public Vector2 Velocity;
        public EntityState State;
        public float Rotation;
        public float RotationVelocity;
        public float Age;
        public object StateData;
    }

    delegate void ParticleExpired(ref Particle p);

    struct Particle
    {
        public float Age;
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Size;
        public ParticleExpired Expired;
        public object Tag;
    }

    class TotalDefenderGame : ArcadeMachine
    {
        #region Enum

        public enum GameState
        {
            Play,
            EndOfWave,
            GameOverTransition,
            GameOver
        }

        #endregion

        #region Fields

        public GameState State;
        public Point ScreenSize;
        public PcgRandom Random;
        public string ScoreText;
        public int HUDHeight;
        public Vector2 PlayerWorldPos;
        public Vector2 PlayerScreenPos;
        public Rectangle PlayerShipScreenRect;
        public EntityState PlayerState;
        public int PlayerDeathTimer;
        public float PlayerSpawnTimer;
        public float PlayerDir;
        public int PlayerLives;
        public int PlayerSmartBombs;
        public Point WorldSize;
        public List<Vector2> Mountains;
        public float[] MountainHeightMap;
        public Vector4[] Bullets;
        public bool[] BulletsAlive;
        public Entity[] Entities;
        public Particle[] Particles;
        public int Wave;
        public Vector2 RadarPos;
        public Point RadarSize;
        public int StarCount = 50;
        public int HumanoidCount = 10;

        float gameOverTransitionTimer;
        int score;
        Vector2 playerVel;
        Vector2 playerSpeed;
        float playerMaxVelX;
        float playerAccelerationX;
        bool upKeyDom;
        float playerEdgeSettle;
        float autoRepeatFireTimer;
        int[] humanoidPassengers;
        float landerLateSpawnTimer;
        float endOfWaveTimer;
        float baiterSpawnTimer;
        Vector2 baiterMaxVelocity;
        Point[] entityBounds;
        Color[] spriteColors;
        Color[] starColors = { Color.Red, Color.Green, Color.Yellow, Color.Blue, Color.White, Color.Purple, Color.Orange };

        #endregion

        #region Properties

        public override bool CanDeactivate { get { return State == GameState.GameOver; } }

        #endregion

        #region Initialization

        public TotalDefenderGame(ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D point, BlockFace face)
            : base(game, map, player, point, face)
        {
        }

        public override void LoadContent(InitState state)
        {
            base.LoadContent(state);

            ScreenSize = new Point(renderTarget.Width, renderTarget.Height);
            State = GameState.GameOver;
            Random = new PcgRandom(new Random().Next());
            score = 0;
            ScoreText = "0";
            HUDHeight = 20;
            playerEdgeSettle = 50;
            playerSpeed = new Vector2(3, 2);
            PlayerShipScreenRect = new Rectangle(0, 0, 15, 6);
            playerMaxVelX = 1.6f;
            baiterMaxVelocity = new Vector2(100, 60);
            WorldSize = new Point(ScreenSize.X * 8, ScreenSize.Y - HUDHeight);
            RadarPos = new Vector2(72, -1);
            RadarSize = new Point(180 - 2, HUDHeight);
            spriteColors = new Color[2500];
            humanoidPassengers = new int[HumanoidCount];
            Bullets = new Vector4[20];
            BulletsAlive = new bool[20];
            entityBounds = new Point[] { new Point(), new Point(15, 6), new Point(3, 8), new Point(9, 8), new Point(9, 8), new Point(6, 7), new Point(9, 8), new Point(9, 8), new Point(9, 8), new Point(2, 2), new Point(1, 1) };

            MountainHeightMap = new float[50];
            Mountains = new List<Vector2>();
            int x = 0, y = WorldSize.Y - 40;
            while (x < WorldSize.X - 30)
            {
                var step = game.Random.Next(30) + 2;
                Mountains.Add(new Vector2(x, y));
                MountainHeightMap[GetMountainHeightMapIndex(x)] = y;
                x += step;
                y += (int)(game.Random.Next(5) == 0 ? 0 : game.Random.Next(step) - step * 0.493f);
                if (y > WorldSize.Y - 10) y = WorldSize.Y - 10; else if (y < WorldSize.Y * 0.65f) y = (int)(WorldSize.Y * 0.65f);
            }

            Entities = new Entity[200];
            Particles = new Particle[1000];

            for (int i = 0; i < StarCount; ++i)
            {
                var p = Particles[i];
                SpawnStar(ref p);
                p.Size = 1;
                p.Expired = SpawnStar;
                p.Tag = i;
                Particles[i] = p;
            }
        }

        #endregion

        #region Helper Methods

        void AddScore(int change)
        {
            score += change;
            ScoreText = score.ToString();

            if (change > 0 && ((score % 10000) < change))
            {
                ++PlayerLives;
                ++PlayerSmartBombs;
            }
        }

        void AddShootScore(EntityType type)
        {
            switch (type)
            {
                case EntityType.Lander:
                case EntityType.Mutant:
                case EntityType.Swarmer:
                    AddScore(150);
                    break;

                case EntityType.Baiter:
                    AddScore(200);
                    break;

                case EntityType.Bomber:
                    AddScore(250);
                    break;

                case EntityType.Pod:
                    AddScore(1000);
                    break;
            }
        }

        void AddEndOfWaveBonusPoints()
        {
            int points = Wave < 5 ? Wave * 100 : 500;

            for (int i = 0; i < HumanoidCount; i++)
            {
                if (Entities[i].Type == EntityType.Humaniod)
                {
                    AddScore(points);
                }
            }
        }

        public float GetScreenLeftEdge()
        {
            return PlayerWorldPos.X - PlayerScreenPos.X;
        }

        public float GetScreenX(float worldX)
        {
            float camX = PlayerWorldPos.X - PlayerScreenPos.X;
            var screenX = worldX - camX;
            if (camX > WorldSize.X - ScreenSize.X * 1.2f)
            {
                if (worldX < WorldSize.X / 2) screenX += WorldSize.X;
            }
            else if (camX < ScreenSize.X * 1.2f)
            {
                if (worldX > WorldSize.X / 2) screenX -= WorldSize.X;
            }
            return screenX;
        }

        public Vector2 GetRadarSpace(Vector2 worldPos)
        {
            float camX = PlayerWorldPos.X - PlayerScreenPos.X + ScreenSize.X * 0.5f;
            worldPos.X -= camX;
            worldPos.X += WorldSize.X * 0.5f;
            if (worldPos.X < 0) worldPos.X += WorldSize.X;
            else if (worldPos.X >= WorldSize.X) worldPos.X -= WorldSize.X;
            var pos = new Vector2(worldPos.X * (RadarSize.X / (float)WorldSize.X), worldPos.Y * (RadarSize.Y / (float)WorldSize.Y));
            return pos;
        }

        int GetMountainHeightMapIndex(float worldX)
        {
            int i = (int)(worldX / (WorldSize.X / MountainHeightMap.Length));
            return Math.Max(0, Math.Min(i, MountainHeightMap.Length - 1));
        }

        Rectangle GetEntityRect(Entity e)
        {
            var rect = new Rectangle();
            rect.Width = entityBounds[(int)e.Type].X;
            rect.Height = entityBounds[(int)e.Type].Y;
            rect.X = (int)(e.Position.X - rect.Width * 0.5f);
            rect.Y = (int)(e.Position.Y - rect.Height * 0.5f);
            return rect;
        }

        Rectangle GetPlayerRect()
        {
            var rect = new Rectangle();
            rect.Width = PlayerShipScreenRect.Width;
            rect.Height = PlayerShipScreenRect.Height;
            rect.X = (int)(PlayerWorldPos.X - rect.Width * 0.5f);
            rect.Y = (int)(PlayerWorldPos.Y - rect.Height * 0.5f);
            return rect;
        }

        Vector2 GetPlayerEstimatedPos(float time)
        {
            var x = PlayerWorldPos.X + playerVel.X * time * 60f;
            var y = PlayerWorldPos.Y + playerVel.Y * time * 60f;
            return new Vector2(x, y);
        }
            
        void SpawnBaiter()
        {
            Vector2 pos;
            pos.X = Random.Next(50) + PlayerWorldPos.X - 25;
            pos.Y = Random.Next(50) + PlayerWorldPos.Y - 25;
            if (pos.Y < 0) pos.Y += 50;
            if (pos.Y > ScreenSize.Y) pos.Y -= 50;
            SpawnEntity(EntityType.Baiter, pos);
        }

        void SpawnRemainingLanders()
        {
            Vector2 pos;
            int landerCount = Wave == 1 ? 7 : 12;
            for (int i = 0; i < landerCount; ++i)
            {
                pos.X = Random.Next(WorldSize.X);
                pos.Y = Random.Next(WorldSize.Y / 2) + 30;
                SpawnEntity(EntityType.Lander, pos);
            }
        }

        int SpawnEntity(EntityType type, Vector2 pos)
        {
            int i = GetNextEntityID();
            if (i >= 0)
            {
                var e = Entities[i];
                e.Type = type;
                e.State = EntityState.Spawning;
                e.Position = pos;
                e.Velocity = Vector2.Zero;
                e.Rotation = 0;
                e.RotationVelocity = 0;
                e.StateData = null;
                Entities[i] = e;
                CreateSpawnParticles(i);
            }
            return i;
        }

        int GetNextEntityID()
        {
            return GetNextEntityID(HumanoidCount);
        }

        int GetNextEntityID(int startIndex)
        {
            for (int i = startIndex; i < Entities.Length; ++i)
                if (Entities[i].Type == EntityType.None) return i;
            return -1;
        }

        int GetEntityCount(EntityType type)
        {
            int result = 0;
            for (int i = 0; i < Entities.Length; ++i)
                if (Entities[i].Type == type) ++result;
            return result;
        }

        bool AllEnemiesDead()
        {
            if (landerLateSpawnTimer > 0) return false;
            for (int i = HumanoidCount; i < Entities.Length; ++i)
            {
                var type = Entities[i].Type;
                switch (type)
                {
                    case EntityType.Lander:
                    case EntityType.Mutant:
                    case EntityType.Bomber:
                    case EntityType.Pod:
                    case EntityType.Swarmer:
                        return false;
                }
            }
            return true;
        }

        #endregion

        #region Particles

        void SpawnParticle(float age, Vector2 position, Vector2 velocity, Color color, float size, ParticleExpired expired)
        {
            SpawnParticle(age, position, velocity, color, size, expired, null);
        }

        void SpawnParticle(float age, Vector2 position, Vector2 velocity, Color color, float size, ParticleExpired expired, object tag)
        {
            int i = GetNextParticleID();
            if (i >= 0)
            {
                var p = new Particle();
                p.Age = age;
                p.Position = position;
                p.Velocity = velocity;
                p.Color = color;
                p.Size = size;
                p.Expired = expired;
                p.Tag = tag;
                Particles[i] = p;
            }
        }

        int GetNextParticleID()
        {
            for (int i = StarCount; i < Particles.Length; ++i)
                if (Particles[i].Age <= 0) return i;
            return -1;
        }

        void SpawnStar(ref Particle p)
        {
            p.Age = (float)(Random.NextDouble() * 2.0 + 1.0);
            p.Position.X = (Random.Next(ScreenSize.X * 2) + GetScreenLeftEdge() - ScreenSize.X / 2) % WorldSize.X;
            p.Position.Y = Random.Next(WorldSize.Y - 20) + 10;
            p.Color = starColors[Random.Next(starColors.Length)];
        }

        void ExplodeEntity(Entity e)
        {
            ExplodeEntity(e, 1, 1, 1, 0);
        }

        void ExplodeEntity(Entity e, float velocityFactor, float ageFactor, int sizeBase, int sizeVar)
        {
            int ti = (int)e.Type;
            if (ti >= TotalDefenderRenderer.TotalDefenderRendererInstance.SpriteAnimations.Length) return;

            var anim = TotalDefenderRenderer.TotalDefenderRendererInstance.SpriteAnimations[ti];
            int frame = (int)((long)(Services.TotalTime * 4) % (long)anim.Rect.Length);
            var rect = anim.Rect[frame];
            TotalDefenderRenderer.TotalDefenderRendererInstance.SpriteSheet.GetData<Color>(0, rect, spriteColors, 0, rect.Width * rect.Height);

            var pos = new Vector2();
            var vel = new Vector2();
            var rectCenter = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            float size = sizeBase;

            for (int y = 0; y < rect.Height; ++y)
            {
                for (int x = 0; x < rect.Width; ++x)
                {
                    var color = spriteColors[x + y * rect.Width];
                    if (color.R > 20 || color.G > 20 || color.B > 20)
                    {
                        pos.X = x + e.Position.X;
                        pos.Y = y + e.Position.Y;
                        for (int p = 0; p < 2; ++p)
                        {
                            vel.X = (x - rectCenter.X * ((float)Random.NextDouble() * 0.9f + 0.2f)) * velocityFactor;
                            vel.Y = (y - rectCenter.Y * ((float)Random.NextDouble() * 0.9f + 0.2f)) * velocityFactor;
                            float age = (0.5f + (float)Random.NextDouble() * 0.25f) * ageFactor;
                            SpawnParticle(age, pos, vel, color, size, null);
                            if (sizeVar > 0 && Random.Next(2) == 0)
                            {
                                ++vel.X;
                                --vel.Y;
                                SpawnParticle(age, pos, vel, color, size, null);
                            }
                        }
                    }
                }
            }
        }

        void ExplodePlayer()
        {
            var pos = PlayerWorldPos;
            var vel = new Vector2();
            float size = 2;

            for (int i = 0; i < 140; ++i)
            {
                vel.X = (float)(Random.NextDouble() - 0.5);
                vel.Y = (float)(Random.NextDouble() - 0.5);
                vel = Vector2.Normalize(vel);
                vel *= (float)((Random.NextDouble() + 0.25) * 2.0);
                SpawnParticle(1.8f + (float)Random.NextDouble() * 0.2f, pos, vel, Color.White, size, null);
            }
        }

        #endregion

        #region Start Game, Game Over, New Wave

        public override void StartGame()
        {
            if (State != GameState.Play)
            {
                if (Credits > 0)
                {
                    ChangeCredits(-1);
                    Wave = 0;
                    PlayerLives = 2;
                    PlayerSmartBombs = 3;
                    AddScore(-score);
                    ChangeState(GameState.Play);
                }
            }
            else
            {
                GameOver(false);
            }
        }

        public void GameOver(bool transition)
        {
            if (transition)
            {
                State = GameState.GameOverTransition;
                gameOverTransitionTimer = 3;
            }
            else
            {
                State = GameState.GameOver;
            }
        }

        void NewWave()
        {
            ++Wave;
            landerLateSpawnTimer = 10;
            baiterSpawnTimer = 30 - Math.Min(20, Wave);

            for (int i = HumanoidCount; i < Entities.Length; ++i) Entities[i].Type = EntityType.None;
            for (int i = 0; i < BulletsAlive.Length; ++i) BulletsAlive[i] = false;
            for (int i = 0; i < humanoidPassengers.Length; ++i) humanoidPassengers[i] = 0;

            int landerCount = 8;
            int bomberCount = Wave == 1 ? 0 : Wave == 2 ? 3 : Wave == 3 ? 4 : 5;
            int podCount = Wave == 1 ? 0 : Wave == 2 ? 1 : Wave == 3 ? 3 : 4;

            Entity e = new Entity();
            if ((Wave % 4) == 1)
            {
                for (int i = 0; i < HumanoidCount; ++i)
                {
                    e.Type = EntityType.Humaniod;
                    e.Position.X = Random.Next(WorldSize.X);
                    e.Position.Y = WorldSize.Y - 12 - Random.Next(4);
                    Entities[i] = e;
                }
            }

            int ie = HumanoidCount;
            for (int i = 0; i < landerCount; ++i)
            {
                e.Type = EntityType.Lander;
                e.State = EntityState.Spawning;
                e.Position.X = Random.Next(WorldSize.X);
                e.Position.Y = Random.Next(WorldSize.Y / 2) + 30;
                Entities[ie++] = e;
            }

            int bomberBaseX = Random.Next(WorldSize.X);
            for (int i = 0; i < bomberCount; ++i)
            {
                e.Type = EntityType.Bomber;
                e.State = EntityState.Spawning;
                e.Position.X = Random.Next(10) == 0
                    ? Random.Next(WorldSize.X)                  // random
                    : bomberBaseX + Random.Next(ScreenSize.X);   // clustered

                e.Position.Y = Random.Next(WorldSize.Y / 2) + 30;
                var sineWave = new Vector2();
                sineWave.X = (float)(Random.NextDouble() * MathHelper.TwoPi);
                sineWave.Y = (float)(Random.NextDouble() * 20 + 50);
                e.StateData = sineWave;
                Entities[ie++] = e;
            }

            int podBaseX = Random.Next(WorldSize.X);
            for (int i = 0; i < podCount; ++i)
            {
                e.Type = EntityType.Pod;
                e.State = EntityState.Spawning;
                e.Position.X = Random.Next(10) == 0
                    ? Random.Next(WorldSize.X)                // random
                    : podBaseX + Random.Next(ScreenSize.X);   // clustered
                e.Position.Y = Random.Next(WorldSize.Y / 2) + 30;
                Entities[ie++] = e;
            }
        }

        void RespawnPlayer()
        {
            PlayerState = EntityState.Default;
            PlayerWorldPos = new Vector2(320 * 2, 100);
            PlayerScreenPos = new Vector2(playerEdgeSettle, 100);
            playerVel = Vector2.Zero;
            playerAccelerationX = 0;
            PlayerDir = 1;
            PlayerDeathTimer = 0;
            PlayerSpawnTimer = 1.5f;
            baiterSpawnTimer += 2;

            for (int i = StarCount; i < Particles.Length; ++i) Particles[i].Age = 0;
            RemoveAllEntities(EntityType.EnemyBullet);
            RemoveAllEntities(EntityType.BomberBomb);
            RemoveAllEntities(EntityType.Baiter);

            // Reset entity spawn
            for (int i = 0; i < Entities.Length; ++i)
            {
                var e = Entities[i];
                if (e.Type != EntityType.None && 
                    e.Type != EntityType.Player)
                {
                    var state = e.State;
                    if (state == EntityState.LanderPickupHumanoid ||
                        state == EntityState.LanderAbductHumanoid)
                    {
                        int hi = (int)e.StateData;
                        ResetHumanoid(hi);
                    }
                    else if (state == EntityState.HumanoidFalling)
                    {
                        ResetHumanoid(i);
                    }
                    if (e.Type != EntityType.Humaniod)
                    {
                        Entities[i].State = EntityState.Spawning;
                        CreateSpawnParticles(i);
                    }
                }
            }

            // Reset Humanoids attached to player ship
            for (int i = 0; i < humanoidPassengers.Length; ++i)
            {
                if (humanoidPassengers[i] > 0)
                {
                    int hi = humanoidPassengers[i] - 1;
                    ResetHumanoid(hi);
                    humanoidPassengers[i] = 0;
                }
            }
        }

        void ResetHumanoid(int i)
        {
            var humanoid = Entities[i];
            if (humanoid.Type == EntityType.Humaniod)
            {
                humanoid.Velocity = Vector2.Zero;
                var y = MountainHeightMap[GetMountainHeightMapIndex(humanoid.Position.X)];
                humanoid.Position.Y = y + 10;
                Entities[i] = humanoid;
            }
        }

        void RemoveAllEntities(EntityType type)
        {
            for (int i = 0; i < Entities.Length; ++i)
            {
                if (Entities[i].Type == type)
                    Entities[i].Type = EntityType.None;
            }
        }

        void CreateSpawnParticles(int entityIndex)
        {
            var e = Entities[entityIndex];
            int ti = (int)e.Type;
            if (ti >= TotalDefenderRenderer.TotalDefenderRendererInstance.SpriteAnimations.Length) return;

            var anim = TotalDefenderRenderer.TotalDefenderRendererInstance.SpriteAnimations[ti];
            var rect = anim.Rect[0];
            TotalDefenderRenderer.TotalDefenderRendererInstance.SpriteSheet.GetData<Color>(0, rect, spriteColors, 0, rect.Width * rect.Height);

            var pos = new Vector2();
            var vel = new Vector2();
            var rectCenter = new Vector2(rect.Width * 0.5f, rect.Height * 0.5f);
            ParticleExpired callback = SpawnEntityFromParticles;

            for (int y = 0; y < rect.Height; ++y)
            {
                for (int x = 0; x < rect.Width; ++x)
                {
                    var color = spriteColors[x + y * rect.Width];
                    if (color.R > 20 || color.G > 20 || color.B > 20)
                    {
                        pos.X = e.Position.X + (x - rectCenter.X) * 60f;
                        pos.Y = e.Position.Y + (y - rectCenter.Y) * 60f;
                        vel.X = (e.Position.X - pos.X) / 60f;
                        vel.Y = (e.Position.Y - pos.Y) / 60f;
                        SpawnParticle(1f, pos, vel, color, 1, callback, entityIndex);
                        callback = null;
                    }
                }
            }
        }

        void SpawnEntityFromParticles(ref Particle particle)
        {
            var i = (int)particle.Tag;
            Entities[i].State = EntityState.Default;
        }

        #endregion

        #region Input

        public override bool HandleInput()
        {
            if (State == GameState.Play)
            {
                if (PlayerState != EntityState.Default) return true;

                playerVel.Y = 0;

                var left = InputManager.GetGamepadLeftStick(tmPlayer.PlayerIndex);
                if (left.Y != 0 || left.X != 0)
                {
                    playerVel.Y -= left.Y;
                }
                else
                {
                    if (InputManager.IsKeyPressedNew(tmPlayer.PlayerIndex, Keys.W)) upKeyDom = true;
                    else if (InputManager.IsKeyPressedNew(tmPlayer.PlayerIndex, Keys.D)) upKeyDom = false;
                    if (InputManager.IsKeyPressed(tmPlayer.PlayerIndex, Keys.W) && upKeyDom)
                    {
                        playerVel.Y = -1;
                    }
                    else if (InputManager.IsKeyPressed(tmPlayer.PlayerIndex, Keys.D) && !upKeyDom)
                    {
                        playerVel.Y = 1;
                    }
                }

                float thrust = InputManager.GetGamepadState(tmPlayer.PlayerIndex).Triggers.Left;
                if (thrust == 0) thrust = InputManager.GetGamepadState(tmPlayer.PlayerIndex).Triggers.Right;
                if (thrust > 0 || InputManager.IsKeyPressed(tmPlayer.PlayerIndex, Keys.OemSemicolon))
                {
                    playerAccelerationX = 0.12f;
                }

                if (InputManager.IsButtonPressedNew(tmPlayer.PlayerIndex, Buttons.RightShoulder) ||
                    InputManager.IsButtonPressedNew(tmPlayer.PlayerIndex, Buttons.LeftShoulder) || 
                    InputManager.IsKeyPressedNew(tmPlayer.PlayerIndex, Keys.Q))
                {
                    PlayerDir = -PlayerDir;
                }

                autoRepeatFireTimer -= Services.ElapsedTime;
                if (autoRepeatFireTimer <= 0 && 
                    (InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.A) || 
                    InputManager.IsKeyPressed(tmPlayer.PlayerIndex, Keys.OemQuotes)))
                {
                    for (int i = 0; i < BulletsAlive.Length; ++i)
                    {
                        if (!BulletsAlive[i])
                        {
                            BulletsAlive[i] = true;
                            Bullets[i] = new Vector4(PlayerScreenPos.X + PlayerShipScreenRect.Width * PlayerDir, 
                                PlayerScreenPos.Y + 1.5f, 20, PlayerDir);
                            break;
                        }
                    }
                    autoRepeatFireTimer = 0.1f;
                }

                if (InputManager.IsButtonPressedNew(tmPlayer.PlayerIndex, Buttons.X))
                {
                    ReleaseSmartBomb();
                }

                if (InputManager1.IsInputReleasedNew(tmPlayer.PlayerIndex, GuiInput.ExitScreen))
                {
                    GameOver(false);
                }

                return true;
            }
            else if (State == GameState.EndOfWave)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Update

        public override void Update()
        {
            if (!tmPlayer.IsInputEnabled) return;

            try
            {
                switch (State)
                {
                    case GameState.Play:
                        UpdatePlayState();
                        break;

                    case GameState.EndOfWave:
                        UpdateEndOfWaveState();
                        break;

                    case GameState.GameOverTransition:
                        UpdateGameOverTransitionState();
                        break;
                }
            }
            catch (Exception e)
            {
                Services.ExceptionReporter.ReportExceptionCaught(1, e);
            }
        }

        void ChangeState(GameState newState)
        {
            if (newState != State)
            {
                State = newState;

                switch (newState)
                {
                    case GameState.Play:
                        NewWave();
                        RespawnPlayer();
                        break;

                    case GameState.EndOfWave:
                        endOfWaveTimer = 5;
                        AddEndOfWaveBonusPoints();
                        break;
                }
            }
        }

        void UpdatePlayState()
        {
            UpdatePlayer();
            if (PlayerState != EntityState.PlayerExplosion)
            {
                UpdateEntities();
                UpdatePlayerBullets();
            }
            UpdateParticles();
        }

        void UpdatePlayer()
        {
            if (PlayerState == EntityState.PlayerDeath)
            {
                ++PlayerDeathTimer;
                if (PlayerDeathTimer >= 60)
                {
                    ExplodePlayer();
                    PlayerState = EntityState.PlayerExplosion;
                    PlayerDeathTimer = 0;
                }
                return;
            }
            else if (PlayerState == EntityState.PlayerExplosion)
            {
                ++PlayerDeathTimer;
                if (PlayerDeathTimer >= 120)
                {
                    if (PlayerLives == 0)
                        ChangeState(GameState.GameOverTransition);
                    else
                    {
                        --PlayerLives;
                        if (AllEnemiesDead())
                            ChangeState(GameState.EndOfWave);
                        else
                        {
                            RespawnPlayer();
                        }
                    }
                }
                return;
            }

            PlayerSpawnTimer -= Services.ElapsedTime;

            // Add thrust to player velocity
            playerVel.X += playerAccelerationX * PlayerDir;
            PlayerWorldPos.Y += playerVel.Y * playerSpeed.Y;

            // Clamp max velocity
            if (playerVel.X < -playerMaxVelX) playerVel.X = -playerMaxVelX; 
            else if (playerVel.X > playerMaxVelX) playerVel.X = playerMaxVelX;

            // Clamp player Y position
            float halfPlayerHeight = PlayerShipScreenRect.Height * 0.5f;
            if (PlayerWorldPos.Y < halfPlayerHeight) PlayerWorldPos.Y = halfPlayerHeight;
            else if (PlayerWorldPos.Y + halfPlayerHeight >= WorldSize.Y) PlayerWorldPos.Y = WorldSize.Y - halfPlayerHeight;
            
            PlayerScreenPos.Y = PlayerWorldPos.Y;
            PlayerScreenPos.X += playerAccelerationX * 3 * PlayerDir;

            // Clamp player X screen position
            float maxForwardPosFromEdge = playerMaxVelX * 30;
            float edgeBuff = playerAccelerationX == 0 ? 0 : maxForwardPosFromEdge;
            //if (playerAccelerationX == 0)
            {
                if (PlayerDir == 1)
                {
                    if (PlayerScreenPos.X > playerEdgeSettle + edgeBuff)
                    {
                        float diff = PlayerScreenPos.X - playerEdgeSettle;
                        if (diff > maxForwardPosFromEdge) diff = maxForwardPosFromEdge;
                        float moveBackSpeed = diff / maxForwardPosFromEdge * 3;
                        PlayerScreenPos.X -= moveBackSpeed;
                        PlayerWorldPos.X -= moveBackSpeed;
                    }
                }
                else
                {
                    if (PlayerScreenPos.X < ScreenSize.X - playerEdgeSettle - edgeBuff)
                    {
                        float diff = ScreenSize.X - playerEdgeSettle - PlayerScreenPos.X;
                        if (diff > maxForwardPosFromEdge) diff = maxForwardPosFromEdge;
                        float moveBackSpeed = diff / maxForwardPosFromEdge * 3;
                        PlayerScreenPos.X += moveBackSpeed;
                        PlayerWorldPos.X += moveBackSpeed;
                    }
                }
            }
            //else
            //{
            //    if (PlayerDir == 1)
            //    {
            //        if (PlayerScreenPos.X > playerEdgeSettle + playerVel.X * 30)
            //        {
            //            PlayerScreenPos.X = playerEdgeSettle + playerVel.X * 30;
            //        }
            //    }
            //    else
            //    {
            //        if (PlayerScreenPos.X < ScreenSize.X - playerEdgeSettle + playerVel.X * 30)
            //        {
            //            PlayerScreenPos.X = ScreenSize.X - playerEdgeSettle + playerVel.X * 30;
            //        }
            //    }
            //}

            playerAccelerationX = 0;

            // Deccelerate player
            if (playerVel.X != 0)
            {
                playerVel.X -= 0.03f * PlayerDir;
                if ((playerVel.X < 0 && PlayerDir == 1) || (playerVel.X > 0 && PlayerDir == -1))
                {
                    playerVel.X = 0;
                }
                else
                {
                    PlayerWorldPos.X += playerVel.X * playerSpeed.X;
                }
            }

            // Wrap player world X position
            if (PlayerWorldPos.X < 0) PlayerWorldPos.X += WorldSize.X;
            else if (PlayerWorldPos.X >= WorldSize.X) PlayerWorldPos.X -= WorldSize.X;

            PlayerShipScreenRect.X = (int)(PlayerScreenPos.X - PlayerShipScreenRect.Width * 0.5f);
            PlayerShipScreenRect.Y = (int)(PlayerScreenPos.Y - PlayerShipScreenRect.Height * 0.5f);

            var y = MountainHeightMap[GetMountainHeightMapIndex(PlayerWorldPos.X)];
            int passengerCount = 0;
            for (int i = 0; i < humanoidPassengers.Length; ++i)
            {
                if (humanoidPassengers[i] > 0)
                {
                    int hi = humanoidPassengers[i] - 1;
                    var humanoid = Entities[hi];
                    if (humanoid.Type == EntityType.Humaniod)
                    {
                        humanoid.Velocity = Vector2.Zero;
                        humanoid.Position = PlayerWorldPos;
                        humanoid.Position.Y += PlayerShipScreenRect.Height * 0.5f + entityBounds[(int)EntityType.Humaniod].Y * 0.5f + 1;
                        humanoid.Position.X += passengerCount++;
                        if (humanoid.Position.Y > y + 10)
                        {
                            humanoidPassengers[i] = 0;
                            AddScore(500);
                        }
                        Entities[hi] = humanoid;
                    }
                }
            }
        }

        void UpdateEntities()
        {
            var playerRect = GetPlayerRect();
            Rectangle rect;

            int entityCount = PlayerSpawnTimer <= 0 ? Entities.Length : HumanoidCount;
            for (int i = 0; i < entityCount; ++i)
            {
                var entity = Entities[i];
                if (entity.Type != EntityType.None && entity.State != EntityState.Spawning)
                {
                    if (entity.Age > 0)
                    {
                        entity.Age -= Services.ElapsedTime;
                        if (entity.Age <= 0)
                        {
                            entity.Type = EntityType.None;
                            Entities[i] = entity;
                            continue;
                        }
                    }

                    // Update the entities position by its velocity
                    entity.Position.X += entity.Velocity.X;
                    entity.Position.Y += entity.Velocity.Y;
                    entity.Position.X = entity.Position.X % WorldSize.X;
                    entity.Rotation += entity.RotationVelocity;

                    // X coord world wrap
                    if (entity.Position.X < 0) entity.Position.X += WorldSize.X;
                    else if (entity.Position.X >= WorldSize.X) entity.Position.X -= WorldSize.X;

                    if (entity.Type != EntityType.EnemyBullet)
                    {
                        // Y coord world wrap
                        var bound = entityBounds[(int)entity.Type];
                        if (entity.Position.Y < bound.Y * 0.5f) entity.Position.Y += WorldSize.Y;
                        else if (entity.Position.Y >= WorldSize.Y + bound.Y * 0.5f) entity.Position.Y -= WorldSize.Y;
                    }
                    else 
                    {
                        if (entity.Position.Y <= 0 || entity.Position.Y >= WorldSize.Y)
                            entity.Type = EntityType.None;
                    }

                    switch (entity.Type)
                    {
                        case EntityType.Lander:
                            UpdateLander(ref entity);
                            break;
                        case EntityType.Mutant:
                            UpdateMutant(ref entity);
                            break;
                        case EntityType.Bomber:
                            UpdateBomber(ref entity);
                            break;
                        case EntityType.Pod:
                            UpdatePod(ref entity);
                            break;
                        case EntityType.Swarmer:
                            UpdateSwarmer(ref entity);
                            break;
                        case EntityType.Baiter:
                            UpdateBaiter(ref entity);
                            break;
                        case EntityType.EnemyBullet:
                        case EntityType.BomberBomb:
                            UpdateEnemyBullet(ref entity);
                            break;
                        case EntityType.Humaniod:
                            UpdateHumanoid(ref entity, i);
                            break;
                    }

                    if (entity.Type != EntityType.Humaniod)
                    {
                        rect = GetEntityRect(entity);
                        if (rect.Intersects(playerRect))
                        {
                            PlayerState = EntityState.PlayerDeath;
                            ExplodeEntity(entity);
                            entity.Type = EntityType.None;
                        }
                    }

                    Entities[i] = entity;
                }
            }

            if (landerLateSpawnTimer > 0)
            {
                landerLateSpawnTimer -= Services.ElapsedTime;
                if (landerLateSpawnTimer <= 0)
                {
                    SpawnRemainingLanders();
                }
            }

            if (baiterSpawnTimer > 0)
            {
                baiterSpawnTimer -= Services.ElapsedTime;
                if (baiterSpawnTimer <= 0)
                {
                    SpawnBaiter();
                    baiterSpawnTimer = 10 - Math.Min(8, Wave / 2);
                }
            }
        }

        void UpdateLander(ref Entity entity)
        {
            switch (entity.State)
            {
                case EntityState.LanderPickupHumanoid:
                    UpdateLanderPickupHumanoid(ref entity);
                    break;
                case EntityState.LanderAbductHumanoid:
                    UpdateLanderAbductHumanoid(ref entity);
                    break;
                default:
                    UpdateLanderDefault(ref entity);
                    break;
            }
        }

        void UpdateLanderDefault(ref Entity lander)
        {
            // Lander just spawned - choose a movement direction
            if (lander.Velocity.X == 0 && lander.Velocity.Y == 0)
            {
                lander.Velocity.X = Random.Next(2) == 0 ? -0.5f : 0.5f;
                lander.Velocity.X *= ((float)Random.NextDouble() * 0.2f + 0.9f);
            }

            // Make lander hover/float over mountains
            var y = MountainHeightMap[GetMountainHeightMapIndex(lander.Position.X)];
            if (lander.Position.Y > y - 30) lander.Velocity.Y = -0.15f;
            else if (lander.Position.Y < y - 32) lander.Velocity.Y = 0.15f;
            else lander.Velocity.Y = 0;

            // Look for a Humanoid to pickup
            Entity e;
            for (int i = 0; i < 10; ++i)
            {
                e = Entities[i];
                if (e.Type == EntityType.Humaniod && e.State == EntityState.Default)
                {
                    if ((int)lander.Position.X == (int)e.Position.X && Random.Next(3) == 0)
                    {
                        lander.State = EntityState.LanderPickupHumanoid;
                        lander.StateData = i;
                        Entities[i].State = EntityState.HumanoidBeingAbducted;
                        break;
                    }
                }
            }

            UpdateLanderShooting(ref lander);
        }

        void UpdateLanderPickupHumanoid(ref Entity lander)
        {
            lander.Velocity.X = 0;
            lander.Velocity.Y = 0.5f;

            var humanoid = (Entity)Entities[(int)lander.StateData];
            if (humanoid.Type == EntityType.None)
            {
                lander.State = EntityState.Default;
                return;
            }

            var r1 = GetEntityRect(lander);
            var r2 = GetEntityRect(humanoid);
            if (r1.Intersects(r2))
            {
                lander.State = EntityState.LanderAbductHumanoid;
            }

            UpdateLanderShooting(ref lander);
        }

        void UpdateLanderAbductHumanoid(ref Entity lander)
        {
            lander.Velocity.X = 0;
            lander.Velocity.Y = -0.5f;

            var humanoid = (Entity)Entities[(int)lander.StateData];
            if (humanoid.Type == EntityType.None)
            {
                lander.State = EntityState.Default;
                return;
            }

            humanoid.Position = lander.Position;
            humanoid.Position.Y += entityBounds[(int)EntityType.Lander].Y;

            if (lander.Position.Y - entityBounds[(int)EntityType.Lander].Y * 0.5f <= 1)
            {
                lander.Velocity.Y = 0;
                lander.Type = EntityType.Mutant;
                lander.State = EntityState.Default;
                ExplodeEntity(humanoid);
                humanoid.Type = EntityType.None;
            }
            else
                UpdateLanderShooting(ref lander);

            Entities[(int)lander.StateData] = humanoid;
        }

        void UpdateLanderShooting(ref Entity lander)
        {
            // Shoot at Player?
            var distance = Vector2.Distance(lander.Position, PlayerWorldPos);
            if (distance < ScreenSize.X)
            {
                bool shoot = false;
                switch (lander.State)
                {
                    case EntityState.LanderPickupHumanoid:
                        shoot = Random.Next(150) == 0;
                        break;
                    case EntityState.LanderAbductHumanoid:
                        shoot = Random.Next(400) == 0;
                        break;
                    default:
                        shoot = Random.Next(500) == 0;
                        break;
                }

                if (shoot)
                {
                    ShootAtPlayer(lander, EntityType.EnemyBullet, 2f, 0);
                }
            }
        }

        void UpdateMutant(ref Entity mutant)
        {
            float mutantSpeedX = 1.5f;
            float mutantSpeedY = 0.4f;

            if (mutant.Position.X > PlayerWorldPos.X)
                mutant.Velocity.X = -mutantSpeedX;
            else if (mutant.Position.X < PlayerWorldPos.X)
                mutant.Velocity.X = mutantSpeedX;

            if (mutant.Position.Y > PlayerWorldPos.Y)
                mutant.Velocity.Y = -mutantSpeedY;
            else if (mutant.Position.Y < PlayerWorldPos.Y)
                mutant.Velocity.Y = mutantSpeedY;

            // Shoot at Player?
            var distance = Vector2.Distance(mutant.Position, PlayerWorldPos);
            if (distance < ScreenSize.X && Random.Next(60) == 0)
            {
                ShootAtPlayer(mutant, EntityType.EnemyBullet, 3f, 0);
            }
        }

        void UpdateBomber(ref Entity bomber)
        {
            // Lander just spawned - choose a movement direction
            if (bomber.Velocity.X == 0 && bomber.Velocity.Y == 0)
            {
                bomber.Velocity.X = Random.Next(2) == 0 ? -0.5f : 0.5f;
                bomber.Velocity.X *= ((float)Random.NextDouble() * 0.2f + 0.9f);
            }

            var sineWave = (Vector2)bomber.StateData;
            sineWave.X += 0.25f / 60f;
            bomber.StateData = sineWave;
            bomber.Velocity.Y = (float)(Math.Sin(sineWave.X) * sineWave.Y) / 240;

            if (Random.Next(20) == 0)
            {
                ShootAtPlayer(bomber, EntityType.BomberBomb, 0, 4);
            }
        }

        void UpdatePod(ref Entity pod)
        {
            // Pod just spawned - choose a movement direction
            if (pod.Velocity.X == 0 && pod.Velocity.Y == 0)
            {
                pod.Velocity.X = Random.Next(2) == 0 ? -0.15f : 0.15f;
                pod.Velocity.X *= ((float)Random.NextDouble() * 0.2f + 0.9f);
                pod.Velocity.Y = Random.Next(2) == 0 ? -0.15f : 0.15f;
                pod.Velocity.Y *= ((float)Random.NextDouble() * 0.2f + 0.9f);
            }
        }

        void UpdateSwarmer(ref Entity swarmer)
        {
            var sineWave = (Vector2)swarmer.StateData;
            sineWave.X += 0.25f / 60f;
            swarmer.StateData = sineWave;
            swarmer.Velocity.Y = (float)(Math.Sin(sineWave.X) * sineWave.Y);

            // Shoot at Player?
            var distance = Vector2.Distance(swarmer.Position, PlayerWorldPos);
            if (distance < ScreenSize.X && Random.Next(300) == 0)
            {
                ShootAtPlayer(swarmer, EntityType.EnemyBullet, 3f, 0);
            }
        }

        void SpawnSwarmers(Entity pod, int podIndex)
        {
            int swarmerCount = GetEntityCount(EntityType.Swarmer);
            int newCount = 0;
            if (Random.Next(256) == 0)
                newCount = Random.Next(1, 3);
            else
                newCount = Random.Next(4, 7);

            if (swarmerCount + newCount > 20) newCount = 20 - swarmerCount;
            int startIndex = podIndex + 1;

            for (int j = 0; j < newCount; ++j)
            {
                int i = GetNextEntityID(startIndex);
                if (i >= 0)
                {
                    var e = new Entity();
                    e.Type = EntityType.Swarmer;
                    e.Position = pod.Position;
                    e.Velocity.X = Random.Next(2) == 0 ? -3f : 3f;
                    var sineWave = new Vector2();
                    sineWave.X = (float)(Random.NextDouble() * MathHelper.TwoPi);
                    sineWave.Y = (float)(Random.NextDouble() * 1 + 1);
                    e.StateData = sineWave;
                    Entities[i] = e;
                }
            }
        }

        void UpdateBaiter(ref Entity baiter)
        {
            if (baiter.StateData == null)
            {
                // Decide new movement destination
                var playerEstPos = GetPlayerEstimatedPos(3);
                Vector2 destPos;
                destPos.X = Random.Next(50) + playerEstPos.X - 25;
                destPos.Y = Random.Next(50) + PlayerWorldPos.Y - 25;
                if (destPos.Y < 0) destPos.Y += 50;
                if (destPos.Y > ScreenSize.Y) destPos.Y -= 50;

                float timeToDestination = 3;// Random.Next(2) + 1;
                float vx = (destPos.X - baiter.Position.X) / timeToDestination / 60f;
                float vy = (destPos.Y - baiter.Position.Y) / timeToDestination / 60;
                baiter.Velocity = Vector2.Clamp(new Vector2(vx, vy), -baiterMaxVelocity, baiterMaxVelocity);
                baiter.StateData = timeToDestination;
            }
            else
            {
                baiter.Position.X += baiter.Velocity.X;
                baiter.Position.Y += baiter.Velocity.Y;
                var timeToDestination = (float)baiter.StateData;
                timeToDestination -= Services.ElapsedTime;
                if (timeToDestination <= 0)
                    baiter.StateData = null;
                else
                    baiter.StateData = timeToDestination;

                // Shoot at Player?
                var distance = Vector2.Distance(baiter.Position, PlayerWorldPos);
                if (distance < ScreenSize.X && Random.Next(60) == 0)
                {
                    ShootAtPlayer(baiter, EntityType.EnemyBullet, 3f, 0);
                }
            }
        }

        void UpdateEnemyBullet(ref Entity bullet)
        {
            //var pos1 = bullet.Position;
            //var pos2 = PlayerWorldPos;
            //pos1.X += WorldSize.X;
            //pos2.X += WorldSize.X;

            var distance = Vector2.Distance(bullet.Position, PlayerWorldPos);
            if (distance > ScreenSize.X)
            {
                bullet.Type = EntityType.None;
            }
        }

        void UpdateHumanoid(ref Entity humanoid, int humanoidIndex)
        {
            if (humanoid.State == EntityState.HumanoidFalling)
            {
                var r1 = GetEntityRect(humanoid);
                var r2 = GetPlayerRect();
                if (r1.Intersects(r2))
                {
                    AttachHumanoidToPlayer(ref humanoid, humanoidIndex);
                }
                else
                {
                    humanoid.Velocity.Y = 0.5f;

                    var y = MountainHeightMap[GetMountainHeightMapIndex(humanoid.Position.X)];
                    if (humanoid.Position.Y > y + 10)
                    {
                        humanoid.Velocity.Y = 0;
                        humanoid.State = EntityState.Default;

                        if ((float)humanoid.StateData < humanoid.Position.Y - 75)
                        {
                            EntityLastAct(humanoidIndex, humanoid);
                            ExplodeEntity(humanoid);
                            humanoid.Type = EntityType.None;
                        }
                        else
                            AddScore(250);
                    }
                }
            }
        }

        void AttachHumanoidToPlayer(ref Entity humanoid, int humanoidIndex)
        {
            for (int i = 0; i < humanoidPassengers.Length; ++i)
            {
                if (humanoidPassengers[i] == 0 || Entities[humanoidPassengers[i] - 1].Type == EntityType.None)
                {
                    humanoidPassengers[i] = humanoidIndex + 1;
                    break;
                }
            }

            humanoid.State = EntityState.Default;
            humanoid.StateData = 0;
            AddScore(500);
        }

        void UpdatePlayerBullets()
        {
            Rectangle rect = new Rectangle();
            Rectangle rect2 = new Rectangle();
            Vector4 bullet;
            Entity entity;
            Point bound;

            for (int i = 0; i < BulletsAlive.Length; ++i)
            {
                if (BulletsAlive[i])
                {
                    bullet = Bullets[i];
                    if (bullet.W > 0)
                    {
                        bullet.X += 3;
                        bullet.Z += 10;
                        if (bullet.X + bullet.Z > ScreenSize.X) BulletsAlive[i] = false;
                    }
                    else
                    {
                        bullet.X -= 3;
                        bullet.Z += 10;
                        if (bullet.X - bullet.Z < 0) BulletsAlive[i] = false;
                    }
                    Bullets[i] = bullet;

                    if (BulletsAlive[i])
                    {
                        for (int j = 0; j < Entities.Length; ++j)
                        {
                            entity = Entities[j];
                            if (entity.Type != EntityType.None && 
                                entity.Type != EntityType.EnemyBullet && 
                                entity.Type != EntityType.BomberBomb &&
                                entity.State != EntityState.Spawning)
                            {
                                rect.X = (int)bullet.X;
                                rect.Y = (int)bullet.Y;
                                rect.Width = (int)bullet.Z;
                                if (bullet.W < 1) rect.X -= rect.Width;
                                rect.Height = 1;
                                bound = entityBounds[(int)entity.Type];
                                rect2.X = (int)(GetScreenX(entity.Position.X - bound.X * 0.5f));
                                rect2.Y = (int)(entity.Position.Y - bound.Y * 0.5f);
                                rect2.Width = bound.X;
                                rect2.Height = bound.Y;
                                if (rect.Intersects(rect2))
                                {
                                    EntityLastAct(j, entity);
                                    BulletsAlive[i] = false;
                                    ExplodeEntity(entity);
                                    AddShootScore(entity.Type);
                                    Entities[j].Type = EntityType.None;
                                    if (entity.Type != EntityType.Humaniod && AllEnemiesDead())
                                    {
                                        ChangeState(GameState.EndOfWave);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        void ShootAtPlayer(Entity entity, EntityType type, float speed, float age)
        {
            var i = GetNextEntityID();
            if (i >= 0)
            {
                var bullet = new Entity();
                bullet.Type = type;
                bullet.State = EntityState.Default;
                bullet.Position = entity.Position;
                var targetPos = new Vector2(PlayerWorldPos.X + Random.Next(100) - 50, PlayerWorldPos.Y + Random.Next(60) - 30);
                bullet.Velocity = Vector2.Normalize(targetPos - entity.Position) * speed;
                bullet.Age = age;
                Entities[i] = bullet;
            }
        }

        void EntityLastAct(int entityIndex, Entity entity)
        {
            if (entity.Type == EntityType.Pod)
            {
                SpawnSwarmers(entity, entityIndex);
            }
            if (entity.State == EntityState.LanderAbductHumanoid)
            {
                Entities[(int)entity.StateData].State = EntityState.HumanoidFalling;
                Entities[(int)entity.StateData].StateData = Entities[(int)entity.StateData].Position.Y;
            }
        }

        void ReleaseSmartBomb()
        {
            if (PlayerSmartBombs == 0) return;
            --PlayerSmartBombs;

            var screenLeftX = GetScreenLeftEdge() - 10;
            var screenRightX = screenLeftX + ScreenSize.X + 20;

            for (int i = 10; i < Entities.Length; ++i)
            {
                var entity = Entities[i];
                switch (entity.Type)
                {
                    case EntityType.None:
                    case EntityType.BomberBomb:
                    case EntityType.EnemyBullet:
                        break;

                    default:
                        if (entity.Position.X > screenLeftX && entity.Position.X < screenRightX)
                        {
                            if (entity.Type != EntityType.Swarmer || Random.Next(10) > 0)
                            {
                                EntityLastAct(i, entity);
                                ExplodeEntity(entity, 0.15f, 5f, 1, 2);
                                AddShootScore(entity.Type);
                                entity.Type = EntityType.None;
                                Entities[i] = entity;
                            }
                        }
                        break;
                }
            }

            if (AllEnemiesDead())
            {
                ChangeState(GameState.EndOfWave);
            }
        }

        void UpdateParticles()
        {
            var p = new Particle();
            int particleCount = PlayerSpawnTimer <= 0 ? Particles.Length : StarCount;
            for (int i = 0; i < particleCount; ++i)
            {
                if (Particles[i].Age > 0)
                {
                    p = Particles[i];
                    p.Age -= Services.ElapsedTime;
                    p.Position.X += p.Velocity.X;
                    p.Position.Y += p.Velocity.Y;
                    if (p.Age <= 0)
                        if (p.Expired != null) p.Expired(ref p);

                    Particles[i] = p;
                }
            }
        }

        void UpdateEndOfWaveState()
        {
            endOfWaveTimer -= Services.ElapsedTime;
            if (endOfWaveTimer <= 0)
            {
                ChangeState(GameState.Play);
            }
        }

        void UpdateGameOverTransitionState()
        {
            gameOverTransitionTimer -= Services.ElapsedTime;
            if (gameOverTransitionTimer > 0)
            {
            }
            else
            {
                GameOver(false);
            }
        }

        #endregion
    }
}
