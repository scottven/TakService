using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace TakEngine.Notation
{
    public class GameRecord
    {
        public Dictionary<string, string> Tags = new Dictionary<string, string>();
        public List<MoveNotation> MoveNotations = new List<MoveNotation>();
        public string ResultCode;

        public static class StandardTags
        {
            /// <summary>
            /// Game size
            /// Expected values: {4,5,6,7,8,4x4,5x5,6x6,etc...}
            /// </summary>
            public const string Size = "Size";
            public const string Result = "Result";
        }

        public int BoardSize
        {
            get
            {
                string s;
                if (!Tags.TryGetValue(StandardTags.Size, out s))
                    throw new ApplicationException("Game size undefined");
                for (int size = 4; size <= 8; size++)
                {
                    string shortVersion = size.ToString();
                    string longVersion = shortVersion + "x" + shortVersion;
                    if (s == shortVersion || s == longVersion)
                        return size;
                }
                throw new ApplicationException("Unsupported game size");
            }
            set
            {
                Tags[StandardTags.Size] = string.Format("{0}x{0}", value);
            }
        }

        public string Result
        {
            get
            {
                string result = null;
                Tags.TryGetValue(StandardTags.Result, out result);
                return result;
            }
            set
            {
                string resultValue = value;
                if (string.IsNullOrEmpty(resultValue))
                    resultValue = "(no result)";
                Tags[StandardTags.Result] = resultValue;
            }
        }

        public void Write(StreamWriter writer)
        {
            foreach (var key in Tags.Keys.OrderBy(x => x))
            {
                var tagValue = Tags[key];

                // escape special characters
                string escaped = tagValue.Replace("\\", "\\\\").Replace("\"", "\\\"");
                writer.WriteLine("[{0,-10} \"{1}\"]", key, escaped);
            }

            int ply = -1;
            foreach (var notation in MoveNotations)
            {
                ply++;
                if (0 == (ply & 1))
                {
                    writer.WriteLine();
                    int turn = (ply >> 1) + 1;
                    writer.Write("{0,2}. ", turn);
                }

                writer.Write("{0,-10}", notation.Text);
                if (0 == (ply & 1))
                    writer.Write(" ");
            }
            if (!string.IsNullOrEmpty(ResultCode))
            {
                writer.Write("   ");
                writer.WriteLine(ResultCode);
            }
            else
                writer.Write(" *");
            writer.WriteLine();
        }
    }
}
