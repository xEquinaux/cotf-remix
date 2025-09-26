using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.World;
using cotf.Collections;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class Potion : Item
    {
        public Condition condition;
        public Skill skill;
        public int charge = 1;
        protected override Color rarity => Color.White;
        public override ToolTip ToolTip => toolTip;
        public override ToolTip SetToolTip()
        {
            return toolTip = new ToolTip(name, "", rarity);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            skill = PotionEffect();
            condition = new Condition(skill, 10);
            name = "Potion of " + skill.toolTip.name;
            width = 32;
            height = 32;
            useStyle = UseStyle.None;
            equipType = EquipType.OffHand;
            defaultColor = Color.Orange;
            friendly = true;
        }
        public override bool UseItem(Player myPlayer)
        {
            if (Main.mouseLeft && !Main.open)
            {
                skill.Cast(myPlayer);
                this.Dispose();
                return true;
            }
            return false;
        }
        private Skill PotionEffect()
        {
            return Skill.SetActive(SkillID.MinorHeal);
        }
        public override string ToString()
        {
            return skill.toolTip + "";
        }
        public override void Dispose()
        {
            if (Main.myPlayer.equipment.Contains(this))
            {
                for (int i = 0; i < Main.myPlayer.equipment.Length; i++)
                {
                    if (Main.myPlayer.equipment[i] == this)
                    {
                        Main.myPlayer.equipment[i].active = false;
                        Main.myPlayer.equipment[i] = null;
                        break;
                    }
                }
            }
            if (Main.item[whoAmI] != this)
                return;
            Main.item[whoAmI].active = false;
            Main.item[whoAmI] = null;
        }
    }
}
