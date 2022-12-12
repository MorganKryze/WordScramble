using System;
using System.Diagnostics;

using static System.Console;
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
            Pause();
            #endregion

            goto Main_Menu;
        }

        public static void Gameloop(Game session, string difficulty, string[] level)
        {
            if(session.Player1.InGame)
            {
                LoadingScreen("[  Loading the game ...  ]");
                session.SelectWords(session.Player1);
                InGameSwitch(session, "player2");
                session.CurrentBoard = new Board(difficulty);
                session.GameToCSV($"dataGames/{session.Name}.csv");

                Gameloop(session, difficulty, level);
            }
            else if (session.Player2.InGame)
            {
                LoadingScreen("[  Loading the game ...  ]");
                session.SelectWords(session.Player2);
                InGameSwitch(session, "player1");
                session.CurrentBoard = new Board(level[Array.IndexOf(level, difficulty) + 1]);
                session.GameToCSV($"dataGames/{session.Name}.csv");

                if (difficulty != "fifth") Gameloop(session, level[Array.IndexOf(level, difficulty) + 1], level);
            }
            else
            {
                InGameSwitch(session, "first");
                LoadingScreen("[  Loading the game ...  ]");
                session.CurrentBoard = new Board("first");
                InGameSwitch(session, "player1");
                session.GameToCSV($"dataGames/{session.Name}.csv");
                session.SelectWords(session.Player1);
                InGameSwitch(session, "player2");
                session.CurrentBoard = new Board("first");
                session.GameToCSV($"dataGames/{session.Name}.csv");

                Gameloop(session, "first", level);
            }
        }
        public static void InGameSwitch(Game session, string turn)
        {
            if (turn == "player1")
            {
                session.Player1.InGame = true;
                session.Player2.InGame = false;
            }
            else if (turn == "player2")
            {
                session.Player1.InGame = false;
                session.Player2.InGame = true;
            }
        }
    }
}