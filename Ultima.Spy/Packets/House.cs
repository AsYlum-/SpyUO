using System;
using System.Collections.Generic;

namespace Ultima.Spy.Packets
{
    [UltimaPacket("Send Custom House", UltimaPacketDirection.FromServer, 0xD8)]
    public class SendCustomHouse : UltimaPacket
    {
        [UltimaPacketProperty("Compression Type ", "0x{0:X}")]
        public int CompressionType { get; set; }

        [UltimaPacketProperty("Enable Response", "0x{0:X}")]
        public uint EnableResponse { get; set; }

        [UltimaPacketProperty("Serial", "0x{0:X}")]
        public uint Serial { get; set; }

        [UltimaPacketProperty("Revision", "0x{0:X}")]
        public uint Revision { get; set; }

        [UltimaPacketProperty("Tiles Length", "0x{0:X}")]
        public int TileLength { get; set; }

        [UltimaPacketProperty("Buffer Length", "0x{0:X}")]
        public int BufferLength { get; set; }

        [UltimaPacketProperty("Plane Count", "0x{0:X}")]
        public byte PlaneCount { get; set; }
        
        [UltimaPacketProperty]
        public List<Item> Items
        {
            get; set;
        }

        public byte[] ItemData { get; set; }

        public int Index { get; set; }

        public bool IsFloor { get; set; }

        protected override void Parse(BigEndianReader reader)
        {
            reader.ReadByte(); // ID
            reader.ReadInt16(); // Size

            CompressionType = reader.ReadByte();
            EnableResponse = reader.ReadByte();
            Serial = reader.ReadUInt32();
            Revision = reader.ReadUInt32();
            TileLength = reader.ReadInt16();
            BufferLength = reader.ReadInt16();
            PlaneCount = reader.ReadByte();

            Items = new List<Item>();

            for (int i = 0; i < PlaneCount; i++)
            {
                byte[] data = reader.ReadBytes(4);
                Index = data[0];
                int uncompressedsize = data[1] + ((data[3] & 0xF0) << 4);
                int compressedLength = data[2] + ((data[3] & 0xF) << 8);
                ItemData = new byte[uncompressedsize];
                Ultima.Package.Zlib64.Decompress(ItemData, ref uncompressedsize, reader.ReadBytes(compressedLength), compressedLength);
                IsFloor = ((Index & 0x20) == 0x20);
                Index &= 0x1F;

                int numTiles = ItemData.Length >> 1;

                if (i == PlaneCount - 1)
                {
                    int index = 0;
                    numTiles = ItemData.Length / 5;
                    for (int j = 0; j < numTiles; j++)
                    {
                        Items.Add(new Item(ItemData, index));
                    }
                }
            }
        }
    }

    public class Item
    {
        [UltimaPacketProperty("Item ID", UltimaPacketPropertyType.Texture)]
        public int ObjectID { get; set; }

        [UltimaPacketProperty]
        public int X { get; set; }

        [UltimaPacketProperty]
        public int Y { get; set; }

        [UltimaPacketProperty]
        public int Z { get; set; }

        public Item(byte[] ItemData, int index)
        {
            ObjectID = (short)((ItemData[index++] << 8) + ItemData[index++]);
            X = (sbyte)ItemData[index++];
            Y = (sbyte)ItemData[index++];
            Z = (sbyte)ItemData[index++];
        }

        public override string ToString()
        {
            return String.Format("ItemID: {0}", ObjectID);
        }
    }
}
