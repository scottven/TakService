using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace TakEngine.Notation
{
    public class TakPGN
    {
        public static DatabaseRecord LoadFromFile(string path)
        {
            return LoadFromString(System.IO.File.ReadAllText(path));
        }
        public static DatabaseRecord LoadFromString(string s)
        {
            var inputStream = new AntlrInputStream(s);
            var lexer = new Generated.TakPGNLexer(inputStream);
            var tokens = new CommonTokenStream(lexer);
            var parser = new Generated.TakPGNParser(tokens);
            var builder = new DatabaseBuilder();
            parser.AddParseListener(builder);
            var tree = parser.parse();
            return builder.Database;
        }
    }
}
