#region File Description
//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        #region Fields

        string message;
        Texture2D texBG;

        #endregion

        #region Events

        public event EventHandler<EventArgs> Accepted;
        public event EventHandler<EventArgs> Cancelled;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "A=ok, B=cancel" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string message)
        {
            this.message = message;

            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            texBG = content.Load<Texture2D>("ui/messagebg");
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;
            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;
            Vector2 halfSize = new Vector2(viewport.Width, viewport.Height)/2;
            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect())
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new EventArgs());

                ExitScreen();
            }
            else if (input.IsMenuCancel())
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new EventArgs());

                ExitScreen();
            }

            Point mouseLoc = new Point(input.CurrentMouseState.X, input.CurrentMouseState.Y);

            Rectangle okRect = new Rectangle((int)halfSize.X - 25, (int)halfSize.Y + 25, 50, 50);
            if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && input.LastMouseState.LeftButton == ButtonState.Released)
            {
                if (okRect.Contains(mouseLoc)) ExitScreen();
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            // Darken down any other screens that were drawn beneath the popup.
            //ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            // Center the message text in the viewport.
            Viewport viewport = ScreenManager.Game.GraphicsDevice.Viewport;
            Vector2 halfSize = new Vector2(viewport.Width, viewport.Height)/2;


            // Fade the popup alpha during transitions.
            Color color = Color.White * TransitionAlpha;

            spriteBatch.Begin();

            // Draw the background rectangle.
            spriteBatch.Draw(texBG, halfSize, null, color, 0f, new Vector2(texBG.Width,texBG.Height)/2,1f,SpriteEffects.None,1);

            // Draw the message box text.
            spriteBatch.DrawString(font, message, halfSize+new Vector2(0,-50f), new Color(128,128,128)*TransitionAlpha,0f,font.MeasureString(message)/2f,1f, SpriteEffects.None,1);

            spriteBatch.DrawString(font, "OK", halfSize + new Vector2(0, 50f), new Color(128, 128, 128) * TransitionAlpha, 0f, font.MeasureString("OK") / 2f, 1f, SpriteEffects.None, 1);


            spriteBatch.End();
        }


        #endregion
    }
}
