using System;
using System.Collections.Generic;
using System.IO;

namespace Ultima.Spy.Packets
{
    [UltimaPacket("Generic Gump", UltimaPacketDirection.FromServer, 0xB0)]
    public class GenericGumpPacket : UltimaPacket, IUltimaEntity
    {
        [UltimaPacketProperty("Serial", "0x{0:X}")]
        public uint Serial { get; set; }

        [UltimaPacketProperty("Gump ID", "0x{0:X}")]
        public int GumpID { get; set; }

        [UltimaPacketProperty]
        public int X { get; set; }

        [UltimaPacketProperty]
        public int Y { get; set; }

        [UltimaPacketProperty]
        public List<GumpEntry> Entries { get; set; }

        [UltimaPacketProperty]
        public List<string> Text { get; set; }

        protected override void Parse(BigEndianReader reader)
        {
            reader.ReadByte(); // ID
            reader.ReadInt16(); // Size

            Serial = reader.ReadUInt32();
            GumpID = reader.ReadInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();

            string layout = reader.ReadAsciiString();

            int textLength = reader.ReadInt16();
            Text = new List<string>(textLength);

            for (int i = 0; i < textLength; i++)
            {
                Text.Add(reader.ReadAsciiString());
            }

            Parse(layout);
        }

        protected void Parse(string layout)
        {
            Entries = new List<GumpEntry>();
            layout = layout.Replace("}", "");
            string[] splt = layout.Substring(1).Split('{');

            foreach (string s in splt)
            {
                try
                {
                    string[] commands = SplitCommands(s);

                    GumpEntry entry = GumpEntry.Create(commands, this);

                    if (entry != null)
                        Entries.Add(entry);
                }
                catch { }
            }
        }

        private static string[] SplitCommands(string s)
        {
            s = s.Trim();

            List<string> ret = new List<string>();
            bool stringCmd = false;
            string command;
            int start = 0;

            for (int i = 0; i < s.Length; i++)
            {
                char ch = s[i];

                if (ch == ' ' || ch == '\t' || ch == ',')
                {
                    if (!stringCmd)
                    {
                        command = s.Substring(start, i - start);

                        if (!String.IsNullOrEmpty(command))
                            ret.Add(command);

                        start = i + 1;
                    }
                }
                else if (ch == '@')
                {
                    stringCmd = !stringCmd;
                }
            }

            command = s.Substring(start, s.Length - start);

            if (!String.IsNullOrEmpty(command))
                ret.Add(command);

            /*var k = string.Join(" , ", ret);

            System.IO.File.AppendAllText("asd.txt", k + Environment.NewLine);*/
            

            return ret.ToArray();
        }
    }

    public abstract class GumpEntry
    {
        public static GumpEntry Create(string[] commands, GenericGumpPacket parent)
        {
            string command = commands[0].ToLower();

            if (command.StartsWith("kr_"))
                command = command.Substring(3, command.Length - 3);
            
            switch (command)
            {
                case "nomove":
                    return new GumpNotDragable(commands, parent);
                case "noclose":
                    return new GumpNotClosable(commands, parent);
                case "nodispose":
                    return new GumpNotDisposable(commands, parent);
                case "noresize":
                    return new GumpNotResizable(commands, parent);
                case "checkertrans":
                    return new GumpAlphaRegion(commands, parent);
                case "resizepic":
                    return new GumpBackground(commands, parent);
                case "button":
                    return new GumpButton(commands, parent);
                case "checkbox":
                    return new GumpCheck(commands, parent);
                case "group":
                    return new GumpGroup(commands, parent);
                case "htmlgump":
                    return new GumpHtml(commands, parent);
                case "xmfhtmlgump":
                    return new GumpHtmlLocalized(commands, parent);
                case "xmfhtmlgumpcolor":
                    return new GumpHtmlLocalizedColor(commands, parent);
                case "xmfhtmltok":
                    return new GumpHtmlLocalizedArgs(commands, parent);
                case "gumppic":
                    return new GumpImage(commands, parent);
                case "gumppictiled":
                    return new GumpImageTiled(commands, parent);
                case "buttontileart":
                    return new GumpImageTiledButton(commands, parent);
                case "tilepic":
                    return new GumpItem(commands, parent);
                case "tilepichue":
                    return new GumpItemColor(commands, parent);
                case "text":
                    return new GumpLabel(commands, parent);
                case "croppedtext":
                    return new GumpLabelCropped(commands, parent);
                case "page":
                    return new GumpPage(commands, parent);
                case "radio":
                    return new GumpRadio(commands, parent);
                case "textentry":
                    return new GumpTextEntry(commands, parent);
                case "tooltip":
                    return new GumpTooltip(commands, parent);
                case "mastergump":
                    return null;
                case "itemproperty":
                    return new GumpItemProperty(commands, parent);
                case "echandleinput":
                    return new ECHandleInput(commands, parent);
                case "textentrylimited":
                    return new GumpTextEntryLimited(commands, parent);

                default:
                    throw new ArgumentException();
            }
        }

        [UltimaPacketProperty]
        public string[] Commands { get; set; }

        public GenericGumpPacket Parent { get; set; }

        public GumpEntry(string[] commands, GenericGumpPacket parent)
        {
            Commands = commands;
            Parent = parent;
        }

        public int GetInt32(int n)
        {
            return Int32.Parse(Commands[n]);
        }

        public uint GetUInt32(int n)
        {
            return UInt32.Parse(Commands[n]);
        }

        public bool GetBoolean(int n)
        {
            return GetInt32(n) != 0;
        }

        public string GetString(int n)
        {
            string cmd = Commands[n];
            return cmd.Substring(1, cmd.Length - 2);
        }

        public string GetText(int n)
        {
            return Parent.Text[n];
        }

        public static string Format(bool b)
        {
            return b ? "true" : "false";
        }

        public static string Format(string s)
        {
            return s.Replace("\t", "\\t");
        }

        public abstract string GetRunUOLine();
    }

    public class GumpNotDragable : GumpEntry
    {
        public GumpNotDragable(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
        }

        public override string GetRunUOLine()
        {
            return "Dragable = false;";
        }

        public override string ToString()
        {
            return "Not Dragable";
        }
    }

    public class GumpNotClosable : GumpEntry
    {
        public GumpNotClosable(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
        }

        public override string GetRunUOLine()
        {
            return "Closable = false;";
        }

        public override string ToString()
        {
            return "Not Closable";
        }
    }

    public class GumpNotDisposable : GumpEntry
    {
        public GumpNotDisposable(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
        }

        public override string GetRunUOLine()
        {
            return "Disposable = false;";
        }

        public override string ToString()
        {
            return "Not Disposable";
        }
    }

    public class GumpNotResizable : GumpEntry
    {
        public GumpNotResizable(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
        }

        public override string GetRunUOLine()
        {
            return "Resizable = false;";
        }

        public override string ToString()
        {
            return "Not Resizable";
        }
    }

    public class GumpAlphaRegion : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public GumpAlphaRegion(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddAlphaRegion( {0}, {1}, {2}, {3} );", X, Y, Width, Height);
        }

        public override string ToString()
        {
            return string.Format("Alpha Region: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\"",
                X, Y, Width, Height);
        }
    }

    public class GumpBackground : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int GumpId { get; set; }

        public GumpBackground(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            GumpId = GetInt32(3);
            Width = GetInt32(4);
            Height = GetInt32(5);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddBackground( {0}, {1}, {2}, {3}, 0x{4:X} );", X, Y, Width, Height, GumpId);
        }

        public override string ToString()
        {
            return string.Format("Background: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", GumpId: \"0x{4:X}\"",
                X, Y, Width, Height, GumpId);
        }
    }

    public class GumpButton : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int NormalId { get; set; }
        public int PressedId { get; set; }
        public int ButtonId { get; set; }
        public int Type { get; set; }
        public int Param { get; set; }

        public GumpButton(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            NormalId = GetInt32(3);
            PressedId = GetInt32(4);
            Type = GetInt32(5);
            Param = GetInt32(6);
            ButtonId = GetInt32(7);
        }

        public override string GetRunUOLine()
        {
            string type = Type == 0 ? "GumpButtonType.Page" : "GumpButtonType.Reply";
            return string.Format("AddButton( {0}, {1}, 0x{2:X}, 0x{3:X}, {4}, {5}, {6} );",
                X, Y, NormalId, PressedId, ButtonId, type, Param);
        }

        public override string ToString()
        {
            return string.Format("Button: \"X: \"{0}\", Y: \"{1}\", NormalId: \"0x{2:X}\", PressedId: \"0x{3:X}\", ButtonId: \"{4}\", Type: \"{5}\", Param: \"{6}\"",
                X, Y, NormalId, PressedId, ButtonId, Type, Param);
        }
    }

    public class GumpCheck : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int InactiveId { get; set; }
        public int ActiveId { get; set; }
        public bool InitialState { get; set; }
        public int SwitchId { get; set; }

        public GumpCheck(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            InactiveId = GetInt32(3);
            ActiveId = GetInt32(4);
            InitialState = GetBoolean(5);
            SwitchId = GetInt32(6);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddCheck( {0}, {1}, 0x{2:X}, 0x{3:X}, {4}, {5} );",
                X, Y, InactiveId, ActiveId, Format(InitialState), SwitchId);
        }

        public override string ToString()
        {
            return string.Format("Check: X: \"{0}\", Y: \"{1}\", InactiveId: \"0x{2:X}\", ActiveId: \"0x{3:X}\", InitialState: \"{4}\", SwitchId: \"{5}\"",
                X, Y, InactiveId, ActiveId, InitialState, SwitchId);
        }
    }

    public class GumpGroup : GumpEntry
    {
        public int Group { get; set; }

        public GumpGroup(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            Group = GetInt32(1);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddGroup( {0} );", Group);
        }

        public override string ToString()
        {
            return string.Format("Group: \"{0}\"", Group);
        }
    }

    public class GumpHtml : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Text { get; set; }
        public bool Background { get; set; }
        public bool Scrollbar { get; set; }

        public GumpHtml(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Text = GetText(GetInt32(5));
            Background = GetBoolean(6);
            Scrollbar = GetBoolean(7);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddHtml( {0}, {1}, {2}, {3}, \"{4}\", {5}, {6} );",
                X, Y, Width, Height, Format(Text), Format(Background), Format(Scrollbar));
        }

        public override string ToString()
        {
            return string.Format("Html: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Text: \"{4}\", Background: \"{5}\", Scrollbar: \"{6}\"",
                X, Y, Width, Height, Text, Background, Scrollbar);
        }
    }

    public class GumpHtmlLocalized : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public uint Number { get; set; }
        public bool Background { get; set; }
        public bool Scrollbar { get; set; }

        public GumpHtmlLocalized(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Number = GetUInt32(5);

            if (commands.Length < 8)
            {
                Background = false;
                Scrollbar = false;
            }
            else
            {
                Background = GetBoolean(6);
                Scrollbar = GetBoolean(7);
            }
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddHtmlLocalized( {0}, {1}, {2}, {3}, {4}, {5}, {6} );",
                X, Y, Width, Height, Number, Format(Background), Format(Scrollbar));
        }

        public override string ToString()
        {
            return string.Format("HtmlLocalized: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Number: \"{4}\", Background: \"{5}\", Scrollbar: \"{6}\"",
                X, Y, Width, Height, Number, Background, Scrollbar);
        }
    }

    public class GumpHtmlLocalizedColor : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public uint Number { get; set; }
        public int Color { get; set; }
        public bool Background { get; set; }
        public bool Scrollbar { get; set; }

        public GumpHtmlLocalizedColor(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Number = GetUInt32(5);
            Background = GetBoolean(6);
            Scrollbar = GetBoolean(7);
            Color = GetInt32(8);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddHtmlLocalized( {0}, {1}, {2}, {3}, {4}, 0x{5:X}, {6}, {7} );",
                X, Y, Width, Height, Number, Color, Format(Background), Format(Scrollbar));
        }

        public override string ToString()
        {
            return string.Format("HtmlLocalizedColor: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Number: \"{4}\", Color: \"0x{5:X}\", \"Background: \"{6}\", Scrollbar: \"{7}\"",
                X, Y, Width, Height, Number, Color, Background, Scrollbar);
        }
    }

    public class GumpHtmlLocalizedArgs : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public uint Number { get; set; }
        public string Args { get; set; }
        public int Color { get; set; }
        public bool Background { get; set; }
        public bool Scrollbar { get; set; }

        public GumpHtmlLocalizedArgs(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Background = GetBoolean(5);
            Scrollbar = GetBoolean(6);
            Color = GetInt32(7);
            Number = GetUInt32(8);
            Args = GetString(9);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddHtmlLocalized( {0}, {1}, {2}, {3}, {4}, \"{5}\", 0x{6:X}, {7}, {8} );",
                X, Y, Width, Height, Number, Format(Args), Color, Format(Background), Format(Scrollbar));
        }

        public override string ToString()
        {
            return string.Format("HtmlLocalizedArgs: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Number: \"{4}\", Args: \"{5}\", Color: \"0x{6:X}\", \"Background: \"{7}\", Scrollbar: \"{8}\"",
                X, Y, Width, Height, Number, Args, Color, Background, Scrollbar);
        }
    }

    public class GumpImage : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int GumpId { get; set; }
        public int Color { get; set; }

        public GumpImage(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            GumpId = GetInt32(3);

            if (commands.Length > 4)
                Color = Int32.Parse(commands[4].Substring(4));
            else
                Color = 0;
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddImage( {0}, {1}, 0x{2:X}{3} );",
                X, Y, GumpId, Color != 0 ? ", 0x" + Color.ToString("X") : "");
        }

        public override string ToString()
        {
            return string.Format("Image: \"X: \"{0}\", Y: \"{1}\", GumpId: \"0x{2:X}\", Color: \"0x{3:X}\"",
                X, Y, GumpId, Color);
        }
    }

    public class GumpImageTiled : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int GumpId { get; set; }

        public GumpImageTiled(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            GumpId = GetInt32(5);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddImageTiled( {0}, {1}, {2}, {3}, 0x{4:X} );",
                X, Y, Width, Height, GumpId);
        }

        public override string ToString()
        {
            return string.Format("ImageTiled: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", GumpId: \"0x{4:X}\"",
                X, Y, Width, Height, GumpId);
        }
    }

    public class GumpImageTiledButton : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int NormalID { get; set; }
        public int PressedID { get; set; }
        public int ButtonID { get; set; }
        public int Type { get; set; }
        public int Param { get; set; }

        public int ItemID { get; set; }
        public int Hue { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public GumpImageTiledButton(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            NormalID = GetInt32(3);
            PressedID = GetInt32(4);
            Type = GetInt32(5);
            Param = GetInt32(6);
            ButtonID = GetInt32(7);
            ItemID = GetInt32(8);
            Hue = GetInt32(9);
            Width = GetInt32(10);
            Height = GetInt32(11);
        }

        public override string GetRunUOLine()
        {
            string type = (Type == 0 ? "GumpButtonType.Page" : "GumpButtonType.Reply");
            return String.Format("AddImageTiledButton( {0}, {1}, 0x{2:X}, 0x{3:X}, 0x{4:X}, {5}, {6}, 0x{7:X}, 0x{8:X}, {9}, {10} );",
                X, Y, NormalID, PressedID, ButtonID, type, Param, ItemID, Hue, Width, Height);
        }

        public override string ToString()
        {
            return string.Format("ImageTiledButton: \"X: \"{0}\", Y: \"{1}\", Id1: \"0x{2:X}\", Id2: \"0x{3:X}\", ButtonId: \"0x{4:X}\", Type: \"{5}\", Param: \"{6}\", ItemId: \"0x{7:X}\", Hue: \"0x{8:X}\", Width: \"{9}\", Height: \"{10}\"",
                X, Y, NormalID, PressedID, ButtonID, Type, Param, ItemID, Hue, Width, Height);
        }
    }

    public class GumpItem : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int GumpId { get; set; }

        public GumpItem(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            GumpId = GetInt32(3);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddItem( {0}, {1}, 0x{2:X} );", X, Y, GumpId);
        }

        public override string ToString()
        {
            return string.Format("Item: \"X: \"{0}\", Y: \"{1}\", GumpId: \"0x{2:X}\"",
                X, Y, GumpId);
        }
    }

    public class GumpItemColor : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int GumpId { get; set; }
        public int Color { get; set; }

        public GumpItemColor(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            GumpId = GetInt32(3);
            Color = GetInt32(4);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddItem( {0}, {1}, 0x{2:X}, 0x{3:X} );",
                X, Y, GumpId, Color);
        }

        public override string ToString()
        {
            return string.Format("ItemColor: \"X: \"{0}\", Y: \"{1}\", GumpId: \"0x{2:X}\", Color: \"0x{3:X}\"",
                X, Y, GumpId, Color);
        }
    }

    public class GumpLabel : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Color { get; set; }
        public string Text { get; set; }

        public GumpLabel(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Color = GetInt32(3);
            Text = GetText(GetInt32(4));
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddLabel( {0}, {1}, 0x{2:X}, \"{3}\" );",
                X, Y, Color, Format(Text));
        }

        public override string ToString()
        {
            return string.Format("Label: \"X: \"{0}\", Y: \"{1}\", Color: \"0x{2:X}\", Text: \"{3}\"",
                X, Y, Color, Text);
        }
    }

    public class GumpLabelCropped : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Color { get; set; }
        public string Text { get; set; }

        public GumpLabelCropped(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Color = GetInt32(5);
            Text = GetText(GetInt32(6));
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddLabelCropped( {0}, {1}, {2}, {3}, 0x{4:X}, \"{5}\" );",
                X, Y, Width, Height, Color, Format(Text));
        }

        public override string ToString()
        {
            return string.Format("LabelCropped: \"X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Color: \"0x{4:X}\", Text: \"{5}\"",
                X, Y, Width, Height, Color, Text);
        }
    }

    public class GumpPage : GumpEntry
    {
        public int Page { get; set; }

        public GumpPage(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            Page = GetInt32(1);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddPage( {0} );", Page);
        }

        public override string ToString()
        {
            return string.Format("Page: \"{0}\"", Page);
        }
    }

    public class GumpRadio : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int InactiveId { get; set; }
        public int ActiveId { get; set; }
        public bool InitialState { get; set; }
        public int SwitchId { get; set; }

        public GumpRadio(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            InactiveId = GetInt32(3);
            ActiveId = GetInt32(4);
            InitialState = GetBoolean(5);
            SwitchId = GetInt32(6);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddRadio( {0}, {1}, 0x{2:X}, 0x{3:X}, {4}, {5} );",
                X, Y, InactiveId, ActiveId, Format(InitialState), SwitchId);
        }

        public override string ToString()
        {
            return string.Format("Radio: X: \"{0}\", Y: \"{1}\", InactiveId: \"0x{2:X}\", ActiveId: \"0x{3:X}\", InitialState: \"{4}\", SwitchId: \"{5}\"",
                X, Y, InactiveId, ActiveId, InitialState, SwitchId);
        }
    }

    public class GumpTextEntry : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Color { get; set; }
        public int EntryId { get; set; }
        public string InitialText { get; set; }

        public GumpTextEntry(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Color = GetInt32(5);
            EntryId = GetInt32(6);
            InitialText = GetText(GetInt32(7));
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddTextEntry( {0}, {1}, {2}, {3}, 0x{4:X}, {5}, \"{6}\" );",
                X, Y, Width, Height, Color, EntryId, Format(InitialText));
        }

        public override string ToString()
        {
            return string.Format("TextEntry: X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Color: \"0x{4:X}\", EntryId: \"{5}\", Text: \"{6}\"",
                X, Y, Width, Height, Color, EntryId, InitialText);
        }
    }

    public class GumpTooltip : GumpEntry
    {
        public int Number { get; set; }

        public GumpTooltip(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            Number = GetInt32(1);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddTooltip( {0} );", Number);
        }

        public override string ToString()
        {
            return string.Format("Tooltip: Number: \"{0}\"", Number);
        }
    }

    public class GumpItemProperty : GumpEntry
    {
        public uint Serial { get; set; }

        public GumpItemProperty(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            Serial = GetUInt32(1);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddItemProperty( 0x{0:X} );",
                Serial);
        }

        public override string ToString()
        {
            return string.Format("itemproperty: \"Serial: \"0x{0:X}\"",
                Serial);
        }
    }

    public class ECHandleInput : GumpEntry
    {
        public ECHandleInput(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddECHandleInput();");
        }

        public override string ToString()
        {
            return string.Format("ECHandleInput");
        }
    }

    public class GumpTextEntryLimited : GumpEntry
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Color { get; set; }
        public int EntryId { get; set; }
        public string InitialText { get; set; }
        public int Textlen { get; set; }

        public GumpTextEntryLimited(string[] commands, GenericGumpPacket parent)
            : base(commands, parent)
        {
            X = GetInt32(1);
            Y = GetInt32(2);
            Width = GetInt32(3);
            Height = GetInt32(4);
            Color = GetInt32(5);
            EntryId = GetInt32(6);
            InitialText = GetText(GetInt32(7));
            Textlen = GetInt32(1);
        }

        public override string GetRunUOLine()
        {
            return string.Format("AddTextEntry( {0}, {1}, {2}, {3}, 0x{4:X}, {5}, {6}, \"{7}\" );",
                X, Y, Width, Height, Color, EntryId, Textlen, Format(InitialText));
        }

        public override string ToString()
        {
            return string.Format("TextEntryLimited: X: \"{0}\", Y: \"{1}\", Width: \"{2}\", Height: \"{3}\", Color: \"0x{4:X}\", EntryId: \"{5}\", Textlen: \"{6}\", Text: \"{7}\"",
                X, Y, Width, Height, Color, EntryId, Textlen, InitialText);
        }
    }
}
