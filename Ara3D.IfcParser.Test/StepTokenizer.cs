using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Ara3D.IfcParser.Test;

// Limited to parsing 2GB maximum, with 64K lines. 

public enum TokenType
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
    public byte*[] Tokens;
    public StepRawRecord[] Entities;
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

    public static byte* AdvancePast(byte* begin, byte* end, string s)
    {
        if (end - begin < s.Length)
            return null;
        foreach (var c in s)
            if (*begin++ != (byte)c)
                return null;
        return begin;
    }

    public const byte SEMICOLON = (byte)';';
    public const byte EQ = (byte)'=';
    public const byte HASH = (byte)'#';
    public const byte SINGLE_QUOTE = (byte)'\'';
    public const byte DOT = (byte)'.';
    public const byte SLASH = (byte)'/';
    public const byte STAR = (byte)'*';
    public const byte SPACE = (byte)' ';
    public const byte TAB = (byte)'\t';

    public static StepTokens? CreateTokens(byte* begin, byte* end)
    {
        if (begin == null || end == null)
            return null;

        begin = AdvancePast(begin, end, "ISO-10303-21;");
        if (begin == null)
            return null;

        // Back up the "end" to the last semi-colon.
        // This prevents the algorithm from going past the end of bounds
        // If the input is well-formed. 
        // An unclosed comment or string could still blow things up though. 
        while (end-- > begin && *end != SEMICOLON)
        { }

        if (end < begin + 5)
            return null;

        var cur = begin;
        var cnt = 0;
        var entityCount = 0;

        // Count the number of tokens.
        while (cur < end)
        {
            cnt++;
            if (*cur == EQ)
                entityCount++;
            CreateToken(ref cur, end);
        }

        var r = new StepTokens();
        var tokensArray = new byte*[cnt];
        r.Tokens = tokensArray;

        fixed (byte** pTokens = tokensArray)
        {
            var entities = new StepRawRecord[entityCount];
            r.Entities = entities;

            // Store the tokens
            cnt = 0;
            cur = begin;
            while (cur < end)
            {
                pTokens[cnt++] = cur;
                CreateToken(ref cur, end);
            }

            // Store the token indices for entity definitions
            var e = 0;
            var i = 0;
            while (i < cnt)
            {
                if (*pTokens[i] == HASH && *pTokens[i + 1] == EQ)
                {
                    var j = i;
                    while (j < cnt && *pTokens[j] != SEMICOLON)
                        j++;

                    entities[e++] = new StepRawRecord(i, j);
                    i = j + 1;
                }
                else
                {
                    i++;
                }
            }
        }

        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CreateToken(ref byte* cur, byte* end)
    {
        switch (*cur++)
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
                while (IsNumberLookup[*cur])
                    cur++;
                return;

            case (byte)'\r':
                if (*cur == (byte)'\n')
                    cur++;
                return;

            case (byte)'\'':
                while (cur < end && *cur++ != SINGLE_QUOTE)
                {
                }

                return;

            case (byte)'.':
                while (cur < end && *cur++ != DOT)
                {
                }

                return;

            case (byte)'#':
                while (IsNumberLookup[*cur])
                    cur++;
                return;

            case (byte)';':
            case (byte)',':
            case (byte)'=':
                while (*cur == SPACE || *cur == TAB)
                    cur++;
                return;

            case (byte)'/':
                var prev = *cur++;
                while (cur < end && (prev != STAR || *cur != SLASH))
                    prev = *cur++;
                cur++;
                return;

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
                while (IsIdentLookup[*cur])
                    cur++;
                return;
            }
        }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void OLD_CreateToken(ref byte* cur, byte* end)
    {
        var type = TokenLookup[*cur++];

        switch (type)
        {
            case TokenType.Ident:
                while (IsIdentLookup[*cur])
                    cur++;
                break;

            case TokenType.String:
                while (cur < end && *cur++ != SINGLE_QUOTE)
                {
                }

                break;

            case TokenType.LineBreak:
                while (IsLineBreak(*cur))
                    cur++;
                break;

            case TokenType.Number:
                while (IsNumberLookup[*cur])
                    cur++;
                break;

            case TokenType.Symbol:
                while (*cur++ != DOT)
                {
                }

                break;

            case TokenType.Id:
                while (IsNumberLookup[*cur])
                    cur++;
                break;

            case TokenType.Comment:
                var prev = *cur++;
                while (cur < end && (prev != STAR || *cur != SLASH))
                    prev = *cur++;
                cur++;
                break;

            case TokenType.Whitespace:
            case TokenType.Definition:
                while (*cur == SPACE || *cur == TAB)
                    cur++;
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