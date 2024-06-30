using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Ara3D.IfcParser.Test;

// Limited to parsing 2GB maximum.

[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct StepToken
{
    public int _start;
    public ushort _length;
    public ushort _type;

    public int Start => _start;
    public int End => Start + Count;
    public int Count => _length;
    public TokenType Type => (TokenType)_type;

    public int GetByteSize()
    {
        if (Type == TokenType.Ident)
            return 2;
        if (Type == TokenType.Number)
            return 8;
        if (Type == TokenType.String)
            return Count - 2;
        if (Type == TokenType.Symbol)
            return 2;
        if (Type == TokenType.Id)
            return 4;
        return Count;
    }

    public StepToken(TokenType type, int start, int end)
    {
        _type = (ushort)type;
        _start = start;
        _length = (ushort)(end - start);
    }

    public string GetString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes, Start, Count);
    }
}

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
    Special,
    Comment,
    Unknown,
    BeginLine,
    EndLine,
    Definition,
    BeginGroup,
    EndGroup,
    LineBreak,
}

public static class StepTokenizer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSeparator(byte b)
    {
        switch (b)
        {
            case (byte)'\n':
            case (byte)';':
            case (byte)'(':
            case (byte)'=':
            case (byte)')':
            case (byte)',':
                return true;
        }

        return false; 
    }

    public static List<int> GetSeparatorIndices(byte[] bytes)
    {
        var r = new List<int>(bytes.Length / 16);
        for (var i=0; i<bytes.Length; i++)
            if (IsSeparator(bytes[i]))
                r.Add(i);
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNumber(byte b)
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
                return TokenType.EndLine;

            case (byte)'(':
                return TokenType.BeginGroup;

            case (byte)'=':
                return TokenType.Definition;

            case (byte)')':
                return TokenType.EndGroup;

            case (byte)',':
                return TokenType.Separator;

            case (byte)'$':
            case (byte)'*':
                return TokenType.Special;

            case (byte)'/':
                return TokenType.Comment;
        }
        
        if (IsIdent(b))
            return TokenType.Ident;
        
        return TokenType.Unknown;
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
    public static bool IsIdentOrDigit(byte b)
        => IsIdent(b) || IsDigit(b);

    /*
    public static List<StepToken> CreateTokens(byte[] bytes)
    {
        var r = new List<StepToken>();
        var i = 0;
        while (i < bytes.Length)
        {
            var st = CreateToken(bytes, i);
            if (st.Type != TokenType.None
                && st.Type != TokenType.Separator
                && st.Type != TokenType.Whitespace)
            {
                r.Add(st);
            }

            i = st.End;
        }

        return r;
    }
    */

    public static unsafe StepToken[] CreateTokens(byte[] bytes)
    {
        var r = new StepToken[bytes.Length / 4];
        var cnt = 0;
        fixed (StepToken* pArray = &r[0])
        {
            var ptr = pArray;
            var i = 0;
            while (i < bytes.Length)
            {
                CreateToken(bytes, i, ptr);
                i = ptr->End;

                switch (ptr->Type)
                {
                    case TokenType.None:
                    case TokenType.Whitespace:
                    case TokenType.Separator:
                    case TokenType.Comment:
                    case TokenType.Unknown:
                    case TokenType.BeginLine:
                    case TokenType.EndLine:
                    case TokenType.BeginGroup:
                    case TokenType.EndGroup:
                    case TokenType.LineBreak:
                    case TokenType.Definition:
                        break;

                    case TokenType.Ident:
                    case TokenType.String:
                    case TokenType.Number:
                    case TokenType.Symbol:
                    case TokenType.Id:
                    case TokenType.Special:
                    default:
                        ptr++;
                        break;
                }
            }

            cnt = (int)(ptr - pArray);
        }

        var r2 = new StepToken[cnt];
        return r;
    }

    public static StepToken CreateToken(byte[] bytes, int start)
    {
        var i = start;
        var n = bytes.Length;
        var state = GetTokenType(bytes[i++]);

        switch (state)
        {
            case TokenType.Ident:
                while (i < n && IsIdentOrDigit(bytes[i]))
                    i++;
                return new StepToken(state, start, i);

            case TokenType.String:
                while (i < n && bytes[i] != '\'')
                    i++;
                return new StepToken(state, start, i + 1);                    
            
            case TokenType.Whitespace:
                while (i < n && IsWhiteSpace(bytes[i]))
                    i++;
                return new StepToken(state, start, i);

            case TokenType.LineBreak:
                while (i < n && IsLineBreak(bytes[i]))
                    i++;
                return new StepToken(TokenType.BeginLine, start, i);

            case TokenType.Number:
                while (i < n && IsNumber(bytes[i]))
                    i++;
                return new StepToken(state, start, i);

            case TokenType.Symbol:
                while (i < n && bytes[i] != '.')
                    i++;
                return new StepToken(state, start, i + 1);

            case TokenType.Id:
                while (i < n && IsDigit(bytes[i]))
                    i++;
                return new StepToken(state, start, i);

            case TokenType.Comment:
                if (bytes[i] != '*')
                    return new StepToken(TokenType.Unknown, start, i);
                while (i < n && (bytes[i - 1] != '*' || bytes[i] != '/'))
                    i++;
                return new StepToken(state, start, i);

            case TokenType.BeginGroup:
            case TokenType.EndGroup:
            case TokenType.Definition:
            case TokenType.EndLine:
            case TokenType.Special:
            case TokenType.Separator:
                return new StepToken(state, start, i);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static unsafe void CreateToken(byte[] bytes, int start, StepToken* ptr)
    {
        var i = start;
        var n = bytes.Length;
        ptr->_start = start;
        ptr->_type = (ushort)GetTokenType(bytes[i++]);

        switch (ptr->_type)
        {
            case (ushort)TokenType.Ident:
                while (i < n && IsIdentOrDigit(bytes[i]))
                    i++;
                break;

            case (ushort)TokenType.String:
                while (i < n && bytes[i] != '\'')
                    i++;
                i += 1;
                break;

            case (ushort)TokenType.Whitespace:
                while (i < n && IsWhiteSpace(bytes[i]))
                    i++;
                break;

            case (ushort)TokenType.LineBreak:
                while (i < n && IsLineBreak(bytes[i]))
                    i++;
                ptr->_type = (ushort)TokenType.BeginLine;
                break;

            case (ushort)TokenType.Number:
                while (i < n && IsNumber(bytes[i]))
                    i++;
                break;

            case (ushort)TokenType.Symbol:
                while (i < n && bytes[i] != '.')
                    i++;
                i += 1;
                break;

            case (ushort)TokenType.Id:
                while (i < n && IsDigit(bytes[i]))
                    i++;
                break;

            case (ushort)TokenType.Comment:
                if (bytes[i] != '*')
                    throw new Exception("Expected comment");
                while (i < n && (bytes[i - 1] != '*' || bytes[i] != '/'))
                    i++;
                i += 1;
                break;

            case (ushort)TokenType.BeginGroup:
            case (ushort)TokenType.EndGroup:
            case (ushort)TokenType.Definition:
            case (ushort)TokenType.EndLine:
            case (ushort)TokenType.Special:
            case (ushort)TokenType.Separator:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        var cnt = (i - ptr->_start);
        if (cnt > ushort.MaxValue)
            throw new Exception($"Line is too long {cnt}");
        ptr->_length = (ushort)cnt;
    }
}