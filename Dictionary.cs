using Newtonsoft.Json;

namespace Word_Scramble
{
    abstract class Dictionary
    {
        #region Json and dictionnary methods
        /// <summary> Read TXT and transform it into dictionnary </summary>
        /// <param name="PathTXT"> Path to TXT dictionnary file </param>
        /// <param name="PathJSON"> Path to JSON dictionnary file </param>
        public static Dictionary<int, string[]> ReadTxtToJSON(string PathTXT, string PathJSON)
        {
            //read the json file and store it in a string
            string[] lines = File.ReadAllLines(PathTXT);
            //create arrays of odd elements in lines (keys)
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