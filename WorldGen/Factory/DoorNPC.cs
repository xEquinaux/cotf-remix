using ArchaeaMod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;

namespace ArchaeaMod.Structure
{
    internal class DoorNPC : ModNPC
    {
        public override string Texture => "ArchaeaMod/Gores/Null";
        public override void SetDefaults()
        {
            NPC.width = 16;
            //  Hallway is set to 6 tiles tall
            NPC.height = 96;
            NPC.value = 0;
            NPC.lifeMax = 60;
            NPC.noGravity = false;
            NPC.behindTiles = false;
        }
        private int open 
        {
            get { return (int)NPC.ai[0]; }
            set { NPC.ai[0] = value; }
        }
        private float weight
        {
            get { return NPC.ai[1]; }
            set { NPC.ai[1] = value; }
        }
        private int ticks
        {
            get { return (int)NPC.ai[2]; }
            set { NPC.ai[2] = value; }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool SpecialOnKill()
        {
            return true;
        }
        public override bool PreAI()
        {
            if (weight == 0f)
            {
                ticks = NPC.height;
                NPC.noGravity = false;
                weight++;
            }
            NPC.life = NPC.lifeMax;
            NPC.timeLeft = 10;
            NPC.active = true;                                                             
            Projectile wrench = Main.projectile.FirstOrDefault(t => t.active && t.type == ProjectileID.MechanicWrench);
            return wrench != default && wrench.Hitbox.Intersects(NPC.Hitbox) && open == 0;
        }
        public override void AI()
        {
            open = 1;
            NPC.netUpdate = true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (open == 1 && ticks > 0)
            {
                ticks--;
            }
            if (ticks == 0)
            {
                NPC.active = false;
            }
            int num = ticks;
            var bmp = new System.Drawing.Bitmap(NPC.width, NPC.height);
            Graphics g = Graphics.FromImage(bmp);
            g.FillRectangle(Brushes.White, new System.Drawing.Rectangle(0, 0, NPC.width, NPC.height));
            HelperUtil.DrawWeightedBar(Fx.FromBitmap(bmp), NPC.position.X, NPC.position.Y, ref num, 32, NPC.height, spriteBatch);
            bmp.Dispose();
            g.Dispose();
            return false;
        }
    }
}