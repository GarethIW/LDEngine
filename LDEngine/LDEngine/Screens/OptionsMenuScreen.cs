#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry fullScreen;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            

        
            
        }

        public override void LoadContent()
        {
            // Create our menu entries.
            fullScreen = new MenuEntry(string.Empty, true);

            SetMenuEntryText();

            MenuEntry back = new MenuEntry("Back", true);

            // Hook up menu event handlers.
            fullScreen.Selected += FullScreenMenuEntrySelected;
            back.Selected += (sender, args) => ExitScreen();
        
            MenuEntries.Add(fullScreen);
            MenuEntries.Add(back);

            base.LoadContent();
        }

        public override void HandleInput(InputState input)
        {
            
            base.HandleInput(input);
        }

        protected override void OnCancel(object sender, EventArgs e)
        {
           

            base.OnCancel(sender, e);
        }

       

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            fullScreen.Text = ScreenManager.Game.GraphicsDevice.PresentationParameters.IsFullScreen ? "Windowed":"Fullscreen";
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void FullScreenMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.Game.ToggleFullScreen();
            SetMenuEntryText();
        }




        #endregion
    }

   

    public class Resolution
    {
        public int Width;
        public int Height;

        public Resolution(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public override string ToString()
        {
            return Width.ToString() + "x" + Height.ToString();
        }
    }

    public class ResolutionComparer : IEqualityComparer<Resolution>
    {
        public bool Equals(Resolution x, Resolution y)
        {
            return x.Width == y.Width && x.Height == y.Height;
        }

        public int GetHashCode(Resolution obj)
        {
            return obj.GetHashCode();
        }
    }
}
