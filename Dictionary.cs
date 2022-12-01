using Newtonsoft.Json;

namespace Word_Scramble
{
    /// <summary>The dictionnary creation class.</summary>
    class Dictionary
    {
        #region Attributes
        /// <summary>The dictionnary itself.</summary>
        public static Dictionary<int, string[]> s_Dictionary = new Dictionary<int, string[]>();
        #endregion
        
        #region Constructor
        /// <summary>The constructor of the class.</summary>
        public Dictionary()
        {
            s_Dictionary = ReadTxtToJSON("dataDictionary/MotsPossiblesFR.txt", "data.json");
        }
        #endregion

        #region Methods
        /// <summary> This method is used to read a TXT and transform it into dictionnary. </summary>
        /// <param name="PathTXT"> Path to TXT dictionnary file </param>
        /// <param name="PathJSON"> Path to JSON dictionnary file </param>
        /// <returns> A dictionnary containing every word indexed upon its length.</returns>
        public static Dictionary<int, string[]> ReadTxtToJSON(string PathTXT, string PathJSON)
        {
            //reads the txt file and store it in a string table
            string[] lines = File.ReadAllLines(PathTXT);
            //creates arrays of odd elements in lines (keys)
            int[] Key = lines.Where((x, i) => i % 2 == 0).ToArray().Select(x => int.Parse(x)).ToArray();
            //create arrays of even elements in lines (values)
            string[][] Value = lines.Where((x, i) => i % 2 != 0).ToArray().Select(x => x.Split(' ')).ToArray();
            //dictionnary creation
            Dictionary<int, string[]> data = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
            string json = JsonConvert.SerializeObject(data);
            File.WriteAllText(PathJSON, json);
            return data;
        }
        #endregion

    }
}