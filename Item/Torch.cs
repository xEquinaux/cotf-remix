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

namespace cotf
{
    public class Torch : Item
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
            name = "Torch";
            width = 32;
            height = 32;
            holdStyle = HoldStyle.HoldOut;
            channel = true;
            friendly = true;
            equipType = EquipType.OffHand;
            defaultColor = Color.LightYellow;
            color = defaultColor;
        }
        protected override void Init()
        {
            //int i = Lamp.NewLamp((int)Center.X, (int)Center.Y, 100f, this);
            //lamp = Main.lamp[i];
            //lamp.active = false;
        }
        public override bool UseItem(Player myPlayer)
        {
            return base.UseItem(myPlayer);
        }
        public override void HoldItem(Player myPlayer)
        {
            base.HoldItem(myPlayer);
        }
        public override void Update(Player myPlayer)
        {
            base.Update(myPlayer);
        }
        public override void WorldUpdate(bool update)
        {
            //lamp.active = update;
            //lamp.position = position;
        }
        public override void Draw(Graphics graphics)
        {
            base.Draw(graphics);
        }
    }
}
