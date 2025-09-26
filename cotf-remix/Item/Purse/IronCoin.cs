using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf;
using cotf.Base;
using cotf.World;
using ToolTip = cotf.Base.ToolTip;

namespace cotf
{
    public class IronCoin : Coin
    {
        protected override Color rarity => base.rarity;
        public override ToolTip ToolTip => toolTip = new ToolTip(name, text, rarity);
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Iron Coin";
            value = 1;
            isCoin = true;
            friendly = true;
        }
        protected override void Init()
        {
            defaultColor = CoinType.cIron;
            base.Init();
        }
    }
}
