using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.IO.File;

namespace Word_Scramble
{
    /// <summary>The dictionnary creation class.</summary>
    class Dictionary
    {
        #region Attributes
        /// <summary>The language of the dictionnary.</summary>
        public static string s_Language = "FR";
        /// <summary>The dictionnary itself, using string arrays.</summary>
        public static Dictionary<int, string[]> s_Dict = new Dictionary<int, string[]>();
        /// <summary>The dictionnary itself, using string Lists.</summary>
        public static Dictionary<int, List<string>> s_DictList = new Dictionary<int, List<string>>();
        #endregion
        
        #region Constructor
        /// <summary>The default constructor of the class.</summary>
        public Dictionary()
        {
            string[] lines = s_Language == "FR"? ReadAllLines("dataDictionary/MotsPossiblesFR.txt"): ReadAllLines("dataDictionary/MotsPossiblesEN.txt");
            int[] Key = lines.Where((x, i) => i % 2 == 0).Select(x => int.Parse(x)).ToArray();
            string[][] Value = lines.Where((x, i) => i % 2 != 0).Select(x => x.Split(' ')).ToArray();

            s_Dict = Key.Zip(Value, (s, i) => new { s, i }).ToDictionary(item => item.s, item => item.i);
        }
        /// <summary>The alternative constructor of the class.</summary>
        public Dictionary(Dictionary<int, string[]> dictionary)
        {
            s_DictList = dictionary.ToDictionary(x => x.Key, x => x.Value.ToList());
        }
        #endregion

        #region Methods
        /// <summary>This method is used to resize the dictionary to a smaller size. This improves performances.</summary>
        /// <param name="dict">The dictionary to resize.</param name>
        public static Dictionary<int, string[]> ResizeDict(Dictionary<int,string[]> dict,int size)
        {
            return dict.Where(x => (x.Key<= size)).ToDictionary(x => x.Key, x => x.Value);
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