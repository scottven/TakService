using System.IO;
using System.Collections.Generic;

namespace TakEngine.Notation
{
    public class DatabaseRecord
    {
        public List<GameRecord> Games = new List<GameRecord>();

        public void Write(StreamWriter writer)
        {
            foreach (var game in Games)
                game.Write(writer);
        }

        public void Save(string path)
        {
            using (var writer = File.CreateText(path))
            {
                Write(writer);
            }
        }
    }
}
