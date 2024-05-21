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

namespace cotf
{
    public class Kobold : Npc
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Kobold";
            width = 18;
            height = 32;
            speed = 10f;
            lifeMax = 50;
            iFramesMax = 60;
            damage = 2;
            hostile = true;
            knockBack = 1.2f;
            defaultColor = Color.BurlyWood;
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
    }
}
