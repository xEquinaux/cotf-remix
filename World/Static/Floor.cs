using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.World
{
    public class Floor : IDisposable
    {
        public bool active;
        public int whoAmI;
        public Npc[] npc;
        public Staircase[] staircase;
        public Tile[,] tile;
        public Floor()
        {
        }
        public void Update()
        {
            if (!active || whoAmI == Main.CurrentFloor || whoAmI < Main.CurrentFloor - 1 || whoAmI > Main.CurrentFloor + 1)
                return;
            foreach (Npc n in npc)
                n.RemoteAI(this);
        }
        public static int NewFloor(Npc[] npc, Staircase[] staircase, Tile[,] tile)
        {
            Floor f = null;
            Main.floor.Add(f = new Floor() 
            { 
                active = true,
                npc = npc,
                staircase = staircase,
                tile = tile
            });
            f.whoAmI = Main.floor.IndexOf(f);
            return f.whoAmI;
        }
        public void Dispose()
        {
            if (Main.floor[whoAmI] != null)
            {
                Main.floor[whoAmI].active = false;
                Main.floor[whoAmI].npc = null;
                Main.floor[whoAmI].staircase = null;
                Main.floor[whoAmI].tile = null;
                Main.floor[whoAmI] = null;
            }
        }
    }
}
