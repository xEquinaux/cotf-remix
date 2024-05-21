using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using cotf;
using cotf.Base;
using cotf.ID;

namespace cotf.World.Traps
{
    internal class RockFall : Trap
    {
        bool trip;
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Rock Fall";
            damage = 20;
        }
        protected override void Init()
        {
        }
        public override void Update()
        {
            if (trip || !base.PreUpdate(true))
                return;
            if (Contains(Main.myPlayer))
            {
                if (Main.myPlayer.IsControlMove())
                {
                    trip = true;
                    Main.myPlayer.Hurt(damage, 5f, 0f);
                }
            }
        }
        public override void Draw(Graphics graphics)
        {
            if (!trip || !base.PreUpdate(true))
                return;
            base.Draw(graphics);
        }
    }
}
