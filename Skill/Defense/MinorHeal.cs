using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using System.Drawing.Imaging;
using System.Drawing;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class MinorHeal : Skill
    {
        int effectAmount = 0;
        public override ToolTip SetToolTip()
        {
            return toolTip = new ToolTip("Minor Heal", "", Color.White);
        }
        public override void SetDefaults()
        {
            hostile = false;
            friendly = true;
            manaCost = 3;
            damage = 0;
            type = SkillID.MinorHeal;
            useTime = 180;
            effectAmount = 15;
        }
        public override void Cast(Player player)
        {
            if (!PreCast(player))
                return;
            base.Cast(player);
            player.Heal(effectAmount);
        }
    }
}
