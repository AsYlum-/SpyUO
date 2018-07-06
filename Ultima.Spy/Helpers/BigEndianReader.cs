using System;
using System.IO;
using System.Text;

namespace Ultima.Spy
{
    /// <summary>
    /// Describes big endian binary reader.
    /// </summary>
    public class BigEndianReader
    {
        #region Properties

        /// <summary>
        /// Gets input stream.
        /// </summary>
        public Stream Input { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of BigEndianReader.
        /// </summary>
        /// <param name="input">Input stream.</param>
        public BigEndianReader(Stream input)
        {
            Input = input;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Reads 32 bit unsigned integer.
        /// </summary>
        /// <returns>32 bit unsigned integer.</returns>
        public uint ReadUInt32()
        {
            return (uint)((Input.ReadByte() << 24) |
                (Input.ReadByte() << 16) |
                (Input.ReadByte() << 8) |
                Input.ReadByte());
        }
        /// <summary>
        /// Reads 32 bit integer.
        /// </summary>
        /// <returns>32 bit integer.</returns>
        public int ReadInt32()
        {
            return ((Input.ReadByte() << 24) |
                (Input.ReadByte() << 16) |
                (Input.ReadByte() << 8) |
                Input.ReadByte());
        }

        /// <summary>
        /// Reads 16 bit unsigned integer.
        /// </summary>
        /// <returns>16 bit unsigned integer.</returns>
        public ushort ReadUInt16()
        {
            return (ushort)((Input.ReadByte() << 8) | Input.ReadByte());
        }

        /// <summary>
        /// Reads 16 bit integer.
        /// </summary>
        /// <returns>16 bit integer.</returns>
        public short ReadInt16()
        {
            return (short)((Input.ReadByte() << 8) | Input.ReadByte());
        }

        /// <summary>
        /// Reads byte.
        /// </summary>
        /// <returns>Byte.</returns>
        public byte ReadByte()
        {
            return (byte)Input.ReadByte();
        }

        /// <summary>
        /// Reads signed byte.
        /// </summary>
        /// <returns>Signed Byte.</returns>
        public sbyte ReadSByte()
        {
            return (sbyte)Input.ReadByte();
        }

        /// <summary>
        /// Read boolean.
        /// </summary>
        /// <returns>Boolean.</returns>
        public bool ReadBoolean()
        {
            if (Input.ReadByte() == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Reads array of bytes.
        /// </summary>
        /// <param name="length">Number of bytes to read.</param>
        /// <returns>Array of bytes.</returns>
        public byte[] ReadBytes(int length)
        {
            byte[] data = new byte[length];

            Input.Read(data, 0, length);

            return data;
        }

        /// <summary>
        /// Reads unicode string.
        /// </summary>
        /// <returns>String.</returns>
        public string ReadUnicodeString()
        {
            int length = ReadInt16();
            byte[] data = new byte[length];

            Input.Read(data, 0, length);

            return Encoding.Unicode.GetString(data);
        }

        /// <summary>
        /// Reads unicode string.
        /// </summary>
        /// <param name="length">Length in characters.</param>
        /// <returns>String.</returns>
        public string ReadUnicodeString(int length)
        {
            if (length == 0)
                return String.Empty;

            int size = length * 2;
            byte[] data = new byte[size];

            Input.Read(data, 0, size);

            return Encoding.Unicode.GetString(data);
        }

        /// <summary>
        /// Reads ascii string.
        /// </summary>
        /// <returns>String.</returns>
        public string ReadAsciiString()
        {
            int length = ReadInt16();
            byte[] data = new byte[length];

            Input.Read(data, 0, length);

            string str = Encoding.ASCII.GetString(data);
            return str.Substring(0, str.IndexOf('\0'));
        }

        /// <summary>
        /// Reads ascii string.
        /// </summary>
        /// <param name="length">Length in characters.</param>
        /// <returns>String.</returns>
        public string ReadAsciiString(int length)
        {
            if (length == 0)
                return String.Empty;

            int size = length;
            byte[] data = new byte[size];

            Input.Read(data, 0, size);

            string str = Encoding.ASCII.GetString(data);
            return str.Substring(0, str.IndexOf('\0'));
        }
        #endregion
    }
}
