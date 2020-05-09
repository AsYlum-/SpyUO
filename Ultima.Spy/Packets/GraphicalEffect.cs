using System.Collections.ObjectModel;
using System.IO;

namespace Ultima.Spy.Packets
{
	public enum GraphicalEffectType
	{
		SourceToDestination		= 0x0,
		LightningStrike			= 0x1,
		StayWithDestination		= 0x2,
		StayWithSource			= 0x3,
		SpecialEffect			= 0x4,
	}

	[UltimaPacket( "Graphical Effect", UltimaPacketDirection.FromBoth, 0x70 )]
	public class GraphicalEffectPacket : UltimaPacket, IUltimaEntity
	{		
		[UltimaPacketProperty( "Type", "{0:D} - {0}" )]
		public GraphicalEffectType Type { get; set; }
       
		[UltimaPacketProperty( "Source", "0x{0:X}" )]
		public uint Source { get; set; }

		[UltimaPacketProperty( "Target", "0x{0:X}" )]
		public uint Serial { get; set; }

		[UltimaPacketProperty( "Object ID", "0x{0:X}" )]
		public int ObjectID { get; set; }

		[UltimaPacketProperty( "Source X" )]
		public int SourceX { get; set; }

		[UltimaPacketProperty( "Source X" )]
		public int SourceY { get; set; }

		[UltimaPacketProperty( "Source Z" )]
		public int SourceZ { get; set; }

		[UltimaPacketProperty( "Target X" )]
		public int TargetX { get; set; }

		[UltimaPacketProperty( "Target X" )]
		public int TargetY { get; set; }

		[UltimaPacketProperty( "Target Z" )]
		public int TargetZ { get; set; }

		[UltimaPacketProperty]
		public int Speed { get; set; }

		[UltimaPacketProperty( "Duration (s)" )]
		public int Duration { get; set; }

		[UltimaPacketProperty( "Fixed Direction" )]
		public bool FixedDirection { get; set; }

		[UltimaPacketProperty]
		public bool Explode { get; set; }

        protected override void Parse( BigEndianReader reader )
		{
			reader.ReadByte(); // ID

			Type = (GraphicalEffectType) reader.ReadByte();
			Source = reader.ReadUInt32();
			Serial = reader.ReadUInt32();
			ObjectID = reader.ReadInt16();
			SourceX = reader.ReadInt16();
			SourceY = reader.ReadInt16();
			SourceZ = reader.ReadSByte();
			TargetX = reader.ReadInt16();
			TargetY = reader.ReadInt16();
			TargetZ = reader.ReadSByte();
			Speed = reader.ReadByte();
			Duration = reader.ReadByte();
			reader.ReadInt16();
            reader.ReadInt16();
            FixedDirection = reader.ReadBoolean();
			Explode = reader.ReadBoolean();
		}
	}
}
