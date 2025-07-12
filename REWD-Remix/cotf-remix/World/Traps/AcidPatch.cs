using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using cotf.Base;
using cotf.ID;

namespace cotf.World.Traps
{
    public class AcidPatch : Trap
    {
        public override void SetDefaults()
        {
            name = "Acid Patch";
            damage = 10;
            base.SetDefaults();
        }
        protected override void Init()
        {
        }
        public override void Update()
        {
            if (!base.PreUpdate(true))
                return;
            if (Contains(Main.myPlayer))
            {
                if (Main.myPlayer.IsMoving())
                {
                    ticks++;
                    if (ticks % 90 == 0)
                    {
                        Main.myPlayer.Hurt(damage, 5f, 0f);
                    }
                }
            }
        }
        public override void Draw(Graphics graphics)
        {
            if (!base.PreUpdate(true))
                return;
            Drawing.TextureLighting(texture, hitbox, this, graphics);
        }
    }
}
