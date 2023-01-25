using System;
using System.Diagnostics;
using System.IO;

using static System.Console;
using static System.IO.File;

using static Word_Scramble.Methods;
using static Word_Scramble.Game;

namespace Word_Scramble
{
    /// <summary>The main program class.</summary>
    class MainProgram
    {
        public static Jump jump = Jump.Continue;
        /// <summary>The Main fonction.</summary>
        public static void Main()
        {
            
            #region Config
            ConsoleConfiguration();
            #endregion

            Main_Menu :

            #region Lobby
            MainMenu();
            if(jump is not Jump.Continue) goto Select;
            Dictionary.CreateDictionary();
            #endregion

            Players_Setup : 

            #region Setting up the players
            Game session = DefineSession();
            if(jump is not Jump.Continue) goto Select;
            #endregion

            #region Game loop
            Gameloop(session, session.CurrentBoard.BoardDifficulty, new string[]{"first", "second", "third", "fourth", "fifth"});
            #endregion

            #region End of game
            if(session.IsGameFinished()) 
            {
                Move($"savedGames/{session.Name}.csv", $"archivedGames/{session.Name}.csv");
                session.DisplayWinner();
            }
            #endregion

            goto Main_Menu;

            Select :

            switch(jump)
            {
                case Jump.Continue: 
                    break;
                case Jump.Main_Menu: 
                    jump = Jump.Continue;
                    goto Main_Menu;
                    
                case Jump.Players_Setup: 
                    jump = Jump.Continue;
                    goto Players_Setup;
                case Jump.Exit: 
                    FinalExit(); 
                    break;
            }
        } 
        public enum Jump
        {
            Continue,
            Main_Menu,
            Players_Setup,
            Exit
        }
    }
}