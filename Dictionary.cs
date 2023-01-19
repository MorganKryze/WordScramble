using System;

using static System.Console;
using static System.IO.File;

using static Word_Scramble.Methods;

namespace Word_Scramble
{
    /// <summary>The dictionnary creation class.</summary>
    public class Dictionary
    {
        #region Fields
        /// <summary>The language of the dictionnary.</summary>
        public static string s_Language;
        /// <summary>The static dictionnary.</summary>
        public static Dictionary<int, string[]> s_Dict = new Dictionary<int, string[]>();
        /// <summary>The dynamic dictionary.</summary>
        public Dictionary<int, List<string>> ListDict = new Dictionary<int, List<string>>();
        #endregion
        
        #region Constructor
        /// <summary>The constructor of the class.</summary>
        public Dictionary()
        {
            ListDict = s_Dict.ToDictionary(x => x.Key, x => x.Value.ToList());
        }
        #endregion

        #region Methods
        /// <summary>This method is used to create a static dictionary.</summary>
        public static void CreateDictionary()
        {
            if (s_Language == null) s_Language = "FR";
            string[] lines = s_Language == "FR" ? ReadAllLines("dataDictionary/MotsPossiblesFR.txt") : ReadAllLines("dataDictionary/MotsPossiblesEN.txt");
            int[] Key = lines.Where((x, i) => i % 2 == 0).Select(x => int.Parse(x)).ToArray();
            string[][] Value = lines.Where((x, i) => i % 2 != 0).Select(x => x.Split(' ')).ToArray();
            s_Dict = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
        }
        #endregion

    }
}