using System;
using System.Diagnostics;
using static System.Console;
using static System.Environment;
using static System.Threading.Thread;
using static System.Convert;
using static System.IO.File;

namespace Word_Scramble
{
    abstract class Methods
    {
        #region Utility Methods
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
        /// <summary>This method is used to pause the program.</summary>
        public static void Pause()
        {
            WriteLine("\nPress [ENTER] to continue...");
            ReadLine();
        }
        
        /// <summary>This method is used to display a loading screen.</summary>
        public static void LoadingScreen(string text)
        {
            for (int i = 1; i < text.Length-1; i++)
            {
                Clear();
                WriteLine($"{text}\n");
                int t_interval = 2000/text.Length;
                ForegroundColor = ConsoleColor.Red;
                char[]loadingBar = new char[text.Length];
                for (int k = 0; k <= loadingBar.Length-1; k++)
                {
                    if (k == 0)loadingBar[0] = '[';
                    else if(k <=i)loadingBar[k]='-';
                    else if (k == loadingBar.Length-1)loadingBar[k] = ']';
                    else loadingBar[k]= ' ';
                }
                if (i == loadingBar.Length-2)
                {
                    ForegroundColor = ConsoleColor.Green;
                    for (int l = 0; l < loadingBar.Length; l++)Write(loadingBar[l]);
                    decimal percentage = (ToDecimal(i+2)/ToDecimal(text.Length))*100;
                    Write($" {(int)percentage} %\n");
                    Sleep(800);
                }
                else
                {
                    for (int l = 0; l < loadingBar.Length; l++)Write(loadingBar[l]);
                    decimal percentage = (ToDecimal(i+2)/ToDecimal(text.Length))*100;
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
        public static void Title (string text,  string additionalText = "",string specialText = "", int recurrence = 0)
        {
            Clear();
            if (recurrence != 0)
            {
                if (text != "")WriteLine($"\n{text}\n");
                if (specialText != "")WriteLine($"\n{specialText}\n");
                if (additionalText != "")WriteLine($"\n{additionalText}\n");
            }else
            {
                if (text != "")WriteLine($"\n{text}\n");
                if (specialText != "")WriteLine($"\n{specialText}\n");
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
        
        /// <summary>This method is used to display a scrolling menu.</summary>
        /// <param name= "choices"> The choices of the menu.</param>
        /// <param name= "text"> The content of the title.</param>
        /// <param name= "additionalText"> The subtitle of the title.</param>
        /// <param name= "specialText"> Special text as a font.</param>
        /// <returns> The position of the choice selected.</returns>
        public static int ScrollingMenu (string[] choices, string text, string additionalText = "", string specialText = "")
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
        
        /// <summary>This method is used to exit the game.</summary>
        public static void FinalExit()
        {
            Clear();
            LoadingScreen("-- Shutting off --");
            Clear();
            ConsoleConfig(false);
            Exit(0);
        }
        #endregion
    }
}