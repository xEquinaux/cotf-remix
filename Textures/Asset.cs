using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using cotf.World.Traps;

namespace cotf.Assets
{
    public sealed class Asset<T> where T : Image
    {
        public static void Initialize(int num = 2, int num2 = 1, int num3 = 1, int num4 = 1, int num5 = 1, int num6 = 1, int num7 = 1)
        {
            _Npc.Init(_Npc.Length = num);
            _Background.Init(_Background.Length = num2);
            _Item.Init(_Item.Length = num3);
            _Projectile.Init(_Projectile.Length = num4);
            _Scenery.Init(_Scenery.Length = num5);
            _Trap.Init(_Trap.Length = num6);
            _Loot.Init(_Loot.Length = num7);
        }
        public static T Request(string name)
        {
            return (T)Bitmap.FromFile("./Textures/" + name + ".png");
        }
        public static T Request(string name, string extension)
        {
            return (T)Bitmap.FromFile("./Textures/" + name + extension);
        }
        public static T Get(Type type, int style)
        {
            switch (type.Name)
            {
                case nameof(Npc):
                    return _Npc.Texture[style];
                case nameof(Background):
                    return _Background.Texture[style];
                case nameof(Item):
                    return _Item.Texture[style];
                case nameof(Projectile):
                    return _Projectile.Texture[style];
                case nameof(Scenery):
                    return _Scenery.Texture[style];
                case nameof(Trap):
                    //  TODO sort texture indices
                    return _Trap.Texture[0];    
                case nameof(Loot):
                    return _Loot.Texture[style];
                default:
                    return (T)Main.pixel;
            }
        }
        static class _Npc
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    Texture[i] = Request("npc" + i);
                }
            }
        }
        static class _Background
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    using (Bitmap bmp = new Bitmap(World.Tile.Size, World.Tile.Size))
                    { 
                        using (Graphics g = Graphics.FromImage(bmp))
                        { 
                            g.FillRectangle(Brushes.White, new Rectangle(0, 0, World.Tile.Size, World.Tile.Size));
                            Texture[i] = (T)(Image)bmp;
                        }
                    }
                }
            }
        }
        static class _Item
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    Texture[i] = Request("item" + i);
                }
            }
        }
        static class _Projectile
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    Texture[i] = Request("projectile" + i);
                }
            }
        }
        static class _Scenery
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    Texture[i] = Request("scenery" + i);
                }
            }
        }
        static class _Trap
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    Texture[i] = Request("trap" + i);
                }
            }
        }
        static class _Loot
        {
            internal static int Length;
            internal static T[] Texture = new T[Length];
            internal static void Init(int length)
            {
                Texture = new T[length];
                for (int i = 0; i < length; i++)
                {
                    Texture[i] = Request("loot" + i);
                }
            }
        }
    }
    /*
    sealed class Content<T> where T : Item
    {
        public static T Request(string name)
        {
            return Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(t=> t.Name == name).DeclaringType as T;
        }
        public static T Request()
        {
            string name = Assembly.GetExecutingAssembly().GetName().Name + "." + typeof(T).Name;
            return Type.GetType(name) as T;
        }
        //public static T Request(string name)
        //{
        //    name = Assembly.GetExecutingAssembly().GetName().Name + "." + name;
        //    return Type.GetType(name) as T;
        //}
    } */
}
