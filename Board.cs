using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using static System.Console;
using static Word_Scramble.Methods;

namespace Word_Scramble
{
    public class Board
    {
        #region Attributes
        /// <summary> The matrix of the board.</summary>
        public char[,] Matrix { get; set; }
        /// <summary> The list of the words to find.</summary>
        public List<string> WordsToFind { get; set; }
        /// <summary> The difficulty of the game.</summary>
        public string BoardDifficulty { get; set; }

        #endregion

        // TODO : Adapter le constructeur
        #region Constructor
        /// <summary> The constructor of the Board class.</summary>
        public Board(string boardDifficulty)
        {
            BoardDifficulty = boardDifficulty;
            Dictionary temporaryDictionary = new Dictionary();
            Dictionary<string, int> config = ConfigurationJson("Settings.json",BoardDifficulty);

            Matrix = new char[config["row"], config["column"]];
            for (int i = 0; i< config["row"]; i++) for (int j = 0; j< config["column"]; j++) Matrix[i,j] = '_';

            List<int> amountToPlace = new List<int>(){config["3-5"],config["6-8"],config["9-11"],config["12-15"]};
            WordsToFind = PlaceWords(temporaryDictionary, config["placementsType"], amountToPlace);
            // ! DEBUG
            WriteLine("Words to find : ");
            foreach (string word in WordsToFind)Write(word+" ");
            WriteLine();
            AfficherTEMP();
            // ! DEBUG
        }
        #endregion

        // ! Temporaire
        /// <summary> Function that draw the board</summary>
        public void AfficherTEMP()
        {
            for (int i=0; i<this.Matrix.GetLength(0);i++)
            {
                for (int j=0; j<this.Matrix.GetLength(1);j++)
                {
                    Write(this.Matrix[i,j]== '_' ?"_ ":$"{this.Matrix[i,j]} ");
                }
                WriteLine($" {i} ");
            }
        }
        //! Temporaire


        #region get rows,column,diagonals char arrays
        /// <summary>This method is used to get every char on a row.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the row.</returns>
        public char[] GetCharOnRow(Position pos) => Enumerable.Range(0, Matrix.GetLength(1)-1).Select(x => Matrix[x, pos.Y]).ToArray();
        /// <summary>This method is used to get every char on a column.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the column.</returns>
        public char[] GetCharOnColumn(Position pos) => Enumerable.Range(0, Matrix.GetLength(0)-1).Select(y => Matrix[pos.X, y]).ToArray();
        /// <summary>This method is used to get every char on a diagonal from pos to North-East.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagNE(Position pos, List<char> diagNE = null)
        {
            if (diagNE == null) diagNE = new List<char>(){Matrix[pos.X, pos.Y]};
            if (pos.X != 0 && pos.Y != Matrix.GetLength(1)-1)
            {
                diagNE.Add(Matrix[pos.X-1, pos.Y+1]);
                return GetCharDiagNE(new Position(pos.X-1, pos.Y+1),diagNE);
            }
            else return diagNE.ToArray();
        }
        /// <summary>This method is used to get every char on a diagonal from pos to South-West.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagSW(Position pos, List<char> diagSW=null)
        {
            if (diagSW == null) diagSW = new List<char>(){Matrix[pos.X,pos.Y]};
            if (pos.X != Matrix.GetLength(0)-1 && pos.Y != 0)
            {
                diagSW.Add(this.Matrix[pos.X+1, pos.Y-1]);
                return GetCharDiagSW(new Position(pos.X+1, pos.Y-1),diagSW);
            }
            else return diagSW.ToArray();
        }
        /// <summary>This method is used to get every char on a diagonal from pos to South-East.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagSE(Position pos, List<char> diagSE = null) 
        {
            if (diagSE == null) diagSE = new List<char>(){Matrix[pos.X,pos.Y]};
            if (pos.X != Matrix.GetLength(0)-1 && pos.Y != Matrix.GetLength(1)-1)
            {
                diagSE.Add(this.Matrix[pos.X+1, pos.Y+1]);
                return(GetCharDiagSE(new Position(pos.X+1, pos.Y+1), diagSE));
            }
            else return diagSE.ToArray();
        }
        /// <summary>This method is used to get every char on a diagonal from pos to North-West.</summary>
        /// <param name="pos">The position to get the char array from.</param>
        /// <returns>The array of char on the diagonal.</returns>
        public char[] GetCharDiagNW(Position pos, List<char> diagNW=null) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (diagNW == null) diagNW = new List<char>(){Matrix[pos.X,pos.Y]};
            if (pos.X != 0 && pos.Y != 0)
            {
                diagNW.Add(this.Matrix[pos.X-1,pos.Y-1]);
                return(GetCharDiagNW(new Position(pos.X-1,pos.Y-1),diagNW));
            }
            else return diagNW.ToArray();
        }
        #endregion

        /// <summary> This method is used to check if a word can fit in a specified line.</summary>
        /// <param name="line">The line to check</param>
        /// <param name="word">The word to check</param>
        /// <returns>True if the word can fit in the line, false otherwise</returns>
        public static bool IsFreeToFill(char[] line, string word)
        {
            if (word.Length > line.Length) return false;
            else for (int i = 0; i < word.Length; i++) if (line[i] != word[i] && line[i] != '_') return false;
            return true;
        }

        
        public bool NewWord(Position position, int numberOfDirections, string word)
        {
            int[] type = Enumerable.Range(1,numberOfDirections*2).OrderBy(x => rnd.Next()).Take(numberOfDirections*2).ToArray();
            foreach (int i in type)
            {
                switch (i)
                {
                    case 1 : return TryPlaceRow(position, word);
                    case 2 : return TryPlaceColumn(position, word);
                    case 3 : return TryPlaceDiagonal1(position, word);
                    case 4: return TryPlaceDiagonal2(position, word);
                }
            }
            return false;
        }


        #region checkDirection
        /// <summary>This method is used to try to place a word in a row from a position.</summary>
        /// <param name="pos">The position to start the word from.</param>
        /// <param name="word">The word to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceRow(Position pos, string word)
        {
            char[] row = GetCharOnRow(pos);
            //get char row that start at position x
            char[] row2 = row.Skip(pos.X).ToArray();
            //get char from positio x to start
            char[] row3 = row.Where((p, i) => i <= pos.X).Reverse().ToArray();

            foreach (int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch (i)
                {
                    case 1 : if (IsFreeToFill(row2, word)) return FillWord(word, pos, new Position(pos.X+word.Length, pos.Y));
                        break;
                    case 2 : if (IsFreeToFill(row3, word)) return FillWord(word, pos, new Position(pos.X-word.Length, pos.Y));
                        break;
                }
            }
            return false;
        }
        /// <summary>This method is used to try to place a word in a column from a position.</summary>
        /// <param name="pos">The position to start the word from.</param>
        /// <param name="word">The word to place.</param>
        /// <returns>True if the word has been placed, false otherwise.</returns>
        public bool TryPlaceColumn(Position pos, string word)
        {
            char[] column = GetCharOnColumn(pos);
            //get char column that start at position x
            char[] column2 = column.Skip(pos.Y).ToArray();
            //get char from positio x to start
            char[] column3 = column.Where((p, i) => i <= pos.Y).Reverse().ToArray();

            foreach (int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch (i)
                {
                    case 1 : if (IsFreeToFill(column2, word)) return FillWord(word, pos, new Position(pos.X, pos.Y+word.Length));
                        break;
                    case 2 : if (IsFreeToFill(column3, word)) return FillWord(word, pos, new Position(pos.X, pos.Y-word.Length));
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
            //get column at position y
            char[] diagonal1 = GetCharDiagNE(pos);
            //get char from positio x to start
            char[] diagonal2 = GetCharDiagSW(pos);
            
            foreach (int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch (i)
                {
                    case 1:
                        if (IsFreeToFill(diagonal1, word)) return FillWord(word, pos, new Position(pos.X-word.Length, pos.Y+word.Length));
                        else if (IsFreeToFill(diagonal1, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X-word.Length, pos.Y+word.Length));
                        break;
                    case 2: 
                        if (IsFreeToFill(diagonal2, word)) return FillWord(word, pos, new Position(pos.X+word.Length, pos.Y-word.Length));
                        else if(IsFreeToFill(diagonal2, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X+word.Length, pos.Y-word.Length));
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
            //get column at position y
            char[] diagonal1 = GetCharDiagSE(pos);
            //get char from positio x to start
            char[] diagonal2 = GetCharDiagNW(pos);

            foreach (int i in Enumerable.Range(1, 2).OrderBy(x => rnd.Next()))
            {
                switch (i)
                {
                    case 1:
                        if (IsFreeToFill(diagonal1, word)) return FillWord(word, pos, new Position(pos.X+word.Length, pos.Y+word.Length));
                        else if(IsFreeToFill(diagonal1, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X+word.Length, pos.Y+word.Length));
                        break;
                    case 2: 
                        if (IsFreeToFill(diagonal2, word)) return FillWord(word, pos, new Position(pos.X-word.Length, pos.Y-word.Length));
                        else if(IsFreeToFill(diagonal2, word.Reverse().ToString())) return FillWord(word.Reverse().ToString(), pos, new Position(pos.X-word.Length, pos.Y-word.Length));
                        break;
                }
            }
            return false;
        }
        public Position RandomPosition() => new Position(rnd.Next(0, Matrix.GetLength(0)), rnd.Next(0, Matrix.GetLength(1)));
        #endregion
        /// <summary>This method is used to fill a word between two positions.</summary>
        /// <param name="word"> The word to fill. </param>
        /// <param name="pos1"> The first position. </param>
        /// <param name="pos2"> The second position. </param>
        /// <returns> True after the word has been filled. </returns>
        public bool FillWord(string word, Position pos1, Position pos2)
        {
            List<Position> positionsToFill = GetPositionsBetween(pos1, pos2);
            for (int i = 0; i < positionsToFill.Count-1; i++)
            {
                Matrix[positionsToFill[i].X, positionsToFill[i].Y] = word[i];
            }
            return true;
        }
        /// <summary>This method is used to create a list of positions between two positions.</summary>
        /// <param name="pos1"> The first position. </param>
        /// <param name="pos2"> The second position. </param>
        /// <returns> A list of positions between the two positions. </returns>
        public static List<Position> GetPositionsBetween (Position pos1, Position pos2)
        {
            List<Position> line = new List<Position>();
            if (pos1.X == pos2.X)
            {
                if (pos1.Y < pos2.Y) for (int i = pos1.Y; i <= pos2.Y; i++) line.Add(new Position(pos1.X,i));
                else for (int i = pos1.Y; i >= pos2.Y; i--) line.Add(new Position(pos1.X,i));
            }
            else if (pos1.Y == pos2.Y)
            {
                if (pos1.X < pos2.X) for (int i = pos1.X; i <= pos2.X; i++) line.Add(new Position(i,pos1.Y));
                else for (int i = pos1.X; i >= pos2.X; i--) line.Add(new Position(i,pos1.Y));
            }
            else
            {
                if (pos1.X < pos2.X && pos1.Y < pos2.Y) for (int i = 0; i <= pos2.X-pos1.X; i++) line.Add(new Position(pos1.X+i,pos1.Y+i));
                else if (pos1.X > pos2.X && pos1.Y < pos2.Y) for (int i = 0; i <= pos1.X-pos2.X; i++) line.Add(new Position(pos1.X-i,pos1.Y+i));
                else if (pos1.X < pos2.X && pos1.Y > pos2.Y) for (int i = 0; i <= pos2.X-pos1.X; i++) line.Add(new Position (pos1.X+i,pos1.Y-i));
                else if (pos1.X > pos2.X && pos1.Y > pos2.Y) for (int i = 0; i <= pos1.X-pos2.X; i++) line.Add(new Position(pos1.X-i,pos1.Y-i));
            }
            return line;
        }

        public List<string> PlaceWords(Dictionary dictionaryList, int difficulty, List<int> amountToPlace, List<string> successfullyPlaced = null)
        {
            if (successfullyPlaced == null) successfullyPlaced = new List<string>();
            for (int i = amountToPlace.Count-1; i >= 0; i--)
            {
                for (int j = 0; j < amountToPlace[i]; j++)
                {
                    bool value =false;
                    while(!value)
                    {
                        Position position = RandomPosition();
                        int n = rnd.Next((i+1) * 3, (i+1) * 3 + 3);
                        string word = dictionaryList.DictList[n][rnd.Next(0,dictionaryList.DictList[n].Count)];
                        value = NewWord(position,difficulty,word);
                        if (value)
                        {
                            dictionaryList.DictList[n].Remove(word);
                            successfullyPlaced.Add(word);
                        }
                    } 
                }
            }
            return successfullyPlaced;
        }
    }
}