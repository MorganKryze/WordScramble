using System;
using System.IO;
using static System.Console;
using Newtonsoft.Json;
namespace World_Scramble
{
    class MainProgram
    {
        //create a function that return  Dictionary<string,string[]> and take path string in argument
        public static Dictionary<string, string[]> ReadTxtToJSON(string path)
        {
            //read the json file and store it in a string
            string[] lines = File.ReadAllLines(path);
            //create arrays of odd elements in lines (keys)
            string[] Key = lines.Where((x, i) => i % 2 == 0).ToArray();
            //create arrays of even elements in lines (values)
            string[][] Value = lines.Where((x, i) => i % 2 != 0).ToArray().Select(x => x.Split(' ')).ToArray();
            //dictionnary creation
            Dictionary<string,string[]> data = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);

            string json=JsonConvert.SerializeObject( data );
            return data;
        }

        static void Main(string[] args)
        {
            Dictionary<string, string[]>dict=ReadTxtToJSON("MotsPossiblesFR.txt");
        }
    }
}