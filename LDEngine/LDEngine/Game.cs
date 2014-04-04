using System;
using System.Collections.Generic;
using System.Linq;
using GameStateManagement;
using LDEngine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TimersAndTweens;

namespace LDEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        private ScreenManager screenManager;

        private TimerController timerController = new TimerController();
        private TweenController tweenController = new TweenController();

        public int RenderWidth = 320;
        public int RenderHeight = 180;

        private RenderTarget2D renderTarget;

        private int DisplayScale = 4;

        private KeyboardState lks;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = RenderWidth * DisplayScale;
            graphics.PreferredBackBufferHeight = RenderHeight * DisplayScale;
            graphics.ApplyChanges();

            screenManager = new ScreenManager(this);
            screenManager.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            screenManager.LoadContent();
            renderTarget = new RenderTarget2D(GraphicsDevice, RenderWidth, RenderHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            spriteBatch = new SpriteBatch(GraphicsDevice);

            lks = Keyboard.GetState();

            screenManager.AddScreen(new GameplayScreen());
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            screenManager.Update(gameTime);

            timerController.Update(gameTime);
            tweenController.Update(gameTime);

            KeyboardState cks = Keyboard.GetState();

            if (cks.IsKeyDown(Keys.PageDown) && !lks.IsKeyDown(Keys.PageDown) && DisplayScale > 1)
            {
                DisplayScale--;
                ChangeDisplayScale();
            }
            if (cks.IsKeyDown(Keys.PageUp) && !lks.IsKeyDown(Keys.PageUp) && DisplayScale < 5)
            {
                DisplayScale++;
                ChangeDisplayScale();
            }

            lks = cks;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            GraphicsDevice.Clear(Color.Black);
            screenManager.Draw(gameTime);

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            spriteBatch.Draw(renderTarget, new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height)/2, null, Color.White, 0f, new Vector2(renderTarget.Width,renderTarget.Height)/2, DisplayScale,SpriteEffects.None,1);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        void ChangeDisplayScale()
        {
            graphics.PreferredBackBufferWidth = RenderWidth * DisplayScale;
            graphics.PreferredBackBufferHeight = RenderHeight * DisplayScale;
            graphics.ApplyChanges();
        }
    }
}

