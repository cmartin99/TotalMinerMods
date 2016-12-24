using System;
using Craig.BlockWorld;
using Craig.Engine;
using Craig.Engine.Core;
using Craig.Engine.Integration;
using Craig.TotalMiner;
using Craig.TotalMiner.API;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

        float gameOverTransitionTimer;
        int score1, score2;
        float paddlePos1, paddlePos2;
        Vector2 ballPos;
        int ballSize;
        Point paddleSize;
        Vector2 ballVel;
        float paddleSpeed;
        float pauseTime;
        float pauseTimer;

        public Rectangle Paddle1Rect { get { return new Rectangle(2, (int)(paddlePos1 - paddleSize.Y / 2f), paddleSize.X, paddleSize.Y); } }
        public Rectangle Paddle2Rect { get { return new Rectangle(ScreenSize.X - 2 - paddleSize.X, (int)(paddlePos2 - paddleSize.Y / 2f), paddleSize.X, paddleSize.Y); } }
        
        public Rectangle BallRect 
        { 
            get 
            { 
                var s = ballSize / 2f;
                return new Rectangle((int)(ballPos.X - s), (int)(ballPos.Y - s), ballSize, ballSize); 
            }
        }

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
            ScoreText1 = ScoreText2 = "Score: 0";
            ballSize = 6;
            paddleSize = new Point(8, 25);
            paddleSpeed = 1;
        }

        public override void StartGame()
        {
            if (State != GameState.Play)
            {
                if (Credits > 0)
                {
                    ChangeCredits(-1);
                    score1 = score2 = 0;
                    ScoreText1 = ScoreText2 = "Score: 0";
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
            if (pauseTime > 0)
            {
                pauseTimer += Services.ElapsedTime;
                if (pauseTimer < pauseTime) return;
                pauseTime = pauseTimer = 0;
            }

            ballPos += ballVel;

            if ((int)paddlePos1 < (int)ballPos.Y) paddlePos1 += paddleSpeed;
            else if ((int)paddlePos1 > (int)ballPos.Y) paddlePos1 -= paddleSpeed;

            if ((int)paddlePos2 < (int)ballPos.Y) paddlePos2 += paddleSpeed;
            else if ((int)paddlePos2 > (int)ballPos.Y) paddlePos2 -= paddleSpeed;

            if (ballPos.X < 0 || ballPos.X > ScreenSize.X)
            {
                if (ballPos.X < 0)
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
                if (ballPos.X < 4 + paddleSize.X && ballPos.Y > paddlePos1 - paddleSize.Y / 2 && ballPos.Y < paddlePos1 + paddleSize.Y / 2)
                {
                    ballVel.X = -ballVel.X;
                    if (ballVel.X < 0) ballVel.X -= 0.1f; else ballVel.X += 0.1f;
                }
                else if (ballPos.X > ScreenSize.X - 4 - paddleSize.X && ballPos.Y > paddlePos2 - paddleSize.Y / 2 && ballPos.Y < paddlePos2 + paddleSize.Y / 2)
                {
                    ballVel.X = -ballVel.X;
                    if (ballVel.X < 0) ballVel.X -= 0.1f; else ballVel.X += 0.1f;
                }
                else if (ballPos.Y < 10 + ballSize / 2 || ballPos.Y > ScreenSize.Y - ballSize / 2 - 1)
                    ballVel.Y = -ballVel.Y;
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
