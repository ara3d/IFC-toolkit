using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Ara3D.IfcParser.Test;

public static class FastFileReader
{
    public static byte[] ReadAllBytes(string path, int bufferSize = 16 * 1024)
    {
        byte[] bytes;
        using (var fs = new FileStream(path, 
                   FileMode.Open, 
                   FileAccess.Read, 
                   FileShare.Read,
                   bufferSize, 
                   false))
        {
            var index = 0;
            var  fileLength = fs.Length;
            if (fileLength > int.MaxValue)
                throw new IOException("File too big: > 2GB");
            var count = (int)fileLength;
            bytes = new byte[count];
            while (count > 0)
            {
                var n = fs.Read(bytes, index, count);
                if (n == 0)
                    break;
                index += n;
                count -= n;
            }
        }

        return bytes;
    }
}