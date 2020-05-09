using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ultima.Spy.Packets
{
    [UltimaPacket( "Global Chat", UltimaPacketDirection.FromBoth, 0xF9 )]
	public class GlobalChatSystem : UltimaPacket
	{
        [UltimaPacketProperty]
        public int Length { get; set; }

        [UltimaPacketProperty]
        public int Length2 { get; set; }

        [UltimaPacketProperty]
        public string XML { get; set; }

        [UltimaPacketProperty]
        public string From { get; set; }

        [UltimaPacketProperty]
        public string Show { get; set; }

        [UltimaPacketProperty]
        public string To { get; set; }

        [UltimaPacketProperty]
        public string Type { get; set; }

        [UltimaPacketProperty]
        public string Id{ get; set; }

        [UltimaPacketProperty]
        public string Name { get; set; }

        [UltimaPacketProperty]
        public string Version { get; set; }

        static string FindParameter(XDocument document, string element, string attr)
        {
            return document.Elements("ultima_stanza")
                          .Elements(element).Select(x => x.Attribute(attr).Value).FirstOrDefault();
        }

        protected override void Parse( BigEndianReader reader )
		{
            reader.ReadByte(); // ID
            Length = reader.ReadByte();
            Length2 = reader.ReadByte();

            if (Length != 0)
            {
                reader.ReadByte();
                XML = Encoding.ASCII.GetString(reader.ReadBytes(Length - 5));

                XDocument docXML = XDocument.Parse(XML);

                From = FindParameter(docXML, "presence", "from");
                Id = FindParameter(docXML, "presence", "id");
                Name = FindParameter(docXML, "presence", "name");
                Show = FindParameter(docXML, "presence", "show");
                Version = FindParameter(docXML, "presence", "version");
            }
            else // client
            { 

                reader.ReadByte();

                XML = Encoding.ASCII.GetString(reader.ReadBytes(Length2 - 5));

                XDocument docXML = XDocument.Parse(XML);

                From = FindParameter(docXML, "presence", "from");
                Show = FindParameter(docXML, "presence", "show");

                To = FindParameter(docXML, "iq", "to");
                Type = FindParameter(docXML, "iq", "type");
            }
        }
    }
}
