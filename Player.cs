using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Word_Scramble
{
    /// <summary>The player class.</summary>
    public class Player
    {
        #region Attributes 
        /// <summary>The name of the player.</summary>
        public string Name { get; set; }
        /// <summary>Wether the player has finished the game or not.</summary>
        public bool InGame { get; set; }

        /// <summary>The score of the player.</summary>
        public int Score { get; set; }
        /// <summary>The maximum number of words found by the player.</summary>
        public int MaxScore { get; set; }
        /// <summary>The words found by the player.</summary>
        public List<string> Words { get; set; }
        #endregion

        #region Constructor
        /// <summary>The constructor of the player class.</summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="inGame">Whether the player's game has ended or not.</param>
        /// <param name="score">The score of the player.</param>
        /// <param name="maxScore">The maximum score of the player.</param>
        /// <param name="words">The list of words found by the player.</param>
        /// <returns>A new player.</returns>
        public Player (string name = "", bool inGame = false, int score = 0, int maxScore = 0,List<string> words = null)
        {
            Name = name;
            InGame = inGame;
            Score = score;
            MaxScore = maxScore;
            Words = words;
        }
        
        /// <summary>Initializes a new instance of the <see cref="Player"/> class.</summary>
        /// <param name="p">The player to copy.</param>
        /// <returns>A new player.</returns>
        public Player (Player p)
        {
            Name = p.Name;
            InGame = p.InGame;
            Score = p.Score;
            MaxScore = p.MaxScore;
            Words = p.Words;
        }
        #endregion

        #region Methods
        /// <summary>Adds a word and its score to the player's list of words and score.</summary>
        /// <param name="word">The word to add.</param>
        public void AddWord(string word)
        {
            if (Words == null) Words = new List<string>();
            Words.Add(word);
            Score += word.Length;
        }
        /// <summary>Adds a bonus to the player's score.</summary>
        /// <param name="bonus">The bonus to add.</param>
        public void AddBonus(int bonus)
        {
            Score += bonus;
        } 
        /// <summary>Compares two players.</summary>
        /// <param name="obj">The player to compare.</param>
        /// <returns>Whether the two players are equal or not.</returns>
        public bool Equals(Player obj) => Name == obj.Name && InGame == obj.InGame && Score == obj.Score && MaxScore == obj.MaxScore && Words == obj.Words;
        /// <summary>This method is used to display every data about the player.</summary>
        /// <returns>A string with every data.</returns>
        public override string ToString() 
        {
            string s = $"Name: {Name}, InGame: {InGame}, Score: {Score}, MaxScore: {MaxScore}, Words:";
            if (Words != null) foreach (string w in Words) s += $" {w},";
            else s += " \"vide\"";
            return s;
        }

        #endregion
    }
}