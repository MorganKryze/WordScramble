using System;
using System.Diagnostics;
using static System.Console;
using static System.Environment;
using static System.Threading.Thread;
using static System.Convert;
using static System.IO.File;

namespace Word_Scramble
{
    /// <summary>The methods class where the core methods of the program are.</summary>
    abstract class Methods
    {

        #region Core Methods
        /// <summary>This method is used to display the main menu.</summary>
        public static void MainMenu()
        {
            switch(ScrollingMenu(new string[]{"Play      ","Options   ","Exit      "},"", "Welcome Adventurer! Use the arrow keys to move and press [ENTER] to confirm.",true))
            {
                case 1 : MainMenu(); break;
                case 2 : case -1: FinalExit();break;
                default: break;
            }
            LoadingScreen("[ Launching the game ]\n");
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
        }*/
        

        #endregion
        #region Utility Methods
        
        /// <summary>This method is used to select multiple characters.</summary>
        /// <param name="matrix">The grill from wich the user chooses the characters.</param>
        /// <returns>A string as the sum of the characters.</returns>
        public static string SelectWord(char[,] matrix)
        {
            Position currentPosition = new Position(0, 0);
            Position anchor1 = new Position(-1, -1);
            Position anchor2 = new Position(-1, -1);
            List<Position> rightPositions = new List<Position>();
            List<Position> selectedPositions = new List<Position>();
            List<Position> possiblePositions = new List<Position>();
            for(int i = 0; i< matrix.GetLength(0); i++)for(int j = 0; j < matrix.GetLength(1); j++)possiblePositions.Add(new Position(i, j));
            string word = "";
            //bool choiceMade = false;
            while(anchor2.X == -1)
            {
                Clear();
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (currentPosition.Equals(new Position(i, j)))
                        {
                            BackgroundColor = ConsoleColor.Green;
                            ForegroundColor = ConsoleColor.Black;
                            Write(matrix[i,j]);
                        }
                        else if (ListContains(selectedPositions, new Position(i, j)))
                        {
                            
                            BackgroundColor = ConsoleColor.Yellow;
                            ForegroundColor = ConsoleColor.Black;
                            Write(matrix[i,j]);
                        }
                        else if (ListContains(rightPositions, new Position(i, j)))
                        {
                            
                            BackgroundColor = ConsoleColor.Red;
                            ForegroundColor = ConsoleColor.Black;
                            Write(matrix[i,j]);
                        }
                        else Write(matrix[i,j]);
                        ConsoleConfig();
                        Write(" ");
                    }
                    WriteLine();
                }
                    switch(ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow : case ConsoleKey.Z :
                            if(ListContains(possiblePositions, new Position(currentPosition.X - 1, currentPosition.Y)))currentPosition.X--;
                            break;
                        case ConsoleKey.DownArrow : case ConsoleKey.S :
                            if(ListContains(possiblePositions, new Position(currentPosition.X + 1, currentPosition.Y)))currentPosition.X++;
                            break;
                        case ConsoleKey.LeftArrow :case ConsoleKey.Q :
                            if(ListContains(possiblePositions, new Position(currentPosition.X, currentPosition.Y - 1)))currentPosition.Y--;
                            break;
                        case ConsoleKey.RightArrow : case ConsoleKey.D :
                            if(ListContains(possiblePositions, new Position(currentPosition.X, currentPosition.Y + 1)))currentPosition.Y++;
                            break;
                        case ConsoleKey.Enter : 
                            if(anchor1.X == -1)
                            {
                                selectedPositions.Add(new Position(currentPosition.X, currentPosition.Y));
                                if(anchor1.Equals(new Position(-1,-1)))
                                {
                                    anchor1 = new Position(currentPosition.X, currentPosition.Y);
                                    for(int i = 0; i < possiblePositions.Count; i++)
                                    {
                                        if(possiblePositions[i].X != anchor1.X && possiblePositions[i].Y != anchor1.Y)
                                        {
                                            possiblePositions.RemoveAt(i);
                                            i--;
                                        }
                                    }
                                }
                            }
                            else 
                                {
                                    anchor2 = new Position(currentPosition.X, currentPosition.Y);
                                    if(anchor1.X == anchor2.X)
                                    {
                                        if(anchor1.Y < anchor2.Y)
                                        {
                                            for(int i = anchor1.Y; i <= anchor2.Y; i++)
                                            {
                                                word+=matrix[anchor1.X, i];
                                                selectedPositions.Add(new Position(anchor1.X, i));
                                            }
                                        }
                                        else
                                        {
                                            for(int i = anchor1.Y; i >= anchor2.Y; i--)
                                            {
                                                word+=matrix[anchor1.X, i];
                                                selectedPositions.Add(new Position(anchor1.X, i));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if(anchor1.X < anchor2.X)
                                        {
                                            for(int i = anchor1.X; i <= anchor2.X; i++)
                                            {
                                                word+=matrix[i, anchor1.Y];
                                                selectedPositions.Add(new Position(i, anchor1.Y));
                                            }
                                        }
                                        else
                                        {
                                            for(int i = anchor1.X ; i >= anchor2.X; i--)
                                            {
                                                word+=matrix[i, anchor1.Y];
                                                selectedPositions.Add(new Position(i, anchor1.Y));
                                            }
                                        }
                                    }
                                }
                            break;
                        case ConsoleKey.Escape :
                            return "No word selected";
                    }
                    
                }
                return word;
            }
            
        
        
        /// <summary>This method is used to display a scrolling menu.</summary>
        /// <param name= "choices"> The choices of the menu.</param>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "additionalText"> The subtitle of the title.</param>
        /// <param name= "specialText"> Special text as a font.</param>
        /// <returns> The position of the choice selected.</returns>
        public static int ScrollingMenu (string[] choices, string text, string additionalText = "", bool specialText = false)
        {
            int position = 0;
            bool choiceMade = false;
            int recurrence = 0;
            while (!choiceMade)
            {
                Clear();
                Title(text,additionalText,specialText,recurrence);
                string[]currentChoice = new string[choices.Length];
                for (int i = 0; i < choices.Length; i++)
                {
                    if (i == position)
                    {
                        currentChoice[i] = $" > {choices[i]}";
                        BackgroundColor = ConsoleColor.Green;
                        ForegroundColor = ConsoleColor.Black;
                        WriteLine(currentChoice[i]);
                        ConsoleConfig();
                    }
                    else 
                    {
                        
                        currentChoice [i]= $"   {choices[i]}";
                        WriteLine(currentChoice[i]);
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
        
        /// <summary>This method is used to check if a position is contained into a List.</summary>
        /// <param name="list">The list reference.</param>
        /// <param name="position">The position to check.</param>
        /// <returns>True if the position is contained into the list, false otherwise.</returns>
        public static bool ListContains(List<Position> list, Position position)
        {
            foreach(Position p in list)if(p.Equals(position))return true;
            return false;
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
                WriteLine($"\n{text}\n");
                if (i == text.Length-1)
                {
                    ForegroundColor = ConsoleColor.Green;
                    for (int l = 0; l < loadingBar.Length; l++)Write(loadingBar[l]);
                    decimal percentage = (ToDecimal(i+1)/ToDecimal(text.Length))*100;
                    Write($" {(int)percentage} %\n");
                    Sleep(1000);
                }
                else
                {
                    ForegroundColor = ConsoleColor.Green;
                    for (int l = 0; l < i; l++)Write(loadingBar[l]);
                    ForegroundColor = ConsoleColor.Red;
                    for (int l = i; l < text.Length; l++)Write(loadingBar[l]);
                    decimal percentage = (ToDecimal(i+1)/ToDecimal(text.Length))*100;
                    Write($" {(int)percentage} %\n");
                    Sleep(t_interval);
                }
                ConsoleConfig();
            }
            

            Clear();
        }
        
        /// <summary>This method is used to display a title.</summary>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "additionalText"> The subtitle of the title.</param>
        /// <param name= "specialText"> Special text as a font.</param>
        /// <param name= "recurrence"> Whether the title has been displayed yet or not.</param>
        public static void Title (string text,  string additionalText = "",bool specialText = false, int recurrence = 0)
        {
            Clear();
            if (recurrence != 0)
            {
                if (text != "")WriteLine($"\n{text}\n");
                if (specialText)PrintSpecialText("Title.txt");
                if (additionalText != "")WriteLine($"\n{additionalText}\n");
            }else
            {
                if (text != "")WriteLine($"\n{text}\n");
                if (specialText)PrintSpecialText("Title.txt");
                if (additionalText != "") 
                {
                    WriteLine("");
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
            foreach(string line in specialText)WriteLine(String.Format("{0," + ((WindowWidth / 2) + (line.Length / 2)) + "}", line));
            WriteLine("\n");
        }
        public static void CenteredWL(string text)
        {
            WriteLine(String.Format("{0," + ((WindowWidth / 2) + (text.Length / 2)) + "}", text));
        }
        public static void CenteredW(string text)
        {
            Write(String.Format("{0," + ((WindowWidth / 2) + (text.Length / 2)) + "}", text));
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
        
        /// <summary>This method is used to exit the game.</summary>

                //define function that compare two char array if second array feet in first array
        public static bool CompareArray(char[] line, char[] word)
        {
            //if wod is longer than line return false
            if (word.Length > line.Length)
            {
                return(false);
            }
            //compare line and wod
            for (int i = 0; i < word.Length; i++)
            {
                if (line[i] != word[i] && line[i] != '_')
                {
                    return(false);
                }
            }
            return(true);
        }
    
        //fill a list of (int,int) position along line between 2 position x,y and x2,y2
        public static List<(int,int)> FillPosition(int x, int y, int x2, int y2)
        {
            List<(int,int)> line = new List<(int,int)>();
            //if line is vertical
            if (x == x2)
            {
                //if line is going down
                if (y < y2)
                {
                    for (int i = y; i <= y2; i++)
                    {
                        line.Add((x,i));
                    }
                }
                //if line is going up
                else
                {
                    for (int i = y; i >= y2; i--)
                    {
                        line.Add((x,i));
                    }
                }
            }
            //if line is horizontal
            else if (y == y2)
            {
                //if line is going right
                if (x < x2)
                {
                    for (int i = x; i <= x2; i++)
                    {
                        line.Add((i,y));
                    }
                }
                //if line is going left
                else
                {
                    for (int i = x; i >= x2; i--)
                    {
                        line.Add((i,y));
                    }
                }
            }
            //if line is diagonal
            else
            {
                //if line is going down right
                if (x < x2 && y < y2)
                {
                    for (int i = 0; i <= x2-x; i++)
                    {
                        line.Add((x+i,y+i));
                    }
                }
                //if line is going down left
                else if (x > x2 && y < y2)
                {
                    for (int i = 0; i <= x-x2; i++)
                    {
                        line.Add((x-i,y+i));
                    }
                }
                //if line is going up right
                else if (x < x2 && y > y2)
                {
                    for (int i = 0; i <= x2-x; i++)
                    {
                        line.Add((x+i,y-i));
                    }
                }
                //if line is going up left
                else if (x > x2 && y > y2)
                {
                    for (int i = 0; i <= x-x2; i++)
                    {
                        line.Add((x-i,y-i));
                    }
                }
            }
            return(line);

        }
    
    }
}