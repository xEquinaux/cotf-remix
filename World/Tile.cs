using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using cotf.Base;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    public class Tile : Entity
    {
        private bool lit = false;
        internal bool Active;
        public const int Size = 50;
        public int i;
        public int j;
        public new int X;
        public new int Y;
        public const float Range = 150f;
        public new Rectangle hitbox;
        private new Brush brush = Brushes.White;
        public new Vector2 position => new Vector2(X, Y);
        private Point point => new Point(hitbox.X + Size / 2, hitbox.Y + Size / 2);
        public override Vector2 Center => new Vector2(X + Size / 2, Y + Size / 2);
        internal bool onScreen = false;
        private Random rand => Main.rand;
        public Tile()
        {
        }
        public Tile(int i, int j)
        {
            X = i;
            Y = j;
            this.i = i / Size;
            this.j = j / Size;
            active(true);
            width = Size;
            height = Size;
            color = Color.White;
            solid = true;
            Init();
        }
        public new bool active(bool active)
        {
            return Active = active;
        }
        private SolidBrush Opacity(byte value = 125)
        {
            return new SolidBrush(Color.FromArgb(value, color.R, color.G, color.B));
        }
        private void Init()
        {
            owner = 255;
            defaultColor = Color.LightGray;

            //  Brush style texture init
            Bitmap bmp = new Bitmap(50, 50);
            using (Graphics gfx = Graphics.FromImage(bmp))
                gfx.FillRectangle(Brushes.LightGray, new Rectangle(0, 0, 50, 50));
            texture = preTexture = bmp;

            return;
            //  Debugging
            int num = Main.tile.Length - 1;
            for (int i = 0; i < Main.tile.GetLength(0); i++)
            {
                for (int j = 0; j < Main.tile.GetLength(1); j++)
                {
                    if (Main.tile[i, j] == null || !Main.tile[i, j].Active)
                    {
                        whoAmI = num;
                        return;
                    }
                }
            }
        }
        private bool PreUpdate()
        {
            hitbox = new Rectangle(X, Y, width, height);
            //if (lamp != null)
            //    lamp.position = Center;
            return onScreen =
                Center.X >= Main.myPlayer.position.X - Main.ScreenWidth / 2 &&
                Center.X <= Main.myPlayer.position.X + Main.ScreenWidth / 2 &&
                Center.Y >= Main.myPlayer.position.Y - Main.ScreenHeight / 2 &&
                Center.Y <= Main.myPlayer.position.Y + Main.ScreenHeight / 2;
        }
        public override void Update()
        {
            //lit = Main.Distance(player.Center, Center) < Range;
            if (!PreUpdate())
                return;
            if (!discovered)
                discovered = Helper.Distance(Center, Main.myPlayer.Center) < Sight;
        }
        public virtual void Draw(Graphics graphics)
        {
            if (Active && onScreen && discovered)
            {
                if (preTexture == null)
                    return;
                base.PostFX();
                Lightmap map;
                if (alpha > 0f)
                {
                    //  Lightmap interaction
                    (map = Main.lightmap[hitbox.X / Tile.Size, hitbox.Y / Tile.Size]).Update(this);
                    Drawing.TextureLighting(preTexture, hitbox, map, this, gamma, alpha, graphics);
                }
                if (alpha < 1f)
                {
                    alpha += 1f / 10f;
                }
                //graphics.DrawImage(Drawing.Lightpass0(preTexture, new Lightmap(10, 10), position, Main.myPlayer.lamp), hitbox);
                //Drawing.BrushLighting(new Lightmap(10, 10), this, Main.myPlayer.lamp, graphics);
            }
        }
        public static Tile GetSafely(int i, int j)
        {
            //if (i < Main.tile.GetLength(0) && i >= 0 && j < Main.tile.GetLength(1) && j >= 0)
            return Main.tile[Math.Max(Math.Min(i, Main.tile.GetLength(0) - 1), 0), Math.Max(Math.Min(j, Main.tile.GetLength(1) - 1), 0)];
            //throw new OutOfBoundsException(i, j);
        }
        public static Tile GetSafely(float x, float y)
        {
            return Main.tile[(int)Math.Max(Math.Min(x / Size, Main.WorldWidth / Size - 1), 0), (int)Math.Max(Math.Min(y / Size, Main.WorldHeight / Size - 1), 0)];
        }
        public static Tile GetSafely(int i, int j, int width, int height, Tile[,] array)
        {
            return array[Math.Max(Math.Min(i, width / Size - 1), 0), Math.Max(Math.Min(j, height / Size - 1), 0)];
        }
        public new void Collision(Entity e, int buffer = 4)
        {
            if (!Active || !solid) 
                return;

            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X, (int)e.position.Y, e.width, e.height)))
                e.collide = true;
            //  Directions
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X, (int)e.position.Y - buffer, e.width, 2)))
                e.colUp = true;
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X, (int)e.position.Y + e.height + buffer, e.width, 2)))
                e.colDown = true;
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X + e.width + buffer, (int)e.position.Y, 2, e.height)))
                e.colRight = true;
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X - buffer, (int)e.position.Y, 2, e.height)))
                e.colLeft = true;
        }
        public override string ToString()
        {
            return $"Name:{name}, Active:{Active}, i:{i}, j:{j}";
        }
        public static Tile Clone(Tile copy)
        {
            Tile tile = new Tile();
            tile.i = copy.i;
            tile.j = copy.j;
            tile.X = copy.X;
            tile.Y = copy.Y;
            tile.active(copy.Active);
            tile.color = copy.color;
            tile.hitbox = copy.hitbox;
            return tile;
        }
        public Tile Clone()
        {
            Tile tile = new Tile();
            tile.i = i;
            tile.j = j;
            tile.X = X;
            tile.Y = Y;
            tile.active(Active);
            tile.color = color;
            tile.hitbox = hitbox;
            return tile;
        }
        public static Door Convert(Tile copy)
        {
            Door tile = new Door();
            tile.i = copy.i;
            tile.j = copy.j;
            tile.X = copy.X;
            tile.Y = copy.Y;
            tile.width = copy.width;
            tile.height = copy.height;
            tile.active(copy.Active);
            tile.color = copy.color;
            tile.hitbox = copy.hitbox;
            return tile;
        }
        public override void Dispose()
        {
            this.active(false);
            this.X = 0;
            this.Y = 0;
            if (Main.tile[i, j] != null)
            {
                Main.tile[i, j].active(false);
                Main.tile[i, j] = null;
            }
        }
    }
    sealed class OutOfBoundsException : Exception
    {
        int i, j;
        public OutOfBoundsException(int i, int j)
        {
            this.i = i;
            this.j = j;
        }
        public override string Message => $"Attempting to access an index of: i:{i}, j:{j} outside the array.";
    }
}
