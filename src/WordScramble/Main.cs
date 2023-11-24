namespace WordScramble
{
    /// <summary>The main program class.</summary>
    class Program
    {
        public static Jump jump = Jump.Continue;
        /// <summary>The Main function.</summary>
        public static void Main()
        {
            
            #region Config
            Methods.ConsoleConfiguration();
            #endregion

            Main_Menu :

            #region Lobby
            Game.MainMenu();
            if(jump is not Jump.Continue) goto Select;
            CustomDictionary.CreateDictionary();
            #endregion

            Players_Setup : 

            #region Setting up the players
            Game session = Game.DefineSession() ?? throw new Exception("The session is null.");
            if(jump is not Jump.Continue) goto Select;
            #endregion

            #region Game loop
            Game.GameLoop(session, session.CurrentBoard.BoardDifficulty, new string[]{"first", "second", "third", "fourth", "fifth"});
            #endregion

            #region End of game
            if(session.IsGameFinished()) 
            {
                File.Move($"savedGames/{session.Name}.csv", $"archivedGames/{session.Name}.csv");
                session.DisplayWinner();
            }
            #endregion

            goto Main_Menu;

            Select :

            switch(jump)
            {
                case Jump.Continue: 
                    break;
                case Jump.Main_Menu: 
                    jump = Jump.Continue;
                    goto Main_Menu;
                    
                case Jump.Players_Setup: 
                    jump = Jump.Continue;
                    goto Players_Setup;
                case Jump.Exit: 
                    Methods.FinalExit(); 
                    break;
            }
        } 
        public enum Jump
        {
            Continue,
            Main_Menu,
            Players_Setup,
            Exit
        }
    }
}