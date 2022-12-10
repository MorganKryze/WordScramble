using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;

namespace Word_Scramble
{
    /// <summary>This class is used to define the course of the game .</summary>
    public abstract class Game
    {
        /// <summary>This method is used to display the main menu.</summary>
        public static void MainMenu()
        {
            switch(Methods.ScrollingMenu("Welcome Adventurer! Use the arrow keys to move and press [ENTER] to confirm.", new string[]{"Play    ","Options ","Exit    "}, "Title.txt"))
            {
                case 0 : break;  
                case 1 : MainMenu(); break;
                case 2 : case -1: Methods.FinalExit();break;
            }
        }
        /// <summary>This method is used to define the current session.</summary>
        /// <param name="player1">The first player.</param>
        /// <param name="player2">The second player.</param>
        /// <returns>A boolean to know whether the user wants to go back or not.</returns>
        public static bool DefinePlayers(Player player1, Player player2)
        {
           switch(Methods.ScrollingMenu("Would you like to play a new game or load a previous one?", new string[]{"New Game  ","Load Game ","Back      "}, "Title.txt"))
           {
               case 0 :
                    player1.Name = Methods.TypePlayerName(player1, "Please write the first player's name below :");
                    player2.Name = Methods.TypePlayerName(player2, "Please write the second player's name below :");
                    break;
               case 1 : 
                    Ranking ranking = new Ranking();
                    int position1 = Methods.ScrollingMenu("Please choose the first player in the list below :", ranking.NotFinished.Select(p => p.Name).ToArray(), "Title.txt");
                    if(position1==-1) return DefinePlayers(player1, player2);
                    else player1 = new Player(ranking.NotFinished[position1]);
                    int position2 = Methods.ScrollingMenu("Please choose the second player in the list below :", ranking.NotFinished.Select(p => p.Name).ToArray(), "Title.txt");
                    if(position2==-1) return DefinePlayers(player1, player2);
                    else player2 = new Player(ranking.NotFinished[position2]);
                    break;
               case 2 : case -1 : return false;
           }return true;
        }
        /// <summary>This method is used to select multiple characters.</summary>
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
        /// <returns>A string as the sum of the characters.</returns>
        public static void SelectWords(char[,] matrix, List<string> wordsToFind)
        {
            
            List<string> wordsLeft = wordsToFind;
            List<Position> correctPositions = new List<Position>();
            while(wordsLeft.Count != 0)
            {
                Position currentPosition = new Position(matrix.GetLength(0)/2,matrix.GetLength(1)/2);
                Position anchor1 = new Position(-1, -1);
                Position anchor2 = new Position(-1, -1);
                List<Position> selectedPositions = new List<Position>();

                List<Position> possiblePositions = new List<Position>();
                for(int i = 0; i< matrix.GetLength(0); i++)for(int j = 0; j < matrix.GetLength(1); j++)possiblePositions.Add(new Position(i, j));

                while(anchor2.X == -1&&wordsLeft.Count != 0)
                {
                    Clear();
                    Methods.PrintMatrix(matrix, selectedPositions,correctPositions, currentPosition);
                    switch(ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow : case ConsoleKey.Z :
                            if(possiblePositions.Contains(new Position(currentPosition.X - 1, currentPosition.Y)))currentPosition.X--;
                            else currentPosition = new Position(Methods.NearestPosition(matrix, currentPosition, possiblePositions,"up"));
                            break;
                        case ConsoleKey.DownArrow : case ConsoleKey.S :
                            if(possiblePositions.Contains(new Position(currentPosition.X + 1, currentPosition.Y)))currentPosition.X++;
                            else currentPosition = new Position(Methods.NearestPosition(matrix, currentPosition, possiblePositions,"down"));
                            break;
                        case ConsoleKey.LeftArrow :case ConsoleKey.Q :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y - 1)))currentPosition.Y--;
                            else currentPosition = new Position(Methods.NearestPosition(matrix, currentPosition, possiblePositions,"left"));
                            break;
                        case ConsoleKey.RightArrow : case ConsoleKey.D :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y + 1)))currentPosition.Y++;
                            else currentPosition = new Position(Methods.NearestPosition(matrix, currentPosition, possiblePositions,"right"));
                            break;
                        case ConsoleKey.Enter : 
                            if(anchor1.X == -1)
                            {
                                anchor1 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Add(new Position(currentPosition.X, currentPosition.Y));
                                possiblePositions = Methods.ValidPositions(matrix, anchor1, possiblePositions);
                            }
                            else if (!anchor1.Equals(currentPosition))
                            {
                                anchor2 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Clear();
                                selectedPositions = Board.GetPositionsBetween(anchor1, anchor2);
                                string word = "";
                                for(int i = 0; i < selectedPositions.Count; i++)
                                {
                                    word += matrix[selectedPositions[i].X, selectedPositions[i].Y];
                                }
                                if(wordsLeft.Contains(word))
                                {
                                    correctPositions.AddRange(selectedPositions);
                                    wordsLeft.Remove(word);
                                }
                                Clear();
                                Methods.PrintMatrix(matrix, selectedPositions, correctPositions, currentPosition, false);
                                Methods.Pause();
                            }
                            break;
                        case ConsoleKey.Escape :
                            wordsLeft.Clear();
                            break;
                    }
                }
            }
        }
    }
}