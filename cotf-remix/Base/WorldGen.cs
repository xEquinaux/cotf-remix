using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.WorldGen
{
	internal class WorldGen
	{
		public static void PlaceTile(int i, int j, short type, bool active)
		{
			Main.tile[i, j].type = type;
			Main.tile[i, j].active(active);
		}
		public static void PlaceTorch(int x, int y, bool staticLamp, Entity ent, int owner = 255)
		{
			//  Unoptimized: causes large slowdown
            int offsetX = Main.rand.Next(Tile.Size);
            int offsetY = Main.rand.Next(Tile.Size);
            int index = Lamp.NewLamp(new Vector2(x + offsetX, y + offsetY), 200f, Lamp.RandomLight(), ent, staticLamp);
			Main.lamp[index].active = true;
            Main.lamp[index].owner = owner;
		}
	}
}
