using System.Diagnostics;
using static System.Console;
using Newtonsoft.Json;
namespace Word_Scramble
{
    class MainProgram
    {

        public static void Main(string[] args)
        {
            Dictionary<string, string[]> dict = DictionaryMethod.ReadTxtToJSON("dataDictionary/MotsPossiblesFR.txt", "data.json");
        }
    }
}