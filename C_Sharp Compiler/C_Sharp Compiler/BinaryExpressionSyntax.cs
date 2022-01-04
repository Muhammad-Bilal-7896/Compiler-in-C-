using System;
using System.Collections.Generic;
using System.Text;

namespace C_Sharp_Compiler
{

    //a = 10
    //a = b = c = 5
    //a + b = c = 5

    // a = b = 5     
    //     =  
    //    / \
    //       =
    //      / \
    //     b   5

    // a + b +5
    //     +  
    //    / \
    //   +   5
    //  / \  
    // a   b

    

    internal sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;
        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }
}
