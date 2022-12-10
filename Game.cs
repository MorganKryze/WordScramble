using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Console;
using static System.ConsoleColor;
using static System.ConsoleKey;
using static Word_Scramble.Methods;

namespace Word_Scramble
{
    /// <summary>This class is used to define the course of the game .</summary>
    public static class Game
    {
        /// <summary>This method is used to display the main menu.</summary>
        public static void MainMenu()
        {
            switch(Methods.ScrollingMenu("Welcome Adventurer! Use the arrow keys to move and press [ENTER] to confirm.", new string[]{"Play    ","Options ","Quit    "}, "Title.txt"))
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
        public static bool DefinePlayers(Player player1, Player player2, Ranking ranking)
        {
           switch(ScrollingMenu("Would you like to play a new game or load a previous one?", new string[]{" New     "," Load    "," Back    "}, "Title.txt"))
           {
               case 0 :
                    player1.Name = TypePlayerName(player1, "Please write the first player's name below :");
                    player2.Name = TypePlayerName(player2, "Please write the second player's name below :");
                    break;
               case 1 : 
                    int position1 = ScrollingMenu("Please choose the first player in the list below :", ranking.NotFinished.Select(p => p.Name).ToArray(), "Title.txt");
                    if(position1==-1) return DefinePlayers(player1, player2, ranking);
                    else 
                    {
                        player1.Name = ranking.NotFinished[position1].Name;
                        player1.InGame = false;
                        player1.Score = ranking.NotFinished[position1].Score;
                        player1.MaxScore = ranking.NotFinished[position1].MaxScore;
                        player1.Words = ranking.NotFinished[position1].Words;
                    }

                    int position2 = ScrollingMenu("Please choose the second player in the list below :", ranking.NotFinished.Select(p => p.Name).ToArray(), "Title.txt");
                    if(position2==-1) return DefinePlayers(player1, player2, ranking);
                    else
                    {
                        player2.Name = ranking.NotFinished[position2].Name;
                        player2.InGame = false;
                        player2.Score = ranking.NotFinished[position2].Score;
                        player2.MaxScore = ranking.NotFinished[position2].MaxScore;
                        player2.Words = ranking.NotFinished[position2].Words;
                    }
                    break;
               case 2 : case -1 : return false;
           }return true;
        }
        /// <summary>This method is used to select words and check if they are in the list.</summary>
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
        /// <returns>A string as the sum of the characters.</returns>
        public static bool SelectWords(char[,] matrix, List<string> wordsToFind, Player player)
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
                    PrintMatrix(matrix, selectedPositions,correctPositions, currentPosition);
                    switch(ReadKey(true).Key)
                    {
                        case UpArrow : case Z :
                            if(possiblePositions.Contains(new Position(currentPosition.X - 1, currentPosition.Y)))currentPosition.X--;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"up"));
                            break;
                        case DownArrow : case S :
                            if(possiblePositions.Contains(new Position(currentPosition.X + 1, currentPosition.Y)))currentPosition.X++;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"down"));
                            break;
                        case LeftArrow :case Q :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y - 1)))currentPosition.Y--;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"left"));
                            break;
                        case RightArrow : case D :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y + 1)))currentPosition.Y++;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"right"));
                            break;
                        case Enter : 
                            if(anchor1.X == -1)
                            {
                                anchor1 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Add(new Position(currentPosition.X, currentPosition.Y));
                                possiblePositions = ValidPositions(matrix, anchor1, possiblePositions);
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
                                    player.AddWord(word);
                                    wordsLeft.Remove(word);
                                    Clear();
                                    WriteLine();
                                    CenteredWL($" Well played ! {word} is in the list of words to find. ", Black, Green);
                                    WriteLine();
                                    PrintMatrix(matrix, selectedPositions, correctPositions, currentPosition, false);
                                }else{
                                    Clear();
                                    WriteLine();
                                    CenteredWL($" Try again. {word} is not in the list of words to find. ", Black, Red);
                                    WriteLine();
                                    PrintMatrix(matrix, selectedPositions, correctPositions, currentPosition, false);
                                }
                                WriteLine();
                                Pause();
                            }
                            break;
                        case Escape :
                            switch(ScrollingMenu("Do you want to quit the game ?", new string[]{" Yes  ", " No  "}, "Title.txt"))
                            {
                                case 0 : case -1 : 
                                    CompletedBoardMessage(player, new string[]{$" {player.Name}, you decided to quit the game. You cannot take back on this decision.",$"Your current score is {player.Score} points."}, Red);
                                    return false;
                                case 1 : break;
                            }
                            break;
                    }
                }
            }
            CompletedBoardMessage(player,new string[]{$"Congratulations {player.Name} ! You found all the words !", $"Your current score is now : {player.Score} points !"});
            return true;
        }
    }
}