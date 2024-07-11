using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using Ara3D.Logging;

namespace Ara3D.IfcParser.Test;

public static unsafe class StepTokenizerV2
{
    public struct StepInstance
    {
        public uint Start;
        public ushort Length;
        public ushort DefStart;
    }

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
    public const byte SQUOTE = (byte)'\'';
    public const byte DOT = (byte)'.';
    public const byte SLASH = (byte)'/';
    public const byte STAR = (byte)'*';
    public const byte SPACE = (byte)' ';
    public const byte TAB = (byte)'\t';
    public const byte NL = (byte)'\n';
    public const byte CR = (byte)'\r';

    public static StepTokens? CreateTokens(byte* begin, byte* end, ILogger logger)
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

        logger.Log("Counting tokens");

        // Count the number of tokens.
        while (cur < end)
        {
            cnt++;
            if (*cur == EQ)
                entityCount++;
            CreateToken(ref cur, end);
        }

        logger.Log("Allocating the arrays for records and tokens");

        var r = new StepTokens();
        var tokensArray = new byte*[cnt];
        var entities = new StepRawRecord[entityCount];
        r.Tokens = tokensArray;
        r.Entities = entities;

        fixed (byte** pTokens = tokensArray)
        {

            // Store the tokens
            logger.Log("Creating the tokens");

            cnt = 0;
            cur = begin;
            while (cur < end)
            {
                pTokens[cnt++] = cur;
                CreateToken(ref cur, end);
            }

            // Store the token indices for record definitions
            logger.Log("Computing records");

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
    public static int IndexOfByte(Vector128<byte> vector, byte pattern)
    {
        // Create a vector with the pattern repeated in all positions
        var patternVector = Vector128.Create(pattern);

        // Compare the input vector with the pattern vector
        var compareResult = Sse2.CompareEqual(vector, patternVector);

        // Create a mask from the comparison result
        var mask = (uint)Sse2.MoveMask(compareResult);

        // Find the index of the first set bit in the mask
        return BitOperations.TrailingZeroCount(mask);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AdvancePast(ref byte* cur, byte pattern)
    {
        while (true)
        {
            var vec = Sse2.LoadVector128(cur);
            var i = IndexOfByte(vec, pattern);
            if (i < 16)
            {
                cur += i + 1;
                return;
            }
            cur += 16;
        }
    }

    public static readonly Vector128<byte> DigitStart = Vector128.Create((byte)48); // ASCII '0'
    public static readonly Vector128<byte> DigitEnd = Vector128.Create((byte)57);   // ASCII '9'


    public static void AdvancePastDigit(ref byte* cur)
    {
        var vec = Sse2.LoadVector128(cur);
        var isLessThanZero = Sse2.CompareLessThan(vec.AsSByte(), DigitStart.AsSByte());
        var isGreaterThanNine = Sse2.CompareGreaterThan(vec.AsSByte(), DigitEnd.AsSByte());
        var compare = Sse2.Or(isLessThanZero, isGreaterThanNine).AsByte();
        // MoveMask extracts the most significant bit of each byte and forms a 16-bit mask.
        var mask = Sse2.MoveMask(compare);

        // Find the first zero bit in the mask, indicating the first non-digit byte.
        var firstNonDigit = BitOperations.TrailingZeroCount((uint)mask);
        Debug.Assert(firstNonDigit < 16);
        cur += firstNonDigit;
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
            case (byte)'#':
                while (IsNumberLookup[*cur])
                    cur++;
                break;

            case (byte)'\r':
                if (*cur == NL)
                    cur++;
                break;

            case (byte)'\'':
                AdvancePast(ref cur, SQUOTE);
                break;

            case (byte)'.':
                AdvancePast(ref cur, DOT);
                break;

            case (byte)';':
            case (byte)',':
            case (byte)'=':
                while (*cur == SPACE || *cur == TAB)
                    cur++;
                break;

            case (byte)'/':
                var prev = *cur++;
                while (cur < end && (prev != STAR || *cur != SLASH))
                    prev = *cur++;
                cur++;
                break;

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
                break;
        }
    }
}