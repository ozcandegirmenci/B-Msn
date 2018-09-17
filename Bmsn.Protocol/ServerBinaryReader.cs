using System;
using System.Drawing;
using System.IO;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Contains helper methods for writing to binary stream
    /// </summary>
    public static class ServerBinaryReader
    {
        #region Public Methods

        /// <summary>
        /// Reads an image from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Image ReadImage(BinaryReader reader)
        {
            Image img = null;
            int len = ReadInt32(reader);
            if (len != 0)
            {
                byte[] temp = reader.ReadBytes(len);
                MemoryStream str = new MemoryStream(temp);
                img = Image.FromStream(str);
            }
            return img;
        }

        /// <summary>
        /// Reads a byte[] from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte[] ReadByteArray(BinaryReader reader)
        {
            int len = ReadInt32(reader);
            if (len == 0)
                return null;
            return reader.ReadBytes(len);
        }

        /// <summary>
        /// Reads a short string from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadShortString(BinaryReader reader)
        {
            short len = reader.ReadInt16();
            if (len == 0)
                return string.Empty;
            return System.Text.Encoding.Unicode.GetString(reader.ReadBytes(len));
        }

        /// <summary>
        /// Reads a string from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadString(BinaryReader reader)
        {
            int len = reader.ReadInt32();
            if (len == 0)
                return string.Empty;
            return System.Text.Encoding.Unicode.GetString(reader.ReadBytes(len));
        }

        /// <summary>
        /// Reads a tiny string from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static string ReadTinyString(BinaryReader reader)
        {
            byte len = reader.ReadByte();
            if (len == 0)
                return string.Empty;
            return System.Text.Encoding.Unicode.GetString(reader.ReadBytes((int)len));
        }

        /// <summary>
        /// Reads a boolean from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static bool ReadBoolean(BinaryReader reader)
        {
            return reader.ReadBoolean();
        }

        /// <summary>
        /// Reads a byte from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static byte ReadByte(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        /// <summary>
        /// Reads a sbyte from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static sbyte ReadSByte(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        /// <summary>
        /// Reads a char from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static char ReadChar(BinaryReader reader)
        {
            return reader.ReadChar();
        }

        /// <summary>
        /// Reads a short from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static short ReadInt16(BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        /// <summary>
        /// Reads a ushort from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ushort ReadUInt16(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        /// <summary>
        /// Reads a int32 from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static int ReadInt32(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        /// <summary>
        /// Reads uint from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static uint ReadUInt32(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        /// <summary>
        /// Reads long from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static long ReadInt64(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        /// <summary>
        /// Reads ulong from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static ulong ReadUInt64(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        /// <summary>
        /// Reads single from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static float ReadSingle(BinaryReader reader)
        {
            return reader.ReadSingle();
        }

        /// <summary>
        /// Reads a decimal from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static decimal ReadDecimal(BinaryReader reader)
        {
            return reader.ReadDecimal();
        }

        /// <summary>
        /// Reads a double from stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static double ReadDouble(BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        /// <summary>
        /// Reads date time from the stream
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DateTime ReadDateTime(BinaryReader reader)
        {
            return DateTime.FromOADate(reader.ReadDouble());
        }

        #endregion
    }
}