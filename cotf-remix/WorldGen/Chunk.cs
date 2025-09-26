using ArchaeaMod;
using cotf.World;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.WorldGen
{
	public class Chunk
	{
		public int x, y;
		public static Tile[,] NewChunk(int x, int y, int size = 50, int width = 3000, int height = 3000, int maxNodes = 15, float range = 200f, float nodeDistance = 300f)
		{
			return Worldgen.Instance.CastleGen(size, width, height, maxNodes, range, nodeDistance);
		}
	}
}
