using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf
{
    public class Lamp : Entity
    {
        public Entity parent;
        public float range;
        public List<Lighting> light = new List<Lighting>();
        public static Color TorchLight => Color.Orange;
        public Rectangle lightBox => new Rectangle((int)(position.X - range), (int)(position.Y - range), width + (int)range * 2, height + (int)range * 2);
        public override Vector2 Center => new Vector2(position.X, position.Y);
        bool onScreen = false;
        public Color lampColor = TorchLight;
        public bool isProj = false;
        public Item itemLink = null;
        public bool staticLamp;
        public Lamp(float range)
        {               
            this.range = range;
        }
        public Lamp(Tile parent, float range)
        {
            this.parent = parent;
            this.range = range;
            this.active = true;
        }
        protected void Init(Item item)
        {
            itemLink = item;
        }
        private bool PreUpdate()
        {
            return onScreen =
                Center.X >= Main.myPlayer.position.X - Main.ScreenWidth / 2 &&
                Center.X <= Main.myPlayer.position.X + Main.ScreenWidth / 2 &&
                Center.Y >= Main.myPlayer.position.Y - Main.ScreenHeight / 2 &&
                Center.Y <= Main.myPlayer.position.Y + Main.ScreenHeight / 2;
        }
        public new void Update()
        {
            if (!PreUpdate())
                return;
            if (parent != null && owner != 255)
            {
                this.position = parent.Center;
            }
            if ((!staticLamp && (itemLink == null || !itemLink.active) && owner == 255) || (Main.myPlayer.whoAmI == owner && !Main.myPlayer.hasTorch()))
            {
                active = false;
            }
        }
        public void Draw(Graphics graphics)
        {
            if (!active || !onScreen)
                return;
        }
        public void WorldLighting()
        {
            if (!active || !onScreen)
                return;
        }
        public void PreDraw(Graphics graphics)
        {
            return;
            #region Legacy drawing
            if (!active || !onScreen || (!isProj && this.owner == Main.myPlayer.whoAmI && !Main.myPlayer.hasTorch()))
                return;
            foreach (Npc npc in Main.npc)
            {
                if (npc == null || !npc.discovered || !npc.active || !SightLine(Center, npc, Tile.Size / 3))
                    continue;
                float num = RangeNormal(npc.Center, this.Center, range);
                var map = Lightmap.GetSafely(npc.X / Tile.Size, npc.Y / Tile.Size);
                npc.alpha += Math.Max(0, num);
                npc.alpha = Math.Min(alpha, 1f);
                npc.color = Ext.AdditiveV2(map.color, lampColor, num);
            }
            foreach (Loot i in Main.loot)
            {
                if (i == null || !i.active || i.item == null || !i.item.active || !SightLine(Center, i, Tile.Size / 3))
                    continue;
                Item item = i.item;
                float num = RangeNormal(item.Center, this.Center, range);
                var map = Lightmap.GetSafely(item.X / Tile.Size, item.Y / Tile.Size);
                item.alpha += Math.Max(0, num);
                item.alpha = Math.Min(alpha, 1f);
                item.color = Ext.AdditiveV2(map.color, lampColor, num);
            }
            foreach (Item t in Main.item)
            {
                if (t == null || !t.active || t.equipped || t.owner != 255)
                    continue;
                Lightmap map;
                Item item = t;
                float num = RangeNormal(item.Center, this.Center, range);
                map = Lightmap.GetSafely(item.X / Tile.Size, item.Y / Tile.Size);
                item.alpha += Math.Max(0, num);
                item.alpha = Math.Min(alpha, 1f);
                item.color = Ext.AdditiveV2(map.color, lampColor, num);
            }
            foreach (Scenery s in Main.scenery)
            {
                if (s == null || !s.active)
                    continue;
                Lightmap map;
                if (!SightLine(Center, s, Tile.Size / 3))
                {
                    map = Lightmap.GetSafely(s.X / Tile.Size, s.Y / Tile.Size);
                    s.color = Ext.AdditiveV2(map.color, lampColor, 1f);
                    continue;
                }
                float num = RangeNormal(s.Center, this.Center, range);
                map = Lightmap.GetSafely(s.X / Tile.Size, s.Y / Tile.Size);
                s.alpha += Math.Max(0, num);
                s.alpha = Math.Min(alpha, 1f);
                s.color = Ext.AdditiveV2(map.color, lampColor, num);
            }
            foreach (World.Traps.Trap t in Main.trap)
            {
                if (t == null || !t.active || !SightLine(Center, t, Tile.Size / 3))
                    continue;
                float num = RangeNormal(t.Center, this.Center, range);
                var map = Lightmap.GetSafely(t.X / Tile.Size, t.Y / Tile.Size);
                t.alpha += Math.Max(0, num);
                t.alpha = Math.Min(alpha, 1f);
                t.color = Ext.AdditiveV2(map.color, lampColor, num);
            }
            #endregion
        }
        public void PostDraw(Graphics graphics)
        {
            return;
            #region Legacy drawing
            if (itemLink != null && owner == Main.myPlayer.whoAmI && !itemLink.equipped)
                return;
            float num = RangeNormal(Main.myPlayer.Center, this.Center, range);
            if (num == 0f)
                return;
            Main.myPlayer.alpha += Math.Min(1f, num);
            Main.myPlayer.color = Ext.Multiply(Main.myPlayer.color, lampColor, num);
            #endregion
        }
        public static Color RandomLight()
        {
            int len = Enum.GetNames<KnownColor>().Length;
            KnownColor c = Enum.Parse<KnownColor>(Enum.GetNames(typeof(KnownColor))[Main.rand.Next(len)]);
            return Color.FromKnownColor(c);
        }
        public static int AddLamp(Lamp lamp)
        {
            int num = Main.lamp.Length - 1;
            for (int i = 0; i < Main.lamp.Length; i++)
            {
                if (Main.lamp[i] == null)
                {
                    num = i;
                    break;
                }
            }
            Main.lamp[num] = lamp;
            Main.lamp[num].whoAmI = num;
            return num;
        }
        public static int NewLamp(int x, int y, float range, bool staticLamp = false, int owner = 255)
        {
            int num = Main.lamp.Length - 1;
            for (int i = 0; i < Main.lamp.Length; i++)
            {
                if (Main.lamp[i] == null)
                {
                    num = i;
                    break;
                }
            }
            Main.lamp[num] = new Lamp(range);
            Main.lamp[num].active = true;
            Main.lamp[num].position = new Vector2(x, y);
            Main.lamp[num].range = range;
            Main.lamp[num].whoAmI = num;
            Main.lamp[num].owner = owner;
            Main.lamp[num].lampColor = RandomLight();
            Main.lamp[num].staticLamp = staticLamp;
            return num;
        }
        public static int NewLamp(int x, int y, float range, Item item, int owner = 255)
        {
            int num = Main.lamp.Length - 1;
            for (int i = 0; i < Main.lamp.Length; i++)
            {
                if (Main.lamp[i] == null)
                {
                    num = i;
                    break;
                }
            }
            Main.lamp[num] = new Lamp(range);
            Main.lamp[num].active = true;
            Main.lamp[num].position = new Vector2(x, y);
            Main.lamp[num].range = range;
            Main.lamp[num].whoAmI = num;
            Main.lamp[num].owner = owner;
            Main.lamp[num].lampColor = RandomLight();
            Main.lamp[num].itemLink = item;
            return num;
        }
        public static int NewLamp(int x, int y, float range, Tile parent, bool staticLamp = true, int owner = 255)
        {
            int num = Main.lamp.Length - 1;
            for (int i = 0; i < Main.lamp.Length; i++)
            {
                if (Main.lamp[i] == null)
                {
                    num = i;
                    break;
                }
            }
            Main.lamp[num] = new Lamp(range);
            Main.lamp[num].active = true;
            Main.lamp[num].position = new Vector2(x, y);
            Main.lamp[num].parent = parent;
            Main.lamp[num].range = range;
            Main.lamp[num].whoAmI = num;
            Main.lamp[num].owner = owner;
            Main.lamp[num].staticLamp = staticLamp;
            return num;
        }
        public static int NewLamp(Vector2 position, float range, Color color, Entity parent, bool staticLamp, int owner = 255)
        {
            int num = Main.lamp.Length - 1;
            for (int i = 0; i < Main.lamp.Length; i++)
            {
                if (Main.lamp[i] == null)
                {
                    num = i;
                    break;
                }
            }
            Main.lamp[num] = new Lamp(range);
            Main.lamp[num].active = true;
            Main.lamp[num].position = position;
            Main.lamp[num].parent = parent;
            Main.lamp[num].range = range;
            Main.lamp[num].whoAmI = num;
            Main.lamp[num].lampColor = color;
            Main.lamp[num].owner = owner;
            Main.lamp[num].staticLamp = staticLamp;
            return num;
        }
        public override void Dispose()
        {
            if (Main.lamp[whoAmI]?.owner < 255)
                return;
            if (Main.lamp[whoAmI]?.itemLink?.owner < 255)
                return;
            if (Main.lamp[whoAmI] != null)
            { 
                Main.lamp[whoAmI].active = false;
                Main.lamp[whoAmI].position = Vector2.Zero;
                Main.lamp[whoAmI] = null;
            }
        }
    }
}
