using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using static System.Console;
using static System.ConsoleColor;
using static System.ConsoleKey;
using static System.IO.File;

using static Word_Scramble.Methods;
using static Word_Scramble.Player;

namespace Word_Scramble
{
    /// <summary>This class is used to define the course of the game .</summary>
    public class Game
    {
        #region Fields
        /// <summary>The name of the game.</summary>
        public string Name;
        /// <summary>The first player.</summary>
        public Player Player1;
        /// <summary>The second player.</summary>
        public Player Player2;
        /// <summary>The current board.</summary>
        public Board CurrentBoard;
        #endregion
        
        #region Constructors
        /// <summary>The default constructor of the game class.</summary>
        public Game()
        {
            Name = TypeGameName("You may write the name of the game below :");
            Player1 = new Player(TypePlayerName("You may write the first player's name below :"));
            Player2 = new Player(TypePlayerName("You may write the second player's name below :"));
            CurrentBoard = new Board();
            GameToCSV($"savedGames/{Name}.csv");
        }
        /// <summary>The path constructor of the game class.</summary>
        /// <param name="path">The path of the game.</param>
        public Game(string path)
        {
                string[] lines = ReadAllLines(path);
                if(lines != null)
                {
                    Name = lines[0];
                    Player1 = new Player(StringToPlayer(lines[1]));
                    Player2 = new Player(StringToPlayer(lines[2]));
                    CurrentBoard = new Board();
                    if (lines.Length > 6)
                    {
                        CurrentBoard.BoardDifficulty = lines[3];
                        CurrentBoard.WordsToFind = lines[4].Split(',').ToList();
                        CurrentBoard.Matrix = new char[lines.Length-5,(lines[5].Length/2)+1];
                        Dictionary<string, int> config = ConfigurationJson("Settings.json",CurrentBoard.BoardDifficulty);
                        CurrentBoard.Timer = config["timer"];

                        for (int i = 5; i < lines.Length; i++)
                        {
                            string[] s = lines[i].Split(';'); 
                            char[] c = new char[s.Length];
                            for (int j = 0; j < s.Length; j++)c[j] = s[j][0];
                            int l = i-5;
                            for (int k = 0; k < CurrentBoard.Matrix.GetLength(1); k++) CurrentBoard.Matrix[l,k] = c[k];
                        }
                    }
                }else throw new Exception("The file is empty.");
        }
        #endregion

        #region Core methods
        /// <summary>This method is used to display the main menu.</summary>
        public static void MainMenu()
        {
            switch(ScrollingMenu("Welcome Adventurer! Use the arrow keys to move and press [ENTER] to confirm.", new string[]{"Play    ","Options ","Quit    "}, "Title.txt"))
            {
                case 0 : 
                    MainProgram.jump = MainProgram.Jump.Continue; 
                    break;
                case 1 : 
                    switch(ScrollingMenu("You may select a language for the words to find:", new string[] { " English  ", " Français " }, "Title.txt"))
                    {
                        case 0:
                            Dictionary.s_Language = "EN";
                            break;
                        case 1:
                            Dictionary.s_Language = "FR";
                            break;
                        case -1 : 
                            break;
                    }
                    MainProgram.jump = MainProgram.Jump.Main_Menu;
                    break;
                case 2 : case -1: 
                    FinalExit();
                    break;
            }
        }
        /// <summary>This method is used to define the current session.</summary>
        /// <returns>A boolean to know whether the user wants to go back or not.</returns>
        public static Game DefineSession()
        {
           switch(ScrollingMenu("Would you like to play a new game or load a previous one?", new string[]{" New     "," Load    "," Back    "}, "Title.txt"))
           {
               case 0 : 
                    MainProgram.jump = MainProgram.Jump.Continue;
                    return new Game();
               case 1 : 
                    string[] files = Directory.GetFiles("savedGames");
                    string[] fileNames = new string[files.Length];
                    for(int i = 0; i < files.Length; i++)
                    {
                        fileNames[i] = files[i].Substring(11);
                        fileNames[i] = fileNames[i].Substring(0, fileNames[i].Length-4);
                        if(fileNames[i].Length < 15) fileNames[i] += new string(' ', 15-fileNames[i].Length);
                    }
                    int position;
                    switch(position = ScrollingMenu("You may select a game to load among those already created:", fileNames, "Title.txt"))
                    {
                        case -1 : 
                            MainProgram.jump = MainProgram.Jump.Players_Setup;
                            break;
                        default : 
                            MainProgram.jump = MainProgram.Jump.Continue;  
                            return new Game(files[position]);
                    }
                    break;
               case 2 : case -1 : 
                    MainProgram.jump = MainProgram.Jump.Main_Menu; 
                    break;
           }
           return null;
        }
        /// <summary>This method is used to play the game.</summary>
        /// <param name="session">The current session.</param>
        /// <param name="difficulty">The current difficulty.</param>
        /// <param name="level">The list of difficulties.</param>
        public static void Gameloop(Game session, string difficulty, string[] level)
        {
            if(session.Player1.InGame)
            {
                LoadingScreen("[  Loading the game ...  ]");
                session.SelectWords(session.Player1);
                InGameSwitch(session, "player2");
                session.CurrentBoard = new Board(difficulty);
                session.GameToCSV($"savedGames/{session.Name}.csv");

                Gameloop(session, difficulty, level);
            }
            else if (session.Player2.InGame)
            {
                LoadingScreen("[  Loading the game ...  ]");
                session.SelectWords(session.Player2);
                
                if (difficulty != "fifth") 
                {
                    InGameSwitch(session, "player1");
                    session.CurrentBoard = new Board(level[Array.IndexOf(level, difficulty) + 1]);
                    session.GameToCSV($"savedGames/{session.Name}.csv");
                    Gameloop(session, level[Array.IndexOf(level, difficulty) + 1], level);
                }
                else 
                {
                    session.Player2.InGame = false;
                    session.GameToCSV($"savedGames/{session.Name}.csv");
                }
                
            }
            else if (difficulty == "first" || difficulty == "")
            {
                InGameSwitch(session, "first");
                LoadingScreen("[  Loading the game ...  ]");
                session.CurrentBoard = new Board("first");
                InGameSwitch(session, "player1");
                session.GameToCSV($"savedGames/{session.Name}.csv");
                session.SelectWords(session.Player1);
                InGameSwitch(session, "player2");
                session.CurrentBoard = new Board("first");
                session.GameToCSV($"savedGames/{session.Name}.csv");

                Gameloop(session, "first", level);
            }
        }
        /// <summary>This method is used to switch the players turn.</summary>
        /// <param name="session">The current session.</param>
        /// <param name="turn">The player who's turn it is.</param>
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
        /// <summary>This method is used to select words and check if they are in the list.</summary>
        /// <param name="player">The current player.</param>
        /// <returns>A string as the sum of the characters.</returns>
        public void SelectWords(Player player)
        {
            List<string> wordsLeft = this.CurrentBoard.WordsToFind;
            List<Position> correctPositions = new List<Position>();
            Stopwatch chrono = new Stopwatch();
            long minutes = CurrentBoard.Timer/60000;
            long seconds = (CurrentBoard.Timer/1000 - minutes*60);
            BoardMessage(new string []{$" {player.Name}, are you ready to start the {CurrentBoard.BoardDifficulty} level? ",$" You have {minutes} minutes {seconds}s!, good luck! "});
            Pause();
            chrono.Start();
            while(wordsLeft.Count != 0 && chrono.ElapsedMilliseconds < CurrentBoard.Timer) 
            {
                Position currentPosition = new Position(CurrentBoard.Matrix.GetLength(0)/2,CurrentBoard.Matrix.GetLength(1)/2);
                Position anchor1 = new Position(-1, -1);
                Position anchor2 = new Position(-1, -1);
                List<Position> selectedPositions = new List<Position>();

                List<Position> possiblePositions = new List<Position>();
                for(int i = 0; i< CurrentBoard.Matrix.GetLength(0); i++)for(int j = 0; j < CurrentBoard.Matrix.GetLength(1); j++)possiblePositions.Add(new Position(i, j));

                while(anchor2.X == -1 && wordsLeft.Count != 0 && chrono.ElapsedMilliseconds < CurrentBoard.Timer)
                {
                    BoardMessage(new string []{$" {player.Name} is playing on the {CurrentBoard.BoardDifficulty} level and has {(CurrentBoard.Timer-chrono.ElapsedMilliseconds)/1000} seconds left. ", $" Your current score is : {player.Score} points. "," Words left : " + string.Join(", ", wordsLeft)+" "});
                    PrintMatrix(CurrentBoard.Matrix, selectedPositions,correctPositions, currentPosition);
                    switch(ReadKey(true).Key)
                    {
                        case UpArrow : case Z :
                            if(possiblePositions.Contains(new Position(currentPosition.X - 1, currentPosition.Y)))currentPosition.X--;
                            else currentPosition = new Position(NearestPosition(CurrentBoard.Matrix, currentPosition, possiblePositions,"up"));
                            break;
                        case DownArrow : case S :
                            if(possiblePositions.Contains(new Position(currentPosition.X + 1, currentPosition.Y)))currentPosition.X++;
                            else currentPosition = new Position(NearestPosition(CurrentBoard.Matrix, currentPosition, possiblePositions,"down"));
                            break;
                        case LeftArrow :case Q :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y - 1)))currentPosition.Y--;
                            else currentPosition = new Position(NearestPosition(CurrentBoard.Matrix, currentPosition, possiblePositions,"left"));
                            break;
                        case RightArrow : case D :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y + 1)))currentPosition.Y++;
                            else currentPosition = new Position(NearestPosition(CurrentBoard.Matrix, currentPosition, possiblePositions,"right"));
                            break;
                        case Enter : 
                            if(anchor1.X == -1)
                            {
                                anchor1 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Add(new Position(currentPosition.X, currentPosition.Y));
                                possiblePositions = ValidPositions(CurrentBoard.Matrix, anchor1, possiblePositions);
                            }
                            else if (!anchor1.Equals(currentPosition))
                            {
                                anchor2 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Clear();
                                selectedPositions = Board.GetPositionsBetween(anchor1, anchor2);
                                string word = "";
                                for(int i = 0; i < selectedPositions.Count; i++)
                                {
                                    word += CurrentBoard.Matrix[selectedPositions[i].X, selectedPositions[i].Y];
                                }
                                if(wordsLeft.Contains(word))
                                {
                                    correctPositions.AddRange(selectedPositions);
                                    player.AddWord(word);
                                    wordsLeft.Remove(word);
                                    Clear();
                                    WriteLine();
                                    CenteredWL($" Well played ! {word} is in the list of words to find. ", Black, Green);
                                    WriteLine();
                                    PrintMatrix(CurrentBoard.Matrix, selectedPositions, correctPositions, currentPosition, false);
                                }else{
                                    Clear();
                                    WriteLine();
                                    CenteredWL($" Try again. {word} is not in the list of words to find. ", Black, Red);
                                    WriteLine();
                                    PrintMatrix(CurrentBoard.Matrix, selectedPositions, correctPositions, currentPosition, false);
                                }
                                WriteLine();
                                Pause();
                            }
                            break;
                        case Escape :
                            chrono.Stop();
                            switch(ScrollingMenu("Do you want to quit the game ?", new string[]{" Resume    ", " Skip turn "," Main menu "}, "Title.txt"))
                            {
                                case 0 : 
                                    chrono.Start(); 
                                    break;
                                case 1 : case -1 : 
                                    BoardMessage(new string[]{$"  {player.Name}, you decided to skip this turn. You cannot take back on this decision. ",$"Your current score is {player.Score} points."}, Red);
                                    Pause();
                                    return;
                                case 2 : 
                                    MainProgram.Main(); 
                                    break;
                            }
                            break;
                    }
                }
            }
            if (wordsLeft.Count == 0) 
            {
                chrono.Stop();
                player.AddBonus((int)chrono.ElapsedMilliseconds/1000);
                BoardMessage(new string[]{$" Congratulations {player.Name} ! You found all the words on time! ",$" Your recieved a bonus of {(int)((CurrentBoard.Timer - chrono.ElapsedMilliseconds)/1000)}s point. ", $" Your current score is now : {player.Score} points ! "});
                Pause();
            }
            else 
            {
                BoardMessage(new string[]{$" Time's up {player.Name} ! You didn't find all the words. ", $" Your current score is now : {player.Score} points ! "}, Red);
                Pause();
            }
        }
        /// <summary>This method is used to display the winner of the game.</summary>
        public void DisplayWinner()
        {
            switch(Player1.Score.CompareTo(Player2.Score))
            {
                case 1:
                    BoardMessage(new string[]{$"Congratulations {Player1.Name}! You won the game with a score of {Player1.Score} points!","", $"Well done {Player2.Name}! You still scored {Player2.Score} points."});
                    break;
                case -1:
                    BoardMessage(new string[]{$"Congratulations {Player2.Name}! You won the game with a score of {Player2.Score} points!","", $"Well done {Player1.Name}! You still scored {Player1.Score} points."});
                    break;
                case 0:
                    BoardMessage(new string[]{$"Congratulations {Player1.Name} and {Player2.Name}! You both won the game with a score of {Player1.Score} points!"});
                    break;
            }
        }
        #endregion

        #region Utility methods
        /// <summary>This method is used to write the game on a CSV.</summary>
        /// <param name="path">The path of the CSV file.</param>
        public void GameToCSV(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(Name);
                sw.WriteLine(Player1);
                sw.WriteLine(Player2);
                sw.WriteLine(CurrentBoard.BoardDifficulty);
                for (int i = 0; i < CurrentBoard.WordsToFind.Count; i++)
                {
                    if (CurrentBoard.WordsToFind.Count !=0)
                    {
                        sw.Write(CurrentBoard.WordsToFind[i]);
                        if (i != CurrentBoard.WordsToFind.Count - 1) sw.Write(",");
                    }
                }
                sw.WriteLine();
                for (int i = 0; i < CurrentBoard.Matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < CurrentBoard.Matrix.GetLength(1); j++)
                    {
                        if (j == CurrentBoard.Matrix.GetLength(1)-1) sw.Write(CurrentBoard.Matrix[i,j]);
                        else sw.Write(CurrentBoard.Matrix[i,j]+";");
                    }
                    if (i != CurrentBoard.Matrix.GetLength(0)-1)sw.WriteLine();
                }
                sw.Close();
            }
        }
        /// <summary>This method is used to check if the game is finished.</summary>
        /// <returns>True if the game is finished, false otherwise.</returns>
        public bool IsGameFinished() => !Player1.InGame && !Player2.InGame && CurrentBoard.BoardDifficulty == "fifth";
        #endregion
    }
}