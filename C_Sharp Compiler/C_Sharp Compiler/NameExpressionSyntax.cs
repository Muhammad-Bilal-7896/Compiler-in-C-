using System;
using System.Collections.Generic;
using System.Text;

namespace C_Sharp_Compiler
{
    internal sealed class NameExpresionSyntax : ExpressionSyntax
    {
        public NameExpresionSyntax(SyntaxToken identifierToken)
        {
            IdentifierToken = identifierToken;
        }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
        public SyntaxToken IdentifierToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return IdentifierToken;
        }
    }
}
