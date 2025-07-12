using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace ArchaeaMod.Structure.Tiles
{
    internal class FactoryTile
    {
        public static void DoorToggle(int i, int j)
        {
            Wiring.TripWire(i, j, 1, 1);
        }
    }
}
