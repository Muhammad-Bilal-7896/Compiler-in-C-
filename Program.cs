using System;

namespace C_Sharp_Compiler_CC_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return;

            if (line == "1 + 2 * 3")
                Console.WriteLine("7");
            else
                Console.WriteLine("ERROR: Invalid expression!");
        }
    }

    enum SyntaxKind
    {
        NumberToken,
        WhitespaceToken,
        PlusToken,
        MinusToken,
        StarToken,

    }

    class SyntaxToken
    {
        public SyntaxToken(SyntaxKind kind, int position, string text,object value)
        {
            Kind = kind;
            Position = position;
        }

        public SyntaxKind Kind { get; }
        public int Position { get; }
        public string Text { get; }
    }

    class Lexer
    {
        private readonly string _text;
        private int _position { get; }

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current
        {
            get
            {
                if (_position >= _text.Length)
                    return "\0";

                return _text[_position];
            }
        }

        public void Next()
        {
            _position++;
        }

        public SyntaxToken NextToken()
        {
            // <Numbers> for now
            // + - * / ( )
            // <whitespace>

            if(char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                return new SyntaxToken(SyntaxKind.NumberToken,start,text,value)
            }

            if(char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                    Next();

                var length = _position - start;
                var text = _text.Substring(start, length);
                int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.WhitespaceToken, start, text, value);
            }

            if(Current == '+')
            {
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+" , null);
            }
            else if (Current == '-')
            {
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            }
            else if (Current == '*')
            {
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            }
            else if (Current == '+')
            {
                return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null);
            }
        }
    }

}
