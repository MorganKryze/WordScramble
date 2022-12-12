using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
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
        #region Attributes
        public string Name {get; set;}
        public Player Player1 {get; set;}
        public Player Player2 {get; set;}
        public Board CurrentBoard {get; set;}
        #endregion
        
        #region Constructors
        public Game()
        {
            Name = TypeGameName("Please write the name of the game below :");
            Player1 = new Player(TypePlayerName("Please write the first player's name below :"));
            Player2 = new Player(TypePlayerName("Please write the second player's name below :"));
            CurrentBoard = new Board();
            GameToCSV($"dataGames/{Name}.csv");
        }
        public Game(bool back)
        {
            Name = "Back";
            Player1 = new Player();
            Player2 = new Player();
            CurrentBoard = new Board();
        }
        public Game(string path)
        {
                string[] lines = ReadAllLines(path);
                if(lines != null)
                {
                    Name = lines[0];
                    Player1 = new Player(StringToPlayer(lines[1]));
                    Player2 = new Player(StringToPlayer(lines[2]));
                    CurrentBoard = new Board();
                    if (lines.Length >= 4)
                    {
                        CurrentBoard.BoardDifficulty = lines[3];
                        CurrentBoard.WordsToFind = lines[4].Split(',').ToList();
                        CurrentBoard.Matrix = new char[lines.Length-5,(lines[5].Length/2)+1];

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
                case 0 : break;  
                case 1 : 
                    switch(ScrollingMenu("Please select a language :", new string[] { " English  ", " Français " }, "Title.txt"))
                    {
                        case 0:
                             Dictionary.s_Language = "EN";
                            break;
                        case 1:
                            Dictionary.s_Language = "FR";
                            break;
                        case -1 : MainMenu();
                            break;
                    }
                    break;
                case 2 : case -1: FinalExit();break;
            }
        }
        /// <summary>This method is used to define the current session.</summary>
        /// <returns>A boolean to know whether the user wants to go back or not.</returns>
        public static Game DefineSession()
        {
           switch(ScrollingMenu("Would you like to play a new game or load a previous one?", new string[]{" New     "," Load    "," Back    "}, "Title.txt"))
           {
               case 0 : return new Game();
               case 1 : 
                    string[] files = Directory.GetFiles("dataGames");
                    string[] fileNames = new string[files.Length];
                    for(int i = 0; i < files.Length; i++)
                    {
                        fileNames[i] = files[i].Substring(10);
                        fileNames[i] = fileNames[i].Substring(0, fileNames[i].Length-4);
                    }
                    int position = ScrollingMenu("Please select a game to load :", fileNames, "Title.txt");
                    switch(position)
                    {
                        case -1 : DefineSession(); break;
                        default : return new Game(files[position]);
                    }
                    break;
               case 2 : case -1 : MainMenu(); 
                    break;
           }return new Game(true);
        }
        /// <summary>This method is used to select words and check if they are in the list.</summary>
        /// <param name="Currentboard.Matrix">The grill from wich the user chooses the characters.</param>
        /// <returns>A string as the sum of the characters.</returns>
        public bool SelectWords( Player player)
        {
            List<string> wordsLeft = this.CurrentBoard.WordsToFind;
            List<Position> correctPositions = new List<Position>();
            while(wordsLeft.Count != 0)
            {
                Position currentPosition = new Position(CurrentBoard.Matrix.GetLength(0)/2,CurrentBoard.Matrix.GetLength(1)/2);
                Position anchor1 = new Position(-1, -1);
                Position anchor2 = new Position(-1, -1);
                List<Position> selectedPositions = new List<Position>();

                List<Position> possiblePositions = new List<Position>();
                for(int i = 0; i< CurrentBoard.Matrix.GetLength(0); i++)for(int j = 0; j < CurrentBoard.Matrix.GetLength(1); j++)possiblePositions.Add(new Position(i, j));

                while(anchor2.X == -1&&wordsLeft.Count != 0)
                {
                    Clear();
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
                            switch(ScrollingMenu("Do you want to quit the game ?", new string[]{" Resume    ", " Skip turn "," Main menu "}, "Title.txt"))
                            {
                                case 0 : break;
                                case 1 : case -1 : 
                                    CompletedBoardMessage(player, new string[]{$"  {player.Name}, you decided to quit the game. You cannot take back on this decision. ",$"Your current score is {player.Score} points."}, Red);
                                    return false;
                                case 2 : MainProgram.Main(); 
                                    break;
                            }
                            break;
                    }
                }
            }
            CompletedBoardMessage(player,new string[]{$" Congratulations {player.Name} ! You found all the words ! ", $"Your current score is now : {player.Score} points !"});
            return true;
        }
        #endregion

        #region Utility methods
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
        #endregion
    }
}