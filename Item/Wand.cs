using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolTip = cotf.Base.ToolTip;
using cotf.Base;
using cotf.World;
using cotf.Collections;

namespace cotf
{
    public class Wand : Item
    {
        public Condition condition;
        public Skill skill;
        public int charge = 1;
        public bool noCharges => charge == 0;
        protected override Color rarity => Color.White;
        public override ToolTip ToolTip => toolTip;
        public override ToolTip SetToolTip()
        {
            return toolTip = new ToolTip(name, "", rarity);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            skill = WandSkill();
            condition = new Condition(skill, 10);
            name = "Wand of " + skill.toolTip.name;
            charge = Main.rand.Next(5, 15);
            width = 32;
            height = 32;
            useStyle = UseStyle.None;
            equipType = EquipType.OffHand;
            holdStyle = HoldStyle.HoldOut;
            defaultColor = Color.Orange;
            friendly = true;
        }
        public override bool UseItem(Player myPlayer)
        {
            if (Main.mouseLeft && !Main.open)
            {
                if (charge-- > 0)
                    skill.Cast(myPlayer);
                return true;
            }
            return false;
        }
        private Skill WandSkill()
        {
            int rand = Main.rand.Next(4);
            if (rand == SkillID.None)
                return Skill.None;
            else return Skill.SetActive((short)rand);
        }
        public override string ToString()
        {
            return skill.toolTip + "";
        }
    }
}
