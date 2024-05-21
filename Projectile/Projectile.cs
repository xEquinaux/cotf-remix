using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Assets;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;

namespace cotf
{
    public class Projectile : Entity
    {
        public Lamp light;
        public int timeLeft;
        public float angle;
        public override Vector2 Center => new Vector2(X - width / 2, Y - height / 2);
        protected bool init;
        public Projectile()
        {
            SetDefaults();
        }
        public virtual void SetDefaults()
        {
            TextureName = $"projectile{0}";
            texture = preTexture = Asset<Bitmap>.Request(TextureName);
        }
        protected virtual void Init()
        {
            SetDefaults();
            color = defaultColor;
        }
        public virtual bool PreAI()
        {
            if (Main.TimeScale == 1f)
            {
                velocity = oldVelocity;
            }
            return true;
        }
        public virtual void AI()
        {
            if (!active || !PreAI())
                return;
            Collide();
            foreach (Npc n in Main.npc)
            {
                if (n == null || !n.active || !n.InProximity(this, 200f))
                    continue;
                HitNPC(n);
            }
            HitPlayer(Main.myPlayer);
            velocity *= Main.TimeScale;
        }
        public virtual bool HitPlayer(Player player)
        {
            if (player.ProjHit(this))
            {
                Dispose();
                return true;
            }
            return false;
        }
        public virtual bool HitNPC(Npc npc)
        {
            if (npc.NpcProjHit(this))
            {
                npc.NpcHurt(damage, knockBack, AngleTo(npc.Center));
                return true;
            }
            return false;
        }
        public virtual void Draw(Graphics graphics)
        {
        }
        public virtual void Collide()
        {
        }
        public static int NewProjectile(Vector2 position, Vector2 velocity, float angle, short type, Entity parent)
        {
            int num = Main.projectile.Length - 1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i] == null || !Main.projectile[i].active)
                {
                    num = i;
                    break;
                }
            }
            newObj(num, type);
            Main.projectile[num].active = true;
            Main.projectile[num].position = position;
            Main.projectile[num].velocity = velocity;
            Main.projectile[num].oldVelocity = velocity;
            Main.projectile[num].type = type;
            Main.projectile[num].whoAmI = num;
            Main.projectile[num].owner = parent.owner;
            Main.projectile[num].angle = angle;
            Main.projectile[num].Init();
            return num;
        }
        private static void newObj(int index, int type)
        {
            switch (type)
            {
                case ProjectileID.Fireball:
                    Main.projectile[index] = new Proj_Fireball();
                    break;
                case ProjectileID.FireBolt:
                    Main.projectile[index] = new Proj_Firebolt();
                    break;
                case ProjectileID.Arrow:
                    Main.projectile[index] = new Proj_Arrow();
                    break;
            }
        }
        public static int NewProjectile(float x, float y, float velX, float velY, float angle, short type, Entity parent)
        {
            int num = Main.projectile.Length - 1;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i] == null || !Main.projectile[i].active)
                {
                    num = i;
                    break;
                }
            }
            newObj(num, type);
            Main.projectile[num].active = true;
            Main.projectile[num].position = new Vector2(x, y);
            Main.projectile[num].velocity = new Vector2(velX, velY);
            Main.projectile[num].type = type;
            Main.projectile[num].whoAmI = num;
            Main.projectile[num].owner = parent.owner;
            Main.projectile[num].angle = angle;
            Main.projectile[num].Init();
            return num;
        }
        public override void Dispose()
        {
            if (Main.projectile[whoAmI] != null)
            { 
                Main.projectile[whoAmI].active = false;
                Main.projectile[whoAmI].position = Vector2.Zero;
                Main.projectile[whoAmI].light?.Dispose();
                Main.projectile[whoAmI] = null;
            }
        }
    }
    public sealed class ProjectileID
    {
        public const short
            None = 0,
            Fireball = 1,
            FireBolt = 2,
            Arrow = 3;
    }
}
