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

            //ligne de test
            Write(Methods.SelectWord(Methods.CsvToMatrix("dataGrills/CasComplexe.csv")));
            
            //Player_Creation_1 :

            #region Setting up the player 1
            #endregion

            //Player_Creation_2 :

            #region Setting up the player 2
            #endregion
            
            Methods.Pause();
            Methods.FinalExit();
        }
    }
}