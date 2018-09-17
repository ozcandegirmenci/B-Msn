using System;
using System.IO;
using System.Drawing;
using System.Reflection;

namespace Bmsn.Protocol
{
    /// <summary>
    /// Provides some helper methods fro writing to the bianry stream
    /// </summary>
    public static class ServerBinaryWriter
    {
        #region Members

        private static FieldInfo _ImageRawDataField = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the raw data field from the Image type
        /// </summary>
        private static FieldInfo ImageRawDataField
        {
            get
            {
                if (_ImageRawDataField == null)
                {
                    Type type = typeof(Image);
                    _ImageRawDataField = type.GetField("rawData",
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.GetField);
                }
                return _ImageRawDataField;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes an image to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteImage(BinaryWriter writer, Image value)
        {
            if (value == null)
            {
                WriteInt32(writer, 0);
            }
            else
            {
                if (ImageRawDataField == null)
                    throw new Exception("RawData field could not found");
                object o = ImageRawDataField.GetValue(value);
                if (o == null)
                {
                    WriteInt32(writer, 0);
                }
                else
                {
                    byte[] data = (byte[])o;
                    WriteInt32(writer, data.Length);
                    writer.Write(data);
                }
            }
        }

        /// <summary>
        /// Writes a tiny string to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteTinyString(BinaryWriter writer, string value)
        {
            byte len = 0;
            if (value == null)
            {
                writer.Write(len);
            }
            else
            {
                byte[] str = System.Text.Encoding.Unicode.GetBytes(value);
                len = (byte)str.Length;
                writer.Write(len);
                writer.Write(str);
            }
        }

        /// <summary>
        /// Writes a short string to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteShortString(BinaryWriter writer, string value)
        {
            short len = 0;
            if (value == null)
            {
                writer.Write(len);
            }
            else
            {
                byte[] str = System.Text.Encoding.Unicode.GetBytes(value);
                len = (short)str.Length;
                writer.Write(len);
                writer.Write(str);
            }
        }

        /// <summary>
        /// Writes given string to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteString(BinaryWriter writer, string value)
        {
            int len = 0;
            if (value == null)
            {
                writer.Write(len);
            }
            else
            {
                byte[] str = System.Text.Encoding.Unicode.GetBytes(value);
                len = str.Length;
                writer.Write(len);
                writer.Write(str);
            }
        }

        /// <summary>
        /// Writes given binary data to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteByteArray(BinaryWriter writer, byte[] value)
        {
            if (value == null)
                WriteInt32(writer, 0);
            else
            {
                WriteInt32(writer, value.Length);
                writer.Write(value);
            }
        }

        /// <summary>
        /// Writes given boolean to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteBoolean(BinaryWriter writer, bool value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given int 16 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteInt16(BinaryWriter writer, short value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given char to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteChar(BinaryWriter writer, char value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given byte to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteByte(BinaryWriter writer, byte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given sbyte to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteSByte(BinaryWriter writer, sbyte value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given uint16 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteUInt16(BinaryWriter writer, ushort value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given int32 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteInt32(BinaryWriter writer, int value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given uint32 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteUInt32(BinaryWriter writer, uint value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given int64 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteInt64(BinaryWriter writer, long value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given uint64 to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteUInt64(BinaryWriter writer, ulong value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given float to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteSingle(BinaryWriter writer, float value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given double to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteDouble(BinaryWriter writer, double value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given decimal to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteDecimal(BinaryWriter writer, decimal value)
        {
            writer.Write(value);
        }

        /// <summary>
        /// Writes given datetime to the stream
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void WriteDateTime(BinaryWriter writer, DateTime value)
        {
            writer.Write(value.ToOADate());
        }

        #endregion
    }
}