using System.Diagnostics;
using static System.Console;
using Newtonsoft.Json;
namespace Word_Scramble
{
    class MainProgram
    {

        public static void Main(string[] args)
        {
            #region Config
            Methods.ConsoleConfig();
            #endregion

            //Main_Menu :

            #region Lobby
            Methods.MainMenu();
            #endregion

            //lignes de test
            Methods.SelectWords(Methods.CsvToMatrix("dataGrills/ComplexGrill.csv"), new List<string>{"TERRE","FERME","RN","RAI"});
            //Methods.SelectWords(Methods.CsvToMatrix("dataGrills/ComplexGrill.csv"), new List<string>{"AGRICULTURE","BREBIS","CANARD","CHEVRES","CIDRE","CONFITURE","ELEVEUR","ESCARGOT","ESTIVE","FERME","FROMAGE"});

            //Player_Creation_1 :

            #region Setting up the player 1
            #endregion

            //Player_Creation_2 :

            #region Setting up the player 2
            #endregion
            
            Methods.MainMenu();
        }
    }
}