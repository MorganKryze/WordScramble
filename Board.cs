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
        public char[] getDiagonal11(int x1,int y1, List<char> dia1=null) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (dia1==null)
            {    dia1=new List<char>();
                dia1.Add(this.board[x1,y1]);
            }
            
            if (x1!=0 && y1!=board.GetLength(1)-1)
            {
                x1--;
                y1++;
                dia1.Add(this.board[x1,y1]);
                return(getDiagonal11(x1,y1,dia1));
            }
            else
            {
                return(dia1.ToArray());
            }
        }

        public char[] getDiagonal12(int x1,int y1, List<char> dia1=null) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            if (dia1==null)
            {   
                dia1=new List<char>();
                dia1.Add(this.board[x1,y1]);
            }
            
            if (x1!=board.GetLength(0)-1 && y1!=0)
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
                dia2.Add(this.board[x1,y1]);
            }
            
            if (x1!=board.GetLength(0)-1 && y1!=board.GetLength(1)-1)
            {
                x1++;
                y1++;
                dia2.Add(this.board[x1,y1]);
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
                dia2.Add(this.board[x1,y1]);
            }
            
            if (x1!=0 && y1!=0)
            {
                x1--;
                y1--;
                dia2.Add(this.board[x1,y1]);
                return(getDiagonal22(x1,y1,dia2));
            }
            else
            {
                return(dia2.ToArray());
            }
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
            for (int i=0; i<this.board.GetLength(0);i++){
                for (int j=0; j<this.board.GetLength(1);j++){
                    Write(this.board[i,j]== '_' ?"_ ":$"{this.board[i,j]} ");
                }
                WriteLine($" {i} ");
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


        public bool NewWord(Position position, int difficulty, Random rnd, string word){

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
                        WriteLine("1Diagonal1");
                        if (CheckDiagonal1(position.X, position.Y, word, rnd)) {
                            return(true);
                        }
                        break;
                    case 4: 
                        Write("2Diagonal2");
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
                        if (CompareArray(row2,word2)) {
                            PasteWord(word2,x,y,x+word.Length,y);
                            return(true);
                        }
                        break;
                    case 2: 
                    if (CompareArray(row3,word2)) {
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
                        if (CompareArray(column2,word2)) {
                            PasteWord(word2,x,y,x,y+word.Length);
                            return(true);
                        };
                        break;
                    case 2: 
                        if (CompareArray(column3,word2)) {
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
                        if (CompareArray(diagonal2,word2)) {
                            PasteWord(word2,x,y,x-word.Length,y+word.Length);
                            return(true);
                        }
                        else if (CompareArray(diagonal2,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x-word.Length,y+word.Length);
                            return(true);
                        }
                        break;
                    case 2: 
                        if (CompareArray(diagonal3,word2)) {
                            PasteWord(word2,x,y,x+word.Length,y-word.Length);
                            return(true);
                        }
                        else if(CompareArray(diagonal3,word2.Reverse().ToArray())){
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
                        if (CompareArray(diagonal2,word2)){ 
                            PasteWord(word2,x,y,x+word.Length,y+word.Length);
                            return(true);}
                        else if(CompareArray(diagonal2,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x+word.Length,y+word.Length);
                            return(true);
                        }
                        break;
                    case 2: 
                        if (CompareArray(diagonal3,word2)) {
                            PasteWord(word2,x,y,x-word.Length,y-word.Length);
                            return(true);
                        }
                        else if(CompareArray(diagonal3,word2.Reverse().ToArray())){
                            PasteWord(word2.Reverse().ToArray(),x,y,x-word.Length,y-word.Length);
                            return(true);
                        }
                        break;
                }
            }
            return(false);
        }
        public Position RandomPosition(Random rnd)
        {
            int x = rnd.Next(0, this.board.GetLength(0));
            int y = rnd.Next(0, this.board.GetLength(1));
            return(new Position(x,y));
        }
        

        #endregion

        public void PasteWord(char[] word, int x, int y, int x2, int y2)
        {
            
            //foreach (int a, int b) in FillPosition (x,y,x2,y2))
            List<Position> positon = GetPositionsBetween (new Position(x,y),new Position(x2,y2));
            for (int i = 0; i < positon.Count-1; i++)
            {
                this.board[positon[i].X,positon[i].Y]=word[i];
            }
        }
        /// <summary>This method is used to create a list of positions between two positions.</summary>
        /// <param name="obj1"> The first position. </param>
        /// <param name="obj2"> The second position. </param>
        /// <returns> A list of positions between the two positions. </returns>
        public static List<Position> GetPositionsBetween (Position obj1, Position obj2)
        {
            List<Position> line = new List<Position>();
            if (obj1.X == obj2.X)
            {
                if (obj1.Y < obj2.Y)
                {
                    for (int i = obj1.Y; i <= obj2.Y; i++)
                    {
                        line.Add(new Position (obj1.X,i));
                    }
                }
                else
                {
                    for (int i = obj1.Y; i >= obj2.Y; i--)
                    {
                        line.Add(new Position (obj1.X,i));
                    }
                }
            }
            else if (obj1.Y == obj2.Y)
            {
                if (obj1.X < obj2.X)
                {
                    for (int i = obj1.X; i <= obj2.X; i++)
                    {
                        line.Add(new Position (i,obj1.Y));
                    }
                }
                else
                {
                    for (int i = obj1.X; i >= obj2.X; i--)
                    {
                        line.Add(new Position (i,obj1.Y));
                    }
                }
            }
            else
            {
                if (obj1.X < obj2.X && obj1.Y < obj2.Y)
                {
                    for (int i = 0; i <= obj2.X-obj1.X; i++)
                    {
                        line.Add(new Position (obj1.X+i,obj1.Y+i));
                    }
                }
                else if (obj1.X > obj2.X && obj1.Y < obj2.Y)
                {
                    for (int i = 0; i <= obj1.X-obj2.X; i++)
                    {
                        line.Add(new Position (obj1.X-i,obj1.Y+i));
                    }
                }
                else if (obj1.X < obj2.X && obj1.Y > obj2.Y)
                {
                    for (int i = 0; i <= obj2.X-obj1.X; i++)
                    {
                        line.Add(new Position (obj1.X+i,obj1.Y-i));
                    }
                }
                else if (obj1.X > obj2.X && obj1.Y > obj2.Y)
                {
                    for (int i = 0; i <= obj1.X-obj2.X; i++)
                    {
                        line.Add(new Position(obj1.X-i,obj1.Y-i));
                    }
                }
            }
            return line;
        }

    }
    
}