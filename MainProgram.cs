using System.Diagnostics;
using Newtonsoft.Json;
using static System.Console;
using static Word_Scramble.Methods;
using static Word_Scramble.Game;

namespace Word_Scramble
{
    class MainProgram
    {

        public static void Main(string[] args)
        {
            /*
            #region Config
            ConsoleConfig();
            #endregion

            Main_Menu :

            #region Lobby
            MainMenu();
            #endregion

            #region Setting up the players
            Ranking ranking = new Ranking();
            Player player1 = new Player();
            Player player2 = new Player();
            if(!DefinePlayers(player1, player2, ranking))goto Main_Menu;
            #endregion

            #region Game loop
            LoadingScreen("[  Loading the game ...  ]");
            if (!SelectWords(CsvToMatrix("dataGrills/ComplexGrill.csv"), new List<string>{"TERRE","FERME","ROCHE","ECHEC"}, player1)) goto Main_Menu;
            if (!SelectWords(CsvToMatrix("dataGrills/ComplexGrill.csv"), new List<string>{"TERRE","ROUE","ROCHE","ECHEC"}, player2)) goto Main_Menu;
            Pause();
            #endregion

            goto Main_Menu;*/

            Dictionary.CreateDictionary();
            Dictionary dictionaryList = new Dictionary(Dictionary.s_Dict);
            Dictionary<string, int> settings =Settings.Load("Settings.json","easy");
            Random rdm=new Random();
            //Writeline all element from dict
            foreach (KeyValuePair<string, int> kvp in settings)
            {
                WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            Board board=new Board(settings["row"],settings["column"]);
            List<int> toPlace=new List<int>();
            toPlace.Add(settings["3-5"]);
            toPlace.Add(settings["6-8"]);
            toPlace.Add(settings["9-11"]);
            toPlace.Add(settings["12-15"]);
            //PlaceWord(board,dictionaryList, settings["placementsType"], rdm, toPlace);
            board.draw();
            
            List<string> placed =PlaceWord(board,dictionaryList, settings["placementsType"], rdm, toPlace);
            //draw list
            foreach (string word in placed)
            {
                WriteLine(word);
            }

            board.draw();
        }
    }
}