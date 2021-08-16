using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;
using System;

namespace ArcadeMachines.TotalRobo
{
    enum EntityType
    {
        None,
        Player,
        PlayerBullet,
        RoboMan,
        zLast
    }

    struct Entity
    {
        public EntityType Type;
        public Vector2 Position;
        public Vector2 Velocity;
    }

    struct EntityData
    {
        public float Speed;
    }

    class TotalRoboGame : ArcadeMachine
    {
        #region Enum

        public enum GameState
        {
            Play,
            GameOverTransition,
            GameOver
        }

        #endregion

        #region Fields

        public GameState State;
        public Point ScreenSize;
        public PcgRandom Random;
        public string ScoreText;
        public Entity[] Entities;
        public int BorderSize = 4;

        float gameOverTransitionTimer;
        int wave;
        int score;
        int enemyCount;
        float playerShootTimer;
        EntityData[] entityData;
        TotalRoboRenderer renderer;

        #endregion

        #region Properties

        public override bool CanDeactivate { get { return State == GameState.GameOver; } }

        #endregion

        #region Initialization

        public TotalRoboGame(ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D point, BlockFace face)
            : base(game, map, player, point, face)
        {
        }

        public override void LoadContent(InitState state)
        {
            base.LoadContent(state);
            renderer = (TotalRoboRenderer)Renderer;

            // todo - create rendertarget 304x255
            ScreenSize = new Point(renderTarget.Width, renderTarget.Height);

            State = GameState.GameOver;
            Random = new PcgRandom(new Random().Next());
            wave = 0;
            score = 0;
            ScoreText = "0";
            entityData = new EntityData[(int)EntityType.zLast];
            entityData[1].Speed = 1;
            entityData[2].Speed = 6;
            entityData[3].Speed = 0.3f;

            Entities = new Entity[100];
            Entities[0].Type = EntityType.Player;
        }

        public override void StartGame()
        {
            if (State != GameState.Play)
            {
                if (Credits > 0)
                {
                    ChangeCredits(-1);
                    ChangeGameState(GameState.Play);
                }
            }
            else
            {
                GameOver(false);
            }
        }

        void ChangeGameState(GameState newState)
        {
            if (newState != State)
            {
                switch (newState)
                {
                    case GameState.Play:
                        NewWave();
                        break;
                }

                State = newState;
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
            ++wave;
            Entities[0].Position = new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2);

            for (int i = 0; i < wave * 5; ++i)
            {
                var e = Entities[i + 1];
                e.Type = EntityType.RoboMan;
                e.Velocity.X = e.Velocity.Y = 0;
                while (true)
                {
                    e.Position.X = Random.Next(ScreenSize.X - BorderSize * 2 - 10);
                    e.Position.Y = Random.Next(ScreenSize.Y - BorderSize * 2 - 30) + 20;
                    var dist = Vector2.Distance(Entities[0].Position, e.Position);
                    if (dist > 60) break;
                }
                Entities[i + 1] = e;
            }

            enemyCount = wave * 5;
        }

        #endregion

        #region Input

        public override bool HandleInput()
        {
            if (State == GameState.Play)
            {
                if (InputManager1.IsInputReleasedNew(tmPlayer.PlayerIndex, GuiInput.ExitScreen))
                {
                    GameOver(false);
                }

                var player = Entities[0];
                player.Velocity.X = player.Velocity.Y = 0;
                var stick = InputManager.GetGamepadLeftStick(tmPlayer.PlayerIndex);
                player.Velocity.X = stick.X * entityData[(int)EntityType.Player].Speed;
                player.Velocity.Y = -stick.Y * entityData[(int)EntityType.Player].Speed;
                Entities[0] = player;

                playerShootTimer -= Services.ElapsedTime;
                if (playerShootTimer <= 0)
                {
                    stick = InputManager.GetGamepadRightStick(tmPlayer.PlayerIndex);
                    if (stick.X != 0 || stick.Y != 0)
                    {
                        stick.Normalize();
                        stick.Y = -stick.Y;
                        SpawnNewEntity(EntityType.PlayerBullet, player.Position, stick * entityData[(int)EntityType.PlayerBullet].Speed);
                        playerShootTimer = 0.05f;
                    }
                }

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

        void UpdatePlayState()
        {
            for (int i = 0; i < Entities.Length; ++i)
            {
                var entity = Entities[i];
                if (entity.Type != EntityType.None)
                {
                    entity.Position.X += entity.Velocity.X;
                    entity.Position.Y += entity.Velocity.Y;

                    switch (entity.Type)
                    {
                        case EntityType.RoboMan:
                            UpdateRoboMan(ref entity);
                            break;

                        case EntityType.PlayerBullet:
                            UpdatePlayerBullet(ref entity);
                            break;

                        case EntityType.Player:
                            UpdatePlayer(ref entity);
                            break;
                    }

                    Entities[i] = entity;
                }
            }

            if (enemyCount <= 0)
                NewWave();
        }

        void UpdatePlayer(ref Entity player)
        {
            var rect = renderer.GetSpriteSrcRect(player);

            var sizeX = rect.Width * 0.5f;
            if (player.Position.X - sizeX < BorderSize)
                player.Position.X = BorderSize + sizeX;
            else if (player.Position.X + sizeX > ScreenSize.X - BorderSize)
                player.Position.X = ScreenSize.X - BorderSize - sizeX;

            var sizeY = rect.Height * 0.5f;
            if (player.Position.Y - sizeY < 20 + BorderSize)
                player.Position.Y = 20 + BorderSize + sizeY;
            else if (player.Position.Y + sizeY > ScreenSize.Y - BorderSize)
                player.Position.Y = ScreenSize.Y - BorderSize - sizeY;
        }

        void UpdatePlayerBullet(ref Entity bullet)
        {
            var sizeX = 1;
            var sizeY = 1;

            if (bullet.Position.X - sizeX < BorderSize)
                bullet.Type = EntityType.None;
            else if (bullet.Position.X + sizeX > ScreenSize.X - BorderSize)
                bullet.Type = EntityType.None;
            else if (bullet.Position.Y - sizeY < 20 + BorderSize)
                bullet.Type = EntityType.None;
            else if (bullet.Position.Y + sizeY > ScreenSize.Y - BorderSize)
                bullet.Type = EntityType.None;

            var p = new Point((int)bullet.Position.X, (int)bullet.Position.Y);
            for (int i = 1; i < Entities.Length; ++i)
            {
                var e = Entities[i];
                if (e.Type > EntityType.None && e.Type != EntityType.PlayerBullet)
                {
                    var rect = GetEntityScreenRect(e);
                    if (rect.Contains(p))
                    {
                        bullet.Type = EntityType.None;
                        e.Type = EntityType.None;
                        Entities[i] = e;
                        --enemyCount;
                        break;
                    }
                }
            }
        }

        void UpdateRoboMan(ref Entity man)
        {
            if (CheckForPlayerCollision(man))
            {
                ChangeGameState(GameState.GameOverTransition);
                return;
            }

            var player = Entities[0];
            var speed = entityData[(int)man.Type].Speed;

            if (player.Position.X < man.Position.X)
                man.Velocity.X = -speed;
            else if (player.Position.X > man.Position.X)
                man.Velocity.X = speed;

            if (player.Position.Y < man.Position.Y)
                man.Velocity.Y = -speed;
            else if (player.Position.Y > man.Position.Y)
                man.Velocity.Y = speed;
        }

        bool CheckForPlayerCollision(Entity entity)
        {
            var playerRect = GetEntityScreenRect(Entities[0]);
            var entityRect = GetEntityScreenRect(entity);
            return entityRect.Intersects(playerRect);
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

        void SpawnNewEntity(EntityType type, Vector2 position, Vector2 velocity)
        {
            int id = GetNextFreeEntityIndex();
            if (id >= 0)
            {
                var e = Entities[id];
                e.Type = type;
                e.Position = position;
                e.Velocity = velocity;
                Entities[id] = e;
            }
        }

        int GetNextFreeEntityIndex()
        {
            for (int i = 1; i < Entities.Length; ++i)
                if (Entities[i].Type == EntityType.None) return i;
            return -1;
        }

        Rectangle GetEntityScreenRect(Entity entity)
        {
            var rect = renderer.GetSpriteSrcRect(entity);
            rect.X = (int)(entity.Position.X - rect.Width * 0.5f);
            rect.Y = (int)(entity.Position.Y - rect.Height * 0.5f);
            return rect;

        }

        #endregion
    }
}
