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

namespace cotf.World
{
    public class Door : Tile
    {
        public bool locked;
        public Direction direction;
        public Door()
        {
        }
        private void Init()
        {
            texture = preTexture = new Bitmap(width, height);
            using (Graphics gfx = Graphics.FromImage(preTexture))
                gfx.FillRectangle(Brushes.BurlyWood, new Rectangle(0, 0, width, height));
        }
        public override void Update()
        {
            base.Update();
        }
        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
        }
        public static int CreateDoor(bool locked, Tile tile, Direction direction, short type = DoorType.Default)
        {
            int num = Main.door.Length - 1;
            for (int n = 0; n < Main.door.Length; n++)
            {
                if (Main.door[n] == null || !Main.door[n].Active)
                {
                    num = n;
                    break;
                }
            }
            Main.door[num] = Tile.Convert(tile);
            Main.door[num].color = Color.BurlyWood;
            Main.door[num].whoAmI = num;
            Main.door[num].type = type;
            Main.door[num].locked = locked;
            Main.door[num].solid = locked;
            Main.door[num].active(true);
            Main.door[num].direction = direction;
            Main.door[num].defaultColor = Color.BurlyWood;
            Main.door[num].Init();
            return num;
        }
        public override void Dispose()
        {
            Main.door[whoAmI]?.active(false);
            Main.door[whoAmI] = null;
        }
    }
    public enum Direction
    {
        None,
        Top,
        Right,
        Bottom,
        Left
    }
    public sealed class DoorType
    {
        public const short
            Default = 0;
    }
}
