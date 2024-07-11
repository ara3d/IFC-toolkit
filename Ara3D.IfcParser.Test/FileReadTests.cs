using Ara3D.Utils;

namespace Ara3D.IfcParser.Test;

public static class FileReadTests
{
    public static unsafe byte[] ReadIfcAndTokenize(FilePath fp, int bufferSize = 4096 * 1024)
    {
        using var fs = new FileStream(fp, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
        var fileLength = fs.Length;
        if (fileLength > int.MaxValue)
            throw new IOException("File too long (> 2GB)");
        var n = (int)fileLength;
        var r = new byte[n];
        fixed (byte* ptr = &r[0])
        {
            ReadIfcAndTokenize(fs, ptr, bufferSize, n);
        }
        return r;
    }

    public static unsafe void ReadIfcAndTokenize(FileStream fs, byte* ptr, int bufferSize, int totalSize)
    {
        var end = ptr + totalSize;
        while (ptr < end)
        {
            var span = new Span<byte>(ptr, bufferSize);
            var n = fs.Read(span);
            if (n == 0)
                return;
            ptr += n;
        } 
    }


}