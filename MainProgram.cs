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

            Main_Menu :

            #region Lobby
            Game.MainMenu();
            #endregion
            
            //Players_Creation :

            #region Setting up the players
            Player player1 = new Player();
            Player player2 = new Player();
            if(!Game.DefinePlayers(player1, player2))goto Main_Menu;
            #endregion

            #region Game loop
            Game.SelectWords(Methods.CsvToMatrix("dataGrills/ComplexGrill.csv"), new List<string>{"TERRE","FERME","ROCHE","ECHEC"});
            #endregion

            goto Main_Menu;
        }
    }
}