using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Word_Scramble
{
    /// <summary>The ranking class.</summary>
    public class Ranking : IEquatable<Ranking>
    {
        #region Properties
        /// <summary>The players that finished the game.</summary>
        public List<Player> Finished { get; set; }
        /// <summary>The players that didn't finish the game.</summary>
        public List<Player> NotFinished { get; set; }
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="Ranking"/> class.</summary>
        /// <param name="finished">if set to <c>true</c> [finished].</param>
        public Ranking(bool finished = false)
        {
            if (finished)
            {
                Finished = new List<Player>();
                string[] lines = File.ReadAllLines("dataPlayers/Finished.txt");
                if (lines != null)
                {
                    foreach(string l in lines)
                    {
                        string[] data = l.Split(';');
                        string Name = data[0];
                        bool InGame = Convert.ToBoolean(data[1]);
                        int Score = Convert.ToInt32(data[2]);
                        int MaxScore = Convert.ToInt32(data[3]);
                        List<string> Words = data[4].Split(',').ToList();
                        Finished.Add(new Player(Name, InGame, Score, MaxScore, Words));
                    }
                }
            }
            else
            {
                NotFinished = new List<Player>();
                string[] lines = File.ReadAllLines("dataPlayers/NotFinished.txt");
                if (lines != null)
                {
                    foreach (string l in lines)
                    {
                        string[] data = l.Split(';');
                        string Name = data[0];
                        bool InGame = Convert.ToBoolean(data[1]);
                        int Score = Convert.ToInt32(data[2]);
                        int MaxScore = Convert.ToInt32(data[3]);
                        List<string> Words = data[4].Split(',').ToList();
                        NotFinished.Add(new Player(Name, InGame, Score, MaxScore, Words));
                    }
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>this method is used to check the equality between two lists.</summary>
        public bool Equals(Ranking other)
        {
            foreach (Player p in Finished)
            {
                if (!other.Finished.Contains(p))
                    return false;
            }return true;
        }
        #endregion
    }
}