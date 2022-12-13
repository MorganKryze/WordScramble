using System;
using System.Diagnostics;
using System.IO;

using static System.IO.File;

using static Word_Scramble.Methods;
using static Word_Scramble.Game;

namespace Word_Scramble
{
    class MainProgram
    {

        public static void Main()
        {
            
            #region Config
            ConsoleConfig();
            #endregion

            Main_Menu :

            #region Lobby
            MainMenu();
            Dictionary.CreateDictionary();
            #endregion

            #region Setting up the players
            Game session = DefineSession();
            if(session.Name == "Back")goto Main_Menu;
            
            #endregion

            #region Game loop
            string[] level = new string [] {"first", "second", "third", "fourth", "fifth"};
            Gameloop(session, session.CurrentBoard.BoardDifficulty, level);
            #endregion

            #region End of game
            if (session.IsGameFinished()) Move($"savedGames/{session.Name}.csv", $"archivedGames/{session.Name}.csv");
            #endregion

            goto Main_Menu;
        }
    }
}