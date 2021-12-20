using System;
using System.Collections.Generic;
using System.Text;

namespace C_Sharp_Compiler
{
    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpenParenthesisToken,
        CloseParenthesisToken,
        BadToken,
        EndOfFileToken,
        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression,
        TrueKeyWord,
        FalseKeyWord,
        IdentifierToken
    }
}
