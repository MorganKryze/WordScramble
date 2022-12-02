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

        //create a function that take a dictionnary and return a list of wordspublic static List<string> GetWords(Dictionary<int, string[]> data)
        public static List<string> GetWords(Dictionary<int, string[]> data)
        {
            List<string[]> words = data.Values.ToList();
            //transform words into a strig list
            List<string> wordsList = new List<string>();
            foreach (string[] word in words)
            {
                //add to wordsList word.toList()
                wordsList.AddRange(word.ToList());
            }
            
            return wordsList;
        }

        #endregion

        #region Dictionnary methods
        /// <summary> Redefine the dictionnary from the tab size </summary>
        /// <param name="data"> Dictionnary of words </param>
        /// <param name="tab"> Actual board </param>
        /// <param name="size"> Size of the new dictionary </param>
        public static Dictionary<int, string[]> redefineDictionary(Dictionary<int,string[]> data,char[,] tab,int size)
        {
            return data.Where(x => (x.Key<= size)).ToDictionary(x => x.Key, x => x.Value);
        }
        #endregion

    }
}