using Newtonsoft.Json;

namespace Word_Scramble
{
    public class Dictionary
    { 
        /// <summary> Dictionnary of the class Dictionary</summary>
        //need a set in order to delete a value once placed in the grid
        public Dictionary<int, List<string>> dictlist{ get; set; }

        /// <summary> Constructor of the class Dictionary</summary>
        public Dictionary(Dictionary<int, string[]> dictionary)
        {
            
            dictlist = dictionary.ToDictionary(x => x.Key, x => x.Value.ToList());
        }


        /// <summary> Function to read a txt file and store it in a dictionnary</summary>
        /// <param name="PathTXT">Path of the txt file</param>
        /// <returns>Dictionary</returns>
        public static Dictionary<int, string[]> ReadTxt(string PathTXT)
        {
            //read the json file and store it in a string
            string[] lines = File.ReadAllLines(PathTXT);
            //create arrays of odd elements in lines (keys) ==> convert string[] into int[]
            int[] Key = lines.Where((x, i) => i % 2 == 0).Select(x => int.Parse(x)).ToArray();
            //create arrays of even elements in lines (values) ==> convert string[] into string[][]
            string[][] Value = lines.Where((x, i) => i % 2 != 0).Select(x => x.Split(' ')).ToArray();
            //dictionnary creation by Zip method
            Dictionary<int, string[]> data = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
            return data;
        }


        /// <summary> Function to redefine the dictionary to the size of the Dictionnary</summary>
        /// <param name="data">Dictionary to redefine</param>
        public static Dictionary<int, string[]> redefineDictionary(Dictionary<int,string[]> data,char[,] tab,int size)
        {
            return data.Where(x => (x.Key<= size)).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}