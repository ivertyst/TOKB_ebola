using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace Bi_31_windows
{
    unsafe class fleshd
    {
        static public string FlashName;
        static public string Start_FlashName;
        static public string SerialNumber;
        public const string Defaulted_FlashName = @"E:";

        enum DesiredAccess : uint
        {
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }

        enum ShareMode : uint
        {
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002,
            FILE_SHARE_DELETE = 0x00000004
        }

        enum CreationDisposition : uint
        {
            CREATE_ALWAYS = 2,
            CREATE_NEW = 1,
            OPEN_ALWAYS = 4,
            OPEN_EXISTING = 3,
            TRUNCATE_EXISTING = 5
        }

        enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTE_ARCHIVE = 0x20,
            FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
            FILE_ATTRIBUTE_HIDDEN = 0x2,
            FILE_ATTRIBUTE_NORMAL = 0x80,
            FILE_ATTRIBUTE_OFFLINE = 0x1000,
            FILE_ATTRIBUTE_READONLY = 0x1,
            FILE_ATTRIBUTE_SYSTEM = 0x4,
            FILE_ATTRIBUTE_TEMPORARY = 0x100
        }

        enum MoveMethod
        {
            FILE_BEGIN = 0,
            FILE_CURRENT = 1,
            FILE_END = 2
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetFilePointerEx(
            IntPtr hFile,              // дескриптор файла
            long liDistanceToMove,     // байты, перемещающие указатель
            ref long lpNewFilePointer, // новый указатель позиции
            uint dwMoveMethod          // точка отсчета
            );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe IntPtr CreateFile
        (
            string FileName,          // file name
            uint DesiredAccess,       // access mode
            uint ShareMode,           // share mode
            uint SecurityAttributes,  // Security Attributes
            uint CreationDisposition, // how to create
            uint FlagsAndAttributes,  // file attributes
            int hTemplateFile         // handle to template file
        );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool ReadFile
        (
            System.IntPtr hFile,      // handle to file
            void* pBuffer,            // data buffer
            int NumberOfBytesToRead,  // number of bytes to read
            int* pNumberOfBytesRead,  // number of bytes read
            int Overlapped            // overlapped buffer
        );

        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool WriteFile
        (
            System.IntPtr hFile,      // handle to file
            void* pBuffer,            // data buffer
            int NumberOfBytesToRead,  // number of bytes to read
            int* pNumberOfBytesRead,  // number of bytes read
            int Overlapped            // overlapped buffer
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            [Out] IntPtr lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            IntPtr lpOverlapped);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hObject">Handle to object</param>
        /// <returns>Result</returns>
        [DllImport("kernel32", SetLastError = true)]
        static extern unsafe bool CloseHandle(IntPtr hObject);

        public static unsafe void WriteString(string diskName, string data)
        {
            string deviceName = string.Format(@"\\.\{0}", diskName);
            IntPtr handle = CreateFile(deviceName, (uint)(DesiredAccess.GENERIC_READ | DesiredAccess.GENERIC_WRITE),
                    (uint)(ShareMode.FILE_SHARE_READ | ShareMode.FILE_SHARE_WRITE),
                    0U, (uint)(CreationDisposition.OPEN_EXISTING), (uint)(FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL), 0);
            try
            {

                if (handle != IntPtr.Zero)
                {
                    long l = 0;
                    bool result = SetFilePointerEx(handle, 512L, ref l, (uint)MoveMethod.FILE_BEGIN);
                    if (result)
                    {
                        byte[] dataString = Encoding.ASCII.GetBytes(data);
                        List<byte> temp = new List<byte>(dataString);
                        temp.AddRange(new byte[512 - dataString.Length]);
                        if (WriteData(handle, temp.ToArray()) == -1)
                        {
                            throw new Exception("Can not write to handle!");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseHandle(handle);
            }
        }

        public static unsafe string ReadString(string diskName)
        {
            string resultString = "";
            string deviceName = string.Format(@"\\.\{0}", diskName);
            IntPtr handle = CreateFile(deviceName, (uint)(DesiredAccess.GENERIC_READ | DesiredAccess.GENERIC_WRITE),
                (uint)(ShareMode.FILE_SHARE_READ | ShareMode.FILE_SHARE_WRITE),
                0U, (uint)(CreationDisposition.OPEN_EXISTING), (uint)(FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL), 0);
            try
            {

                if (handle != IntPtr.Zero)
                {
                    long l = 0;
                    bool result = SetFilePointerEx(handle, 512L, ref l, (uint)MoveMethod.FILE_BEGIN);
                    if (result)
                    {
                        byte[] buffer = new byte[512];
                        if (ReadData(handle, buffer, buffer.Length) == -1)
                        {
                            throw new Exception("Can not read from handle!");
                        }
                        resultString = Encoding.ASCII.GetString(buffer);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                CloseHandle(handle);
            }
            return resultString;
        }

        static unsafe int WriteData(IntPtr handle, byte[] buffer)
        {
            int n = 0;
            fixed (byte* pointer = buffer)
            {
                if (!WriteFile(handle, pointer, buffer.Length, &n, 0))
                {
                    return -1;
                }
            }
            return n;
        }
        static unsafe int ReadData(IntPtr handle, byte[] buffer, int bytesToRead)
        {
            int n = 0;
            fixed (byte* pointer = buffer)
            {
                if (!ReadFile(handle, pointer, bytesToRead, &n, 0))
                {
                    return -1;
                }
            }
            return n;
        }
        /*static unsafe void Main()
        {
            string[] proverka = new string[] { "11", "3", "8", "9", "4", "5", "10", "15", "6", "14", "16", "1", "12", "13", "2", "7" };
            string result = "";
            string data = String.Concat<string>(proverka);
            string diskName = "G:";
            WriteString(diskName, data);
            result = ReadString(diskName);
            Console.WriteLine(result);*/
    }
}
