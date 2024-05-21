using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Base;
using cotf.World;

namespace cotf.World
{
    public class Room
    {
        public short type;
        public const byte NumTypes = 11;
        public short[,] region = new short[,] { };
        public Rectangle bounds;
        public int width => region.GetLength(0);
        public int height => region.GetLength(1);
        public int X => bounds.Left;
        public int Y => bounds.Top;
        public bool fog, heat;
        public Room(short type)
        {
            this.type = type;
        }
        public void InitRoom()
        {
            region = Scenery.Decorate(width, height, type);
        }
        public void Update()
        {

        }
        public static void InitAllRooms()
        {
            for (int i = 0; i < Main.room.Count; i++)
            {
                Main.room[i].region = Scenery.Decorate(Main.room[i].width, Main.room[i].height, Main.room[i].type);
            }
        }
        public void ConstructRoom()
        {
            for (int x = X; x < X + width; x++)
            {
                for (int y = Y; y < Y + height; y++)
                {
                    int i = (x - X) / Tile.Size;
                    int j = (y - Y) / Tile.Size;
                    Scenery.NewScenery(x, y, 50, 50, region[i, j]);
                }
            }
        }
        public static void ConstructAllRooms()
        {
            for (int n = 0; n < Main.room.Count; n++)
            {
                if (Main.room[n] == null)
                    continue;
                var r = Main.room[n];
                for (int x = r.X; x < r.X + r.bounds.Width; x += Tile.Size)
                {
                    for (int y = r.Y; y < r.Y + r.bounds.Height; y += Tile.Size)
                    {
                        int i = (x - r.X) / Tile.Size;
                        int j = (y - r.Y) / Tile.Size;
                        if (r.region[i, j] > 0)
                            Scenery.NewScenery(x, y, 50, 50, r.region[i, j]);
                    }
                }
            }
        }
        public void Draw(Graphics graphics)
        {
            //  Has draw priority, need a texture effect
            if (fog)
            {
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.WhiteSmoke)), bounds);
            }
            if (heat)
            {
                //graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Firebrick)), bounds);
            }
        }
    }

    public sealed class RoomType
    {
        public const byte
            Empty = 0,
            Simple = 1,
            Trapped = 2,
            Challenge = 4,
            Pillars = 5,
            Webbed = 6,
            MonsterDen = 7,
            Camp = 8,
            Lighted = 9,
            Dais = 10,
            Mausoleum = 11,
            Heated = 12;
    }
}
