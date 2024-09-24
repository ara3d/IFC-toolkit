using System.Runtime.InteropServices;
using Ara3D.Buffers;
using Ara3D.StepParser;

namespace Ara3D.IfcParser.Test;

/*
 * Encodes 
 * Related work:
 * https://www.sciencedirect.com/science/article/pii/S0926580519311288
 *
 */
public class BinaryIfcSerializer
{
    public enum IfcTokenType : byte
    {
        None,
        Record,
        Group,
        Id,
        Instance,
        Symbol,
        Number,
        String,
        Redeclared,
        Unassigned,
        EmptyString,
        GlobalId,
        Zero,
        True,
        False,
        Point,
        EmptyGroup,
        Tuple1,
        Tuple2,
        Tuple3,
        Vector1,
        Vector2,
        Vector3,
    }

    public List<byte> Bytes = new();

    /// <summary>
    /// Given a tokens structure (a list of byte pointers into an STEP file)
    /// Returns a list of bytes representing a binary encoding.
    /// </summary>
    public List<byte> Serialize(StepDocument doc)
    {
        Bytes = new List<byte>();
        /*
        foreach (var rec in doc.GetRecords())
        {
            Write((byte)IfcTokenType.Record);
            Write((uint)rec.Id);
            Write(rec.Value);
        }*/

        return Bytes;
    }

    public void Write(StepValue value)
    {
        switch (value)
        {
            case StepList stepAggregate:
                var cnt = stepAggregate.Values.Count;
                if (cnt > ushort.MaxValue)
                    throw new Exception("Too many items in group");
                switch (cnt)
                {
                    case 0:
                        Write((byte)IfcTokenType.EmptyGroup);
                        break;
                    case 1:
                        Write((byte)IfcTokenType.Tuple1);
                        break;
                    case 2:
                        Write((byte)IfcTokenType.Tuple2);
                        break;
                    case 3:
                        Write((byte)IfcTokenType.Tuple3);
                        break;
                    default:
                        Write((byte)IfcTokenType.Group);
                        Write((ushort)cnt);
                        break;
                }
                foreach (var val in stepAggregate.Values)
                    Write(val);
                break;
            case StepId stepId:
                Write((byte)IfcTokenType.Id);
                Write((uint)stepId.Id);
                break;
            case StepNumber stepNumber:
                if ((stepNumber.Value) <= double.Epsilon)
                {
                    Write((byte)IfcTokenType.Zero);
                }
                else
                {
                    Write((byte)IfcTokenType.Number);
                    Write(stepNumber.Value);
                }
                break;
            case StepRedeclared stepRedeclared:
                Write((byte)IfcTokenType.Redeclared);
                break;
            case StepString stepString:
                if (stepString.Value.Length == 22)
                {
                    WriteGlobalId(stepString.Value);
                }
                else if (stepString.Value.Length == 0)
                {
                    Write((byte)IfcTokenType.EmptyString);
                }
                else
                {
                    Write((byte)IfcTokenType.String);
                    Write(stepString.Value);
                }
                break;
            case StepSymbol stepSymbol:
                if (stepSymbol.Name.Equals('T'))
                {
                    Write((byte)IfcTokenType.True);
                }
                else if (stepSymbol.Name.Equals('F'))
                {
                    Write((byte)IfcTokenType.False);
                }
                else
                {
                    Write((byte)IfcTokenType.Symbol);
                    Write((ushort)88);
                }
                break;
            case StepUnassigned stepUnassigned:
                Write((byte)IfcTokenType.Unassigned);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(value));
        }
    }

    public void WriteGlobalId(ByteSpan span)
    {
        Write((byte)IfcTokenType.GlobalId);
        for (var i=0; i < 16; i++)
            Write(span.At(i));
    }

    public void Write(byte b)
    {
        Bytes.Add(b);
    }

    public void Write(ByteSpan span)
    {
        if (span.Length > ushort.MaxValue)
            throw new Exception("String is too long");
        Write((ushort)span.Length);
        for (var i=0; i < span.Length; i++)
            Write(span.At(i));
    }

    public void Write(uint u)
    {
        Write((byte)u);
        Write((byte)(u >> 8));
        Write((byte)(u >> 16));
        Write((byte)(u >> 24));
    }

    public void Write(ushort u)
    {
        Write((byte)u);
        Write((byte)(u >> 8));
    }

    public void Write(ulong u)
    {
        Write((byte)u);
        Write((byte)(u >> 8));
        Write((byte)(u >> 16));
        Write((byte)(u >> 24));
        Write((byte)(u >> 32));
        Write((byte)(u >> 40));
        Write((byte)(u >> 48));
        Write((byte)(u >> 56));
    }

    public unsafe void Write(double d)
    {
        Write(*(ulong*)&d);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct IfcRecord
    {
        public uint Id;
        public IfcEntity Entity; 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public struct IfcEntity
    {
        public ushort EntityTypeId;
        public IfcGroup Group;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct IfcGroup
    {
        public ushort NumBytes;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public struct IfcInstanceId
    {
        public uint Id; 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct IfcConstant
    {
        public ushort ConstantId;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    public struct IfcNumber
    {
        public double Value;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct IfcString
    {
        public ushort NumBytes;
    }

}