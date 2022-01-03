using System;
using System.Collections.Generic;
using System.Text;

namespace C_Sharp_Compiler
{
    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}
