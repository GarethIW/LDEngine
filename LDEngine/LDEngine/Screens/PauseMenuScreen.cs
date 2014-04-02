#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using System;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base("Paused")
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("Resume Game", true);
            MenuEntry optionsMenuEntry = new MenuEntry("Options", true);
            MenuEntry quitGameMenuEntry = new MenuEntry("Quit Game", true);



            resumeGameMenuEntry.Selected += resumeGameMenuEntry_Selected;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);


        }

        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen());
        }

        void resumeGameMenuEntry_Selected(object sender, EventArgs e)
        {
            ScreenManager.CloseAllScreens();
        }


        #endregion

        #region Handle Input


        protected override void OnCancel(object sender, EventArgs e)
        {
            ScreenManager.CloseAllScreens();
            base.OnCancel(sender, e);
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
           
            
        }

        void SaveGameMenuEntrySelected(object sender, EventArgs e)
        {
            OnCancel(sender, e);
        }

        void LoadGameMenuEntrySelected(object sender, EventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, false, new MultiplayerGameplayScreen());
                                                           
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new MainMenuScreen());
        }


        #endregion
    }
}
