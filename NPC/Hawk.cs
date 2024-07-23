using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToolTip = cotf.Base.ToolTip;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using System.Diagnostics.Eventing.Reader;

namespace cotf
{
    public class Hawk : Npc
    {
        int frameTicks;
        public override void SetDefaults()
        {
            TextureName = "Sky_3";
            name = "Hawk";
            width = 64;
            height = 58;
            speed = 12f;
            lifeMax = 65;
            iFramesMax = 40;
            damage = 4;
            hostile = true;
            knockBack = 1.5f;
            defaultColor = Color.White;
            frameCount = 10;
            frameHeight = 58;
            frame = Main.rand.Next(4);
            texture = Assets.Asset<Bitmap>.Request("Sky_3");
        }
        public override void AI()
        {
            base.AI();
            if (NpcSight(Main.myPlayer))
            {
                idle = false;
                if (InProximity(Main.myPlayer, 50))
                {
                    velocity = Vector2.Zero;
                    return;
                }
                if (ticks++ < int.MaxValue)
                {
                    if (ticks % 30 == 0)
                    {
                        velocity = -Helper.AngleToSpeed(Helper.AngleTo(Main.myPlayer.Center, Center), speed);
                    }
                }
                else ticks = 0;
            }
            else if (!idle) velocity = Vector2.Zero;
            velocity.X = MathHelper.Clamp(velocity.X, -1f, 1f);
            velocity.Y = MathHelper.Clamp(velocity.Y, -1f, 1f);
            velocity *= Main.TimeScale;
        }
        int ticks = 0;
		public override void Draw(Graphics graphics, ref int frameTicks, int interval, int startTicks)
		{
            if (++ticks == int.MaxValue)
            {
                ticks = 0;
            }
			base.Draw(graphics, ref ticks, 4, 2);
		}
	}
}
