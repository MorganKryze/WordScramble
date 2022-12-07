using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Word_Scramble
{
    public class Settings
    {
        //create a function that take path in arg and retun a dictionnary of string that give a dictionnary of string to int
        public static Dictionary<string, int> Load(string path,string difficulty)
        {
            //read settings.json with Newtonsoft.Json
            string json = System.IO.File.ReadAllText("settings.json");
            //if json is empty return empty dictionnary
            if (json == "")
            {
                return new Dictionary<string, int>();
            }

            //deserialize into disctionary of strings to dictionnary of strings to int
            Dictionary<string, Dictionary<string, int>> settings = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(json);
            return (settings[difficulty]);
            
        }
    }
}