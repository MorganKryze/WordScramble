using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Word_Scramble
{
    public class Board
    {
        public static char[] getRow(int row, char[,] board)
        {
            return( Enumerable.Range(0, board.GetLength(1)).Select(y => board[row, y]).ToArray());
        }
        public static char[] getColumn(int column, char[,] board)
        {
            return( Enumerable.Range(0, board.GetLength(1)).Select(x => board[x, column]).ToArray());
        }
        public static char[] getDiagonal1(int diagonal, char[,] board) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            return(Enumerable.Range(0, diagonal+2).Select(x => board[x,diagonal+1-x]).ToArray());
        }
        public static char[] getDiagonal2(int diagonal, char[,] board) //diagonal ceetween 0 and Max(GetLength(0),GetLength(1))
        {
            return(diagonal<0?Enumerable.Range(0, Math.Min(tab.GetLength(0)-Math.Abs(diagonal),tab.GetLength(1))).Select(x => tab[Math.Abs(diagonal)+x,x]).ToArray():Enumerable.Range(0, Math.Min(board.GetLength(0),board.GetLength(1)-diagonal)).Select(x => board[x,diagonal+x]).ToArray());
        }
    }
}