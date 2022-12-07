using System.Diagnostics;
using static System.Console;
using Newtonsoft.Json;
namespace Word_Scramble
{
    class MainProgram
    {

        public struct Coords
        {
            public int X { get; set;}
            public int Y { get; set;}
        
                    
            public Coords(Random rnd, char[,]board)
            {
                X = rnd.Next(0, board.GetLength(0));
                Y = rnd.Next(0, board.GetLength(1));
            }
        }
        public static void Main(string[] args)
        {

            string path = @"./dataDictionary/MotsPossiblesFR.txt";
            Dictionary<int, string[]> dictionary = Dictionary.ReadTxt(path);
            Dictionary dict = new Dictionary(dictionary);


            Board board = new Board(10, 10);
            board.draw();
            Random rnd = new Random();
            

        }
    }
}