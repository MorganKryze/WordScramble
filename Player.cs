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
        public Player(string name = "", bool inGame = false, int score = 0, int maxScore = 0,List<string> words = null)
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
        /// <summary>This method is used to display every data about the player.</summary>
        /// <returns>A string with every data.</returns>
        public override string ToString() => $"Name: {Name}, InGame: {InGame}, Score: {Score}, MaxScore: {MaxScore}, Words: {Words}";

        #endregion
    }
}