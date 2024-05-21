using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf;
using cotf.Base;
using cotf.World;

namespace cotf
{
    public class Coin : Item
    {
        public Coin()
        {
        }
        public override void Update()
        {
            if (!active || !PreUpdate(true))
                return;
            base.Update();

        }
        public override void WorldUpdate(bool update)
        {
            base.WorldUpdate(update);
        }
    }
    public sealed class CoinType
    {
        public const int
            Iron = 0,
            Copper = 1,
            Silver = 2,
            Gold = 3,
            Platinum = 4;
        public static Color cIron => Ext.FromFloat(1f, 0.722f, 0.451f, 0.20f);
        public static Color cCopper => Ext.FromFloat(1f, 0.804f, 0.498f, 0.196f);
        public static Color cSilver => Color.Silver;
        public static Color cGold => Color.Gold;
        public static Color cPlatinum => Ext.FromFloat(1f, 0.898f, 0.894f, 0.886f);
    }
}
