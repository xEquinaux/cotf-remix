using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using cotf;
using cotf.Base;
using cotf.ID;

namespace cotf.World.Traps
{
    internal class Teleport : Trap
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Teleport";
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
                Main.myPlayer.FindRandomTile(true);
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
