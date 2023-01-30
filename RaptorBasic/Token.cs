namespace RaptorBasic
{
    public class Token
    {
        public TokenType tokenType;
        public object Value;
        public int line;
        public Token(TokenType t, object v, int line)
        {
            tokenType = t;
            Value = v;
            this.line = line;
        }
    }
}