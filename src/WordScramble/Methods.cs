namespace WordScramble
{
    /// <summary>The vocation of the Methods class is to be accessible from anywhere.It contains a random variable, utility and core methods.</summary>
    public static class Methods
    {
        #region Field
        /// <summary>The random variable, usable everywhere.</summary>
        public static Random rnd = new Random();
        #endregion

        #region Core Methods
        /// <summary>This method is used to define the name of a player.</summary>
        /// <param name="message">The message to display.</param>
        /// <returns>The name of the player.</returns>
        public static string TypePlayerName(string message)
        {
            string playerName = "";
            do 
            {
                Title(message, "data/Title.txt");
                Console.Write("{0,"+((Console.WindowWidth / 2) - (message.Length / 2)) + "}","");
                Console.Write("> ");
                ConsoleConfiguration(false);
                playerName = Console.ReadLine() ?? throw new NullReferenceException();
                ConsoleConfiguration();
            }while(playerName == "");
            return playerName;
        }
        /// <summary>This method is used to define the name of a game.</summary>
        /// <param name="message">The message to display.</param>
        /// <returns>The name of the game.</returns>
        public static string TypeGameName(string message)
        {
            string gameName = "";
            do 
            {
                Title(message, "data/Title.txt");
                Console.Write("{0,"+((Console.WindowWidth / 2) - (message.Length / 2)) + "}","");
                Console.Write("> ");
                ConsoleConfiguration(false);
                gameName = Console.ReadLine() ?? throw new NullReferenceException();
                ConsoleConfiguration();
            }while(gameName == "");
            return gameName;
        }
        /// <summary>This method is used to find the nearest position in a direction.</summary>
        /// <param name="matrix">The grill from which the user chooses the characters.</param>
        /// <param name="currentPosition">The current position of the cursor.</param>
        /// <param name="possiblePositions">The list of possible positions.</param>
        /// <param name="direction">The direction in which the user wants to go.</param>
        /// <returns>The nearest position in the direction.</returns>
        public static Position NearestPosition(char[,] matrix, Position currentPosition, List<Position> possiblePositions, string direction)
        {
            switch (direction)
            {
                case "up":
                    for(int i = currentPosition.X-1; i >= 0; i--)
                    {
                        if(possiblePositions.Contains(new Position(i, currentPosition.Y)))return new Position(i, currentPosition.Y);
                    }
                    break;
                case "down":
                    for(int i = currentPosition.X+1; i < matrix.GetLength(0); i++)
                    {
                        if(possiblePositions.Contains(new Position(i, currentPosition.Y)))return new Position(i, currentPosition.Y);
                    }
                    break;
                case "left":
                    for(int i = currentPosition.Y-1; i >= 0; i--)
                    {
                        if(possiblePositions.Contains(new Position(currentPosition.X, i)))return new Position(currentPosition.X, i);
                    }
                    break;
                case "right":
                    for(int i = currentPosition.Y+1; i < matrix.GetLength(1); i++)
                    {
                        if(possiblePositions.Contains(new Position(currentPosition.X, i)))return new Position(currentPosition.X, i);
                    }
                    break;
            }
            return currentPosition;
        }
        /// <summary>This method is used to find the valid positions.</summary>
        /// <param name="matrix">The grill from which the user chooses the characters.</param>
        /// <param name="anchor1">The first anchor.</param>
        /// <param name="possiblePositions">The list of possible positions.</param>
        /// <returns>The list of valid positions.</returns>
        public static List<Position> ValidPositions(char[,] matrix, Position anchor1, List<Position> possiblePositions)
        {
            List<Position> validPositions = new List<Position>();
            for(int i = 0; i < possiblePositions.Count; i++)
            {
                if(possiblePositions[i].X == anchor1.X || possiblePositions[i].Y == anchor1.Y)
                {
                    validPositions.Add(new Position(possiblePositions[i].X, possiblePositions[i].Y));
                }else
                {
                    if(possiblePositions[i].X - anchor1.X == possiblePositions[i].Y - anchor1.Y)
                    {
                        validPositions.Add(new Position(possiblePositions[i].X, possiblePositions[i].Y));
                    }
                    else if(possiblePositions[i].X - anchor1.X == anchor1.Y - possiblePositions[i].Y)
                    {
                        validPositions.Add(new Position(possiblePositions[i].X, possiblePositions[i].Y));
                    }
                }
            }
            return validPositions;
        }
        /// <summary>This method is used to print the matrix.</summary>
        /// <param name="matrix">The grill from which the user chooses the characters.</param>
        /// <param name="selectedPositions">The list of selected positions.</param>
        /// <param name="correctPositions">The list of right positions.</param>
        /// <param name="currentPosition">The current position of the cursor.</param>
        /// <param name="displayCursor">A boolean that indicates if the cursor should be displayed.</param>
        public static void PrintMatrix(char[,] matrix, List<Position> selectedPositions, List<Position> correctPositions, Position currentPosition, bool displayCursor = true)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write("{0,"+((Console.WindowWidth / 2) - (matrix.GetLength(1))) + "}","");
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (currentPosition.Equals(new Position(i, j)) && displayCursor)
                    {
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(matrix[i, j]);
                    }
                    else if (selectedPositions.Contains(new Position(i, j)))
                    {

                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(matrix[i, j]);
                    }
                    else if (correctPositions.Contains(new Position(i, j)))
                    {

                        Console.BackgroundColor = ConsoleColor.Blue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(matrix[i, j]);
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(matrix[i, j]);
                    }
                    ConsoleConfiguration();
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        /// <summary>This method is used to print a message telling whether the player has won or not.</summary>
        /// <param name="message">The message to be displayed.</param>
        /// <param name="backColor">The background color of the message.</param>
        public static void BoardMessage(string[] message, ConsoleColor backColor = ConsoleColor.Green)
        {
            Console.Clear();
            Console.WriteLine();
            string max = message[0];
            for (int i = 0; i < message.Length; i++)
            {
                if (max.Length < message[i].Length) max = message[i];
            }
            int index = Array.IndexOf(message, max);
            CenteredWL(String.Format("{0,"+message[index].Length+"}", ""), ConsoleColor.Black, backColor);
            for (int i = 0; i < message.Length; i++)
            {
                if (message[index].Length > message[i].Length)
                {
                    for (int j = 0; message[index].Length != message[i].Length; j++ )
                    {
                        if(j % 2 == 0)message[i] +=" ";
                        else message[i] = " " + message[i];
                    }
                }
                CenteredWL(String.Format("{0,"+message[index].Length+"}", message[i]), ConsoleColor.Black, backColor);
            }
            CenteredWL(String.Format("{0,"+message[index].Length+"}", ""), ConsoleColor.Black, backColor);
            Console.WriteLine();
        }
        /// <summary>This method is used to read a json configuration file to get the configurations attributes.</summary>
        /// <param name="path">The path of the json file.</param>
        /// <param name="difficulty">The difficulty of the game.</param>
        /// <returns>The dictionary containing the configurations for the defined difficulty.</returns>
        public static Dictionary<string, int> ConfigurationJson(string path,string difficulty)
        {
            string json = File.ReadAllText("settings.json");
            if (json == "" || json == null) return new Dictionary<string, int>();
            else 
            {
                Dictionary<string, Dictionary<string, int>> configuration = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, int>>>(json) ?? throw new NullReferenceException();
                return configuration[difficulty];
            }
        }
        #endregion

        #region Utility Methods
        /// <summary> This method is used to set the console configuration. </summary>
        /// <param name="start"> Wether the config is used as the default config (true) or for the end of the program (false). </param>
        public static void ConsoleConfiguration(bool start = true)
        {
            if (start)
            {
                Console.CursorVisible = false;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else Console.CursorVisible = true;
        }
        /// <summary>This method is used to display a title.</summary>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "pathSpecialText"> A special text stored in a file.</param>
        /// <param name= "recurrence"> Whether the title has been displayed yet or not.</param>
        public static void Title (string text = "",string pathSpecialText = "", int recurrence = 0)
        {
            Console.Clear();
            if (pathSpecialText != "")PrintSpecialText(pathSpecialText);
            if(text != "")
            {
                if (recurrence != 0)CenteredWL(text);
                else
                {
                    Console.Write("{0,"+((Console.WindowWidth / 2) - (text.Length / 2)) + "}","");
                    for(int i = 0; i < text.Length; i++)
                    {
                        Console.Write(text[i]);
                        Thread.Sleep(50);
                        if(Console.KeyAvailable)
                        {
                            ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                            if(keyPressed.Key == ConsoleKey.Enter||keyPressed.Key == ConsoleKey.Escape)
                            {
                                Console.Write(text.Substring(i+1));
                                break;
                            }
                        }
                    }
                    Console.Write("\n");
                }
                Console.Write("\n");
                
            }
            
        }
        /// <summary>This method is used to display a scrolling menu.</summary>
        /// <param name="question"> The question to be displayed.</param>
        /// <param name="choices"> The choices to be displayed.</param>
        /// <param name="specialText"> A special text stored in a file.</param>
        /// <returns> The position of the choice selected.</returns>
        public static int ScrollingMenu(string question, string[] choices, string specialText = "Title.txt")
        {
            int position = 0;
            int recurrence = 0;
            while(true)
            {
                Console.Clear();
                Title(question,specialText,recurrence);
                string[]currentChoice = new string[choices.Length];
                for (int i = 0; i < choices.Length; i++)
                {
                    if (i == position)
                    {
                        currentChoice[i] = $" > {choices[i]}";
                        CenteredWL(currentChoice[i], ConsoleColor.Black, ConsoleColor.Green);
                        ConsoleConfiguration();
                    }
                    else 
                    {
                        currentChoice [i]= $"   {choices[i]}";
                        CenteredWL(currentChoice[i]);
                    }
                }
                switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow : case ConsoleKey.Z : if(position == 0)position = choices.Length-1; else if(position > 0) position--;break;
                    case ConsoleKey.DownArrow : case ConsoleKey.S : if(position == choices.Length-1)position = 0; else if(position < choices.Length-1)position++;break;
                    case ConsoleKey.Enter : return position;
                    case ConsoleKey.Escape : return -1;
                }
                recurrence++;
            }
        }
        /// <summary>This method is used to display a special text, stored in a txt file. </summary>
        /// <param name="path">the relative path of the file.</param>
        public static void PrintSpecialText(string path)
        {
            string[] specialText = File.ReadAllLines(path);
            foreach(string line in specialText)Console.WriteLine("{0," + ((Console.WindowWidth / 2) + (line.Length / 2)) + "}", line);
            Console.WriteLine("\n");
        }
        /// <summary>This method is used to display a centered text and come back to line.</summary>
        /// <param name="text"> The text to display. </param>
        /// <param name="fore"> The foreground color of the text. </param>
        /// <param name="back"> The background color of the text. </param>
        public static void CenteredWL(string text, ConsoleColor fore = ConsoleColor.White, ConsoleColor back = ConsoleColor.Black)
        {
            ConsoleConfiguration();
            Console.Write("{0,"+((Console.WindowWidth / 2) - (text.Length / 2)) + "}","");
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
            Console.WriteLine(text);
            ConsoleConfiguration();
        }
        /// <summary>This method is used to display a centered text.</summary>
        /// <param name="text"> The text to display. </param>
        /// <param name="fore"> The foreground color of the text. </param>
        /// <param name="back"> The background color of the text. </param>
        public static void CenteredW(string text, ConsoleColor fore = ConsoleColor.White, ConsoleColor back = ConsoleColor.Black)
        {
            ConsoleConfiguration();
            Console.Write("{0,"+((Console.WindowWidth / 2) - (text.Length / 2)) + "}","");
            Console.ForegroundColor = fore;
            Console.BackgroundColor = back;
            Console.Write(text);
            ConsoleConfiguration();
        }   
        /// <summary>This method is used to display a loading screen.</summary>
        /// <param name="text"> The text to display. </param>
        public static void LoadingScreen(string text)
        {
            Console.Clear();
            int t_interval = (int) 2000/text.Length;
            char[] loadingBar = new char[text.Length];
            for (int j = 0; j < loadingBar.Length; j++) loadingBar[j] = '█';
            for (int i = 0; i <= text.Length; i++)
            {
                Title(text, "data/Title.txt", 1);
                if(Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    if(keyPressed.Key == ConsoleKey.Enter||keyPressed.Key == ConsoleKey.Escape)
                    {
                        i = text.Length;
                        break;
                    }
                }
                Console.WriteLine("\n");
                string bar = "";
                for (int l = 0; l < i; l++) bar += loadingBar[l];
                Console.Write("{0,"+((Console.WindowWidth / 2) - (text.Length / 2)) + "}","");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(bar);
                bar = ""; 
                if (i != text.Length)
                {
                    for (int l = i; l < text.Length; l++)bar += loadingBar[l];
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(bar);
                    Thread.Sleep(t_interval);
                }
                else Thread.Sleep(1000);
                ConsoleConfiguration();
                Console.Clear();
            }
        }
        /// <summary>This method is used to pause the program.</summary>
        public static void Pause()
        {
            CenteredWL("Press [ENTER] to continue...");
            while(Console.ReadKey(true).Key != ConsoleKey.Enter) Thread.Sleep(5);
        }
        /// <summary>This method is used to exit the game.</summary>
        public static void FinalExit()
        {
            LoadingScreen("[  Shutting down ...  ]");
            ConsoleConfiguration(false);
            Console.Clear();
            Environment.Exit(0);
        } 
        #endregion
    }
}