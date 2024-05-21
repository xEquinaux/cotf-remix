using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using cotf.Collections;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    public class Staircase : IDisposable
    {
        #region variables
        public bool active = false;
        public Rectangle hitbox => new Rectangle((int)position.X, (int)position.Y, width, height); 
        public Vector2 position
        {
            get { return new Vector2(X, Y); }
            set 
            { 
                X = (int)value.X;
                Y = (int)value.Y;
            }
        }
        public Vector2 Center => new Vector2(position.X + width / 2, position.Y + height / 2);
        public int X
        {
            get;
            set;
        }
        public int Y
        {
            get; 
            set;
        }
        public int width = Tile.Size;
        public int height = Tile.Size;
        public StaircaseDirection direction;
        #endregion
        public int whoAmI;
        public bool discovered = false;
        Brush brush = Brushes.CadetBlue;
        Bitmap texture;
        public Staircase link;
        public Staircase()
        {
        }
        private bool PreUpdate()
        {
            Bitmap image = new Bitmap(width, height);
            using (Graphics gfx  = Graphics.FromImage(image))
                gfx.FillRectangle(brush, new Rectangle(0, 0, width, height));
            texture = image;
            if (!discovered)
                discovered = Main.myPlayer.Discovered(Center);
            return active;
        }
        public void Update()
        {
            if (!PreUpdate())
                return;
            if (X % Tile.Size != 0)
                X++;
            if (Y % Tile.Size != 0)
                Y++;
        }
        public void Draw(Graphics graphics)
        {
            if (!active || !discovered || texture == null)
                return;
            graphics.DrawImage(texture, hitbox);
        }
        public static int NewStaircase(int x, int y, StaircaseDirection direction)
        {
            int num = Main.staircase.Length - 1;
            for (int i = 0; i < Main.staircase.Length; i++)
            {
                if (Main.staircase[i] == null || !Main.staircase[i].active)
                {
                    num = i;
                    break;
                }
            }
            Main.staircase[num] = new Staircase();
            Main.staircase[num].active = true;
            Main.staircase[num].direction = direction;
            Main.staircase[num].position = new Vector2(x, y);
            Main.staircase[num].whoAmI = num;
            Main.staircase[num].SetPosition();
            return num;
        }
        public static int NewStaircase(int x, int y, Staircase link, StaircaseDirection direction)
        {
            int num = Main.staircase.Length - 1;
            for (int i = 0; i < Main.staircase.Length; i++)
            {
                if (Main.staircase[i] == null || !Main.staircase[i].active)
                {
                    num = i;
                    break;
                }
            }
            Main.staircase[num] = new Staircase();
            Main.staircase[num].active = true;
            Main.staircase[num].direction = direction;
            Main.staircase[num].position = new Vector2(x, y);
            Main.staircase[num].link = link;
            Main.staircase[num].whoAmI = num;
            Main.staircase[num].SetPosition();
            return num;
        }
        private void SetPosition()
        {
            for (int i = 0; i < Main.scenery.Length; i++)
            {
                if (Main.scenery[i] != null && Main.scenery[i].active && Main.scenery[i].hitbox.IntersectsWith(hitbox))
                {
                    Main.scenery[i].Dispose();
                }
            }
        }
        public bool InProximity(Entity e, float radius)
        {
            return Center.X >= e.Center.X - radius &&
                Center.X <= e.Center.X + radius &&
                Center.Y >= e.Center.Y - radius &&
                Center.Y <= e.Center.Y + radius;
        }
        public void Dispose()
        {
            if (Main.staircase[whoAmI] != null)
            {
                Main.staircase[whoAmI].X = 0;
                Main.staircase[whoAmI].Y = 0;
                Main.staircase[whoAmI].direction = StaircaseDirection.None;
                Main.staircase[whoAmI].active = false;
                Main.staircase[whoAmI] = null;
            }
        }
    }
    public enum StaircaseDirection : byte
    {
        None,
        LeadingUp,
        LeadingDown
    }
}
