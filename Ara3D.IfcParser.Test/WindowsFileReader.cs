using System.ComponentModel;
using System.Runtime.InteropServices;

public static class WindowsFileReader
{
    private const uint GENERIC_READ = 0x80000000;
    private const uint OPEN_EXISTING = 3;
    private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        nint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        nint hTemplateFile);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadFile(
        nint hFile,
        nint lpBuffer,
        uint nNumberOfBytesToRead,
        out uint lpNumberOfBytesRead,
        nint lpOverlapped);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetFileSizeEx(nint hFile, out long lpFileSize);

    public static unsafe byte[] ReadAllBytes(string filePath, int bufferSize = 4096 * 1024)
    {
        var handle = CreateFile(
            filePath,
            GENERIC_READ,
            0,
            nint.Zero,
            OPEN_EXISTING,
            FILE_ATTRIBUTE_NORMAL,
            nint.Zero);

        if (handle == -1)
            throw new Win32Exception(Marshal.GetLastWin32Error());

        if (!GetFileSizeEx(handle, out var fileSize))
            throw new Win32Exception(Marshal.GetLastWin32Error());

        if (fileSize >= int.MaxValue)
            throw new Exception("File size is too big to load into memory");

        try
        {
            var buffer = new byte[(int)fileSize];

            fixed (byte* pBuffer = &buffer[0])
            {
                var ptr = pBuffer;
                while (true)
                {
                    var readSuccess = ReadFile(handle, (nint)ptr, (uint)bufferSize, out var bytesRead, nint.Zero);
                    if (!readSuccess)
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    if (bytesRead < bufferSize)
                        break;
                    ptr += bytesRead;
                } 
            }

            return buffer;
        }
        finally
        {
            CloseHandle(handle);
        }
    }
}