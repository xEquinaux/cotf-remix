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
            this.subject = subject;
            this.type = type;
            Init(subject.name);
        }
        private static string 
            psPath, 
            msPath;
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
                    break;
                case SaveType.Map:
                    name = Path.Combine(msPath, name);
                    //  Something similar to SaveType.Player, perhaps putting data in OS
                    //  %userprofile%\\Documents\\"My Games"
                    break;
                case SaveType.World:
                    break;
            }
            file = new FileStream(name, FileMode.OpenOrCreate);
            br = new BinaryReader(file);
            bw = new BinaryWriter(file);
            content = new StreamReader(file).ReadToEnd();
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
            file.Position = 0;
            if (manager == Manager.Save)
            {
                int tileLen = 0;
                bw.Write(Main.tile.Length);
                for (int k = 0; k < Main.tile.GetLength(0); k++)
                { 
                    for (int l = 0; l < Main.tile.GetLength(1); l++)
                    {
                        Tile item1 = Main.tile[k, l];
                        if (item1 != null)
                        {
                            string name = $"tile{tileLen++}";
                            bw.Write(item1.whoAmI);
                            bw.Write(item1.position);
                            bw.Write(item1.Active);
                            bw.Write(item1.discovered);
                            bw.Write(item1.solid);
                            bw.Write(item1.width);
                            bw.Write(item1.height);
                            bw.Write(item1.color);
                        }
                    }
                }
                int bgLen = 0;
                bw.Write(Main.background.Length);
                foreach (Background item2 in Main.background)
                {
                    if (item2 != null)
                    {
                        string name = $"background{bgLen++}";
                        bw.Write(item2.whoAmI);
                        bw.Write(item2.position);
                        bw.Write(item2.active);
                        bw.Write(item2.discovered);
                        bw.Write(item2.width);
                        bw.Write(item2.height);
                    }
                }
                int roomLen = 0;
                bw.Write(Main.room.Values.Count(t => t != null));
                for (int i = 0; i < Main.room.Count; i++)
                {
                    Room item3 = Main.room[i];
                    if (item3 != null)
                    {
                        string name = $"room{roomLen++}";
                        //bw.Write(i);
                        bw.Write(item3.bounds.X);
                        bw.Write(item3.bounds.Y);
                        bw.Write(item3.bounds.Width);
                        bw.Write(item3.bounds.Height);
                        bw.Write(item3.type);
                    }
                }
                int stairLen = 0;
                bw.Write(Main.staircase.Count(t => t != null && t.active));
                foreach (Staircase s in Main.staircase)
                {
                    if (s != null && s.active)
                    {
                        string name = $"stair{stairLen++}";
                        bw.Write(s.whoAmI);
                        bw.Write(s.position);
                        bw.Write(s.discovered);
                        bw.Write((byte)s.direction);
                        bw.Write(Tile.Size);
                    }
                }
                int sceneryLen = 0;
                bw.Write(Main.scenery.Count(t => t != null && t.active));
                foreach (Scenery scenery in Main.scenery)
                {
                    if (scenery != null && scenery.active)
                    {
                        string name = $"scenery{sceneryLen++}";
                        bw.Write(scenery.whoAmI);
                        bw.Write(scenery.position);
                        bw.Write(scenery.active);
                        bw.Write(scenery.discovered);
                        bw.Write(scenery.solid);
                        bw.Write(scenery.width);
                        bw.Write(scenery.height);
                        bw.Write(scenery.type);
                    }
                }
                int lampLen = 0;
                bw.Write(Main.lamp.Count(t => t != null && t.active));
                foreach (Lamp lamp in Main.lamp)
                {
                    if (lamp != null && lamp.active)
                    {
                        string name = $"lamp{lampLen++}";
                        bw.Write(lamp.whoAmI);
                        bw.Write(lamp.position);
                        bw.Write(lamp.active);
                        bw.Write(lamp.staticLamp);
                        bw.Write(lamp.width);
                        bw.Write(lamp.height);
                        bw.Write(lamp.owner);
                        bw.Write(lamp.lampColor);
                        bw.Write(lamp.range);
                    }
                }
                int npcLen = 0;
                bw.Write(Main.npc.Count(t => t != null && t.active));
                foreach (Npc npc in Main.npc)
                {
                    if (npc != null && npc.active)
                    {
                        string name = $"npc{npcLen++}";
                        bw.Write(npc.whoAmI);
                        bw.Write(npc.position);
                        bw.Write(npc.active);
                        bw.Write(npc.width);
                        bw.Write(npc.height);
                        bw.Write(npc.life);
                        bw.Write(npc.defaultColor);
                        //  If mana value, save here
                        bw.Write(npc.type);
                        //  If cursed or enchanted, save -- or if items carried are such and so on
                        //  Look into saving items carried
                    }
                }
                int itemLen = 0;
                bw.Write(Main.item.Count(t => t != null && t.active));
                foreach (Item item in Main.item)
                {
                    if (item != null && item.active)
                    {
                        bw.Write(item);
                    }
                }
                int trapLen = 0;
                bw.Write(Main.trap.Count(t => t != null && t.active));
                foreach (Trap trap in Main.trap)
                {
                    if (trap != null && trap.active)
                    {   
                        string name = $"trap{trapLen++}";
                        bw.Write(trap.whoAmI);
                        bw.Write(trap.position);
                        bw.Write(trap.active);
                        bw.Write(trap.width);
                        bw.Write(trap.height);
                        bw.Write(trap.life);
                        bw.Write(trap.defaultColor);
                        bw.Write(trap.type);
                        bw.Write(trap.rotation);
                    }
                }
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
                int tileLen = br.ReadInt32();
                int size = (int)Math.Sqrt(tileLen);
                Main.tile = new Tile[size, size];
                Main.WorldWidth = size;
                Main.WorldHeight = size;
                int num = 0;
                for (int k = 0; k < size; k++)
                    for (int l = 0; l < size; l++)
                    {                                                   
                        string name = $"tile{num}";
                        Main.tile[k, l] = new Tile(k, l);
                        Main.tile[k, l].whoAmI = br.ReadInt32();
                        var v2 = br.ReadVector2();
                        Main.tile[k, l].X = (int)v2.X;
                        Main.tile[k, l].Y = (int)v2.Y;
                        Main.tile[k, l].active(br.ReadBoolean());
                        Main.tile[k, l].discovered = br.ReadBoolean();
                        Main.tile[k, l].solid = br.ReadBoolean();
                        Main.tile[k, l].width = br.ReadInt32();
                        Main.tile[k, l].height = br.ReadInt32();
                        Main.tile[k, l].color = br.ReadColor();
                        num++;
                    }
                int num2 = 0;
                int bgLen = br.ReadInt32();
                Main.background = new Background[size, size];
                for (int k = 0; k < size; k++)
                    for (int l = 0; l < size; l++)
                    {
                        string name = $"background{num2++}";
                        Main.background[k, l] = new Background(k, l, Tile.Size);
                        Main.background[k, l].whoAmI = br.ReadInt32();
                        Main.background[k, l].position = br.ReadVector2();
                        Main.background[k, l].active = br.ReadBoolean();
                        Main.background[k, l].discovered = br.ReadBoolean();
                        Main.background[k, l].width = br.ReadInt32();
                        Main.background[k, l].height = br.ReadInt32();
                    }
                int roomLen = br.ReadInt32();
                int num3 = 0;
                for (int i = 0; i < roomLen; i++)
                {
                    string name = $"room{i}";
                    //int id = br.ReadInt32();
                    int x = br.ReadInt32();
                    int y = br.ReadInt32();
                    int width = br.ReadInt32();
                    int height = br.ReadInt32();
                    short type = br.ReadInt16();
                    Main.room.Add(num3++, new Room(type)    //  TODO: create way to init region (scenery) array on load from file
                    {
                        bounds = new Rectangle(x, y, width, height),
                    });
                }
                int stairLen = br.ReadInt32();
                for (int i = 0; i < stairLen; i++)
                {
                    string name = $"stair{i}";
                    int whoAmI = br.ReadInt32();
                    Vector2 v2 = br.ReadVector2();
                    bool d = br.ReadBoolean();
                    StaircaseDirection dir = (StaircaseDirection)br.ReadByte();
                    int index = Staircase.NewStaircase((int)v2.X, (int)v2.Y, dir);
                    Main.staircase[index].discovered = d;
                    br.ReadInt32();   //  unused
                }
                int sceneryLen = br.ReadInt32();
                for (int i = 0; i < sceneryLen; i++)
                {
                    string name = $"scenery{i}";
                    int whoAmI = br.ReadInt32();
                    Vector2 v2 = br.ReadVector2();
                    bool a = br.ReadBoolean();
                    bool d = br.ReadBoolean();
                    bool s = br.ReadBoolean();
                    int w = br.ReadInt32();
                    int h = br.ReadInt32();
                    short t = br.ReadInt16();
                    int j = Scenery.NewScenery((int)v2.X, (int)v2.Y, w, h, t);
                    Main.scenery[j].active = a;
                    Main.scenery[j].discovered = d;
                    Main.scenery[j].solid = s;
                }
                int lampLen = br.ReadInt32();
                for (int i = 0; i < lampLen; i++)
                {
                    string name = $"lamp{i}";
                    int id = br.ReadInt32();
                    Main.lamp[id] = new Lamp(0);
                    Main.lamp[id].whoAmI = id;
                    Main.lamp[id].position = br.ReadVector2();
                    Main.lamp[id].active = br.ReadBoolean();
                    Main.lamp[id].staticLamp = br.ReadBoolean();
                    Main.lamp[id].width = br.ReadInt32();
                    Main.lamp[id].height = br.ReadInt32();
                    Main.lamp[id].owner = br.ReadInt32();
                    Main.lamp[id].color = br.ReadColor();
                    Main.lamp[id].range = br.ReadSingle();
                }
                int npcLen = br.ReadInt32();
                for (int i = 0; i < npcLen; i++)
                {
                    string name = $"npc{i}";
                    int id = br.ReadInt32();
                    Vector2 v2 = br.ReadVector2();
                    bool a = br.ReadBoolean();
                    int w = br.ReadInt32();
                    int h = br.ReadInt32();
                    int l = br.ReadInt32();
                    Color c = br.ReadColor();
                    //  If mana value, save here
                    short t = br.ReadInt16();
                    //  If cursed or enchanted, save -- or if items carried are such and so on
                    //  Look into saving items carried
                    int j = Npc.NewNPC(v2.X, v2.Y, t);
                    Main.npc[j].active = a;
                    Main.npc[j].life = l;
                    Main.npc[j].defaultColor = c;
                }
                int itemLen = br.ReadInt32();
                for (int i = 0; i < itemLen; i++)
                {
                    Main.item[i] = br.ReadItem();
                }
                int trapLen = br.ReadInt32();
                for (int i = 0; i < trapLen; i++)
                {
                    string name = $"trap{i}";
                    int id = br.ReadInt32();
                    Vector2 v2 = br.ReadVector2();
                    bool a = br.ReadBoolean();
                    int w = br.ReadInt32();
                    int h = br.ReadInt32();
                    int l = br.ReadInt32();
                    Color c = br.ReadColor();
                    short t = br.ReadInt16();
                    float r = br.ReadSingle();
                    Trap.NewTrap(v2.X, v2.Y, w, h, t, active: a);
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
            br.Dispose();
            bw.Dispose();
            file.Dispose();
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
