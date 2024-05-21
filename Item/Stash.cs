using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTip = cotf.Base.ToolTip;
using cotf;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;

namespace cotf.Collections
{
    public class Stash : Entity
    {
        public Item stashItem;
        public List<Item> item = new List<Item>();
        public Item[] content = new Item[24];
        public bool open = false;
        public Stash()
        {
        }
        public void Init()
        {
            defaultColor = Color.Yellow;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] != null)
                { 
                    content[i].inStash = true;
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if (!discovered)
                discovered = Discovered(Main.myPlayer);
            if (!active)
                return;
            if (open && ticks == 1)
            {
                Main.myPlayer.OpenInventory(true);
                ticks = 0;
            }
            if (Main.mouseLeft && this.hitbox.Contains((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y) && InProximity(Main.myPlayer, LootRange))
            {
                Item.stash = this;
                open = true;
                ticks = 1;
            }
            if (content.All(t => t == null || (t != null && !t.active)))
            {
                foreach (var i in content)
                    i.lamp?.Dispose();
                item.Clear();
                Dispose();
            }
        }
        public void Draw(Graphics graphics)
        {
            if (!active || !discovered)
                return;
            Drawing.DrawScale(Main.texture90, position, width, height, Color.Transparent, graphics, Drawing.SetColor(Color.BurlyWood));
        }
        public bool Remove(Item item)
        {
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] != null && content[i].active && content[i] == item)
                {
                    content[i].active = false;
                    content[i] = null;
                    return true;
                }
            }
            return false;
        }
        public static int NewStash(int x, int y, short type, Item[] item, int owner = 255)
        {
            int num = Main.stash.Length - 1;
            for (int i = 0; i < Main.stash.Length; i++)
            {
                if (Main.stash[i] == null || !Main.stash[i].active)
                {
                    num = i;
                    break;
                }
            }
            Main.stash[num] = new Stash();
            Main.stash[num].active = true;
            Main.stash[num].position = new Vector2(x, y);
            Main.stash[num].content = item;
            Main.stash[num].width = 36;
            Main.stash[num].height = 36;
            Main.stash[num].type = type;
            Main.stash[num].whoAmI = num;
            Main.stash[num].owner = owner;
            Main.stash[num].Init();
            return num;
        }
        public override void Dispose()
        {
            if (Main.stash[whoAmI] != null)
            {
                Main.stash[whoAmI].active = false;
                Main.stash[whoAmI] = null;
            }
        }
    }
}
