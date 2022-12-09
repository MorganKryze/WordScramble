using System;
using System.Diagnostics;
using static System.Console;
using static System.Environment;
using static System.Threading.Thread;
using static System.Convert;
using static System.IO.File;
using static System.ConsoleColor;

namespace Word_Scramble
{
    /// <summary>The methods class where the core methods of the program are.</summary>
    abstract class Methods
    {

        #region Core Methods
        /// <summary>This method is used to display the main menu.</summary>
        public static void MainMenu()
        {
            switch(ScrollingMenu("", new string[]{"Play    ","Options ","Exit    "}, "Welcome Adventurer! Use the arrow keys to move and press [ENTER] to confirm.","Title.txt"))
            {
                case 0 : LoadingScreen("[ Launching the game ]\n");break;  
                case 1 : MainMenu(); break;
                case 2 : case -1: FinalExit();break;
            }
        }
        
         /*
        /// <summary>This method is used to define the current session.</summary>
        /// <param name= "ranking"> Display the choices among all of the sessions.</param>
        /// <param name= "player"> The player who is playing the game.</param>
        /// <returns> Wether or not the client chose to step back.</returns>
        public static bool DefineSession(Ranking ranking,Player player)
        {
            string[]sessionChoices = new string[ranking.PlayersList.Count+1];
            for(int i = 0; i < sessionChoices.Length; i++)
            {
                if(i == 0)sessionChoices[0]= "New player...       ";
                else sessionChoices[i]= ranking.PlayersList[i-1].Name;
            }
            int position = ScrollingMenu(sessionChoices, "-- Game session selection --","Please choose your game session wisely, or create one if yours is missing from the list:");
            if (position == -1)return true;
            else if (position == 0)
            {
                do
                {
                    Clear();
                    Title("-- Game session selection --","Please write a new session name: ");
                    player.Name = ReadLine()+String.Empty;
                    if (player.Name.Length<21&&player.Name!=String.Empty)
                    {
                        while(player.Name.Length<20)
                        {
                            player.Name+=" ";
                        }
                        if(!player.IsNewPlayer())
                        {
                            WriteLine("\nThis session already exists, please choose another name.");
                            player.Name = String.Empty;
                            Pause();
                        }
                    }
                }while (player.Name == "");
                Player.CutName = player.Name;
            }else
            {
                player.Name = ranking.PlayersList[position-1].Name;
                player.Scores = ranking.PlayersList[position-1].Scores;
                Player.CutName = player.Name;
            }
            ConsoleConfig();
            Clear();
            return false;
        }
        */

        /// <summary>This method is used to define the current session.</summary>
        public static bool DefinePlayers(Player player)
        {
           //switch(ScrollingMenu())
           // TODO : Add the option to create a new player.
           return true;
        }
        #endregion
        #region Utility Methods
        
        /// <summary>This method is used to select multiple characters.</summary>
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
        /// <returns>A string as the sum of the characters.</returns>
        public static void SelectWords(char[,] matrix, List<string> wordsToFind)
        {
            
            List<string> wordsLeft = wordsToFind;
            List<Position> correctPositions = new List<Position>();
            while(wordsLeft.Count != 0)
            {
                Position currentPosition = new Position(matrix.GetLength(0)/2,matrix.GetLength(1)/2);
                Position anchor1 = new Position(-1, -1);
                Position anchor2 = new Position(-1, -1);
                List<Position> selectedPositions = new List<Position>();

                List<Position> possiblePositions = new List<Position>();
                for(int i = 0; i< matrix.GetLength(0); i++)for(int j = 0; j < matrix.GetLength(1); j++)possiblePositions.Add(new Position(i, j));

                while(anchor2.X == -1&&wordsLeft.Count != 0)
                {
                    Clear();
                    PrintMatrix(matrix, selectedPositions,correctPositions, currentPosition);
                    switch(ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow : case ConsoleKey.Z :
                            if(possiblePositions.Contains(new Position(currentPosition.X - 1, currentPosition.Y)))currentPosition.X--;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"up"));
                            break;
                        case ConsoleKey.DownArrow : case ConsoleKey.S :
                            if(possiblePositions.Contains(new Position(currentPosition.X + 1, currentPosition.Y)))currentPosition.X++;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"down"));
                            break;
                        case ConsoleKey.LeftArrow :case ConsoleKey.Q :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y - 1)))currentPosition.Y--;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"left"));
                            break;
                        case ConsoleKey.RightArrow : case ConsoleKey.D :
                            if(possiblePositions.Contains(new Position(currentPosition.X, currentPosition.Y + 1)))currentPosition.Y++;
                            else currentPosition = new Position(NearestPosition(matrix, currentPosition, possiblePositions,"right"));
                            break;
                        case ConsoleKey.Enter : 
                            if(anchor1.X == -1)
                            {
                                anchor1 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Add(new Position(currentPosition.X, currentPosition.Y));
                                possiblePositions = ValidPositions(matrix, anchor1, possiblePositions);
                            }
                            else if (!anchor1.Equals(currentPosition))
                            {
                                anchor2 = new Position(currentPosition.X, currentPosition.Y);
                                selectedPositions.Clear();
                                selectedPositions = Board.GetPositionsBetween(anchor1, anchor2);
                                string word = "";
                                for(int i = 0; i < selectedPositions.Count; i++)
                                {
                                    word += matrix[selectedPositions[i].X, selectedPositions[i].Y];
                                }
                                if(wordsLeft.Contains(word))
                                {
                                    correctPositions.AddRange(selectedPositions);
                                    wordsLeft.Remove(word);
                                }
                                Clear();
                                PrintMatrix(matrix, selectedPositions, correctPositions, currentPosition, false);
                                Pause();
                            }
                            break;
                        case ConsoleKey.Escape :
                            wordsLeft.Clear();
                            break;
                    }
                }
            }
        }

        /// <summary>This method is used to find the nearest position in a direction.</summary>
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
        /// <param name="currentPosition">The current position of the cursor.</param>
        /// <param name="possiblePositions">The list of possible positions.</param>
        /// <param name="direction">The direction in wich the user wants to go.</param>
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
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
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
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
        /// <param name="selectedPositions">The list of selected positions.</param>
        /// <param name="correctPositions">The list of right positions.</param>
        /// <param name="currentPosition">The current position of the cursor.</param>
        /// <param name="displayCursor">A boolean that indicates if the cursor should be displayed.</param>
        public static void PrintMatrix(char[,] matrix, List<Position> selectedPositions, List<Position> correctPositions, Position currentPosition, bool displayCursor = true)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (currentPosition.Equals(new Position(i, j)) && displayCursor)
                    {
                        BackgroundColor = ConsoleColor.Green;
                        ForegroundColor = ConsoleColor.Black;
                        Write(matrix[i, j]);
                    }
                    else if (correctPositions.Contains(new Position(i, j)))
                    {

                        BackgroundColor = ConsoleColor.Blue;
                        ForegroundColor = ConsoleColor.Black;
                        Write(matrix[i, j]);
                    }
                    else if (selectedPositions.Contains(new Position(i, j)))
                    {

                        BackgroundColor = ConsoleColor.Yellow;
                        ForegroundColor = ConsoleColor.Black;
                        Write(matrix[i, j]);
                    }
                    else
                    {
                        BackgroundColor = ConsoleColor.Black;
                        ForegroundColor = ConsoleColor.White;
                        Write(matrix[i, j]);
                    }
                    ConsoleConfig();
                    Write(" ");
                }
                WriteLine();
            }
        }

        /// <summary>This method is used to display a scrolling menu.</summary>
        /// <param name= "choices"> The choices of the menu.</param>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "additionalText"> The subtitle of the title.</param>
        /// <param name= "specialText"> Special text as a font.</param>
        /// <returns> The position of the choice selected.</returns>
        public static int ScrollingMenu (string question, string[] choices, string additionalText = "", string specialText = "Title.txt")
        {
            int position = 0;
            bool choiceMade = false;
            int recurrence = 0;
            while (!choiceMade)
            {
                Clear();
                Title(question,additionalText,specialText,recurrence);
                string[]currentChoice = new string[choices.Length];
                for (int i = 0; i < choices.Length; i++)
                {
                    if (i == position)
                    {
                        currentChoice[i] = $" > {choices[i]}";
                        BackgroundColor = ConsoleColor.Green;
                        ForegroundColor = ConsoleColor.Black;
                        CenteredWL(currentChoice[i], Black, Black, Green, Black);
                        ConsoleConfig();
                    }
                    else 
                    {
                        
                        currentChoice [i]= $"   {choices[i]}";
                        CenteredWL(currentChoice[i]);
                    }
                }
                switch(ReadKey().Key)
                {
                    case ConsoleKey.UpArrow : case ConsoleKey.Z : if(position == 0)position = choices.Length-1; else if(position > 0) position--;break;
                    case ConsoleKey.DownArrow : case ConsoleKey.S : if(position == choices.Length-1)position = 0; else if(position < choices.Length-1)position++;break;
                    case ConsoleKey.Enter : return position;
                    case ConsoleKey.Escape : return -1;
                }
                recurrence++;
            }
            return position;
        }
        
        /// <summary>This method is used to convert a CSV file into a char matrix.</summary>
        /// <param name="path">The path of the CSV file.</param>
        /// <returns>The generated matrix.</returns>
        public static char[,] CsvToMatrix(string path)
        {
            string[] lines = File.ReadAllLines(path);
            char[,] matrix = new char[lines.Length,(lines[0].Length/2)+1];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] s = lines[i].Split(';'); 
                char[] c = new char[s.Length];
                for (int j = 0; j < s.Length; j++)c[j] = s[j][0];
                for (int j = 0; j < matrix.GetLength(1); j++)matrix[i,j] = c[j];
            }
            return matrix;
        }

        /// <summary>This method is used to pause the program.</summary>
        public static void Pause()
        {
            WriteLine("\nPress [ENTER] to continue...");
            while(ReadKey(true).Key!=ConsoleKey.Enter)Sleep(5);
        }
        
        /// <summary>This method is used to display a loading screen.</summary>
        /// <param name="text"> The text to display. </param>
        public static void LoadingScreen(string text)
        {
            int t_interval = (int) 2000/text.Length;
            char[]loadingBar = new char[text.Length];
            for (int j = 0; j < loadingBar.Length; j++)loadingBar[j]= '█';
            for (int i = 0; i < text.Length; i++)
            {
                Clear();
                CenteredWL(text);
                string bar = "";
                if (i == text.Length-1)
                {
                    
                    for (int l = 0; l < loadingBar.Length; l++) bar += loadingBar[l];
                    bar += $" {(int)((ToDecimal(i+1)/ToDecimal(text.Length))*100)} %\n";
                    Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
                    ForegroundColor = Green;
                    Write(bar);
                    //CenteredWL(bar, Black, Black, Black, Green);
                    Sleep(1000);
                }
                else
                {
                    Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
                    for (int l = 0; l < i; l++) bar += loadingBar[l];
                    ForegroundColor = Green;
                    Write(bar);
                    bar = ""; 
                    for (int l = i; l < text.Length; l++)bar += loadingBar[l];
                    bar += $"  {(int)((ToDecimal(i+1)/ToDecimal(text.Length))*100)} %\n";
                    ForegroundColor = Red;
                    Write(bar);
                    ConsoleConfig();
                    Sleep(t_interval);
                }
                ConsoleConfig();
            }
            Clear();
        }
        
        /// <summary>This method is used to display a title.</summary>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "additionalText"> The subtitle of the title.</param>
        /// <param name= "pathSpecialText"> Special text as a font.</param>
        /// <param name= "recurrence"> Whether the title has been displayed yet or not.</param>
        public static void Title (string text,  string additionalText = "",string pathSpecialText = "", int recurrence = 0)
        {
            Clear();
            if (text != "")CenteredWL($"\n{text}\n");
            if (pathSpecialText != "")PrintSpecialText(pathSpecialText);
            if(additionalText != "")
            {
                if (recurrence != 0)CenteredWL(additionalText+"\n");
                else
                {
                    Write("{0,"+((WindowWidth / 2) - (additionalText.Length / 2)) + "}","");
                    for(int i = 0; i < additionalText.Length; i++)
                    {
                        Write(additionalText[i]);
                        Sleep(50);
                        if(KeyAvailable)
                        {
                            ConsoleKeyInfo keyPressed = ReadKey(true);
                            if(keyPressed.Key == ConsoleKey.Enter||keyPressed.Key == ConsoleKey.Escape)
                            {
                                Write(additionalText.Substring(i+1));
                                break;
                            }
                        }
                    }
                    WriteLine("\n");
                }
                
            }
            
        }

        /// <summary>This method is used to display a special text, stored in a txt file. </summary>
        /// <param name="path">the relative path of the file.</param>
        public static void PrintSpecialText(string path)
        {
            string[] specialText = File.ReadAllLines(path);
            foreach(string line in specialText)WriteLine("{0," + ((WindowWidth / 2) + (line.Length / 2)) + "}", line);
            WriteLine("\n");
        }
        
        /// <summary>This method is used to display a centered text and come back to line.</summary>
        /// <param name="text"> The text to display. </param>
        public static void CenteredWL(string text, ConsoleColor back1 = Black, ConsoleColor fore1 = White, ConsoleColor back2 = Black, ConsoleColor fore2 = White)
        {
            
            BackgroundColor = back1;
            ForegroundColor = fore1;
            Write("{0,"+((WindowWidth / 2) - (text.Length / 2)) + "}","");
            BackgroundColor = back2;
            ForegroundColor = fore2;
            WriteLine(text);
            ConsoleConfig();
            
        }   

        /// <summary> This method is used to set the console configuration. </summary>
        /// <param name="state"> Wether the config is used as the default config (true) or for the end of the program (false). </param>
        public static void ConsoleConfig(bool state = true)
        {
            if (state)
            {
                CursorVisible = false;
                BackgroundColor = ConsoleColor.Black;
                ForegroundColor = ConsoleColor.White;
            }
            else CursorVisible = true;
        }

        /// <summary>This method is used to exit the game.</summary>
        public static void FinalExit()
        {
            LoadingScreen("[ Shutting off ]\n");
            ConsoleConfig(false);
            Exit(0);
        }
        
        #endregion
    }
}