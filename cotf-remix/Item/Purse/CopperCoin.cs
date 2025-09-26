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
    public class CopperCoin : Coin
    {
        protected override Color rarity => base.rarity;
        public override ToolTip ToolTip => toolTip = new ToolTip(name, text, rarity);
        public override void SetDefaults()
        {
            base.SetDefaults();
            name = "Copper Coin";
            value = 10;
            isCoin = true;
            friendly = true;
        }
        protected override void Init()
        {
            defaultColor = CoinType.cCopper;
            base.Init();
        }
    }
}
