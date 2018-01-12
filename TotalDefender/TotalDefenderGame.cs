using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace TotalDefender
{
    class TotalDefenderGame : ArcadeMachine
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
        public int GameScreenY;
        public Vector2 PlayerWorldPos;
        public Vector2 PlayerScreenPos;
        public Rectangle PlayerRect;
        public Vector2[] Mountains;
        public Vector3[] Bullets;
        public bool[] BulletsAlive;

        float gameOverTransitionTimer;
        int score;
        Vector2 playerVel;
        Vector2 playerSpeed;
        int worldWidth;
        bool upKeyDom;

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
            GameScreenY = 16;
            playerSpeed = new Vector2(2, 2);
            PlayerWorldPos = new Vector2(320 * 2, 100);
            PlayerScreenPos = new Vector2(50, 100);
            PlayerRect = new Rectangle(0, 0, 16, 10);
            worldWidth = 1280;
            Bullets = new Vector3[20];
            BulletsAlive = new bool[20];

            Mountains = new Vector2[40];
            int x = 0;
            for (int i = 0; i < Mountains.Length; ++i)
            {
                Mountains[i].X = x;
                Mountains[i].Y = ScreenSize.Y - game.Random.Next(45) - 5;
                x += worldWidth / 40;// game.Random.Next(20) + 30;
            }
        }

        public override void StartGame()
        {
            if (State != GameState.Play)
            {
                if (Credits > 0)
                {
                    ChangeCredits(-1);
                    State = GameState.Play;
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

        #endregion

        #region Input

        public override bool HandleInput()
        {
            if (State == GameState.Play)
            {
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

                if (InputManager.IsKeyPressed(tmPlayer.PlayerIndex, Keys.OemSemicolon))
                {
                    playerVel.X = 1;
                }

                if (InputManager.IsKeyPressedNew(tmPlayer.PlayerIndex, Keys.OemQuotes))
                {
                    for (int i = 0; i < BulletsAlive.Length; ++i)
                    {
                        if (!BulletsAlive[i])
                        {
                            BulletsAlive[i] = true;
                            Bullets[i] = new Vector3(PlayerScreenPos.X + PlayerRect.Width, PlayerScreenPos.Y + PlayerRect.Height * 0.5f - 1, 20);
                            break;
                        }
                    }
                }

                if (InputManager1.IsInputReleasedNew(tmPlayer.PlayerIndex, GuiInput.ExitScreen))
                {
                    GameOver(false);
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
            PlayerWorldPos.Y += playerVel.Y * playerSpeed.Y;
            if (PlayerWorldPos.Y < GameScreenY + 2 + PlayerRect.Height * 0.5f) PlayerWorldPos.Y = GameScreenY + 2 + PlayerRect.Height * 0.5f;
            else if (PlayerWorldPos.Y > ScreenSize.Y - 2 - PlayerRect.Height * 0.5f) PlayerWorldPos.Y = ScreenSize.Y - 2 - PlayerRect.Height * 0.5f;
            PlayerScreenPos.Y = PlayerWorldPos.Y;

            if (playerVel.X != 0)
            {
                playerVel.X -= 0.03f;
                if (playerVel.X < 0)
                {
                    playerVel.X = 0;
                }
                else
                {
                    PlayerWorldPos.X += playerVel.X * playerSpeed.X;
                    if (PlayerWorldPos.X < 0) PlayerWorldPos.X += worldWidth;
                    else if (PlayerWorldPos.X >= worldWidth) PlayerWorldPos.X -= worldWidth;
                }
            }

            PlayerRect.X = (int)(PlayerScreenPos.X - PlayerRect.Width * 0.5f);
            PlayerRect.Y = (int)(PlayerScreenPos.Y - PlayerRect.Height * 0.5f);

            for (int i = 0; i < BulletsAlive.Length; ++i)
            {
                if (BulletsAlive[i])
                {
                    Bullets[i].X += 3;
                    Bullets[i].Z += 10;
                    if (Bullets[i].X + Bullets[i].Z > ScreenSize.X) BulletsAlive[i] = false;
                }
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
