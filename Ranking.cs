using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Word_Scramble
{
    /// <summary>The ranking class.</summary>
    public class Ranking
    {
        public List<Player> Finished { get; set; }
        public List<Player> NotFinished { get; set; }

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
    }
}