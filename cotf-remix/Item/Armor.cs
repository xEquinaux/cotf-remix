using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cotf
{
    public partial class Item
    {
        public bool armor = false;
        public virtual void OnLootDrop()
        {
            qlType = StatusEffect();
        }
        public virtual void OnPickup()
        {
        }
        public Effect StatusEffect()
        {
            int num = 0;
            if (dropped.ql > 0.33M)
            {
                num++;
            }
            if (dropped.ql > 0.67M)
            {
                num += 2;
            }
            if (Main.floorType == FloorType.Jackpot)
            {
                num = 2;
            }
            if (Main.floorType == FloorType.Haunted)
            {
                num--;
            }
            return (Effect)num;
        }
        public virtual void OnUnequip(Player player)
        {
            if (armor)
            { 
                player.statDefense -= defenseRate;
            }
        }
    }
}
