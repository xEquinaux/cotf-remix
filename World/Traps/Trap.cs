using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using cotf.Assets;
using cotf.Base;
using cotf.ID;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using ToolTip = cotf.Base.ToolTip;

namespace cotf.World.Traps
{
    public class Trap : Entity
    {
        public float rotation;
        public int elapse;
        public Trap()
        {
        }
        public virtual ToolTip ToolTip()
        {
            return new ToolTip(name, text, color);
        }
        public virtual void SetDefaults()
        {
            TextureName = $"trap{0}";
            texture = preTexture = Asset<Bitmap>.Request(TextureName);
        }
        protected virtual void Init()
        {
            lifeMax = life;
        }
        protected void _init()
        {
            defaultColor = Color.White;
            texture = (Bitmap)Main.trapTexture[type];
            texture = Asset<Bitmap>.Request(TextureName);
            preTexture = (Bitmap)texture;
        }
        public virtual bool ItemHit(Item item)
        {
            return iFrames == 0 && item.inUse && item.equipped && item.type != ItemID.Torch && item.friendly && item.InProximity(this, 100f) && item.hurtbox.IntersectsWith(hitbox);
        }
        public virtual bool ProjHit(Projectile projectile)
        {
            return iFrames == 0 && projectile.InProximity(this, 100f) && projectile.hitbox.IntersectsWith(hitbox) && projectile.friendly;
        }
        public virtual void Hurt(int damage)
        {
            CombatText.NewText(damage, this);
            this.iFrames = iFramesMax;
            this.life -= damage;
            if (life <= 0)
                Dispose();
        }
        public bool Contains(Player player) => hitbox.Contains((int)CenterX(player), (int)CenterY(player));
        public float CenterX(Player player) => player.Center.X;
        public float CenterY(Player player) => player.Center.Y;
        public virtual void Draw(Graphics graphics)
        {
            this.PostFX();
            Drawing.LightmapHandling(Main.grass, this, gamma, graphics);
        }
        private static void newObj(int index, int type)
        {
            switch (type)
            {
                case TrapID.Trapdoor:
                    Main.trap[index] = new TrapDoor();
                    break;
                case TrapID.Spikes:
                    Main.trap[index] = new Spikes();
                    break;
                case TrapID.Teleport:
                    Main.trap[index] = new Teleport();
                    break;
                case TrapID.CrossbowTurret:
                    Main.trap[index] = new CrossbowTurret();
                    break;
                case TrapID.WoodenCage:
                    Main.trap[index] = new WoodenCage();
                    break;
                case TrapID.RockFall:
                    Main.trap[index] = new RockFall();
                    break;
                case TrapID.FlameGeyser:
                    Main.trap[index] = new FlameGeyser();
                    break;
                case TrapID.FogMachine:
                    Main.trap[index] = new FogMachine();
                    break;
                case TrapID.AcidPatch:
                    Main.trap[index] = new AcidPatch();
                    break;
                case TrapID.MagicTurret:
                    Main.trap[index] = new MagicTurret();
                    break;
                default:
                    Main.trap[index] = new Trap();
                    break;
            }
        }
        public static int NewTrap(float x, float y, int width, int height, short type, int owner = 255, bool solid = false, bool active = true)
        {
            int num = 100;
            for (int i = 0; i < Main.trap.Length; i++)
            {
                if (Main.trap[i] == null)
                {
                    num = i;
                    break;
                }
                if (i == num)
                {
                    return num;
                }
            }
            newObj(num, type);
            Main.trap[num].active = active;
            Main.trap[num].position = new Vector2(x, y);
            Main.trap[num].width = width;
            Main.trap[num].height = height;
            Main.trap[num].whoAmI = num;
            Main.trap[num].solid = solid;
            Main.trap[num].owner = owner;
            Main.trap[num].type = type;
            Main.trap[num].TextureName = $"trap{0}";
            Main.trap[num]._init();
            Main.trap[num].Init();
            Main.trap[num].SetDefaults();
            return num;
        }
        public override void Dispose()
        {
            if (whoAmI < Main.trap.Length)
            {
                if (Main.trap[whoAmI] != null)
                {
                    Main.trap[whoAmI].active = false;
                    Main.trap[whoAmI].solid = false;
                    Main.trap[whoAmI].position = Vector2.Zero;
                    Main.trap[whoAmI].owner = 255;
                    Main.trap[whoAmI] = null;
                }
            }
        }
    }
}
