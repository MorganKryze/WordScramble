using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static System.Console;

namespace Word_Scramble
{
    public class Board
    {
        /// <summary> Board of the class Board</summary>
        public char[,] board { get; set; }

        /// <summary> Constructor of the class Board</summary>
        public Board(int sizeX, int sizeY)
        {
            board = new char[sizeX, sizeY];
            fillBoard();
        }

        #region get rows,column,diagonals of the board
        /// <summary> Function that return a Row</summary>
        public char[] getRow(int row)
        {
            return( Enumerable.Range(0, this.board.GetLength(1)-1).Select(x => this.board[x, row]).ToArray());
        }

        /// <summary> Function that return a Column</summary>
        public char[] getColumn(int column)
        {
            return( Enumerable.Range(0, this.board.GetLength(0)-1).Select(y => this.board[column, y]).ToArray());
        }

        /// <summary> Function that return a Diagonal</summary>
        public char[] getDiagonal1(int x,int y) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            return(Enumerable.Range(0, x+y+1).Select(i => this.board[i,x+y-i]).ToArray());
        }

        /// <summary> Function that return a Diagonal</summary>
        public char[] getDiagonal2(int x,int y) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            int abs=Math.Abs(x-y);
            return(x>y?Enumerable.Range(0, Math.Min(this.board.GetLength(0) - Math.Abs(x-y),this.board.GetLength(1))).Select(i => this.board[Math.Abs(x-y)+i,+i]).ToArray():Enumerable.Range(0, Math.Min(this.board.GetLength(0),this.board.GetLength(1)- Math.Abs(x-y))).Select(i => this.board[i,Math.Abs(x-y)+i]).ToArray());
        }
        #endregion
    
        //fill board with '_' character
        /// <summary> Function that fill the board with '_' character</summary>
        public void fillBoard()
        {
            for (int i=0; i<this.board.GetLength(0);i++){
                for (int j=0; j<this.board.GetLength(1);j++){
                    this.board[i,j]='_';
                }
            }
        }


        /// <summary> Function that draw the board</summary>
        public void draw()
        {
            for (int j=0; j<this.board.GetLength(1);j++){
                for (int i=0; i<this.board.GetLength(0);i++){
                    Write(this.board[i,j]== '_' ?"_ ":this.board[i,j]);
                }
                WriteLine($" {j} ");
            }
        }

        public static bool CompareArray(char[] line, char[] word)
        {
            //if wod is longer than line return false
            if (word.Length > line.Length)
            {
                return(false);
            }
            //compare line and wod
            for (int i = 0; i < word.Length; i++)
            {
                if (line[i] != word[i] && line[i] != '_')
                {
                    return(false);
                }
            }
            return(true);
        }

        public (int,int) CheckRow(int x, int y, string word, Random rnd)
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
                        if (CompareArray(row2,word2)) {
                            PasteWord(word2,x,y,x+word.Length,y);
                            return(x+word.Length,y);
                        }
                        break;
                    case 2: 
                    if (CompareArray(row3,word2)) {
                        PasteWord(word2,x,y,x-word.Length,y);
                        return(x-word.Length,y);
                    }
                    break;
                }
            }
            return((-1,-1));
        }

        public (int,int) CheckColumn(int x, int y, string word, Random rnd)
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
                        if (CompareArray(column2,word2)) {
                            PasteWord(word2,x,y,x,y+word.Length);
                            return(x,y+word.Length);
                        };
                        break;
                    case 2: 
                        if (CompareArray(column3,word2)) {
                            PasteWord(word2,x,y,x,y-word.Length);
                            return(x,y-word.Length);
                        }
                        break;
                }
            }
            return(-1,-1);
        }

        public (int,int) CheckDiagonal1(int x, int y, string word, Random rnd)
        {
            //get column at position y
            char[] diagonal = getDiagonal1(x,y);
            //get char column that start at position x
            char[] diagonal2 = diagonal.Skip(x).ToArray();
            //get char from positio x to start
            char[] diagonal3 = diagonal.Where((p, i) => i <= x).Reverse().ToArray();

            //foreach element between 1 and 2 randomly choose number
            int[] choose = Enumerable.Range(1,2).OrderBy(x => rnd.Next()).Take(2).ToArray();
            char[] word2 = word.Cast<char>().ToArray();

            //create switch from 1 to 4
            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        WriteLine("diagonal1");
                        if (CompareArray(diagonal2,word2)) {
                            PasteWord(word2,x,y,x+word.Length,y-word.Length);
                            return(1,1);
                        }
                        break;
                    case 2: 
                        WriteLine("diagonal2");
                        if (CompareArray(diagonal3,word2)) {
                            PasteWord(word2,x,y,x-word.Length,y+word.Length);
                            return(2,2);
                        }
                        break;
                }
            }
            return(0,0);
        }

        public (int,int) CheckDiagonal2(int x,int y, string word, Random rnd)
        {
            //get column at position y
            char[] diagonal = getDiagonal2(x,y);
            //get char column that start at position x
            char[] diagonal2 = diagonal.Skip(x).ToArray();
            //get char from positio x to start
            char[] diagonal3 = diagonal.Where((p, i) => i <= x).Reverse().ToArray();

            //foreach element between 1 and 2 randomly choose number
            int[] choose = Enumerable.Range(1,2).OrderBy(x => rnd.Next()).Take(2).ToArray();
            char[] word2 = word.Cast<char>().ToArray();

            //create switch from 1 to 4
            foreach (int i in choose)
            {
                switch (i)
                {
                    case 1:
                        if (CompareArray(diagonal2,word2)){ 
                            PasteWord(word2,x,y,x+word.Length,y+word.Length);
                            return(1,1);}
                        break;
                    case 2: 
                    if (CompareArray(diagonal3,word2)) {
                        PasteWord(word2,x,y,x-word.Length,y-word.Length);
                        return(2,2);
                        }
                        break;
                }
            }
            return(0,0);
        }

        public void PasteWord(char[] word, int x, int y, int x2, int y2)
        {
            
            //foreach (int a, int b) in FillPosition (x,y,x2,y2))
            List<(int,int)> positon=Methods.FillPosition (x,y,x2,y2);
            for (int i = 0; i < positon.Count-1; i++)
            {
                this.board[positon[i].Item1,positon[i].Item2]=word[i];
            }
        }

    }
    
}