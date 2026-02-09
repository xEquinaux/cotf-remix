using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cotf
{
    public class Armor : Item
    {
        public Effect qlType;
        public ArmorType style;
        public int defenseRate;
        public Npc dropped;
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
        public virtual void OnEquipArmor(Player player)
        {
            player.statDefense += defenseRate;
        }
        public virtual void OnUnequip(Player player)
        {
            player.statDefense -= defenseRate;
        }
    }
}
