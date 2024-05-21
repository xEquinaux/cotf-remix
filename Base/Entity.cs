using cotf.Buff;
using cotf.ID;
using cotf.World;
using Microsoft.Xna.Framework;
using System.Drawing.Imaging;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.Base
{
    public class Entity : Object, IDisposable
    {
        public string TextureName;
        public string Name { get; private set; }
        public string name = "";
        public string text = "";
        public virtual string Text()
        {
            return text;
        }
        public int SetNameLength()
        {
            return NameLen = Name.Length;
        }
        public string SetSuffix(string suffix)
        {
            return Name = Name.Substring(0, NameLen) + suffix;
        }
        public bool active = false;
        public bool 
            discovered = false,
            inShadow = true,
            hidden = false,
            solid = false,
            onScreen = false;
        public bool 
            hostile,
            friendly;
        public bool
            collide,
            colUp,
            colRight,
            colDown,
            colLeft;
        public int width, height;
        public int ticks;
        public int whoAmI;
        public int damage;
        public int 
            life, 
            lifeMax;
        public int iFrames;
        public int iFramesMax;
        public int floorNumber = 0;
        public int Sight => 150 + (Main.myPlayer.hasTorch() ? 50 : 0);
        public int NameLen
        {
            get; 
            private set;
        }
        public const int LootRange = 50;
        public short type;
        public float scale;
        public float alpha = 0f;
        public float knockBack = 1f;
        public float gamma = 1f;
        public Debuff debuff;
        public Purse purse;
        public Bitmap texture;
        public Color color;
        public Color defaultColor;
        public Color lightColor;
        public Color borderColor = Color.White;
        public Brush brush;
        public Rectangle box;
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 oldVelocity;
        public Bitmap preTexture;
        public List<Entity> objChild = new List<Entity>();
        public ImageAttributes colorTransform = new ImageAttributes();
        public virtual Rectangle hitbox => new Rectangle(X, Y, width, height);
        public Lamp lamp;
        public int owner = 255;
        public virtual Vector2 Center => new Vector2(X + width / 2, Y + height / 2);
        public virtual int X => (int)position.X;
        public virtual int Y => (int)position.Y;
        public Background parent => Background.GetSafely(X / Tile.Size, Y / Tile.Size);

        public override string ToString()
        {
            return $"Name:{name}, Active:{active}, Index:{whoAmI}";
        }
        public static Entity None => new Entity() { Name = "null", NameLen = "null".Length };
        public virtual bool PreUpdate(bool onScreenCheck = false)
        {
            if (!active)
                return false;
            if (preTexture == null)
                preTexture = (Bitmap)texture;
            Background bg = Background.GetSafely((int)Center.X / Tile.Size, (int)Center.Y / Tile.Size);
            if (bg == null || !bg.active)
                return false;
            inShadow = !(bg.lit && discovered);
            if (!discovered)
            {
                return discovered = Discovered(Main.myPlayer);
            }
            if (onScreenCheck)
            { 
                return onScreen =
                    position.X >= Main.myPlayer.position.X - Main.ScreenWidth / 2 &&
                    position.X <= Main.myPlayer.position.X + Main.ScreenWidth / 2 &&
                    position.Y >= Main.myPlayer.position.Y - Main.ScreenHeight / 2 &&
                    position.Y <= Main.myPlayer.position.Y + Main.ScreenHeight / 2;
            }
            return true;
        }
        public virtual void PostFX()
        {
            //gamma = (float)Helper.NormalizedRadius((float)Main.myPlayer.Center.Distance(this.Center), Main.ScreenWidth);
        }
        public virtual void Update()
        {
            if (!active)
                return;
            Background bg = Background.GetSafely((int)Center.X / Tile.Size, (int)Center.Y / Tile.Size);
            if (bg == null || !bg.active)
                return;
            debuff.Update(this);
            inShadow = !(bg.lit && discovered);
            if (!discovered)
                discovered = Discovered(Main.myPlayer);
        }
        public bool Discovered(Player player)
        {
            return Distance(player.Center) < Sight && SightLine(Center, player, Tile.Size / 5);
        }
        public void LampEffect(Lamp lamp)
        {
            if (!onScreen || !active)
                return;
            if (!solid && !Entity.SightLine(lamp.Center, this, Tile.Size / 3))
                return;
            float num = Background.RangeNormal(lamp.Center, this.Center, Tile.Range);
            if (num == 0f)
                return;
            alpha = 0f; //  DEBUG, experimental
            alpha += Math.Max(0, num);
            alpha = Math.Min(alpha, 1f);
            color = Ext.AdditiveV2(color, lamp.lampColor, num);
        }
        public void AddBuff(Debuff buff)
        {
            this.debuff = buff;
        }
        public void RemoveBuff()
        {
            this.debuff = Debuff.NewDebuff(DebuffID.None, 0);
        }
        public static Vector2 GetTileLine(Entity target, float angle, float step)
        {
            var v2 = target.Center + Helper.AngleToSpeed(angle, step);
            var tile = Tile.GetSafely((int)v2.X / Tile.Size, (int)v2.Y / Tile.Size);
            if (tile.Active)
            {
                return v2;
            }
            return Vector2.Zero;
        }
        public static bool SightLine(Lightmap map, Entity target, int step)
        {
            for (int n = 0; n < Helper.Distance(map.Center, target.Center); n += step)
            {
                var v2 = map.Center + Helper.AngleToSpeed(Helper.AngleTo(map.Center, target.Center), n);
                if (Tile.GetSafely((int)v2.X / Tile.Size, (int)v2.Y / Tile.Size).Active)
                {
                    return false;
                }
            }
            return true;
        }
        public bool SightLine(Vector2 target, int step)
        {
            for (int n = 0; n < Helper.Distance(Center, target); n += step)
            {
                var v2 = Center + Helper.AngleToSpeed(Helper.AngleTo(Center, target), n);
                if (Tile.GetSafely((int)v2.X / Tile.Size, (int)v2.Y / Tile.Size).Active)
                {
                    return false;
                }
            }
            return true;
        }
        public bool SightLine(Entity target)
        {
            for (int n = 0; n < Distance(target.Center); n += Tile.Size / 5)
            {
                var v2 = Center + Helper.AngleToSpeed(Helper.AngleTo(Center, target.Center), n);
                if (Tile.GetSafely((int)v2.X / Tile.Size, (int)v2.Y / Tile.Size).Active)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool SightLine(Vector2 from, Entity target, int step)
        {
            for (int n = 0; n < Helper.Distance(from, target.Center); n += step)
            {
                var v2 = from + Helper.AngleToSpeed(Helper.AngleTo(from, target.Center), n);
                if (Tile.GetSafely((int)v2.X / Tile.Size, (int)v2.Y / Tile.Size).Active)
                {
                    return false;
                }
            }
            return true;
        }
        public double Distance(Vector2 other)
        {
            return Math.Sqrt(Math.Pow(other.X - position.X, 2) + Math.Pow(other.Y - position.Y, 2));
        }
        public float AngleTo(Vector2 other)
        {
            return (float)Math.Atan2(other.Y - Y, other.X - X);
        }
        public float AngleTo(Vector2 from, Vector2 to)
        {
            return (float)Math.Atan2(to.Y - from.Y, to.X - from.X);
        }
        public Vector2 AngleToSpeed(float angle, float amount)
        {
            float cos = (float)(amount * Math.Cos(angle));
            float sine = (float)(amount * Math.Sin(angle));
            return new Vector2(cos, sine);
        }
        internal Color OpaqueColorShift(Color newColor, float distance)
        {
            return Color.FromArgb(
                color.A,
                (int)Math.Max(Math.Min(color.R * (newColor.R / 255f) * distance, 255), 0),
                (int)Math.Max(Math.Min(color.G * (newColor.G / 255f) * distance, 255), 0),
                (int)Math.Max(Math.Min(color.B * (newColor.B / 255f) * distance, 255), 0));
        }
        internal Color FullColorShift(Color newColor, float distance)
        {
            return Color.FromArgb(
                (int)Math.Max(Math.Min(color.A * distance, 255), 0),
                (int)Math.Max(Math.Min(color.R * (newColor.R / 255f) * distance, 255), 0),
                (int)Math.Max(Math.Min(color.G * (newColor.G / 255f) * distance, 255), 0),
                (int)Math.Max(Math.Min(color.B * (newColor.B / 255f) * distance, 255), 0));
        }
        internal Color FullColorShift(Color color, Color newColor, float distance)
        {
            return Color.FromArgb(
                (int)Math.Max(Math.Min(color.A * distance, 255), 0),
                (int)Math.Max(Math.Min(color.R * (newColor.R / 255f * distance), 255), 0),
                (int)Math.Max(Math.Min(color.G * (newColor.G / 255f * distance), 255), 0),
                (int)Math.Max(Math.Min(color.B * (newColor.B / 255f * distance), 255), 0));
        }
        public static float RangeNormal(float value, float range = 100f)
        {
            return Math.Max((value * -1f + range) / range, 0);
        }
        public static float RangeNormal(Vector2 to, Vector2 from, float range = 100f)
        {
            return Math.Max(((float)Helper.Distance(from, to) * -1f + range) / range, 0);
        }
        [Obsolete("Draw priority fixed in Drawing helper methods")]
        public Color DynamicTorch(Color newColor, float range, ColorTranslate type)
        {
            var list = Main.lamp.ToList();
            list.RemoveAll(t => t == null);
            var lamp = list.OrderBy(t => t.Distance(this.Center)).FirstOrDefault(t => t != null && t.active);
            if (lamp == default || !lamp.active || (lamp.owner == Main.myPlayer.whoAmI && !Main.myPlayer.hasTorch()))
                return Color.Transparent;
            float num = RangeNormal(Center, lamp.Center, range);
            switch (type)
            { 
                case ColorTranslate.Opaque:
                    return OpaqueColorShift(newColor, num);
                case ColorTranslate.Translucent:
                    return FullColorShift(newColor, num);
                default:
                    return newColor;
            }
        }
        private SolidBrush Opacity(Color color, float value)
        {  
            byte min = (byte)(Math.Min(value, 1f) * 255);
            return new SolidBrush(Color.FromArgb(min, color.R, color.G, color.B));
        }
        public bool InProximity(Entity e, float radius)
        {
            return Center.X >= e.Center.X - radius &&
                Center.X <= e.Center.X + radius &&
                Center.Y >= e.Center.Y - radius &&
                Center.Y <= e.Center.Y + radius;
        }
        public virtual bool Collision(Entity e, int buffer = 4)
        {
            if (!active)
                return false;

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

            return e.colUp || e.colRight || e.colDown || e.colLeft;
        }
        public static void DrawChain(Image texture, Entity ent, Vector2 target, double angle, Graphics graphics, bool collision = false)
        {
            for (int n = 0; n < Helper.Distance(ent.Center, target); n += texture.Width - 1)
            {
                double f = 1d;
                double cos = ent.Center.X + n * Math.Cos(angle);
                double sine = ent.Center.Y + n * Math.Sin(angle);
                if (collision)
                {
                    var tile = Tile.GetSafely((float)cos, (float)sine);
                    if (tile != null && tile.Active && tile.solid && tile.hitbox.Contains((int)cos, (int)sine))
                    {
                        return;
                    }
                }
                Drawing.DrawRotate(texture, new Vector2((float)cos, (float)sine),
                    new RectangleF(0, 0, texture.Width, (int)(texture.Width * f)), Helper.ToDegrees((float)angle - Helper.Radian * 90f), new PointF(texture.Width / 2, texture.Height / 2), Color.Black, RotateType.GraphicsTransform, graphics);
            }
        }
        public static void DrawChain(Image texture, Entity ent, Vector2 target, Graphics graphics, bool collision = false)
        {
            Vector2 limit = Vector2.Zero;
            for (int n = 0; n < Helper.Distance(ent.Center, target); n += texture.Width - 1)
            {
                double f = 1d;
                float angle = Helper.AngleTo(ent.Center, target);
                double cos = ent.Center.X + n * Math.Cos(angle);
                double sine = ent.Center.Y + n * Math.Sin(angle);
                if (collision)
                {
                    var tile = Tile.GetSafely((float)cos, (float)sine);
                    if (tile != null && tile.Active && tile.solid && tile.hitbox.Contains((int)cos, (int)sine))
                    {
                        cos = ent.Center.X + n * Math.Cos(angle);
                        sine = ent.Center.Y + n * Math.Sin(angle);
                        limit = new Vector2((float)cos, (float)sine);
                        return;
                    }
                }
                //if (flag) f = Helper.Distance(ent.Center, limit) % texture.Width / texture.Width;
                Drawing.DrawRotate(texture, new Vector2((float)cos, (float)sine),
                    new RectangleF(0, 0, texture.Width, (int)(texture.Width * f)), Helper.ToDegrees(angle - Helper.Radian * 90f), new PointF(texture.Width / 2, texture.Height / 2), Color.Black, RotateType.GraphicsTransform, graphics);
            }
        }
        public static void DrawChain(Image texture, Entity ent, Vector2 target, Graphics graphics, float rangeLimit = 300f, bool collision = false)
        {
            Vector2 limit = Vector2.Zero;
            for (int n = 0; n < Helper.Distance(ent.Center, target); n += texture.Width - 1)
            {
                if (n > rangeLimit)
                    return;
                double f = 1d;
                float angle = Helper.AngleTo(ent.Center, target);
                double cos = ent.Center.X + n * Math.Cos(angle);
                double sine = ent.Center.Y + n * Math.Sin(angle);
                if (collision)
                {
                    var tile = Tile.GetSafely((float)cos, (float)sine);
                    if (tile != null && tile.Active && tile.solid && tile.hitbox.Contains((int)cos, (int)sine))
                    {
                        cos = ent.Center.X + n * Math.Cos(angle);
                        sine = ent.Center.Y + n * Math.Sin(angle);
                        limit = new Vector2((float)cos, (float)sine);
                        return;
                    }
                }
                //if (flag) f = Helper.Distance(ent.Center, limit) % texture.Width / texture.Width;
                Drawing.DrawRotate(texture, new Vector2((float)cos, (float)sine),
                    new RectangleF(0, 0, texture.Width, (int)(texture.Width * f)), Helper.ToDegrees(angle - Helper.Radian * 90f), new PointF(texture.Width / 2, texture.Height / 2), Color.Black, RotateType.GraphicsTransform, graphics);
            }
        }
        public static void DrawChain(Image texture, Entity ent, Tile tile, Graphics graphics)
        {
            for (int n = 0; n < tile.Distance(ent.Center); n += texture.Width)
            {
                double f = 1f;
                if (n > tile.Distance(ent.Center) - texture.Width)
                    f = (tile.Distance(ent.Center) - n) / texture.Width;
                float angle = tile.AngleTo(ent.Center);
                double cos = tile.X + n * Math.Cos(angle);
                double sine = tile.Y + n * Math.Sin(angle);
                Drawing.DrawRotate(texture, new Vector2((float)cos, (float)sine),
                    new RectangleF(0, 0, texture.Width, (int)(texture.Width * f)), Helper.ToDegrees(angle - Helper.Radian * 90f), PointF.Empty, Color.Black, RotateType.GraphicsTransform, graphics);
            }
        }
        public static void DrawChain(Image texture, Entity ent, Tile[] ground, Graphics graphics)
        {
            for (int i = 0; i < ground.Length; i++)
            {
                for (int n = 0; n < ground[i].Distance(ent.Center); n += texture.Width)
                {
                    double f = 1f;
                    if (n > ground[i].Distance(ent.Center) - texture.Width)
                        f = (ground[i].Distance(ent.Center) - n) / texture.Width;
                    float angle = ground[i].AngleTo(ent.Center);
                    double cos = ground[i].X + n * Math.Cos(angle);
                    double sine = ground[i].Y + n * Math.Sin(angle);
                    Drawing.DrawRotate(texture, new Vector2((float)cos, (float)sine), 
                        new RectangleF(0, 0, texture.Width, (int)(texture.Width * f)), Helper.ToDegrees(angle - Helper.Radian * 90f), PointF.Empty, Color.Black, RotateType.GraphicsTransform, graphics);
                }
            }
        }
        public virtual void Dispose()
        {
        }
    }
    public enum ColorTranslate
    {
        Opaque,
        Translucent
    }
}
