using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C_Sharp_Compiler
{
    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<Diagnostics> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<Diagnostics> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }
}
