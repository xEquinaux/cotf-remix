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
    public class Purse : Item
    {
        protected override Color rarity => Color.White;
        public override ToolTip ToolTip => toolTip = new ToolTip(name, text, rarity);
        //public Stash Content;
        public Purse(uint copper)
        {
            //Content = Stash.DoConvert(copper);
        }
        protected override void Init()
        {
            //value = purse.Content.TotalCopper();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            purse = this;
            name = "Purse";
            width = 32;
            height = 32;
            useStyle = UseStyle.None;
            equipType = EquipType.Purse;
            defaultColor = Color.Purple;
            friendly = true;           
        }
        public override void Update()
        {
            //text = //$"Iron: {Content.iron}\n" +
            //       $"Copper: {Content.copper}\n" +
            //       $"Silver: {Content.silver}\n" +
            //       $"Gold: {Content.gold}\n" +
            //       $"Platinum: {Content.platinum}";
            base.Update();
        }
        public override string Text()
        {
            return text;
        }
    }
}
