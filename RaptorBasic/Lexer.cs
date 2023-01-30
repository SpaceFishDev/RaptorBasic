namespace RaptorBasic
{
    public class Lexer
    {
        public string Source;
        public int CurrentLine;
        public Lexer(string Source)
        {
            this.Source = Source.Replace("\r", "");
        }
        public List<Token> Tokenize()
        {
            List<Token> tokens = new List<Token>();
            int Position = 0;
            while (Position < Source.Length - 1)
            {
                if (Source[Position] == ';')
                {
                    tokens.Add(new Token(TokenType.symbol, Source[Position], CurrentLine));
                    ++Position;
                }
                else if (char.IsDigit(Source[Position]))
                {
                    int start = Position;
                    while (char.IsDigit(Source[Position]))
                    {
                        ++Position;
                    }
                    string str = Source.Substring(start, Position - start);
                    int ob = int.Parse(str);
                    tokens.Add(new Token(TokenType.number, ob, CurrentLine));
                }
                else if (char.IsLetter(Source[Position]))
                {
                    int start = Position;
                    while (char.IsLetterOrDigit(Source[Position]))
                    {
                        ++Position;
                    }
                    string str = Source.Substring(start, Position - start);
                    tokens.Add(new Token(TokenType.word, str, CurrentLine));
                }
                else if (char.IsSymbol(Source[Position]) || char.IsPunctuation(Source[Position]) && Source[Position] != '"')
                {
                    int start = Position;
                    while (char.IsSymbol(Source[Position]) || char.IsPunctuation(Source[Position]) && Source[Position] != '"' && Source[Position] != ' ')
                    {
                        ++Position;
                    }
                    string str = Source.Substring(start, Position - start);
                    tokens.Add(new Token(TokenType.symbol, str, CurrentLine));
                }
                else
                {
                    switch (Source[Position])
                    {
                        case ' ':
                        case '\t':
                            {
                                ++Position;
                                break;
                            }
                        case '"':
                            {
                                Position++;
                                int start = Position;
                                char last = ' ';
                                while (Source[Position] != '"')
                                {
                                    last = Source[Position];
                                    ++Position;
                                }
                                string str = Source.Substring(start, Position - start);
                                ++Position;
                                tokens.Add(new Token(TokenType.str, str, CurrentLine));
                            }
                            break;
                        case '\n':
                            {
                                ++CurrentLine;
                                ++Position;
                            }
                            break;
                        default:
                            {
                                // Error
                            }
                            break;
                    }

                }

            }
            return tokens;
        }

        public List<Command> GetCommands()
        {
            List<Command> commands = new List<Command>();
            List<Token> tokens = this.Tokenize();
            int Position = 0;
            while (Position < tokens.Count)
            {
                if (tokens[Position].tokenType != TokenType.word)
                {
                    //error
                    return null;
                }
                Command cmd = new Command();
                cmd.name = tokens[Position].Value.ToString();
                ++Position;
                while (Position < tokens.Count && tokens[Position].Value.ToString() != ";")
                {
                    cmd.args.Add(tokens[Position]);
                    ++Position;
                }
                ++Position;
                commands.Add(cmd);
            }
            return commands;
        }

    }
}