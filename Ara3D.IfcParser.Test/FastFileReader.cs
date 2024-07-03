using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Ara3D.IfcParser.Test;

public static class FastFileReader
{
    public static unsafe IntPtr ReadFileToUnmanagedMemory(string filePath, out long fileSize)
    {
        fileSize = new FileInfo(filePath).Length;
        IntPtr unmanagedPointer = Marshal.AllocHGlobal(new IntPtr(fileSize));

        using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
        {
            using (var accessor = mmf.CreateViewAccessor(0, fileSize, MemoryMappedFileAccess.Read))
            {
                byte* pointer = null;
                accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
                try
                {
                    byte* destinationPointer = (byte*)unmanagedPointer.ToPointer();
                    for (long i = 0; i < fileSize; i++)
                    {
                        destinationPointer[i] = pointer[i];
                    }
                }
                finally
                {
                    accessor.SafeMemoryMappedViewHandle.ReleasePointer();
                }
            }
        }

        return unmanagedPointer;
    }

    public static void FreeUnmanagedMemory(IntPtr pointer)
    {
        Marshal.FreeHGlobal(pointer);
    }
}