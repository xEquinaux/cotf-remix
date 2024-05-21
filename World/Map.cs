using cotf.Base;
using cotf.Collections;
using cotf.World.Traps;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    public static class Map
    {
        public static void NewMap()
        {
            Main.tile = new Tile[,] { };
            Main.background = new Background[,] { };
            //Main.lightmap = new Lightmap[,] { };
            Main.room = new Dictionary<int, Room>();
            Main.staircase = new Staircase[6];
            Main.scenery = new Scenery[256];
            Main.lamp = new Lamp[10];
            Main.npc = new Npc[128];
            Main.item = new Item[256];
            Main.trap = new Trap[101];
            Main.stash = new Stash[101];
        }
        public static void Unload()
        {
            foreach (Tile item1 in Main.tile)
            {
                item1?.Dispose();
            }
            foreach (Background item2 in Main.background)
            {
                item2?.Dispose();
            }
            //foreach (Lightmap item3 in Main.lightmap)
            //{
            //    item3?.Dispose();
            //}
            Main.room.Clear();
            //Main.room = null; //  Not reinitialized anywhere
            Array.ForEach(Main.staircase, t => t?.Dispose());
            Array.ForEach(Main.scenery, t => t?.Dispose());
            Array.ForEach(Main.lamp, t => t?.Dispose());
            Array.ForEach(Main.npc, t => t?.Dispose());
            Array.ForEach(Main.item, t => t?.Dispose(true));
            Array.ForEach(Main.trap, t => t?.Dispose());
        }
        public static void GenerateFloor(Margin margin)
        {
            int width = margin.Right;
            int height = margin.Bottom;
            Main.WorldWidth = width;
            Main.WorldHeight = height;
            new Lighting().Init(width, height);
            Main.tile = Main.worldgen.CastleGen(Tile.Size, width, height, width / 250, 300f, 600f);
            Room.ConstructAllRooms();
        }
    }
}
