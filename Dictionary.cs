using Newtonsoft.Json;

namespace AUTRE
{
    /// <summary>The dictionnary creation class.</summary>
    class Dictionary
    {
        #region Attributes
        public static string s_Language = "FR";
        /// <summary>The dictionnary itself.</summary>
        public static Dictionary<int, string[]> s_Dict = new Dictionary<int, string[]>();
        public static Dictionary<int, List<string>> s_DictList = new Dictionary<int, List<string>>();
        #endregion
        
        #region Constructor
        /// <summary>The constructor of the class.</summary>
        public Dictionary()
        {
            string[] lines = File.ReadAllLines("dataDictionary/MotsPossiblesFR.txt");
            int[] Key = lines.Where((x, i) => i % 2 == 0).Select(x => int.Parse(x)).ToArray();
            string[][] Value = lines.Where((x, i) => i % 2 != 0).Select(x => x.Split(' ')).ToArray();

            s_Dict = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
        }
        public Dictionary(Dictionary<int, string[]> dictionary)
        {
            
            s_DictList = dictionary.ToDictionary(x => x.Key, x => x.Value.ToList());
        }
        #endregion

        #region Methods
        /// <summary> Function to redefine the dictionary to the size of the Dictionnary</summary>
        /// <param name="data">Dictionary to redefine</param name>
        public static Dictionary<int, string[]> redefineDictionary(Dictionary<int,string[]> data,int size)
        {
            return data.Where(x => (x.Key<= size)).ToDictionary(x => x.Key, x => x.Value);
        }
        /*
        public static bool Recherche(string mot)
        {
            return RechercheDicho(mot,s_Dictionary[mot.Length],0,s_Dictionary[mot.Length].Length-1);
        }
        public static bool RechercheDicho(string mot, string[]liste, int départ, int arrivée)
        {
            int milieu = (int)arrivée/2;
            if (mot.Equals(liste[milieu]))return true;
            else if(mot.CompareTo(liste[milieu])>0)return RechercheDicho(mot,s_Dictionary[mot.Length],départ,milieu);
            else return RechercheDicho(mot,s_Dictionary[mot.Length],milieu,arrivée);
        }
        public static bool RechercheDicho(string mot, string[]liste, int départ, int arrivée)
        {
            if(départ>arrivée)return false;
            int milieu = (int)arrivée/2;
            if (mot.Equals(liste[milieu]))return true;
            else if(mot.CompareTo(liste[milieu])>0)return RechercheDicho(mot,s_Dictionary[mot.Length],départ,milieu);
            else return RechercheDicho(mot,s_Dictionary[mot.Length],milieu,arrivée);
        }*/
        #endregion

    }
}