using System;
using System.Collections.Generic;
using System.Text;

namespace Ultima.Spy.Packets
{
    [UltimaPacket("Compressed Gump", UltimaPacketDirection.FromServer, 0xDD)]
    public class CompressedGump : GenericGumpPacket
    {
        int compressedEntriesLength;
        int decompressedEntriesLength;
        byte[] compressedEntries;
        byte[] decompressedEntries;
        string entries;
        int lineCount;
        int compressedStringsLength;
        int decompressedStringsLength;
        byte[] compressedStrings;
        byte[] decompressedStrings;
        string strings;
        int start;

        protected override void Parse(BigEndianReader reader)
        {
            reader.ReadByte(); // ID
            reader.ReadInt16(); // Size

            Serial = reader.ReadUInt32();
            GumpID = reader.ReadInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();

            compressedEntriesLength = reader.ReadInt32() - 4;
            decompressedEntriesLength = reader.ReadInt32();

            compressedEntries = reader.ReadBytes(compressedEntriesLength);
            decompressedEntries = new byte[decompressedEntriesLength];

            if (SystemInfo.IsX64)
                Ultima.Package.Zlib64.Decompress(decompressedEntries, ref decompressedEntriesLength, compressedEntries, compressedEntriesLength);
            else
                Ultima.Package.Zlib32.Decompress(decompressedEntries, ref decompressedEntriesLength, compressedEntries, compressedEntriesLength);

            entries = Encoding.ASCII.GetString(decompressedEntries);

            lineCount = reader.ReadInt32();
            compressedStringsLength = reader.ReadInt32() - 4;
            decompressedStringsLength = reader.ReadInt32();                           

            if (compressedStringsLength >= 0)
            {
                compressedStrings = reader.ReadBytes(compressedStringsLength);
                decompressedStrings = new byte[decompressedStringsLength];

                if (SystemInfo.IsX64)
                    Ultima.Package.Zlib64.Decompress(decompressedStrings, ref decompressedStringsLength, compressedStrings, compressedStringsLength);
                else
                    Ultima.Package.Zlib32.Decompress(decompressedStrings, ref decompressedStringsLength, compressedStrings, compressedStringsLength);

                strings = Encoding.ASCII.GetString(decompressedStrings);

                Text = new List<string>();

                start = 0;

                for (int i = 0; i < strings.Length; i++)
                {
                    if (strings[i] == 0)
                    {
                        if (i - start > 0)
                            Text.Add(strings.Substring(start, i - start));
                        else
                            Text.Add(String.Empty);

                        start = i + 1;
                    }
                }
            }

            Parse(entries);
        }
    }
}
