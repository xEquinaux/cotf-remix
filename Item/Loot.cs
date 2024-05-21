using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Assets;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;

namespace cotf
{
    public class Loot : Entity
    {
        public Item item;
        public List<Item> pile = new List<Item>();
        public Loot()
        {
        }
        public void Init()
        {
            TextureName = "loot0";
            texture = preTexture = Asset<Bitmap>.Request("loot0");
        }
        public override void Update()
        {
            if (!discovered)
                discovered = Discovered(Main.myPlayer);
            base.Update();
            pile.RemoveAll(t => t == null || !t.active || t.inStash || string.IsNullOrWhiteSpace(t.name));
        }
        public static Loot NewLoot(Item item)
        {
            int num = Main.loot.Length - 1;
            for (int i = 0; i < Main.loot.Length; i++)
            {
                if (Main.loot[i] == null || !Main.loot[i].active)
                {
                    num = i;
                    break;
                }
            }
            Main.loot[num] = new Loot();
            Main.loot[num].whoAmI = num;
            Main.loot[num].active = true;
            Main.loot[num].position = item.position;
            Main.loot[num].item = item.DeepClone(true);
            Main.loot[num].item.owner = 255;
            Main.loot[num].hidden = Main.rand.NextFloat() < 0.25f && item.type != ItemID.Torch;
            Main.loot[num].Init();
            return Main.loot[num];
        }
        public static Loot NewPile(int x, int y, Item[] item)
        {
            int num = Main.loot.Length - 1;
            for (int i = 0; i < Main.loot.Length; i++)
            {
                if (Main.loot[i] == null || !Main.loot[i].active)
                {
                    if (Main.loot.Any(t => t != null && t.whoAmI != i && t.Distance(new Vector2(x, y)) <= Item.PileDistance))
                        return default(Loot);
                    num = i;
                    break;
                }
            }
            Main.loot[num] = new Loot();
            Main.loot[num].whoAmI = num;
            Main.loot[num].active = true;
            Main.loot[num].width = 42;
            Main.loot[num].height = 42;
            Main.loot[num].position = new Vector2(x, y);
            Main.loot[num].pile = item.ToList();
            Main.loot[num].Init();
            return Main.loot[num];
        }
        public void Draw(Graphics graphics)
        {
            if (!active || !discovered /*|| item == null || (item.type != ItemID.Torch && item.hidden)*/)
                return;
            if (pile.Count > 0)
            {
                Drawing.LightmapHandling(texture, this, gamma, graphics);
                //  Original
                //Drawing.DrawScale(Main.texture90, position, width, height, Color.Black, graphics, Drawing.SetColor(Color.LightYellow));
            }
            return; //  For a player Search technique for finding hidden entities
            if (!pile.Contains(item) && item.discovered && item.owner == 255 && !item.equipped)
            {
                if (hidden)
                {
                    if (!Main.myPlayer.hasTorch())
                        return;
                    if (!InProximity(Main.myPlayer, Sight))
                        return;
                }
                Drawing.LightmapHandling(item.texture, this, gamma, graphics);
            }
        }
        public override void Dispose()
        {
            if (Main.loot[whoAmI] != null)
            { 
                Main.loot[whoAmI].active = false;
                Main.loot[whoAmI] = null;
            }
        }
    }
}
