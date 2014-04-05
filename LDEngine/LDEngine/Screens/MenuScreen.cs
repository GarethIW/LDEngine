#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class MenuScreen : GameScreen
    {
        #region Fields

        // the number of pixels to pad above and below menu entries for touch input
        const int menuEntryPadding = -10;

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

 

        ContentManager content;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {

            

            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            //ScreenManager.Game.IsMouseVisible = true;

       

            base.LoadContent();
        }


        #endregion

        #region Handle Input

        /// <summary>
        /// Allows the screen to create the hit bounds for a particular menu entry.
        /// </summary>
        protected virtual Rectangle GetMenuEntryHitBounds(MenuEntry entry)
        {
            // the hit bounds are the entire width of the screen, and the height of the entry
            // with some additional padding above and below.
            return new Rectangle(
                0,
                (int)entry.Position.Y + menuEntryPadding,
                ScreenManager.Game.GraphicsDevice.Viewport.Width,
                entry.GetHeight(this)+menuEntryPadding);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            
            if (input.IsMenuCancel())
            {
                OnCancel(null, null);
            }

            if (input.IsMenuUp())
            {
                selectedEntry--;
                if (selectedEntry < 0) selectedEntry = menuEntries.Count - 1;
                while (!menuEntries[selectedEntry].Enabled)
                {
                    selectedEntry--;
                    if (selectedEntry < 0) selectedEntry = menuEntries.Count - 1;
                }
            }
            if (input.IsMenuDown())
            {
                selectedEntry++;
                if (selectedEntry >= menuEntries.Count) selectedEntry = 0;
                while (!menuEntries[selectedEntry].Enabled)
                {
                    selectedEntry++;
                    if (selectedEntry >= menuEntries.Count) selectedEntry = 0;
                }
            }
            if (input.IsMenuLeft()) menuEntries[selectedEntry].Left();
            if (input.IsMenuRight()) menuEntries[selectedEntry].Right();

            if (input.IsMenuSelect()) OnSelectEntry(selectedEntry);
            if (selectedEntry < 0) selectedEntry = menuEntries.Count - 1;
            if (selectedEntry >= menuEntries.Count) selectedEntry = 0;


            Point mouseLocation = ScreenManager.ScaledMousePos;

            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                if (GetMenuEntryHitBounds(menuEntry).Contains(mouseLocation))
                {
                    selectedEntry = i;

                    // Mouse left click?
                    if (input.CurrentMouseState.LeftButton == ButtonState.Released && input.LastMouseState.LeftButton == ButtonState.Pressed)
                    {
                       menuEntry.Click(mouseLocation.X, mouseLocation.Y);
                    }
                }
            }
            
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[entryIndex].OnSelectEntry();
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(object sender, EventArgs e)
        {
            ExitScreen();
        }


        

        public override void UnloadContent()
        {
           // ScreenManager.Game.IsMouseVisible = false;
            base.UnloadContent();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // start at Y = 175; each X value is generated per entry
            Vector2 position = new Vector2(0f, (ScreenManager.Game.GraphicsDevice.Viewport.Height/2) + 12f);

            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                
                // each entry is to be centered horizontally
                position.X = -(400f - ((i*40f))) + (420f * TransitionAlpha);
                if (position.X > 20f) position.X = 20f;
                //menuEntry.Zoom = 1f;

                //if (ScreenState == ScreenState.TransitionOn)
                //    menuEntry.Zoom += transitionOffset * 10f;
                //else
                //    menuEntry.Zoom = 1f - transitionOffset;

                // set the entry's position
                menuEntry.Position = position;

                // move down for the next entry the size of this entry plus our padding
                position.Y += menuEntry.GetHeight(this) + (menuEntryPadding);
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }

        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            GraphicsDevice graphics = ScreenManager.Game.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;


            spriteBatch.Begin();

           
            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                menuEntry.Draw(this, isSelected, gameTime);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            //Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
            //Vector2 titleOrigin = font.MeasureString(menuTitle) / 2;
            //Color titleColor = new Color(192, 192, 192) * TransitionAlpha;
            //float titleScale = 1.25f;

            //titlePosition.Y -= transitionOffset * 100;

            //spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
            //                       titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
