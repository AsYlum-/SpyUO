using System.IO;

namespace Ultima.Package
{
    public enum FileCompression
    {
        /// <summary>
        /// No compression
        /// </summary>
        None,

        /// <summary>
        /// ZLIB compression.
        /// </summary>
        Zlib,
    }

    public class UltimaPackageFile
    {
        #region Properties
        /// <summary>
        /// File header size.
        /// </summary>
        public const int FileHeaderSize = 34;

        /// <summary>
        /// Gets package that owns this file.
        /// </summary>
        public UltimaPackage Package { get; set; }

        /// <summary>
        /// Gets or sets file address.
        /// </summary>
        public long FileAddress { get; set; }

        /// <summary>
        /// Gets or sets compression type.
        /// </summary>
        public FileCompression Compression { get; set; }

        /// <summary>
        /// Gets compressed size.
        /// </summary>
        public int CompressedSize { get; set; }

        /// <summary>
        /// Gets decompressed size.
        /// </summary>
        public int DecompressedSize { get; set; }

        /// <summary>
        /// Gets file name hash.
        /// </summary>
        public ulong FileNameHash { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new instance of UltimaPackageFile.
        /// </summary>
        /// <param name="package">Pacakge that contains this file.</param>
        /// <param name="reader">Reader to read from.</param>
        public UltimaPackageFile(UltimaPackage package, BinaryReader reader)
        {
            Package = package;
            FileAddress = reader.ReadInt64();
            FileAddress += reader.ReadInt32();
            CompressedSize = reader.ReadInt32();
            DecompressedSize = reader.ReadInt32();
            FileNameHash = reader.ReadUInt64();

            reader.ReadInt32(); // Header hash

            switch (reader.ReadInt16())
            {
                case 0: Compression = FileCompression.None; break;
                case 1: Compression = FileCompression.Zlib; break;
            }
        }
        #endregion
    }
}
