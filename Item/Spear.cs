using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Base;
using cotf.World;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class Spear : Item
    {
        protected override Color rarity => base.rarity;
        public override ToolTip ToolTip => toolTip;
        public override ToolTip SetToolTip()
        {
            return toolTip = new ToolTip(name, text, rarity);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Spear";
            autoReuse = false;
            channel = false;
            width = 48;
            height = 18;
            damage = 10;
            knockBack = 1.5f;
            friendly = true;
            useTime = 60;
            useSpeed = 45;
            useStyle = UseStyle.Stab;
            equipType = EquipType.MainHand;
            defaultColor = Color.LightGray;
        }
        protected override void Init()
        {
            Item.RollStatus(this);
            base.Init();
        }
        protected override void CursedStats()
        {
            base.CursedStats();
        }
        public override bool UseItem(Player myPlayer)
        {
            return base.UseItem(myPlayer);
        }
        public override void HoldItem(Player myPlayer)
        {
        }
        public override void Update(Player myPlayer)
        {
            base.Update(myPlayer);
        }
    }
}
