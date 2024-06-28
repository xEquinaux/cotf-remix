using cotf;
using cotf.Base;
using cotf.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using ArchaeaMod;
using ArchaeaMod.Structure;
using TileID = ArchaeaMod.TileID;

namespace cotf.WorldGen
{
	public class Factory
	{
		private static int
			Width = 150,
			Height;
		internal static int
			Top => /*Main.UnderworldLayer -*/ Height;
		public static short
			Air = 0,
			Tile = 1,//ArchaeaWorld.factoryBrick,
			Wall = 1,//ArchaeaWorld.factoryBrickWallUnsafe,
			Tile2 = 1,//ArchaeaWorld.Ash,
			ConveyerL = 0,//TileID.ConveyorBeltLeft,
			ConveyerR = 0,//TileID.ConveyorBeltRight,
			Door = 0;//ArchaeaWorld.factoryMetalDoor;
		public static IList<Room> room = new List<Room>();
		public static void CastleGen(Tile[,] brush, Background[,] background, int width, int height, int size = 4, int maxNodes = 50, float nodeDistance = 60, bool clearCenterColumn = false)
		{
			//	TODO: generation might be too big (clearly) without this
			//width /= cotf.World.Tile.Size;
			//height /= cotf.World.Tile.Size;
			Width = width;
			Height = height;

			Vector2[] nodes = new Vector2[maxNodes];
			int numNodes = 0;

			//  Generating vector nodes
			int randX = 0,
				randY = 0;
			while (numNodes < maxNodes)
			{
				foreach (Vector2 node in nodes)
				{
					do
					{
						randX = Main.rand.Next(size, width);
						randY = Main.rand.Next(size, height - size * 4);
						nodes[numNodes] = new Vector2(randX, randY);
					} while (nodes.All(t => t.Distance(nodes[numNodes]) < nodeDistance));
					numNodes++;
				}
			}

			//  Carve out rooms
			int W = 0, H = 0;
			int maxSize = 7;
			int border = 8;
			foreach (Vector2 node in nodes)
			{
				Room r = new Room((short)Main.rand.Next(RoomID.Total));
				int rand = 0;//Main.rand.Next(2);
				switch (rand)
				{
					case 0:
						W = Main.rand.Next(4, maxSize) * size;
						H = Main.rand.Next(4, maxSize) * size;
						//  Room construction
						int X1 = (int)node.X - W / 2;
						int X2 = (int)node.X + W / 2;
						int Y1 = (int)node.Y - H / 2;
						int Y2 = (int)node.Y + H / 2;
						//TODO
						//r.bound = new Rectangle(X1 - border, Y1 - border, W + border, H + border);
						//if (room.FirstOrDefault(t => t.bound.Intersects(r.bound)) != default)
						//{
						//    continue;
						//}
						for (int i = X1 - border; i < X2 + border; i++)
						{
							for (int j = Y1 - border; j < Y2 + border; j++)
							{
								//  If tile in-bounds
								if (i > 0 && j > 0 && i < width && j < height)
								{
									if (i < brush.GetLength(0) && j < brush.GetLength(1))
									{
										if (brush[i, j] == null) continue;
										brush[i, j].type = Air;
										if (i <= X1 || i >= X2 || j <= Y1 || j >= Y2)
										{
											if (i > X1 && i < X2 && j >= Y2)
											{
												//  Floor
												brush[i, j].type = Tile2;
												continue;
											}
											//  Ceiling and walls
											brush[i, j].type = Tile;
											continue;
										}
									}
								}
							}
						}
						room.Add(r);
						break;
					default:
						break;
				}
			}

			//  Generating hallways
			nodes = nodes.OrderBy(t => t.Distance(new Vector2(0, height / 2))).ToArray();
			for (int k = 1; k < nodes.Length; k++)
			{
				int X, Y;
				Vector2 start,
						end;

				//  Normal pass
				start = nodes[k - 1];
				end = nodes[k];

				#region Hallway carving
				if (start.X < end.X && start.Y < end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (Y++ <= (start.Y + end.Y) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
				}
				else if (start.X > end.X && start.Y < end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (Y++ <= (start.Y + end.Y) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
				}
				else if (start.X < end.X && start.Y > end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (X++ <= (start.X + end.X) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
				}
				else if (start.X > end.X && start.Y > end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (X++ <= (start.X + end.X) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
				}
				#endregion
				#region Reversed pass
				start = nodes[k];
				end = nodes[k - 1];

				if (start.X < end.X && start.Y < end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (Y++ <= (start.Y + end.Y) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
				}
				else if (start.X > end.X && start.Y < end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (Y++ <= (start.Y + end.Y) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
				}
				else if (start.X < end.X && start.Y > end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (X++ <= (start.X + end.X) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
				}
				else if (start.X > end.X && start.Y > end.Y)
				{
					X = (int)start.X + (int)start.X % size;
					Y = (int)start.Y + (int)start.Y % size;

					while (X++ <= (start.X + end.X) / 2 + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);
					}
					while (Y++ <= end.Y + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
					while (X++ <= end.X + size)
					{
						CarveHall(ref brush, ref background, X, Y, 6);

					}
				}
				#endregion
			}

			//  Clear center column
			if (clearCenterColumn)
			{
				for (int i = 0; i < brush.GetLength(0); i++)
				{
					for (int j = 0; j < brush.GetLength(1); j++)
					{
						int cx = brush.GetLength(0) / 2 - 10;
						int cx2 = brush.GetLength(0) / 2 + 10;
						if (i >= cx && i <= cx2)
						{
							brush[i, j].type = Air;
						}
					}
				}
			}

			//  Return value
			//tile = brush;

			//	Clear the empty tiles (air)
			foreach (var item in Main.tile)
			{
				if (item.type == Air || item.type == Door || item.type == ConveyerL || item.type == ConveyerR)
				{
					item.active(false);
				}
			}
		}
		private static void CarveHall(ref Tile[,] tile, ref Background[,] wall, int x, int y, int size = 10)
		{
			int border = 4;
			bool flag = Main.rand.NextBool(8);
			bool flag2 = Main.rand.NextBool();
			for (int i = -border; i < size + border; i++)
			{
				for (int j = -border; j < size + border; j++)
				{
					int X = Math.Max(0, Math.Min(x + i, Width - 1)) / cotf.World.Tile.Size;
					int Y = Math.Max(0, Math.Min(y + j, Height - 1)) / cotf.World.Tile.Size;
					//TODO
					//var r = room.FirstOrDefault(t => t.bound.Intersects(new Rectangle(X, Y, size + border, size + border)));
					//if (r != default)
					//{
					//    continue;
					//}
					if (wall[X, Y] == null || tile[X, Y] == null) continue;
					if (wall[X, Y].type != Wall)
					{
						tile[X, Y].type = Tile;
					}
					if (flag && j == size - 1)
					{
						tile[X, Y].type = flag2 ? ConveyerL : ConveyerR;
					}
				}
			}
			for (int j = 0; j < size; j++)
			{
				for (int i = 0; i < size; i++)
				{
					int X = Math.Max(0, Math.Min(x + i, Width - 1)) / cotf.World.Tile.Size;
					int Y = Math.Max(0, Math.Min(y + j, Height - 1)) / cotf.World.Tile.Size;

					if (GetSafely(X, Y) == null || !GetSafely(X, Y - 1).Active && GetSafely(X, Y + 1).Active)// && (tile[X, Y].type == ConveyerL || tile[X, Y].type == ConveyerR))
					{
						continue;
					}
					tile[X, Y].type = Air;
					//wall[X, Y] = Wall;
					if (j == 0 && Main.rand.NextBool(60))
					{
						for (int l = 0; l < 6; l++)
						{
							if (Y + l < tile.GetLength(1))
							{
								tile[X, Y + l].type = Door;
							}
						}
					}
				}
			}
		}
		//  Beams, steps, balconies, chains, platforms
		static ushort[,] stepsRight = new ushort[,]
		{
			{ 0, 1, 1, 1, 2 },
			{ 0, 0, 1, 1, 2 },
			{ 0, 0, 0, 1, 2 },
			{ 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0 }
		};
		static ushort[,] stepsLeft = new ushort[,]
		{
			{ 0, 1, 1, 1, 2 },
			{ 0, 0, 1, 1, 2 },
			{ 0, 0, 0, 1, 2 },
			{ 0, 0, 0, 0, 0 },
			{ 0, 0, 0, 0, 0 }
		};
		public static Tile GetSafely(int i, int j, int buffer = 20)
		{
			int m = Math.Max(buffer, Math.Min(/*Main.maxTilesX*/4800 - buffer, i));
			int n = Math.Max(buffer, Math.Min(/*Main.maxTilesY*/1600 - buffer, j));
			return Main.tile[m, n];
		}
		public static void Decorate(int x, int y, int width, int height)
		{
			Treasures t = new Treasures();
			//  Beams
			for (int i = 0; i < width; i++)
			{
				if (i % (Main.rand.Next(23, 31) + 1) == 0)
				{
					for (int j = y; j < y + height; j++)
					{
						if (/*GetSafely(i, j).WallType == Wall && */!GetSafely(i - 1, j).Active || GetSafely(i + 1, j).Active)
						{
							if (GetSafely(i, j + 1).type != Tile && GetSafely(i, j + 1).type == Tile)
							{
								var _t = GetSafely(i, j + 1);
								_t.Active = false;
								Main.tile[i, j + 1].type = TileID.Timers;
							}
							var tile = GetSafely(i, j);
							tile.active(true);
							tile.type = TileID.AdamantiteBeam;
							//tile.RedWire = true;
							//tile.HasActuator = true;
							//tile.IsActuated = true;
							//var tile2 = GetSafely(i, j + 1);
							//tile2.RedWire = true; 
						}
					}
				}
			}
			//  Steps
			int countLeftY = 0;
			int countRightY = 0;
			for (int i = 0; i < width; i++)
			{
				countLeftY = 0;
				countRightY = 0;
				for (int j = y; j < y + height; j++)
				{
					if (/*GetSafely(i, j).WallType == Wall && */!GetSafely(i, j).Active && GetSafely(i - 1, j).Active && GetSafely(i - 1, j).type == Tile)
					{
						countLeftY++;
					}
				}
				if (countLeftY > 15)
				{
					for (int j = y; j < y + height; j++)
					{
						if (j % 15 == 0)
						{
							if (/*GetSafely(i, j).WallType == Wall && */!GetSafely(i, j).Active && GetSafely(i - 1, j).Active && GetSafely(i - 1, j).type == Tile)
							{
								for (int m = 0; m < stepsLeft.GetLength(1); m++)
								{
									for (int n = 0; n < stepsLeft.GetLength(0); n++)
									{
										switch (stepsLeft[m, n])
										{
											case 1:
												Main.tile[i + m, j + n].type = Tile2;
												//WorldGen.PlaceTile(i + m, j + n, Tile2, true, true);
												break;
											case 2:
												Main.tile[i + m, j + n].type = Tile;
												//WorldGen.PlaceTile(i + m, j + n, Tile, true, true);
												break;
										}
									}
								}
							}
						}
					}
				}
				for (int j = y; j < y + height; j++)
				{
					if (/*GetSafely(i, j).WallType == Wall && */!GetSafely(i, j).Active && GetSafely(i + 1, j).Active && GetSafely(i + 1, j).type == Tile)
					{
						countRightY++;
					}
				}
				if (countRightY > 15)
				{
					for (int j = y; j < y + height; j++)
					{
						if (j % 15 == 7)
						{
							if (/*GetSafely(i, j).WallType == Wall && */!GetSafely(i, j).Active && GetSafely(i + 1, j).Active && GetSafely(i + 1, j).type == Tile)
							{
								for (int m = 0; m < stepsRight.GetLength(1); m++)
								{
									for (int n = 0; n < stepsRight.GetLength(0); n++)
									{
										switch (stepsRight[m, n])
										{
											case 1:
												Main.tile[i + m, j + n].type = Tile2;
												//WorldGen.PlaceTile(i + m, j + n, Tile2, true, true);
												break;
											case 2:
												Main.tile[i + m, j + n].type = Tile;
												//WorldGen.PlaceTile(i + m, j + n, Tile, true, true);
												break;
										}
									}
								}
							}
						}
					}
				}
			}
			//  Balconies 
			foreach (Room r in room)
			{
				//TODO
				int Top = 0;//y + r.bound.Y;
				int Right = 0;//r.bound.Right;
				int Bottom = 0;//y + r.bound.Bottom;
				int Left = 0;//r.bound.X;
				for (int i = Left; i < Right; i++)
				{
					for (int j = Top; j < Bottom; j++)
					{
						if (Main.rand.NextBool())
						{
							if (GetSafely(i, j - 8).Active && !GetSafely(i, j - 7).Active && /*GetSafely(i, j).WallType == Wall &&*/  GetSafely(i - 1, j).Active && GetSafely(i - 1, j).type == Tile && !GetSafely(i, j).Active)
							{
								for (int m = 0; m < 5; m++)
								{
									Main.tile[i + m, j].type = Tile;
									Main.tile[i + m, j + 1].type = Tile;
								}
								bool placed = false;
								for (int m = 0; m < 5; m++)
								{
									if (!placed)
									{
										//t.PlaceTile(i + m, j + 2, (ushort)ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>(), true, false, 4, false);
										//placed = Main.tile[i + m, j + 2].type == ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>();
									}
									if (m == 2)
									{
										if (Main.rand.NextBool(8))
										{
											//WorldGen.PlaceTile(i + m, j - 1, ModContent.TileType<ArchaeaMod.Tiles.m_chair>(), true, true);
										}
									}
								}
								goto END;
							}
						}
						if (Main.rand.NextBool())
						{
							if (GetSafely(i, j - 8).Active && !GetSafely(i, j - 7).Active &&/* GetSafely(i, j).WallType == Wall &&*/ GetSafely(i + 1, j).Active && GetSafely(i + 1, j).type == Tile && !GetSafely(i, j).Active)
							{
								for (int m = 0; m < 5; m++)
								{
									Main.tile[i - m, j].type = Tile;
									Main.tile[i - m, j + 1].type = Tile;
								}
								bool placed = false;
								for (int m = 0; m < 5; m++)
								{
									if (!placed)
									{
										//t.PlaceTile(i + m, j + 2, (ushort)ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>(), true, false, 4, false);
										//placed = Main.tile[i + m, j + 2].type == ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>();
									}
									if (m == 2)
									{
										if (Main.rand.NextBool(8))
										{
											//WorldGen.PlaceTile(i - m, j - 1, ModContent.TileType<ArchaeaMod.Tiles.m_chair>(), true, true);
										}
									}
								}
								goto END;
							}
						}
					}
				}
			END: { continue; }
			}
			//  Chains
			for (int i = 0; i < width; i++)
			{
				if (i % (Main.rand.Next(30, 40) + 1) == 0)
				{
					for (int j = y; j < y + height; j++)
					{
						if (GetSafely(i - 1, j).Active || GetSafely(i + 1, j).Active)// && GetSafely(i, j).WallType == Wall)
						{
							Main.tile[i, j].type = TileID.Chain;//WorldGen.PlaceTile(i, j, TileID.Chain);
						}
					}
				}
			}
			//  Platforms
			bool flag = false;
			for (int j = y; j < y + height; j++)
			{
				for (int i = 0; i < width; i++)
				{
					if (/*GetSafely(i + 1, j).WallType == Wall && */GetSafely(i, j).type == Tile && GetSafely(i, j).Active && !GetSafely(i, j - 1).Active && !GetSafely(i + 1, j).Active)
					{
						flag = true;
					}
					if (flag)
					{
						for (int m = 0; m < 10; m++)
						{
							//WorldGen.PlaceTile(i + m, j, TileID.Platforms, true, false); 
							if (GetSafely(i + m + 1, j).Active)
							{
								break;
							}
						}
						flag = false;
					}
				}
			}
		}
	}
}
