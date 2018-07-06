using System;
using System.Collections.Generic;

namespace Ultima.Spy.Packets
{
    [UltimaPacket("Gump Response", UltimaPacketDirection.FromClient, 0xB1)]
    public class GumpResponsePacket : UltimaPacket, IUltimaEntity
    {
        [UltimaPacketProperty("Serial", "0x{0:X}")]
        public uint Serial { get; set; }

        [UltimaPacketProperty("Gump ID", "0x{0:X}")]
        public int GumpID { get; set; }

        [UltimaPacketProperty("Button ID")]
        public int ButtonID { get; set; }

        [UltimaPacketProperty]
        public List<int> Switches { get; set; }

        [UltimaPacketProperty("Text Entries")]
        public List<GumpResponseTextEntry> TextEntries { get; set; }

        protected override void Parse(BigEndianReader reader)
        {
            reader.ReadByte(); // ID
            reader.ReadInt16(); // Size

            Serial = reader.ReadUInt32();
            GumpID = reader.ReadInt32();
            ButtonID = reader.ReadInt32();

            int switchCount = reader.ReadInt32();
            Switches = new List<int>(switchCount);

            for (int i = 0; i < switchCount; i++)
                Switches.Add(reader.ReadInt32());

            int entryCount = reader.ReadInt32();
            TextEntries = new List<GumpResponseTextEntry>(entryCount);

            for (int i = 0; i < entryCount; i++)
                TextEntries.Add(new GumpResponseTextEntry(reader));
        }
    }

    public class GumpResponseTextEntry
    {
        [UltimaPacketProperty("Text Entry ID")]
        public int EntryID { get; set; }

        [UltimaPacketProperty("Text")]
        public string Text { get; set; }

        public GumpResponseTextEntry(BigEndianReader reader)
        {
            EntryID = reader.ReadInt16();
            Text = reader.ReadUnicodeString();
        }

        public override string ToString()
        {
            return String.Format("{0}", EntryID);
        }
    }
}
