namespace WordScramble.models
{
    /// <summary>The player class.</summary>
    public class Player : IEquatable<Player>
    {
        #region Attributes 
        /// <summary>The name of the player.</summary>
        public string Name;
        /// <summary>Wether the player has finished the game or not.</summary>
        public bool InGame;
        /// <summary>The score of the player.</summary>
        public int Score;
        /// <summary>The maximum number of words found by the player.</summary>
        public int MaxScore;
        /// <summary>The words found by the player.</summary>
        public List<string> Words;
        #endregion

        #region Constructors
        /// <summary>The constructor of the player class.</summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="inGame">Whether the player's game has ended or not.</param>
        /// <param name="score">The score of the player.</param>
        /// <param name="maxScore">The maximum score of the player.</param>
        /// <param name="words">The list of words found by the player.</param>
        /// <returns>A new player.</returns>
        public Player (string name = "", bool inGame = false, int score = 0, int maxScore = 0,List<string>? words = null)
        {
            Name = name;
            InGame = inGame;
            Score = score;
            MaxScore = maxScore;
            Words = new List<string>();
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
        public bool Equals(Player? obj)
        {
            if (obj is null)
                return false;

            return Name == obj.Name && InGame == obj.InGame && Score == obj.Score && MaxScore == obj.MaxScore && Words == obj.Words;
        }

        /// <summary>This method is used to display every data about the player.</summary>
        /// <returns>A string with every data.</returns>
        public override string ToString() 
        {
            string s = $"{Name};{InGame};{Score};{MaxScore};";
            if (Words != null && Words.Count != 0)
            {
                for (int i = 0; i < Words.Count; i++)
                {
                    s += Words[i];
                    if (i != Words.Count - 1) s += ",";
                }
            }
            return s;
        }
        /// <summary>this method is used to convert a string to a player.</summary>
        /// <param name="line">The string to convert.</param>
        /// <returns>The player.</returns>
        public static Player StringToPlayer(string line)
        {
            string[] data = line.Split(';');
            Player p = new Player(data[0], bool.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));
            if(data[4] is not null)
            {
                string[] words = data[4].Split(',');
                foreach(string w in words) p.Words.Add(w);
            }
            return p;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Player);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, InGame, Score, MaxScore, Words);
        }
        #endregion
    }
}