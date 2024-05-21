using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.Base
{
    public class Lightmap : IDisposable
    {
        public bool active;
        private int i, j;
        public Color color;
        public Color DefaultColor = _DefaultColor;
        public static Color _DefaultColor => Color.FromArgb(255, 20, 20, 20);
        public Vector2 position;
        public Vector2 Center => position + new Vector2(Size.Width / 2, Size.Height / 2);
        public Rectangle Hitbox => new Rectangle((int)position.X, (int)position.Y, Size.Width, Size.Height);
        public static Size Size => new Size(Tile.Size, Tile.Size);
        public Entity parent;
        public float alpha;
        bool keepLit = false;
        public int ScaleX, ScaleY;
        private Lightmap()
        {
        }
        public Lightmap(int i, int j)
        {
            this.i = i;
            this.j = j;
            position = new Vector2(i * Size.Width, j * Size.Height);
            active = true;
            color = DefaultColor;
        }
        public bool onScreen =>
                position.X >= Main.myPlayer.position.X - Main.ScreenWidth / 2 &&
                position.X <= Main.myPlayer.position.X + Main.ScreenWidth / 2 &&
                position.Y >= Main.myPlayer.position.Y - Main.ScreenHeight / 2 &&
                position.Y <= Main.myPlayer.position.Y + Main.ScreenHeight / 2;
        public Color Update(Entity ent)
        {
            if (!active)
                return ent.color;
            ent.color = color;
            if (parent == null)
            {
                parent = ent;
            }
            return color;
        }
        public Color Update()
        {
            if (!active)
                return DefaultColor;
            return color;
        }
        public void LampEffect(Lamp lamp)
        {
            float num = 0;
            if (!onScreen || !active)
                return;
            // Ignore world lamps due to prerendered lighting
            if (lamp.owner == 255 && parent != null && parent.GetType() == typeof(Background))
            {
                num = keepLit ? 0.5f : Background.RangeNormal(lamp.Center, this.Center, Tile.Range);
                if (num == 0f)
                    return;
                alpha = 0f;
                alpha += Math.Max(0, num);
                alpha = Math.Min(alpha, 1f);
                color = Ext.AdditiveV2(color, lamp.lampColor, num / 2f);
                return;
            }
            if (parent != null && !parent.solid && !Entity.SightLine(lamp.Center, parent, Tile.Size / 5)) 
                return;
            num = keepLit ? 0.5f : Background.RangeNormal(lamp.Center, this.Center, Tile.Range);
            if (num == 0f)
                return;
            alpha = 0f;
            alpha += Math.Max(0, num);
            alpha = Math.Min(alpha, 1f);
            color = Ext.AdditiveV2(color, lamp.lampColor, num);
        }
        public static Lightmap GetSafely(int x, int y)
        {
            return Main.lightmap[Math.Max(0, Math.Min(x, Main.WorldHeight / Tile.Size - 1)), Math.Max(0, Math.Min(y, Main.WorldWidth / Tile.Size - 1))];
        }
        public void Dispose()
        {
            if (Main.lightmap[i, j] != null)
            {
                Main.lightmap[i, j].active = false;
                Main.lightmap[i, j] = null;
            }
        }
        public override string ToString()
        {
            return $"Color:{color}, Alpha:{alpha}, Active:{active}";
        }
    }
}
