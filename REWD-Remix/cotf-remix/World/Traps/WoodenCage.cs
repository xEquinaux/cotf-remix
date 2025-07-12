using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using cotf;
using cotf.Base;
using cotf.ID;
using Microsoft.Xna.Framework;

namespace cotf.World.Traps
{
    public class WoodenCage : Trap
    {
        bool trip;
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Wooden Cage";
            iFramesMax = 60;
            life = lifeMax = 50;
        }
        protected override void Init()
        {
        }
        public override void Update()
        {
            if (iFrames > 0)
                iFrames--;
            if (!base.PreUpdate(true))
                return;
            if (!trip && Contains(Main.myPlayer))
            {
                if (Main.myPlayer.IsControlMove())
                {
                    trip = true;
                }
            }
            if (trip)
            {
                if (Main.myPlayer.Collision(this))
                {
                    Main.myPlayer.velocity = Vector2.Zero;
                }
            }
            if (Main.myPlayer.equipment[EquipType.MainHand] != null  &&
                Main.myPlayer.equipment[EquipType.MainHand].active   &&
                Main.myPlayer.equipment[EquipType.MainHand].equipped &&
                ItemHit(Main.myPlayer.equipment[EquipType.MainHand]))
            {
                Hurt(Main.myPlayer.equipment[EquipType.MainHand].damage);
                iFrames = iFramesMax;
            }
            foreach (Projectile proj in Main.projectile)
            {
                if (proj != null && proj.active)
                {
                    if (ProjHit(proj))
                    {
                        Hurt(proj.damage);
                        iFrames = iFramesMax;
                        proj.Dispose();
                    }
                }
            }
            if (life <= 0)
            {
                Dispose();
            }
        }
        public override void Draw(Graphics graphics)
        {
            if (!base.PreUpdate(true))
                return;
            base.Draw(graphics);
        }
    }
}
