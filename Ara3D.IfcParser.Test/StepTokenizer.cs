using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.IfcParser.Test;

// Limited to parsing 2GB maximum, with 64K lines. 

public enum TokenType : byte
{
    None,
    Ident,
    String,
    Whitespace,
    Number,
    Symbol,
    Id,
    Separator,
    Redeclared,
    Unassigned,
    Comment,
    Unknown,
    BeginGroup,
    EndGroup,
    LineBreak,
    EndOfLine,
    Definition,
}

public unsafe class StepTokens
{
    public int[] Tokens;
    public StepRawRecord[] Records;
}

public static unsafe class StepTokenizer
{
    public static readonly TokenType* TokenLookup =
        CreateTokenLookup();

    public static readonly bool* IsNumberLookup =
        CreateNumberLookup();

    public static readonly bool* IsIdentLookup =
        CreateIdentLookup();

    public static TokenType* CreateTokenLookup()
    {
        var r = new TokenType[256];
        for (var i = 0; i < 256; i++)
            r[i] = GetTokenType((byte)i);
        var h = GCHandle.Alloc(r, GCHandleType.Pinned);
        return (TokenType*)h.AddrOfPinnedObject().ToPointer();
    }
    
    public static bool* CreateNumberLookup()
    {
        var r = new bool[256];
        for (var i = 0; i < 256; i++)
            r[i] = IsNumberChar((byte)i);
        var h = GCHandle.Alloc(r, GCHandleType.Pinned);
        return (bool*)h.AddrOfPinnedObject().ToPointer();
    }
    
    public static bool* CreateIdentLookup()
    {
        var r = new bool[256];
        for (var i = 0; i < 256; i++)
            r[i] = IsIdentOrDigitChar((byte)i);
        var h = GCHandle.Alloc(r, GCHandleType.Pinned);
        return (bool*)h.AddrOfPinnedObject().ToPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TokenType LookupToken(byte b)
        => TokenLookup[b];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumberChar(byte b)
    {
        switch (b)
        {
            case (byte)'0':
            case (byte)'1':
            case (byte)'2':
            case (byte)'3':
            case (byte)'4':
            case (byte)'5':
            case (byte)'6':
            case (byte)'7':
            case (byte)'8':
            case (byte)'9':
            case (byte)'E':
            case (byte)'e':
            case (byte)'+':
            case (byte)'-':
            case (byte)'.':
                return true;
        }

        return false;
    }

    public static TokenType GetTokenType(byte b)
    {
        switch (b)
        {
            case (byte)'0':
            case (byte)'1':
            case (byte)'2':
            case (byte)'3':
            case (byte)'4':
            case (byte)'5':
            case (byte)'6':
            case (byte)'7':
            case (byte)'8':
            case (byte)'9':
            case (byte)'+':
            case (byte)'-':
                return TokenType.Number;

            case (byte)' ':
            case (byte)'\t':
                return TokenType.Whitespace;

            case (byte)'\n':
            case (byte)'\r':
                return TokenType.LineBreak;

            case (byte)'\'':
                return TokenType.String;

            case (byte)'.':
                return TokenType.Symbol;

            case (byte)'#':
                return TokenType.Id;

            case (byte)';':
                return TokenType.EndOfLine;

            case (byte)'(':
                return TokenType.BeginGroup;

            case (byte)'=':
                return TokenType.Definition;

            case (byte)')':
                return TokenType.EndGroup;

            case (byte)',':
                return TokenType.Separator;

            case (byte)'$':
                return TokenType.Unassigned;

            case (byte)'*':
                return TokenType.Redeclared;

            case (byte)'/':
                return TokenType.Comment;

            case (byte)'a':
            case (byte)'b':
            case (byte)'c':
            case (byte)'d':
            case (byte)'e':
            case (byte)'f':
            case (byte)'g':
            case (byte)'h':
            case (byte)'i':
            case (byte)'j':
            case (byte)'k':
            case (byte)'l':
            case (byte)'m':
            case (byte)'n':
            case (byte)'o':
            case (byte)'p':
            case (byte)'q':
            case (byte)'r':
            case (byte)'s':
            case (byte)'t':
            case (byte)'u':
            case (byte)'v':
            case (byte)'w':
            case (byte)'x':
            case (byte)'y':
            case (byte)'z':
            case (byte)'A':
            case (byte)'B':
            case (byte)'C':
            case (byte)'D':
            case (byte)'E':
            case (byte)'F':
            case (byte)'G':
            case (byte)'H':
            case (byte)'I':
            case (byte)'J':
            case (byte)'K':
            case (byte)'L':
            case (byte)'M':
            case (byte)'N':
            case (byte)'O':
            case (byte)'P':
            case (byte)'Q':
            case (byte)'R':
            case (byte)'S':
            case (byte)'T':
            case (byte)'U':
            case (byte)'V':
            case (byte)'W':
            case (byte)'X':
            case (byte)'Y':
            case (byte)'Z':
            case (byte)'_':
                return TokenType.Ident;

            default:
                return TokenType.Unknown;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace(byte b)
        => b == ' ' || b == '\t';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsLineBreak(byte b)
        => b == '\n' || b == '\r';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIdent(byte b)
        => b >= 'A' && b <= 'Z' || b >= 'a' && b <= 'z' || b == '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDigit(byte b)
        => b >= '0' && b <= '9';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsIdentOrDigitChar(byte b)
        => IsIdent(b) || IsDigit(b);

    public static int AdvancePast(byte* begin, int index, int cnt, string s)
    {
        if (s.Length > cnt)
            throw new Exception("Data is too short");
        foreach (var c in s)
            if (begin[index++] != (byte)c)
                throw new Exception($"Expected a {c} not a {(char)begin[index - 1]}");
        return s.Length;
    }

    public static StepTokens? CreateTokens(byte* ptr, int length)
    {
        if (ptr == null || length <= 0)
            return null;

        var start = AdvancePast(ptr, 0, length, "ISO-10303-21;");
        var curByte = start;

        // Back up the "end" to the last semi-colon.
        // This prevents the algorithm from going past the end of bounds
        // If the input is well-formed. 
        // An unclosed comment or string could still blow things up though. 
        while (length > curByte && ptr[length] != ';')
        {
            length--;
        }

        if (length <= curByte + 5)
            throw new Exception("Insufficient data to parse");
        
        var cnt = 0;
        var recordCount = 0;

        // Count the number of tokens.
        while (curByte < length)
        {
            cnt++;
            if (ptr[curByte] == (byte)'=')
                recordCount++;
            CreateToken(ref curByte, ptr, length);
        }

        var r = new StepTokens();
        var tokensArray = new int[cnt];
        var records = new StepRawRecord[recordCount];
        r.Tokens = tokensArray;
        r.Records = records;

        fixed (int* pTokens = tokensArray)
        {
            // Store the tokens
            var curToken = 0;
            curByte = start;
            var curRecord = 0;
            while (curByte < length)
            {
                pTokens[curToken] = curByte;
                
                if (ptr[curByte] == (byte)'=')
                {
                    records[curRecord++].BeginToken = curToken - 1;
                }
                else if (ptr[curByte] == (byte)';')
                {
                    // We might encounter a ';' after the last record which is superfluous. 
                    if (curRecord < recordCount && records[curRecord].EndToken == 0)
                        records[curRecord].EndToken = curToken;
                }

                CreateToken(ref curByte, ptr, length);
                curToken++;
            }
        }

        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateToken(ref int i, byte* ptr, int length)
    {
        var type = TokenLookup[ptr[i++]];

        switch (type)
        {
            case TokenType.Ident:
                while (IsIdentLookup[ptr[i]])
                    i++;
                break;

            case TokenType.String:
                while (i < length && ptr[i++] != '\'')
                { }
                break;

            case TokenType.LineBreak:
                while (IsLineBreak(ptr[i]))
                    i++;
                break;

            case TokenType.Number:
                while (IsNumberLookup[ptr[i]])
                    i++;
                break;

            case TokenType.Symbol:
                while (i < length && ptr[i++] != '.')
                { }
                break;

            case TokenType.Id:
                while (IsNumberLookup[ptr[i]])
                    i++;
                break;

            case TokenType.Comment:
                var prev = ptr[i++];
                while (i < length && (prev != '*' || ptr[i] != '/'))
                    prev = ptr[i++];
                i++;
                break;

            case TokenType.Whitespace:
            case TokenType.Definition:
                while (ptr[i] == ' ' || ptr[i] == '\t')
                    i++;
                break;

            case TokenType.BeginGroup:
            case TokenType.EndGroup:
            case TokenType.Unassigned:
            case TokenType.Redeclared:
            case TokenType.Separator:
            case TokenType.EndOfLine:
            default: 
                break;
        }
    }
}