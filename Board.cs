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
        /// <remarks>Not working</remarks>
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
        public static bool IsFreeToFill(char[] line, char[] word)
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
                    case 1 : return CheckRow(position.X, position.Y, word, rnd);
                    case 2 : return CheckColumn(position.X, position.Y, word, rnd);
                    case 3 : return CheckDiagonal1(position.X, position.Y, word, rnd);
                    case 4: return CheckDiagonal2(position.X, position.Y, word, rnd);
                }
            }
            return false;
        }


        #region checkDirection
        public bool CheckRow(int x, int y, string word, Random rnd)
        {
            //get row at position y
            char[] row = GetCharOnRow(new Position(x,y));
            //get char row that start at position x
            char[] row2 = row.Skip(x).ToArray();
            //get char from positio x to start
            char[] row3 = row.Where((p, i) => i <= x).Reverse().ToArray();

            //foreach element between 1 and 2 randomly choose number
            int[] choose = Enumerable.Range(1,2).OrderBy(x => rnd.Next()).Take(2).ToArray();
            char[] word2 = word.Cast<char>().ToArray();

            //create switch from 1 to 4
            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        if (IsFreeToFill(row2,word2)) {
                            PasteWord(word2,x,y,x+word.Length,y);
                            return(true);
                        }
                        break;
                    case 2: 
                    if (IsFreeToFill(row3,word2)) {
                        PasteWord(word2,x,y,x-word.Length,y);
                        return(true);
                    }
                    break;
                }
            }
            return(false);
        }

        public bool CheckColumn(int x, int y, string word, Random rnd)
        {
            //get column at position y
            char[] column = GetCharOnColumn(new Position(x,y));
            //get char column that start at position x
            char[] column2 = column.Skip(y).ToArray();
            //get char from positio x to start
            char[] column3 = column.Where((p, i) => i <= y).Reverse().ToArray();

            //foreach element between 1 and 2 randomly choose number
            int[] choose = Enumerable.Range(1,2).OrderBy(x => rnd.Next()).Take(2).ToArray();
            char[] word2 = word.Cast<char>().ToArray();

            //create switch from 1 to 4
            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        if (IsFreeToFill(column2,word2)) {
                            PasteWord(word2,x,y,x,y+word.Length);
                            return(true);
                        };
                        break;
                    case 2: 
                        if (IsFreeToFill(column3,word2)) {
                            PasteWord(word2,x,y,x,y-word.Length);
                            return(false);
                        }
                        break;
                }
            }
            return(false);
        }

        public bool CheckDiagonal1(int x, int y, string word, Random rnd)
        {
            //get column at position y
            char[] diagonal2 = GetCharDiagNE(new Position(x,y));
            //get char from positio x to start
            char[] diagonal3 = GetCharDiagSW(new Position(x,y));

            //foreach element between 1 and 2 randomly choose number
            int[] choose = Enumerable.Range(1,2).OrderBy(x => rnd.Next()).Take(2).ToArray();
            char[] word2 = word.Cast<char>().ToArray();

            //create switch from 1 to 4
            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        if (IsFreeToFill(diagonal2,word2)) {
                            PasteWord(word2,x,y,x-word.Length,y+word.Length);
                            return(true);
                        }
                        else if (IsFreeToFill(diagonal2,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x-word.Length,y+word.Length);
                            return(true);
                        }
                        break;
                    case 2: 
                        if (IsFreeToFill(diagonal3,word2)) {
                            PasteWord(word2,x,y,x+word.Length,y-word.Length);
                            return(true);
                        }
                        else if(IsFreeToFill(diagonal3,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x+word.Length,y-word.Length);
                            return(true);
                        }
                        break;
                }
            }
            return(false);
        }

        public bool CheckDiagonal2(int x,int y, string word, Random rnd)
        {
            //get column at position y
            char[] diagonal2 = GetCharDiagSE(new Position(x,y));
            //get char from positio x to start
            char[] diagonal3 = GetCharDiagNW(new Position(x,y));

            //foreach element between 1 and 2 randomly choose number
            int[] choose = Enumerable.Range(1,2).OrderBy(x => rnd.Next()).Take(2).ToArray();
            char[] word2 = word.Cast<char>().ToArray();

            //create switch from 1 to 4
            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        if (IsFreeToFill(diagonal2,word2)){ 
                            PasteWord(word2,x,y,x+word.Length,y+word.Length);
                            return(true);}
                        else if(IsFreeToFill(diagonal2,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x+word.Length,y+word.Length);
                            return(true);
                        }
                        break;
                    case 2: 
                        if (IsFreeToFill(diagonal3,word2)) {
                            PasteWord(word2,x,y,x-word.Length,y-word.Length);
                            return(true);
                        }
                        else if(IsFreeToFill(diagonal3,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x-word.Length,y-word.Length);
                            return(true);
                        }
                        break;
                }
            }
            return(false);
        }
        public Position RandomPosition() => new Position(rnd.Next(0, Matrix.GetLength(0)), rnd.Next(0, Matrix.GetLength(1)));
        #endregion

        public void PasteWord(char[] word, int x, int y, int x2, int y2)
        {
            List<Position> positionsToFill = GetPositionsBetween(new Position(x,y), new Position(x2,y2));
            for (int i = 0; i < positionsToFill.Count-1; i++)
            {
                Matrix[positionsToFill[i].X, positionsToFill[i].Y] = word[i];
            }
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

        public List<string> PlaceWords(Dictionary dictionaryList, int difficulty, List<int> toPlace, List<string> placed = null)
        {
            if (placed == null)
            {
                placed = new List<string>();
            }
            for (int i=toPlace.Count-1;i>=0;i--)
            {
                //for j in range of i
                for (int j=0;j<toPlace[i];j++)
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
                            placed.Add(word);
                        }
                    } 
                }
            }
            return placed;
        }
    }
}