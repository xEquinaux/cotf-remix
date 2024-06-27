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
        public static T Request(string name)
        {
            return (T)Bitmap.FromFile("./Textures/" + name + ".png");
        }
        public static T Request(string name, string extension)
        {
            return (T)Bitmap.FromFile("./Textures/" + name + extension);
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
