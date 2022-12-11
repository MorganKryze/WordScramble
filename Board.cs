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


        #region get rows,column,diagonals of the board
        public char[] getRow(int row)
        {
            return Enumerable.Range(0, Matrix.GetLength(1)-1).Select(x => Matrix[x, row]).ToArray();
        }
        public char[] getColumn(int column)
        {
            return( Enumerable.Range(0, this.Matrix.GetLength(0)-1).Select(y => this.Matrix[column, y]).ToArray());
        }
        public char[] getDiagonal11(int x1,int y1, List<char> dia1=null) //diagonal beetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (dia1==null)
            {    dia1=new List<char>();
                dia1.Add(this.Matrix[x1,y1]);
            }
            
            if (x1!=0 && y1!=Matrix.GetLength(1)-1)
            {
                x1--;
                y1++;
                dia1.Add(this.Matrix[x1,y1]);
                return(getDiagonal11(x1,y1,dia1));
            }
            else return(dia1.ToArray());
        }

        public char[] getDiagonal12(int x1,int y1, List<char> dia1=null) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (dia1==null)
            {   
                dia1=new List<char>();
                dia1.Add(this.Matrix[x1,y1]);
            }
            
            if (x1!=Matrix.GetLength(0)-1 && y1!=0)
            {
                x1++;
                y1--;

                return(getDiagonal12(x1,y1,dia1));
            }
            else
            {
                return(dia1.ToArray());
            }
        }
        public char[] getDiagonal21(int x1,int y1, List<char> dia2=null) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (dia2==null)
            {
                dia2=new List<char>();
                dia2.Add(this.Matrix[x1,y1]);
            }
            
            if (x1!=Matrix.GetLength(0)-1 && y1!=Matrix.GetLength(1)-1)
            {
                x1++;
                y1++;
                dia2.Add(this.Matrix[x1,y1]);
                return(getDiagonal21(x1,y1,dia2));
            }
            else
            {
                return(dia2.ToArray());
            }
        }

        public char[] getDiagonal22(int x1,int y1, List<char> dia2=null) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (dia2==null)
            {
                dia2=new List<char>();
                dia2.Add(this.Matrix[x1,y1]);
            }
            
            if (x1!=0 && y1!=0)
            {
                x1--;
                y1--;
                dia2.Add(this.Matrix[x1,y1]);
                return(getDiagonal22(x1,y1,dia2));
            }
            else
            {
                return(dia2.ToArray());
            }
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


        public bool NewWord(Position position, int difficulty, string word)
        {

            int[] choose = Enumerable.Range(1,difficulty *2).OrderBy(x => rnd.Next()).Take(difficulty*2).ToArray();

            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        if (CheckRow(position.X, position.Y, word, rnd)) {
                            return(true);
                        }
                        break;
                    case 2: 
                        if (CheckColumn(position.X, position.Y, word, rnd)) {
                            return(true);
                        }                        
                        break;
                    case 3: 
                        if (CheckDiagonal1(position.X, position.Y, word, rnd)) {
                            return(true);
                        }
                        break;
                    case 4: 
                        if (CheckDiagonal2(position.X, position.Y, word, rnd)) {
                            return(true);
                        }                        
                        break;
                }
            }

            return(false);
        }


        #region checkDirection
        public bool CheckRow(int x, int y, string word, Random rnd)
        {
            //get row at position y
            char[] row = getRow(y);
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
            char[] column = getColumn(x);
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
            char[] diagonal2 = getDiagonal11(x,y);
            //get char from positio x to start
            char[] diagonal3 = getDiagonal12(x,y);

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
            char[] diagonal2 = getDiagonal21(x,y);
            //get char from positio x to start
            char[] diagonal3 = getDiagonal22(x,y);

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