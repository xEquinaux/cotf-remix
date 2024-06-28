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
using Microsoft.Xna.Framework.Graphics;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class Broadsword : Item
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
            name = "Broadsword";
            autoReuse = false;
            channel = false;
            damage = 12;
            knockBack = 1.2f;
            friendly = true;
            width = 48;
            height = 48;
            useTime = 60;
            useSpeed = 45;
            useStyle = UseStyle.Swing;
            equipType = EquipType.MainHand;
            defaultColor = Color.LightSlateGray;
            texture = Main.cinnabar;
        }
        protected override void Init()
        {
            base.Init();
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
        public override void Draw(Graphics graphics, SpriteBatch sb)
        {
            base.Draw(graphics, sb);
        }
    }
}
