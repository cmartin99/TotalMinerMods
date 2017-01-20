using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StudioForge.BlockWorld;
using StudioForge.Engine;
using StudioForge.Engine.Core;
using StudioForge.Engine.Integration;
using StudioForge.TotalMiner;
using StudioForge.TotalMiner.API;

namespace ArcadeGames
{
    class PongGame : ArcadeMachine
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
        public string ScoreText1;
        public string ScoreText2;
        public int GameScreenY;

        float gameOverTransitionTimer;
        int score1, score2;
        float paddlePos1, paddlePos2;
        Vector2 ballPos;
        int ballSize;
        float ballSize2;
        Point paddleSize;
        Vector2 paddleSize2;
        Vector2 ballVel;
        float paddleSpeed;
        float pauseTime;
        float pauseTimer;
        int paddleIndent;
        float velInc;
        bool cpu1, cpu2;

        public Rectangle Paddle1Rect { get { return new Rectangle(paddleIndent, (int)(paddlePos1 - paddleSize2.Y), paddleSize.X, paddleSize.Y); } }
        public Rectangle Paddle2Rect { get { return new Rectangle(ScreenSize.X - paddleIndent - paddleSize.X, (int)(paddlePos2 - paddleSize2.Y), paddleSize.X, paddleSize.Y); } }
        public Rectangle BallRect { get { return new Rectangle((int)(ballPos.X - ballSize2), (int)(ballPos.Y - ballSize2), ballSize, ballSize);  } }

        #endregion

        #region Properties

        public override bool CanDeactivate { get { return State == GameState.GameOver; } }

        #endregion

        #region Initialization

        public PongGame(ITMGame game, ITMMap map, ITMPlayer player, GlobalPoint3D point, BlockFace face)
            : base(game, map, player, point, face)
        {
        }

        public override void LoadContent(InitState state)
        {
            base.LoadContent(state);

            ScreenSize = new Point(renderTarget.Width, renderTarget.Height);
            State = GameState.GameOver;
            Random = new PcgRandom(new Random().Next());
            score1 = score2 = 0;
            ScoreText1 = ScoreText2 = "Score: 0";
            ballSize = 6;
            ballSize2 = ballSize * 0.5f;
            paddleSize = new Point(8, 32);
            paddleSize2 = new Vector2(paddleSize.X * 0.5f, paddleSize.Y * 0.5f);
            paddleSpeed = 3;
            paddleIndent = 4;
            GameScreenY = 16;
            velInc = 0.2f;
            cpu1 = cpu2 = true;
        }

        public override void StartGame()
        {
            if (State != GameState.Play)
            {
                if (Credits > 0)
                {
                    ChangeCredits(-1);
                    paddlePos1 = paddlePos2 = ScreenSize.Y / 2;
                    ballPos = new Vector2(ScreenSize.X / 2, ScreenSize.Y / 2);
                    ballVel = new Vector2(0.3f, 0.3f);
                    pauseTime = 5;
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
                var left = InputManager.GetGamepadLeftStick(tmPlayer.PlayerIndex);
                if (left.Y != 0 || left.X != 0)
                {
                    paddlePos1 -= left.Y * paddleSpeed;
                    cpu1 = false;
                    result = true;
                }

                var right = InputManager.GetGamepadRightStick(tmPlayer.PlayerIndex);
                if (right.Y != 0 || right.X != 0)
                {
                    paddlePos2 -= right.Y * paddleSpeed;
                    cpu2 = false;
                    result = true;
                }

                var mouse = InputManager.GetMousePosDelta(tmPlayer.PlayerIndex);
                if (mouse.Y != 0)
                {
                    paddlePos2 += mouse.Y;
                    cpu2 = false;
                    result = true;
                }

                if (InputManager1.IsInputReleasedNew(tmPlayer.PlayerIndex, GuiInput.ExitScreen))
                {
                    GameOver(false);
                    result = true;
                }
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
            if (pauseTime > 0)
            {
                pauseTimer += Services.ElapsedTime;
                if (pauseTimer < pauseTime)
                {
                    ClampPaddles();
                    return;
                }
                pauseTime = pauseTimer = 0;
            }

            ballPos += ballVel;

            if (cpu1)
            {
                var diff = ballPos.Y - paddlePos1;
                if (diff > 0) paddlePos1 += Math.Min(diff, paddleSpeed);
                else if (diff < 0) paddlePos1 -= Math.Min(-diff, paddleSpeed);
            }
            if (cpu2)
            {
                var diff = ballPos.Y - paddlePos2;
                if (diff > 0) paddlePos2 += Math.Min(diff, paddleSpeed);
                else if (diff < 0) paddlePos2 -= Math.Min(-diff, paddleSpeed);
            }

            ClampPaddles();

            if (ballPos.X < -ballSize || ballPos.X > ScreenSize.X + ballSize)
            {
                if (ballPos.X < ScreenSize.X / 2)
                {
                    ++score2;
                    ScoreText2 = "Score: " + score2;
                }
                else
                {
                    ++score1;
                    ScoreText1 = "Score: " + score1;
                }
                GameOver(true);
                return;
            }
            else
            {
                var f = float.MaxValue;

                if (ballPos.X < paddleIndent + paddleSize.X && ballPos.Y > paddlePos1 - paddleSize2.Y && ballPos.Y < paddlePos1 + paddleSize2.Y)
                {
                    f = (ballPos.Y - paddlePos1) * 0.1f;
                }
                else if (ballPos.X > ScreenSize.X - paddleIndent - paddleSize.X && ballPos.Y > paddlePos2 - paddleSize2.Y && ballPos.Y < paddlePos2 + paddleSize2.Y)
                {
                    f = (ballPos.Y - paddlePos2) * 0.1f;
                }
                else if (ballPos.Y <= GameScreenY + ballSize2 || ballPos.Y >= ScreenSize.Y - ballSize2)
                    ballVel.Y = -ballVel.Y;

                if (f != float.MaxValue)
                {
                    ballVel.X = -ballVel.X;
                    if (ballVel.X < 0) ballVel.X -= velInc; else ballVel.X += velInc;
                    ballVel.Y += f;
                }
            }
        }

        void ClampPaddles()
        {
            if (paddlePos1 - paddleSize2.Y <= GameScreenY + 2) paddlePos1 = GameScreenY + paddleSize2.Y + 2;
            if (paddlePos2 - paddleSize2.Y <= GameScreenY + 2) paddlePos2 = GameScreenY + paddleSize2.Y + 2;
            if (paddlePos1 + paddleSize2.Y >= ScreenSize.Y - 2) paddlePos1 = ScreenSize.Y - paddleSize2.Y - 2;
            if (paddlePos2 + paddleSize2.Y >= ScreenSize.Y - 2) paddlePos2 = ScreenSize.Y - paddleSize2.Y - 2;
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
