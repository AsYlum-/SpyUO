using System.Collections.ObjectModel;
using System.IO;

namespace Ultima.Spy.Packets
{
	public enum HuedEffectType
    {
        Moving = 0x00,
        Lightning = 0x01,
        FixedXYZ = 0x02,
        FixedFrom = 0x03
    }

    public enum EffectLayer
    {
        Head = 0,
        RightHand = 1,
        LeftHand = 2,
        Waist = 3,
        LeftFoot = 4,
        RightFoot = 5,
        CenterFeet = 7
    }

    [UltimaPacket( "Hued Effect", UltimaPacketDirection.FromServer, 0xC0)]
	public class HuedEffectPacket : UltimaPacket, IUltimaEntity
	{		
		[UltimaPacketProperty( "Type", "{0:D} - {0}" )]
		public HuedEffectType Type { get; set; }

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

        [UltimaPacketProperty]
        public int Hue { get; set; }

        [UltimaPacketProperty]
        public int RenderMode { get; set; }

        [UltimaPacketProperty]
        public int Effect { get; set; }

        [UltimaPacketProperty]
        public int ExplodeEffect { get; set; }

        [UltimaPacketProperty]
        public int ExplodeSound { get; set; }

        [UltimaPacketProperty("Type", "{0:D} - {0}")]
        public EffectLayer Layer { get; set; }

        protected override void Parse( BigEndianReader reader )
		{
			reader.ReadByte(); // ID

			Type = (HuedEffectType) reader.ReadByte();
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
            Hue = reader.ReadInt16();
            RenderMode = reader.ReadInt16();
            Effect = reader.ReadInt16();
            ExplodeEffect = reader.ReadInt16();
            ExplodeSound = reader.ReadInt16();
            reader.ReadUInt32();
            Layer = (EffectLayer)reader.ReadByte();
        }
	}
}
