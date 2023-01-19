using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.VisualBasic.FileIO;

using static System.Console;
using static System.IO.File;
using static Word_Scramble.Methods;

namespace Word_Scramble
{
    /// <summary>The board creation class.</summary>
    public class Board
    {
        #region Fields
        /// <summary>The matrix of the board.</summary>
        public char[,] Matrix;
        /// <summary>The list of the words to find.</summary>
        public List<string> WordsToFind;
        /// <summary>The difficulty of the game.</summary>
        public string BoardDifficulty;
        /// <summary>The timer of the game.</summary>
        public long Timer;
        #endregion

        #region Constructors
        /// <summary>The default constructor of the Board class.</summary>
        public Board()
        {
            Matrix = new char[0,0];
            WordsToFind = new List<string>();
            BoardDifficulty = "";
            Timer = 0;
        }
        /// <summary>The constructor of the Board class from a difficulty.</summary>
        /// <param name="boardDifficulty">The difficulty of the board.</param>
        public Board(string boardDifficulty)
        {
            BoardDifficulty = boardDifficulty;
            Dictionary temporaryDictionary = new Dictionary();
            Dictionary<string, int> config = ConfigurationJson("Settings.json",BoardDifficulty);
            Timer = config["timer"];
            Matrix = new char[config["row"], config["column"]];
            FillMatrix();
            
            List<int> amountToPlace = new List<int>(){config["3-5"],config["6-8"],config["9-11"],config["12-15"]};
            WordsToFind = PlaceRandomWords(temporaryDictionary, config["placementsType"], amountToPlace);
            
            FillMatrix(true);
        }
        #endregion
        
        #region PlaceRandomWords
        /// <summary>This method is used to place a certain amount of word, form a certain difficulty from a dictionary into the matrix.</summary>
        /// <param name="dictionary">The dictionary to use.</param>
        /// <param name="difficulty">The difficulty of the words to place.</param>
        /// <param name="amountToPlace">The amount of words to place.</param>
        /// <param name="successfullyPlaced">The list of words that have been placed.</param>
        /// <returns>The list of words that have been placed.</returns>
        public List<string> PlaceRandomWords(Dictionary dictionary, int difficulty, List<int> amountToPlace, List<string> successfullyPlaced = null)
        {
            if(successfullyPlaced is null) successfullyPlaced = new List<string>();
            for(int i = amountToPlace.Count-1; i >= 0; i--)
            {
                for(int j = 0; j < amountToPlace[i]; j++)
                {
                    bool isPlaced = false;
                    while(!isPlaced)
                    {
                        Position position = RandomPosition();
                        int n = rnd.Next((i+1) * 3, (i+1) * 3 + 3);
                        string word = dictionary.ListDict[n][rnd.Next(0, dictionary.ListDict[n].Count)];
                        isPlaced = TryPlaceAWord(position, difficulty, word);
                        if(isPlaced)
                        {
                            dictionary.ListDict[n].Remove(word);
                            successfullyPlaced.Add(word);
                        }
                    } 
                }
            }
            return successfullyPlaced;
        }
        /// <summary>This method is used to try to place a word in a random direction from a position.</summary>
        /// <param name="position">The position to start the word from.</param>
        /// <param name="numberOfDirections">The number of directions to try.</param>
        /// <param name="word">The word to try to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceAWord(Position position, int numberOfDirections, string word)
        {
            int[] type = Enumerable.Range(1, numberOfDirections*2).OrderBy(x => rnd.Next()).Take(numberOfDirections*2).ToArray();
            foreach(int i in type)
            {
                switch(i)
                {
                    case 1 : return TryPlaceColumn(position, word);
                    case 2 : return TryPlaceRow(position, word);
                    case 3 : return TryPlaceDiagonal1(position, word);
                    case 4 : return TryPlaceDiagonal2(position, word);
                }
            }
            return false;
        }
        #endregion

        #region Get rows,column,diagonals to char arrays
        /// <summary>This method is used to get every char on a row.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the row.</returns>
        public char[] GetCharOnColumn(Position pos) => Enumerable.Range(0, Matrix.GetLength(1) - 1).Select(x => Matrix[x, pos.Y]).ToArray();
        /// <summary>This method is used to get every char on a column.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the column.</returns>
        public char[] GetCharOnRow(Position pos) => Enumerable.Range(0, Matrix.GetLength(0) - 1).Select(y => Matrix[pos.X, y]).ToArray();
        /// <summary>This method is used to get every char on a diagonal from pos to North-East.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <param name="diagNE">The list of char on the diagonal.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagNE(Position pos, List<char> diagNE = null)
        {
            if(diagNE is null) diagNE = new List<char>(){Matrix[pos.X, pos.Y]};
            if(pos.X != 0 && pos.Y != Matrix.GetLength(1) - 1)
            {
                diagNE.Add(Matrix[pos.X - 1, pos.Y + 1]);
                return GetCharDiagNE(new Position(pos.X - 1, pos.Y + 1), diagNE);
            }
            else return diagNE.ToArray();
        }
        /// <summary>This method is used to get every char on a diagonal from pos to South-West.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <param name="diagSW">The list of char on the diagonal.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagSW(Position pos, List<char> diagSW = null)
        {
            if(diagSW is null) diagSW = new List<char>(){Matrix[pos.X,pos.Y]};
            if(pos.X != Matrix.GetLength(0) - 1 && pos.Y != 0)
            {
                diagSW.Add(this.Matrix[pos.X + 1, pos.Y - 1]);
                return GetCharDiagSW(new Position(pos.X + 1, pos.Y - 1), diagSW);
            }
            else return diagSW.ToArray();
        }
        /// <summary>This method is used to get every char on a diagonal from pos to South-East.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <param name="diagSE">The list of char on the diagonal.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagSE(Position pos, List<char> diagSE = null) 
        {
            if(diagSE is null) diagSE = new List<char>(){Matrix[pos.X, pos.Y]};
            if(pos.X != Matrix.GetLength(0) - 1 && pos.Y != Matrix.GetLength(1) - 1)
            {
                diagSE.Add(this.Matrix[pos.X + 1, pos.Y + 1]);
                return GetCharDiagSE(new Position(pos.X + 1, pos.Y + 1), diagSE);
            }
            else return diagSE.ToArray();
        }
        /// <summary>This method is used to get every char on a diagonal from pos to North-West.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <param name="diagNW">The list of char on the diagonal.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagNW(Position pos, List<char> diagNW = null)
        {
            if(diagNW is null) diagNW = new List<char>(){Matrix[pos.X, pos.Y]};
            if(pos.X != 0 && pos.Y != 0)
            {
                diagNW.Add(this.Matrix[pos.X - 1, pos.Y - 1]);
                return GetCharDiagNW(new Position(pos.X - 1,pos.Y - 1), diagNW);
            }
            else return diagNW.ToArray();
        }
        #endregion

        #region Methods TryPlace
        /// <summary>This method is used to try to place a word in a row from a position.</summary>
        /// <param name="pos">The position to start the word from.</param>
        /// <param name="word">The word to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceColumn(Position pos, string word)
        {
            char[] completeColumn = GetCharOnColumn(pos);
            char[] downSide = completeColumn.Skip(pos.X).ToArray();
            char[] upSide = completeColumn.Where((p, i) => i <= pos.X).Reverse().ToArray();
            foreach(int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch(i)
                {
                    case 1 : 
                        if(IsFreeToFill(downSide, word)) return FillWord(word, pos, new Position(pos.X + word.Length, pos.Y));
                        break;
                    case 2 : 
                        if(IsFreeToFill(upSide, word)) return FillWord(word, pos, new Position(pos.X - word.Length, pos.Y));
                        break;
                }
            }
            return false;
        }
        /// <summary>This method is used to try to place a word in a column from a position.</summary>
        /// <param name="pos">The position to start the word from.</param>
        /// <param name="word">The word to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceRow(Position pos, string word)
        {
            char[] completeRow = GetCharOnRow(pos);
            char[] rightSide = completeRow.Skip(pos.Y).ToArray();
            char[] leftSide = completeRow.Where((p, i) => i <= pos.Y).Reverse().ToArray();
            foreach(int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch(i)
                {
                    case 1 : 
                        if(IsFreeToFill(rightSide, word)) return FillWord(word, pos, new Position(pos.X, pos.Y + word.Length));
                        break;
                    case 2 : 
                        if(IsFreeToFill(leftSide, word)) return FillWord(word, pos, new Position(pos.X, pos.Y - word.Length));
                        break;
                }
            }
            return false;
        }
        /// <summary>This method is used to try to place a word in a diagonal (SW to NE) from a position.</summary>
        /// <param name="pos">The position to start the word from.</param>
        /// <param name="word">The word to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceDiagonal1(Position pos, string word)
        {
            char[] diagonal1 = GetCharDiagNE(pos);
            char[] diagonal2 = GetCharDiagSW(pos);
            
            foreach(int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch(i)
                {
                    case 1:
                        if(IsFreeToFill(diagonal1, word)) return FillWord(word, pos, new Position(pos.X - word.Length, pos.Y + word.Length));
                        else if (IsFreeToFill(diagonal1, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X - word.Length, pos.Y + word.Length));
                        break;
                    case 2: 
                        if(IsFreeToFill(diagonal2, word)) return FillWord(word, pos, new Position(pos.X + word.Length, pos.Y - word.Length));
                        else if(IsFreeToFill(diagonal2, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X + word.Length, pos.Y - word.Length));
                        break;
                }
            }
            return false;
        }
        /// <summary>This method is used to try to place a word in a diagonal (NW to SE) from a position.</summary>
        /// <param name="pos">The position to start the word from.</param>
        /// <param name="word">The word to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceDiagonal2(Position pos, string word)
        {
            char[] diagonal1 = GetCharDiagSE(pos);
            char[] diagonal2 = GetCharDiagNW(pos);
            foreach(int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch(i)
                {
                    case 1:
                        if(IsFreeToFill(diagonal1, word)) return FillWord(word, pos, new Position(pos.X + word.Length, pos.Y + word.Length));
                        else if(IsFreeToFill(diagonal1, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X+word.Length, pos.Y+word.Length));
                        break;
                    case 2: 
                        if(IsFreeToFill(diagonal2, word)) return FillWord(word, pos, new Position(pos.X - word.Length, pos.Y - word.Length));
                        else if(IsFreeToFill(diagonal2, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X - word.Length, pos.Y - word.Length));
                        break;
                }
            }
            return false;
        }
        /// <summary>This method is used to check if a word can fit in a specified line.</summary>
        /// <param name="line">The line to check</param>
        /// <param name="word">The word to check</param>
        /// <returns>True if the word can fit in the line, false otherwise</returns>
        public bool IsFreeToFill(char[] line, string word)
        {
            if(word.Length > line.Length) return false;
            else for(int i = 0; i < word.Length; i++) if(line[i] != word[i] && line[i] != '_') return false;
            return true;
        }
        /// <summary>This method is used to fill a word between two positions.</summary>
        /// <param name="word">The word to fill. </param>
        /// <param name="pos1">The first position. </param>
        /// <param name="pos2">The second position. </param>
        /// <returns>True after the word has been filled. </returns>
        public bool FillWord(string word, Position pos1, Position pos2)
        {
            List<Position> positionsToFill = GetPositionsBetween(pos1, pos2);
            for (int i = 0; i < positionsToFill.Count-1; i++)
            {
                Matrix[positionsToFill[i].X, positionsToFill[i].Y] = word[i];
            }
            return true;
        }
        #endregion
        
        #region General Utiliy Methods
        /// <summary>This method is used to get a random position in the matrix.</summary>
        /// <returns>A random position in the matrix.</returns>
        public Position RandomPosition() => new Position(rnd.Next(0, Matrix.GetLength(0)), rnd.Next(0, Matrix.GetLength(1)));
        /// <summary>This method is used to create a list of positions between two positions.</summary>
        /// <param name="pos1">The first position. </param>
        /// <param name="pos2">The second position. </param>
        /// <returns>A list of positions between the two positions. </returns>
        public static List<Position> GetPositionsBetween(Position pos1, Position pos2)
        {
            List<Position> line = new List<Position>();
            if(pos1.X.Equals(pos2.X))
            {
                if(pos1.Y < pos2.Y) for(int i = pos1.Y; i <= pos2.Y; i++) line.Add(new Position(pos1.X, i));
                else for(int i = pos1.Y; i >= pos2.Y; i--) line.Add(new Position(pos1.X, i));
            }
            else if(pos1.Y.Equals(pos2.Y))
            {
                if(pos1.X < pos2.X) for(int i = pos1.X; i <= pos2.X; i++) line.Add(new Position(i, pos1.Y));
                else for(int i = pos1.X; i >= pos2.X; i--) line.Add(new Position(i, pos1.Y));
            }
            else
            {
                if(pos1.X < pos2.X && pos1.Y < pos2.Y) for(int i = 0; i <= pos2.X-pos1.X; i++) line.Add(new Position(pos1.X + i, pos1.Y + i));
                else if(pos1.X > pos2.X && pos1.Y < pos2.Y) for(int i = 0; i <= pos1.X-pos2.X; i++) line.Add(new Position(pos1.X - i, pos1.Y + i));
                else if(pos1.X < pos2.X && pos1.Y > pos2.Y) for(int i = 0; i <= pos2.X-pos1.X; i++) line.Add(new Position (pos1.X + i, pos1.Y - i));
                else if(pos1.X > pos2.X && pos1.Y > pos2.Y) for(int i = 0; i <= pos1.X-pos2.X; i++) line.Add(new Position(pos1.X - i, pos1.Y - i));
            }
            return line;
        }
        /// <summary>This method is used to fill the matrix with random letters or underscores.</summary>
        public void FillMatrix(bool fillWithLetters = false)
        {
            if(fillWithLetters)
            {
                for(int i = 0; i < Matrix.GetLength(0); i++)
                {
                    for(int j = 0; j < Matrix.GetLength(1); j++) 
                    {
                        if(Matrix[i, j] == '_') Matrix[i, j] = (char)rnd.Next(65, 91);
                    }
                }
            } 
            else for(int i = 0; i < Matrix.GetLength(0); i++) for(int j = 0; j < Matrix.GetLength(1); j++) Matrix[i, j] = '_';
        }
        /// <summary>This method is used to diplay the matrix into the console.</summary>
        public void DisplayMatrix()
        {
            for(int i = 0; i < Matrix.GetLength(0); i++)
            {
                for(int j = 0; j < Matrix.GetLength(1); j++)
                {
                    Write(Matrix[i, j]);
                }
                WriteLine($" {i} ");
            }
        }
        #endregion
    }
}