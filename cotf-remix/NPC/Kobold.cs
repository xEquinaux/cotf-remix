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

using Color = System.Drawing.Color;
using System.Numerics;

namespace cotf
{
    public class Kobold : Npc
    {
        public override void SetDefaults()
        {
            name = "Kobold";
            width = 24;
            height = 42;
            speed = 10f;
            lifeMax = 50;
            iFramesMax = 60;
            damage = 2;
            hostile = true;
            knockBack = 1.2f;
            defaultColor = Color.White;//Color.BurlyWood;
            frameCount = 4;
            frameHeight = 48;
            frame = Main.rand.Next(4);
            if (Main.rand.NextBool())
            { 
                texture = Assets.Asset<Bitmap>.Request("Sky_1");
            }
            else
            {
                int rand = Main.rand.Next(3) + 1;
                string result = "_1";
                switch (rand)
                { 
                    default:
                    case 1:
                        break;
                    case 2:
                        result = "_2";
                        break;
                    case 3:
                        result = "_3";
                        break;
                }
                texture = Assets.Asset<Bitmap>.Request("Sky_1" + result);
            }
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
            velocity.X = Helper.Clamp(velocity.X, -1f, 1f);
            velocity.Y = Helper.Clamp(velocity.Y, -1f, 1f);
            velocity.X *= Main.TimeScale;
            velocity.Y *= Main.TimeScale;
        }
        int ticks = 0;
		public override void Draw(Graphics graphics, ref int frameTicks, int interval, int startTicks)
		{
            if (++ticks == int.MaxValue)
            {
                ticks = 0;
            }
			base.Draw(graphics, ref ticks, 5, 0);
		}
	}
}
