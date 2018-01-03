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
        public Vector2 PlayerPos;
        public Rectangle PlayerRect;
        public Vector2[] Mountains;

        float gameOverTransitionTimer;
        int score;
        Vector2 playerVel;
        Vector2 playerSpeed;

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
            playerSpeed = new Vector2(0, 2);
            PlayerPos = new Vector2(50, 100);
            PlayerRect = new Rectangle(0, 0, 16, 10);

            Mountains = new Vector2[30];
            int x = 0;
            for (int i = 0; i < Mountains.Length; ++i)
            {
                Mountains[i].X = x;
                Mountains[i].Y = ScreenSize.Y - game.Random.Next(45) - 5;
                x += game.Random.Next(20) + 30;
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
            bool result = false;

            if (State == GameState.Play)
            {
                playerVel.X = playerVel.Y = 0;

                var left = InputManager.GetGamepadLeftStick(tmPlayer.PlayerIndex);
                if (left.Y != 0 || left.X != 0)
                {
                    playerVel.Y -= left.Y;
                    result = true;
                }

                if (InputManager1.IsInputReleasedNew(tmPlayer.PlayerIndex, GuiInput.ExitScreen))
                {
                    GameOver(false);
                    result = true;
                }

                return result ||
                    InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.A) ||
                    InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.X) ||
                    InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.Y) ||
                    InputManager1.IsInputPressed(tmPlayer.PlayerIndex, PlayerInput.LeftHand) ||
                    InputManager1.IsInputPressed(tmPlayer.PlayerIndex, PlayerInput.RightHand);
            }

            return result ||
                InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.A) ||
                InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.X) ||
                InputManager.IsButtonPressed(tmPlayer.PlayerIndex, Buttons.Y);
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
            PlayerPos += playerVel * playerSpeed;
            PlayerRect.X = (int)(PlayerPos.X - PlayerRect.Width * 0.5f);
            PlayerRect.Y = (int)(PlayerPos.Y - PlayerRect.Height * 0.5f);
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
