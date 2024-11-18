using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using cotf.World;
using cotf.World.Traps;
using cotf.Collections.Unused;
using Microsoft.Xna.Framework;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Color = System.Drawing.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Background = cotf.World.Background;
using Rectangle = System.Drawing.Rectangle;
using CirclePrefect.Dotnet;
using static cotf.NPC.Factory;

namespace cotf.Base
{
    public enum SaveType : byte
    {
        None = 0,
        Player = 1,
        Map = 2,
        World = 3
    }
    public sealed class TagCompound : IDisposable
    {
        public TagCompound(Entity subject)
        {
            this.subject = subject;
        }
        public TagCompound(Entity subject, SaveType type)
        {
            string name = string.Empty;
            this.subject = subject;
            this.type = type;
            if (type == SaveType.Player)
                name = "LocalPlayer";
            else name = subject.Name.Replace(".dat", "");
            Init(name);
        }
        private static string 
            psPath, 
            msPath;
        private string fileName;
        private Entity subject;
        private SaveType type;
        private FileStream file;
        private BinaryReader br;
        private BinaryWriter bw;
        private string content;
        internal static bool Exists(SaveType type, string name)
        {
            switch (type)
            {
                case SaveType.Player:
                    return File.Exists(Path.Combine(psPath, name));
                case SaveType.World:
                    return false;
                default:
                    return false;
            }
        }
        internal static void SetPaths(string playerSavePath, string mapSavePath)   //  Called in Game.Initialize
        {
            psPath = playerSavePath;
            msPath = mapSavePath;
            if (!Directory.Exists(psPath))
            {
                Directory.CreateDirectory(psPath);
            }
            if (!Directory.Exists(msPath))
            {
                Directory.CreateDirectory(msPath);
            }
        }
        private void Init(string name)
        {
            switch (type)
            {
                default:
                case SaveType.None:
                    break;
                case SaveType.Player:
                    name = Path.Combine(psPath, name); //  Has path separator at end
                    file = new FileStream(name, FileMode.OpenOrCreate);
                    br = new BinaryReader(file);
                    bw = new BinaryWriter(file);
                    content = new StreamReader(file).ReadToEnd();
                    break;
                case SaveType.Map:
                    name = Path.Combine(msPath, name);
                    //  Something similar to SaveType.Player, perhaps putting data in OS
                    //  %userprofile%\\Documents\\"My Games"
                    break;
                case SaveType.World:
                    break;
            }
            fileName = name;
        }
        public void WorldInit(string name)
        {
            name = Path.Combine(msPath, name);
            file = new FileStream(name, FileMode.OpenOrCreate);
            br = new BinaryReader(file);
            bw = new BinaryWriter(file);
        }
        [Obsolete("Searching an entire file stream one byte at a time is expensive.")]
        private object GetValue(string tag, Type type, bool _OLD)
        {
            object value = -1;
            while (file.Position < file.Length)
            {
                int read = 0;
                if ((read = file.ReadByte()) != -1)
                {
                    if (Encoding.ASCII.GetString(new[] { (byte)read }).StartsWith(tag[0].ToString()))
                    {
                        byte[] buf = Encoding.ASCII.GetBytes(tag);
                        byte[] compare = new byte[buf.Length - 1];
                        string output = tag;
                        if (tag.Length > 1)
                        {
                            file.Read(compare, 0, compare.Length);
                            output = Encoding.ASCII.GetString(compare);
                            output = output.Insert(0, tag[0].ToString());
                        }
                        if (tag == output)
                        {
                            if (type == typeof(bool))
                            {
                                return value = br.ReadBoolean();
                            }
                            else if (type == typeof(byte))
                            {
                                return value = br.ReadByte();
                            }
                            else if (type == typeof(Int16))
                            {
                                return value = br.ReadInt16();
                            }
                            else if (type == typeof(Int32))
                            { 
                                return value = br.ReadInt32();
                            }
                            else if (type == typeof(UInt16))
                            {
                                return value = br.ReadUInt16();
                            }
                            else if (type == typeof(float))
                            {
                                return value = br.ReadSingle();
                            }
                            else if (type == typeof(string))
                            {
                                return value = br.ReadString();
                            }
                            else if (type == typeof(Int64))
                            { 
                                return value = br.ReadInt64();
                            }
                            else if (type == typeof(double))
                            {
                                return value = br.ReadDouble();
                            }
                            else if (type == typeof(Vector2))
                            {
                                return value = br.ReadVector2();
                            }
                            else if (type == typeof(Color))
                            {
                                return value = br.ReadColor();
                            }
                            else if (type == typeof(Purse))
                            {
                                return value = br.ReadPurse();
                            }
                        }
                    }
                }
            }
            if (file.Position == file.Length)
            {
                file.Position = 0;
            }
            return value;
        }
        private object GetValue(string tag, Type type)
        {
            object value = -1;
            if (!content.Contains(tag))
            {
                throw TagDoesNotExistException(tag);
            }
            br.BaseStream.Seek(content.IndexOf(tag), SeekOrigin.Begin);
            byte[] search = Encoding.ASCII.GetBytes(tag);
            int len = search.Length;
            byte[] buffer = new byte[len];
            while (br.Read(buffer, 0, len) == len)
            {
                if (buffer.SequenceEqual(search))
                {
                    break;
                }
            }
            if (type == typeof(bool))
            {
                return value = br.ReadBoolean();
            }
            else if (type == typeof(byte))
            {
                return value = br.ReadByte();
            }
            else if (type == typeof(Int16))
            {
                return value = br.ReadInt16();
            }
            else if (type == typeof(Int32))
            {
                return value = br.ReadInt32();
            }
            else if (type == typeof(UInt16))
            {
                return value = br.ReadUInt16();
            }
            else if (type == typeof(float))
            {
                return value = br.ReadSingle();
            }
            else if (type == typeof(string))
            {
                return value = br.ReadString();
            }
            else if (type == typeof(Int64))
            {
                return value = br.ReadInt64();
            }
            else if (type == typeof(double))
            {
                return value = br.ReadDouble();
            }
            else if (type == typeof(Vector2))
            {
                return value = br.ReadVector2();
            }
            else if (type == typeof(Color))
            {
                return value = br.ReadColor();
            }
            else if (type == typeof(Purse))
            {
                return value = br.ReadPurse();
            }
            else if (type == typeof(Item))
            {
                return value = br.ReadItem();
            }
            else return value;
        }
        private bool TagExists(string tag)
        {
            while (file.Position < file.Length)
            {
                int read = 0;
                if ((read = file.ReadByte()) != -1)
                {
                    long current = Math.Min(file.Length - 1, file.Position + 1);
                    if (Encoding.ASCII.GetString(new []{ (byte)read }).StartsWith(tag[0].ToString()))
                    {
                        byte[] buf = Encoding.ASCII.GetBytes(tag);
                        byte[] compare = new byte[buf.Length - 1];
                        string output = tag;
                        if (tag.Length > 1)
                        {
                            file.Read(compare, 0, compare.Length);
                            output = Encoding.ASCII.GetString(compare);
                            output = output.Insert(0, tag[0].ToString());
                        }
                        if (tag == output)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public long Position
        {
            get { return file.Position; }
            set { file.Position = value; }
        }
        public void Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            { 
                case SeekOrigin.Begin:
                    file.Position = offset;
                    break;
                case SeekOrigin.Current:
                    file.Position += offset;
                    break;
                case SeekOrigin.End:
                    file.Position = file.Length + offset;
                    break;
            }
        }
        public void WorldMap(Manager manager)
        {
            //file.Position = 0;
            if (manager == Manager.Save)
            {
                DataStore data = new DataStore(fileName);
                data.NewBlock(new string[] { "tile_len" }, new object[] { Main.tile.Length }, "tileLen");
                int tileLen = 0;
                for (int k = 0; k < Main.tile.GetLength(0); k++)
                { 
                    for (int l = 0; l < Main.tile.GetLength(1); l++)
                    {
                        Tile item1 = Main.tile[k, l];
                        if (item1 != null)
                        {
                            string name = $"tile{tileLen++}";
                            data.NewBlock(new string[] 
                            {
                                "whoAmI",
                                "positionX",
                                "positionY",
                                "active",
                                "discovered",
                                "solid",
                                "width",
                                "height",
                                "colorR",
                                "colorB",
                                "colorG"
                            },
                            new object[]
                            {
                                item1.whoAmI,
                                item1.position.X,
                                item1.position.Y,
                                item1.Active,
                                item1.discovered,
                                item1.solid,
                                item1.width,
                                item1.height,
                                item1.color.R,
                                item1.color.B,
                                item1.color.G
                            }, name);
                        }
                    }
                }
                data.NewBlock(new string[] { "background_len" }, new object[] { Main.background.Length }, "backgroundLen");
                int bgLen = 0;
                foreach (Background item2 in Main.background)
                {
                    if (item2 != null)
                    {
                        string name = $"background{bgLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "active",
                            "discovered",
                            "width",
                            "height"
                        },
                        new object[]
                        {
                            item2.whoAmI,
                            item2.position.X,
                            item2.position.Y,
                            item2.active,
                            item2.discovered,
                            item2.width,
                            item2.height
                        }, name);
                    }
                }
                data.NewBlock(new string[] { "room_len" }, new object[] { Main.room.Values.Count(t => t != null) }, "roomLen");
                int roomLen = 0;
                for (int i = 0; i < Main.room.Count; i++)
                {
                    Room item3 = Main.room[i];
                    if (item3 != null)
                    {
                        string name = $"room{roomLen++}";
                        data.NewBlock(new string[] 
                        {
                            "positionX",
                            "positionY",
                            "width",
                            "height",
                            "type"
                        },
                        new object[]
                        {
                            item3.bounds.X,
                            item3.bounds.Y,
                            item3.bounds.Width,
                            item3.bounds.Height,
                            item3.type
                        }, name);
                    }
                }
                data.NewBlock(new string[] { "stair_len" }, new object[] { Main.staircase.Count(t => t != null && t.active) }, "stairLen");
                int stairLen = 0;
                foreach (Staircase s in Main.staircase)
                {
                    if (s != null && s.active)
                    {
                        string name = $"stair{stairLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "discovered",
                            "direction",
                            "size"
                        },
                        new object[]
                        {
                            s.whoAmI,
                            s.position.X,
                            s.position.Y,
                            s.discovered,
                            (byte)s.direction,
                            Tile.Size
                        }, name);
                    }
                }
                data.NewBlock(new string[] { "scenery_len" }, new object[] { Main.scenery.Count(t => t != null && t.active) }, "sceneryLen");
                int sceneryLen = 0;
                foreach (Scenery scenery in Main.scenery)
                {
                    if (scenery != null && scenery.active)
                    {
                        string name = $"scenery{sceneryLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "active",
                            "discovered",
                            "solid",
                            "width",
                            "height",
                            "type"
                        },
                        new object[]
                        {
                            scenery.whoAmI,
                            scenery.position.X,
                            scenery.position.Y,
                            scenery.active,
                            scenery.discovered,
                            scenery.solid,
                            scenery.width,
                            scenery.height,
                            scenery.type
                        }, name);
                    }
                }
                data.NewBlock(new string[] { "lamp_len" }, new object[] { Main.lamp.Count(t => t != null && t.active) }, "lampLen");
                int lampLen = 0;
                foreach (Lamp lamp in Main.lamp)
                {
                    if (lamp != null && lamp.active)
                    {
                        string name = $"lamp{lampLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "active",
                            "staticlamp",
                            "width",
                            "height",
                            "colorR",
                            "colorG",
                            "colorB",
                            "range"
                        },
                        new object[]
                        {
                            lamp.whoAmI,
                            lamp.position.X,
                            lamp.position.Y,
                            lamp.active,
                            lamp.staticLamp,
                            lamp.width,
                            lamp.height,
                            lamp.owner,
                            lamp.lampColor.R,
                            lamp.lampColor.G,
                            lamp.lampColor.B,
                            lamp.range
                        }, name);
                    }
                }
                data.NewBlock(new string[] { "npc_len" }, new object[] { Main.npc.Count(t => t != null && t.active) }, "npcLen");
                int npcLen = 0;
                foreach (Npc npc in Main.npc)
                {
                    if (npc != null && npc.active)
                    {
                        string name = $"npc{npcLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "active",
                            "width",
                            "height",
                            "life",
                            "colorR",
                            "colorG",
                            "colorB",
                            "damage",
                            "type"
                        },
                        new object[]
                        {
                            npc.whoAmI,
                            npc.position.X,
                            npc.position.Y,
                            npc.active,
                            npc.width,
                            npc.height,
                            npc.owner,
                            npc.defaultColor.R,
                            npc.defaultColor.G,
                            npc.defaultColor.B,
                            npc.damage,
                            npc.type
                        }, name);
                    }
                    //  If mana value, save here
                    //  If cursed or enchanted, save -- or if items carried are such and so on
                    //  Look into saving items carried
                }
                data.NewBlock(new string[] { "item_len" }, new object[] { Main.item.Count(t => t != null && t.active) }, "itemLen");
                int itemLen = 0;
                foreach (Item item in Main.item)
                {
                    if (item != null && item.active)
                    {
                        string name = $"item{itemLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "active",
                            "width",
                            "height",
                            "type",
                            "owner",
                            "colorR",
                            "colorG",
                            "colorB",
                            "enchanted",
                            "cursed",
                            "equipType",
                            "equipped"
                        },
                        new object[]
						{
							item.whoAmI,
                            item.position.X,
							item.position.Y,
                            item.active,
                            item.width,
                            item.height,
                            item.type,
                            item.owner,
                            item.color.R,
                            item.color.G,
                            item.color.B,
                            item.enchanted,
                            item.cursed,
                            item.equipType,
                            item.equipped
                        }, name);
			            //  Write purse handling here
			            //if (item.purse != null && item.purse.Content != null)
			            //{
				        //    bw.Write('p');
				        //    bw.Write(item.purse);
			            //}
			            //else bw.Write('n');
                    }
                }
                data.NewBlock(new string[] { "trap_len" }, new object[] { Main.trap.Count(t => t != null && t.active) }, "trapLen");
                int trapLen = 0;
                foreach (Trap trap in Main.trap)
                {
                    if (trap != null && trap.active)
                    {   
                        string name = $"trap{trapLen++}";
                        data.NewBlock(new string[] 
                        {
                            "whoAmI",
                            "positionX",
                            "positionY",
                            "active",
                            "width",
                            "height",
                            "life",
                            "colorR",
                            "colorG",
                            "colorB",
                            "type",
                            "rotation"
                        },
                        new object[]
						{
							trap.whoAmI,
                            trap.position.X,
							trap.position.Y,
                            trap.active,
                            trap.width,
                            trap.height,
                            trap.life,
                            trap.defaultColor.R,
                            trap.defaultColor.G,
                            trap.defaultColor.B,
                            trap.type,
                            trap.rotation
                        }, name);
                    }
                }
                data.WriteToFile();
                return;
                int stashLen = 0;
                bw.Write(Main.stash.Count(t => t != null && t.active));
                foreach (Stash stash in Main.stash)
                {
                    if (stash != null && stash.active)
                    {
                        string name = $"stash{stashLen}";
                        bw.Write(stash.whoAmI);
                        bw.Write(stash.position);
                        bw.Write(stash.active);
                        bw.Write(stash.width);
                        bw.Write(stash.height);
                        bw.Write(stash.defaultColor);
                        int len = stash.content == null ? 0 : stash.content.Length;
                        bw.Write(len);
                        if (stash.content != null && stash.content.Length > 0)
                        {
                            int contentLen = 0;
                            foreach (Item i in stash.content)
                            {
                                string _name = $"stash{stashLen}_content{contentLen}";
                                bw.Write(i.whoAmI);
                                bw.Write(i.position);
                                bw.Write(i.active);
                                bw.Write(i.width);
                                bw.Write(i.height);
                                bw.Write(i.defaultColor);
                                bw.Write(i.type);
                                //if (i.purse != null && i.purse.Content != null)
                                //{
                                //    bw.Write(i.purse);
                                //}
                                //else 
                                    //bw.Write(new Purse(0));
                                contentLen++;
                            }
                        }
                        stashLen++;
                    }
                }
            }
            else if (manager == Manager.Load)
            {
                Map.Unload();
                DataStore data = new DataStore(fileName);
                var b0 = data.GetBlock("tileLen");
                int tileLen = int.Parse(b0.GetValue("tile_len"));
                
                    int size = (int)Math.Sqrt(tileLen);
                    Main.tile = new Tile[size, size];
                    Main.WorldWidth = size;
                    Main.WorldHeight = size;
                
                int num = 0;
                for (int k = 0; k < size; k++)
                    for (int l = 0; l < size; l++)
                    {
                        string name = $"tile{num++}";
                        var _b1 = data.GetBlock(name);
                        Main.tile[k, l] = new Tile(k, l);
                        Main.tile[k, l].whoAmI = int.Parse(_b1.GetValue("whoAmI"));
                        Main.tile[k, l].X = int.Parse(_b1.GetValue("positionX"));
                        Main.tile[k, l].Y = int.Parse(_b1.GetValue("positionY"));
                        Main.tile[k, l].active(bool.Parse(_b1.GetValue("active")));
                        Main.tile[k, l].discovered = bool.Parse(_b1.GetValue("discovered"));
                        Main.tile[k, l].solid = bool.Parse(_b1.GetValue("solid"));
                        Main.tile[k, l].width = int.Parse(_b1.GetValue("width"));
                        Main.tile[k, l].height = int.Parse(_b1.GetValue("height"));
                        Main.tile[k, l].color = Color.FromArgb
                        (
                            int.Parse(_b1.GetValue("colorR")),
                            int.Parse(_b1.GetValue("colorG")),
                            int.Parse(_b1.GetValue("colorB"))
                        );
                    }
                int num2 = 0;
                Main.background = new Background[size, size];
                for (int k = 0; k < size; k++)
                    for (int l = 0; l < size; l++)
                    {
                        string name = $"background{num2++}";
                        var b1 = data.GetBlock(name);
                        Main.background[k, l] = new Background(k, l, Tile.Size);
                        var b2 = data.GetBlock(name);
                        Main.background[k, l].whoAmI = int.Parse(b1.GetValue("whoAmI"));
                        Main.background[k, l].position.X = int.Parse(b1.GetValue("positionX"));
                        Main.background[k, l].position.Y = int.Parse(b1.GetValue("positionY"));
                        Main.background[k, l].active = bool.Parse(b1.GetValue("active"));
                        Main.background[k, l].discovered = bool.Parse(b1.GetValue("discovered"));
                        Main.background[k, l].width = int.Parse(b1.GetValue("width"));
                        Main.background[k, l].height = int.Parse(b1.GetValue("height"));
                    }
                var b3 = data.GetBlock("roomLen");
                int roomLen = int.Parse(b3.GetValue("room_len"));
                int num3 = 0;
                for (int i = 0; i < roomLen; i++)
                {
                    string name = $"room{i}";
                    var b1 = data.GetBlock(name);
                    int x = int.Parse(b1.GetValue("positionX"));
                    int y = int.Parse(b1.GetValue("positionY"));
                    int width = int.Parse(b1.GetValue("width"));
                    int height = int.Parse(b1.GetValue("height"));
                    short type = short.Parse(b1.GetValue("type"));
                    Main.room.Add(num3++, new Room(type)    //  TODO: create way to init region (scenery) array on load from file
                    {
                        bounds = new Rectangle(x, y, width, height),
                    });
                }
                var b5 = data.GetBlock("stairLen");
                int stairLen = int.Parse(b5.GetValue("stair_len"));
                Main.staircase = new Staircase[stairLen];
                for (int i = 0; i < stairLen; i++)
                {
                    string name = $"stair{i}";
                    var b1 = data.GetBlock(name);
                    int whoAmI = int.Parse(b1.GetValue("whoAmI"));
                    int x = int.Parse(b1.GetValue("positionX"));
                    int y = int.Parse(b1.GetValue("positionY"));
                    bool d = int.Parse(b1.GetValue("direction")) == 2 ? true : false;
                    StaircaseDirection dir = d ? StaircaseDirection.LeadingDown : StaircaseDirection.LeadingUp;
                    int index = Staircase.NewStaircase(x, y, dir);
                    Main.staircase[index].discovered = bool.Parse(b1.GetValue("discovered"));
                }
                b5 = data.GetBlock("sceneryLen");
                int sceneryLen = int.Parse(b5.GetValue("scenery_len"));
                for (int i = 0; i < sceneryLen; i++)
                {
                    string name = $"scenery{i}";
                    var b1 = data.GetBlock(name);
                    int whoAmI = int.Parse(b1.GetValue("whoAmI"));
                    float x = float.Parse(b1.GetValue("positionX"));
                    float y = float.Parse(b1.GetValue("positionY"));
                    bool a = bool.Parse(b1.GetValue("active"));
                    bool d = bool.Parse(b1.GetValue("discovered"));
                    bool s = bool.Parse(b1.GetValue("solid"));
                    int w = int.Parse(b1.GetValue("width"));
                    int h = int.Parse(b1.GetValue("height"));
                    short t = short.Parse(b1.GetValue("type"));
                    int j = Scenery.NewScenery((int)x, (int)y, w, h, t);
                    Main.scenery[j].active = a;
                    Main.scenery[j].discovered = d;
                    Main.scenery[j].solid = s;
                }
                b5 = data.GetBlock("lampLen");
                int lampLen = int.Parse(b5.GetValue("lamp_len"));
                for (int i = 0; i < lampLen; i++)
                {
                    string name = $"lamp{i}";
                    var b1 = data.GetBlock(name);
                    int id = int.Parse(b1.GetValue("whoAmI"));
                    Main.lamp[id] = new Lamp(0);
                    Main.lamp[id].whoAmI = id;
                    Main.lamp[id].position.X = float.Parse(b1.GetValue("positionX"));
                    Main.lamp[id].position.Y = float.Parse(b1.GetValue("positionY"));
                    Main.lamp[id].active = bool.Parse(b1.GetValue("active"));
                    Main.lamp[id].staticLamp = int.Parse(b1.GetValue("staticLamp")) == 0 ? false : true;
                    Main.lamp[id].width = int.Parse(b1.GetValue("width"));
                    Main.lamp[id].height = int.Parse(b1.GetValue("height"));
                    Main.lamp[id].owner = int.Parse(b1.GetValue("owner"));
                    byte r = byte.Parse(b1.GetValue("colorR"));
                    byte g = byte.Parse(b1.GetValue("colorG"));
                    byte b = byte.Parse(b1.GetValue("colorB"));
                    Main.lamp[id].color = Color.FromArgb(r, g, b);
                    Main.lamp[id].range = float.Parse(b1.GetValue("range"));
                }
                b5 = data.GetBlock("npcLen");
                int npcLen = int.Parse(b5.GetValue("npc_len"));
                for (int i = 0; i < npcLen; i++)
                {
                    string name = $"npc{i}";
                    var b1 = data.GetBlock(name);
                    int id = int.Parse(b1.GetValue("whoAmI"));
                    float x = float.Parse(b1.GetValue("positionX"));
                    float y = float.Parse(b1.GetValue("positionY"));
                    bool a = bool.Parse(b1.GetValue("active"));
                    int w = int.Parse(b1.GetValue("width"));
                    int h = int.Parse(b1.GetValue("height"));
                    int l = int.Parse(b1.GetValue("life"));
                    byte r, g, b;
                    r = byte.Parse(b1.GetValue("colorR"));
                    g = byte.Parse(b1.GetValue("colorG"));
                    b = byte.Parse(b1.GetValue("colorB"));
                    int damage = int.Parse(b1.GetValue("damage"));
                    Color c = Color.FromArgb(r, g, b);
                    //  If mana value, save here
                    short t = short.Parse(b1.GetValue("type"));
                    //  If cursed or enchanted, save -- or if items carried are such and so on
                    //  Look into saving items carried
                    int j = Npc.NewNPC(x, y, t, damage);
                    Main.npc[j].active = a;
                    Main.npc[j].life = l;
                    Main.npc[j].defaultColor = c;
                }
                b5 = data.GetBlock("itemLen");
                int itemLen = int.Parse(b5.GetValue("item_len"));
                for (int i = 0; i < itemLen; i++)
                {
                    string name = $"item{i}";
                    var b1 = data.GetBlock(name);
                    int whoAmI = int.Parse(b1.GetValue("whoAmI"));
			        float x = float.Parse(b1.GetValue("positionX"));
                    float y = float.Parse(b1.GetValue("positionY"));
			        short type = short.Parse(b1.GetValue("type"));
			        bool active = bool.Parse(b1.GetValue("active"));
			        int width = int.Parse(b1.GetValue("width"));
			        int height = int.Parse(b1.GetValue("height"));
			        int owner = int.Parse(b1.GetValue("owner"));
                    byte r = byte.Parse(b1.GetValue("colorR"));
                    byte g = byte.Parse(b1.GetValue("colorG"));
                    byte b = byte.Parse(b1.GetValue("colorB"));
			        Color color = Color.FromArgb(r, g, b);
			        bool enchanted = bool.Parse(b1.GetValue("enchanted"));
			        bool cursed = bool.Parse(b1.GetValue("cursed"));
			        int equipType = int.Parse(b1.GetValue("equipType"));
			        bool equipped = bool.Parse(b1.GetValue("equipped"));
                    int index = Item.NewItem(x, y, width, height, type, (byte)owner);
                    Main.item[index].equipType = equipType;
                    Main.item[index].equipped = equipped;
                    Main.item[index].EquipItem(Main.myPlayer);
                    Main.item[index].Enchanted(enchanted);
                    Main.item[index].Cursed(cursed);
                    Main.item[index].color = color;
                    if (owner != 255 && !equipped)
                    {
                        Main.myPlayer.PickupItem(ref Main.item[index]);
                    }
                }
                b5 = data.GetBlock("trapLen");
                int trapLen = int.Parse(b5.GetValue("trapLen"));
                for (int i = 0; i < trapLen; i++)
                {
                    string name = $"trap{i}";
                    var b1 = data.GetBlock(name);
                    int id = int.Parse(b1.GetValue("whoAmI"));
                    float x = float.Parse(b1.GetValue("positionX"));
                    float y = float.Parse(b1.GetValue("positionY"));
                    bool a = bool.Parse(b1.GetValue("active"));
                    int w = int.Parse(b1.GetValue("width"));
                    int h = int.Parse(b1.GetValue("height"));
                    int l = int.Parse(b1.GetValue("life"));
                    byte r = byte.Parse(b1.GetValue("colorR"));
                    byte g = byte.Parse(b1.GetValue("colorG"));
                    byte b = byte.Parse(b1.GetValue("colorB"));
                    Color c = Color.FromArgb(r, g, b);
                    short t = short.Parse(b1.GetValue("type"));
                    float _r = float.Parse(b1.GetValue("rotation"));
                    int index = Trap.NewTrap(x, y, w, h, t, active: a);
                    Main.trap[index].color = c;
                    Main.trap[index].rotation = _r;
                    Main.trap[index].life = l;
                }
                return;
                int stashLen = br.ReadInt32();
                for (int i = 0; i < stashLen; i++)
                {
                    string name = $"stash{i}";
                    int id = br.ReadInt32();
                    Vector2 v2 = br.ReadVector2();
                    bool a = br.ReadBoolean();
                    int w = br.ReadInt32();
                    int h = br.ReadInt32();
                    Color c = br.ReadColor();
                    int contentLen = br.ReadInt32();
                    if (contentLen > 0)
                    {
                        Item[] content = new Item[contentLen];
                        for (int j = 0; j < content.Length; j++)
                        {
                            string _name = $"stash{i}_content{j}";
                            content[j] = new Item();
                            content[j].whoAmI = br.ReadInt32();
                            content[j].position = br.ReadVector2();
                            content[j].active = br.ReadBoolean();
                            content[j].width = br.ReadInt32();
                            content[j].height = br.ReadInt32();
                            content[j].defaultColor = br.ReadColor();
                            content[j].type = br.ReadInt16();
                            //content[j].purse = br.ReadPurse();
                        }
                        Stash.NewStash((int)v2.X, (int)v2.Y, 0, content);
                    }
                    else continue;
                }
            }
        }
        #region save value
        public void SaveValue(string tag, bool value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, byte value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, Int16 value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, Int32 value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, Int64 value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, UInt16 value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, Single value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, Double value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, string value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value);
        }
        public void SaveValue(string tag, Vector2 value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value.X);
            bw.Write(value.Y);
        }
        public void SaveValue(string tag, Color value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value.A);
            bw.Write(value.R);
            bw.Write(value.G);
            bw.Write(value.B);
        }
        public void SaveValue(string tag, Purse value)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(value.Content.copper);
            bw.Write(value.Content.silver);
            bw.Write(value.Content.gold);
            bw.Write(value.Content.platinum);
        }
        public void SaveValue(string tag, Item item)
        {
            if (!TagExists(tag))
            {
                //throw new Exception($"Tag, {tag}, already exists");
                bw.Write(tag);
            }
            bw.Write(item);
        }
        #endregion
        #region variable retrieve
        public bool GetBool(string name)
        {
            try
            {
                return (bool)GetValue(name, typeof(bool));
            }
            catch 
            { 
                return default;
            }
        }
        public byte GetByte(string name)
        {
            try
            { 
                return (byte)GetValue(name, typeof(byte));
            }
            catch
            {
                return default;
            }
        }
        public short GetInt16(string name)
        {
            try
            { 
                return (Int16)GetValue(name, typeof(Int16));
            }
            catch
            {
                return default;
            }
        }
        public int GetInt32(string name)
        {
            try
            { 
                return (Int32)GetValue(name, typeof(Int32));
            }
            catch
            {
                return default;
            }
        }
        public long GetInt64(string name)
        {
            try
            { 
                return (Int64)GetValue(name, typeof(Int64));
            }
            catch
            {
                return default;
            }
        }
        public float GetSingle(string name)
        {
            try
            { 
                return (Single)GetValue(name, typeof(Single));
            }
            catch
            {
                return default;
            }
        }
        public double GetDouble(string name)
        {
            try
            { 
                return (double)GetValue(name, typeof(double));
            }
            catch
            {
                return default;
            }
        }
        public string GetString(string name)
        {
            try
            { 
                return (string)GetValue(name, typeof(string));
            }
            catch
            {
                return default;
            }
        }
        public Vector2 GetVector2(string name)
        {
            try
            { 
                return (Vector2)GetValue(name, typeof(Vector2));
            }
            catch
            {
                return default;
            }
        }
        public Color GetColor(string name)
        {
            try
            { 
                return (Color)GetValue(name, typeof(Color));
            }
            catch
            {
                return default;
            }
        }
        public CirclePrefect.Native.Stash GetStash(string name)
        {
            try
            { 
                return (CirclePrefect.Native.Stash)GetValue(name, typeof(CirclePrefect.Native.Stash));
            }
            catch
            {
                return default;
            }
        }
        public Item GetItem(string name)
        {
            try
            { 
                return (Item)GetValue(name, typeof(Item));
            }
            catch
            {
                return default;
            }
        }
        #endregion
        public void Dispose()
        {
            br?.Dispose();
            bw?.Dispose();
            file?.Dispose();
        }
        public enum Manager
        {
            Save,
            Load
        }
        private Exception TagAlreadyExistsException(string tag)
        {
            return new Exception($"Tag, {tag}, already exists.");
        }
        private Exception TagDoesNotExistException(string tag)
        {
            return new Exception($"Tag, {tag}, does not exist.");
        }
    }
}
