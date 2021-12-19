using System;
using System.Collections.Generic;
using System.Linq;

namespace C_Sharp_Compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            bool showTree = false;

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
                    continue;
                }
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);

                if (showTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ForegroundColor = color;
                }

                if (!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diagnostic in syntaxTree.Diagnostics)
                        Console.WriteLine(diagnostic);

                    Console.ForegroundColor = color;
                }
            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == lastChild);
        }
    }

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
        ParenthesizedExpression
    }

    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxKind kind, int position, string text, object value)
        {
            Kind = kind;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position;
        private List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return '\0';

                return _text[_position];
            }
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken Lex()
        {
            // <numbers>
            // + - * / ( )
            // <whitespace>

            if (_position >= _text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                if (!int.TryParse(text, out var value))
                    _diagnostics.Add($"The number {_text} isn't valid Int32.");

                return new SyntaxToken(SyntaxKind.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, null);
            }

            if (Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            else if (Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
            else if (Current == '*')
                return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
            else if (Current == '/')
                return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
            else if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, _position++, "(", null);
            else if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, _position++, ")", null);

            _diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }

    abstract class SyntaxNode
    {
        public abstract SyntaxKind Kind { get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class ExpressionSyntax : SyntaxNode
    {
    }

    sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public override SyntaxKind Kind => SyntaxKind.LiteralExpression;
        public SyntaxToken LiteralToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralToken;
        }
    }

    sealed class BinaryExpressionSyntax : ExpressionSyntax
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

    //Adding Support for unary operator
    sealed class UnaryExpressionSyntax : ExpressionSyntax
    {
        public UnaryExpressionSyntax(SyntaxToken operatorToken, ExpressionSyntax operand)
        {
            OperatorToken = operatorToken;
            Operand = operand;
        }

        public override SyntaxKind Kind => SyntaxKind.UnaryExpression;
        public SyntaxToken OperatorToken { get; }
        public ExpressionSyntax Operand { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OperatorToken;
            yield return Operand;
        }
    }

    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisToken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }

    sealed class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileToken)
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFileToken = endOfFileToken;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public ExpressionSyntax Root { get; }
        public SyntaxToken EndOfFileToken { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }

    internal static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 3;

                default:
                    return 0;
            }
        }


        public static int GetBinaryOperatorPrecedence(this SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StarToken:
                case SyntaxKind.SlashToken:
                    return 2;

                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                    return 1;

                default:
                    return 0;

            }
        }

    }

    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;

        private List<string> _diagnostics = new List<string>();
        private int _position;

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();

                if (token.Kind != SyntaxKind.WhitespaceToken &&
                    token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];

            return _tokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();
            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                    break;

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        

        public SyntaxTree Parse()
        {
            var expresion = ParseExpression();
            var endOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expresion, endOfFileToken);
        }

        //private ExpressionSyntax ParseTerm()
        //{
        //    var left = ParseFactor();

        //    while (Current.Kind == SyntaxKind.PlusToken ||
        //           Current.Kind == SyntaxKind.MinusToken)
        //    {
        //        var operatorToken = NextToken();
        //        var right = ParseFactor();
        //        left = new BinaryExpressionSyntax(left, operatorToken, right);
        //    }

        //    return left;
        //}

        //private ExpressionSyntax ParseFactor()
        //{
        //    var left = ParsePrimaryExpression();

        //    while (Current.Kind == SyntaxKind.StarToken ||
        //           Current.Kind == SyntaxKind.SlashToken)
        //    {
        //        var operatorToken = NextToken();
        //        var right = ParsePrimaryExpression();
        //        left = new BinaryExpressionSyntax(left, operatorToken, right);
        //    }

        //    return left;
        //}

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Match(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = Match(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }

    class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is LiteralExpressionSyntax n)
                return (int)n.LiteralToken.Value;

            if (node is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);

                if (u.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return operand;
                else if (u.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return -operand;
                else
                    throw new Exception($"Unexpected unary operator {u.OperatorToken.Kind}");
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                else if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                else if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                else if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }

            if (node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}
