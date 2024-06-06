using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.GameContent.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

using ArchaeaMod;
using ArchaeaMod.GenLegacy;
using ArchaeaMod.Items;
using ArchaeaMod.Items.Alternate;
using ArchaeaMod.Merged;
using ArchaeaMod.Merged.Items;
using ArchaeaMod.Merged.Tiles;
using ArchaeaMod.Merged.Walls;

namespace ArchaeaMod.Structure
{
    public class Factory
    {
        private static int
            Width = 150, 
            Height;
        internal static int
            Top => Main.UnderworldLayer - Height;
        public static ushort
            Air = 1,
            Tile = ArchaeaWorld.factoryBrick,
            Wall = ArchaeaWorld.factoryBrickWallUnsafe,
            Tile2 = ArchaeaWorld.Ash,
            ConveyerL = TileID.ConveyorBeltLeft,
            ConveyerR = TileID.ConveyorBeltRight,
            Door = ArchaeaWorld.factoryMetalDoor;
        public static IList<Room> room = new List<Room>();
        public void CastleGen(out ushort[,] tile, out ushort[,] background, int width, int height, int size = 4, int maxNodes = 50, float nodeDistance = 60)
        {
            Width = width;
            Height = height;

            var brush = new ushort[width + size * 2, height + size * 2];
            background = new ushort[width, height];

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
                Room r = new Room(Main.rand.Next(RoomID.Total));
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
                        r.bound = new Rectangle(X1 - border, Y1 - border, W + border, H + border);
                        if (room.FirstOrDefault(t => t.bound.Intersects(r.bound)) != default)
                        {
                            continue;
                        }
                        for (int i = X1 - border; i < X2 + border; i++)
                        {
                            for (int j = Y1 - border; j < Y2 + border; j++)
                            {
                                //  If tile in-bounds
                                if (i > 0 && j > 0 && i < width && j < height)
                                {
                                    if (i < brush.GetLength(0) && j < brush.GetLength(1))
                                    {
                                        brush[i, j] = Air;
                                        if (i <= X1 || i >= X2 || j <= Y1 || j >= Y2)
                                        {
                                            if (i > X1 && i < X2 && j >= Y2)
                                            {
                                                //  Floor
                                                brush[i, j] = Tile2;
                                                continue;
                                            }
                                            //  Ceiling and walls
                                            brush[i, j] = Tile;
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
            for (int i = 0; i < brush.GetLength(0); i++)
            {
                for (int j = 0; j < brush.GetLength(1); j++)
                {
                    int cx  = brush.GetLength(0) / 2 - 10;
                    int cx2 = brush.GetLength(0) / 2 + 10;
                    if (i >= cx && i <= cx2)
                    {
                        brush[i, j] = Air;
                    }
                }
            }
            
            //  Return value
            tile = brush;
        }
        private void CarveHall(ref ushort[,] tile, ref ushort[,] wall, int x, int y, int size = 10)
        {
            int border = 4;
            bool flag = Main.rand.NextBool(8);
            bool flag2 = Main.rand.NextBool();
            for (int i = -border; i < size + border; i++)
            {
                for (int j = -border; j < size + border; j++)
                {
                    int X = Math.Max(0, Math.Min(x + i, Width - 1));
                    int Y = Math.Max(0, Math.Min(y + j, Height - 1));
                    var r = room.FirstOrDefault(t => t.bound.Intersects(new Rectangle(X, Y, size + border, size + border)));
                    if (r != default)
                    {
                        continue;
                    }
                    if (wall[X, Y] != Wall)
                    { 
                        tile[X, Y] = Tile;
                    }
                    if (flag && j == size - 1)
                    {
                        tile[X, Y] = flag2 ? ConveyerL : ConveyerR;
                    }
                }
            }
            for (int j = 0; j < size; j++)
            {
                for (int i = 0; i < size; i++)
                {
                    int X = Math.Max(0, Math.Min(x + i, Width - 1));
                    int Y = Math.Max(0, Math.Min(y + j, Height - 1));
                    
                    if (!GetSafely(X, Y - 1).HasTile && GetSafely(X, Y + 1).HasTile && (tile[X, Y] == ConveyerL || tile[X, Y] == ConveyerR))
                    {
                        continue;
                    }
                    tile[X, Y] = Air;
                    wall[X, Y] = Wall;
                    if (j == 0 && Main.rand.NextBool(60))
                    {
                        for (int l = 0; l < 6; l++)
                        {
                            tile[X, Y + l] = Door;
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
            int m = Math.Max(buffer, Math.Min(Main.maxTilesX - buffer, i));
            int n = Math.Max(buffer, Math.Min(Main.maxTilesY - buffer, j));
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
                        if (GetSafely(i, j).WallType == Wall && (!GetSafely(i - 1, j).HasTile || GetSafely(i + 1, j).HasTile))
                        {
                            if (GetSafely(i, j + 1).TileType != Tile && GetSafely(i, j + 1).TileType == Tile)
                            {
                                var _t = GetSafely(i, j + 1);
                                _t.HasTile = false;
                                WorldGen.PlaceTile(i, j + 1, TileID.Timers, true, true);
                            }
                            var tile = GetSafely(i, j);
                            tile.active(true);
                            tile.type = TileID.AdamantiteBeam;
                            tile.RedWire = true;
                            tile.HasActuator = true;
                            tile.IsActuated = true;
                            var tile2 = GetSafely(i, j + 1);
                            tile2.RedWire = true; 
                        }
                    }
                }
            }
            //  Steps
            int countLeftY = 0;
            int countRightY = 0;
            for (int i = 0; i < width; i++)
            {
                countLeftY  = 0;
                countRightY = 0;
                for (int j = y; j < y + height; j++)
                {
                    if (GetSafely(i, j).WallType == Wall && !GetSafely(i, j).HasTile && GetSafely(i - 1, j).HasTile && GetSafely(i - 1, j).TileType == Tile)
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
                            if (GetSafely(i, j).WallType == Wall && !GetSafely(i, j).HasTile && GetSafely(i - 1, j).HasTile && GetSafely(i - 1, j).TileType == Tile)
                            {
                                for (int m = 0; m < stepsLeft.GetLength(1); m++)
                                {
                                    for (int n = 0; n < stepsLeft.GetLength(0); n++)
                                    {
                                        switch (stepsLeft[m, n])
                                        {
                                            case 1:
                                                WorldGen.PlaceTile(i + m, j + n, Tile2, true, true);
                                                break;
                                            case 2:
                                                WorldGen.PlaceTile(i + m, j + n, Tile, true, true);
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
                    if (GetSafely(i, j).WallType == Wall && !GetSafely(i, j).HasTile && GetSafely(i + 1, j).HasTile && GetSafely(i + 1, j).TileType == Tile)
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
                            if (GetSafely(i, j).WallType == Wall && !GetSafely(i, j).HasTile && GetSafely(i + 1, j).HasTile && GetSafely(i + 1, j).TileType == Tile)
                            {
                                for (int m = 0; m < stepsRight.GetLength(1); m++)
                                {
                                    for (int n = 0; n < stepsRight.GetLength(0); n++)
                                    {
                                        switch (stepsRight[m, n])
                                        {
                                            case 1:
                                                WorldGen.PlaceTile(i - m, j + n, Tile2, true, true);
                                                break;
                                            case 2:
                                                WorldGen.PlaceTile(i - m, j + n, Tile, true, true);
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
                int Top = y + r.bound.Y;
                int Right = r.bound.Right;
                int Bottom = y + r.bound.Bottom;
                int Left = r.bound.X;
                for (int i = Left; i < Right; i++)
                {
                    for (int j = Top; j < Bottom; j++)
                    {
                        if (Main.rand.NextBool())
                        { 
                            if (GetSafely(i, j - 8).HasTile && !GetSafely(i, j - 7).HasTile && GetSafely(i, j).WallType == Wall &&  GetSafely(i - 1, j).HasTile && GetSafely(i - 1, j).TileType == Tile && !GetSafely(i, j).HasTile)
                            {
                                for (int m = 0; m < 5; m++)
                                {
                                    WorldGen.PlaceTile(i + m, j, Tile, true, true);
                                    WorldGen.PlaceTile(i - m, j + 1, Tile, true, true);
                                }
                                bool placed = false;
                                for (int m = 0; m < 5; m++)
                                {
                                    if (!placed)
                                    {
                                        t.PlaceTile(i + m, j + 2, (ushort)ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>(), true, false, 4, false);
                                        placed = Main.tile[i + m, j + 2].type == ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>();
                                    }
                                    if (m == 2) 
                                    {
                                        if (Main.rand.NextBool(8))
                                        { 
                                            WorldGen.PlaceTile(i + m, j - 1, ModContent.TileType<ArchaeaMod.Tiles.m_chair>(), true, true);
                                        }
                                    }
                                }
                                goto END;
                            }
                        }
                        if (Main.rand.NextBool())
                        {
                            if (GetSafely(i, j - 8).HasTile && !GetSafely(i, j - 7).HasTile && GetSafely(i, j).WallType == Wall && GetSafely(i + 1, j).HasTile && GetSafely(i + 1, j).TileType == Tile && !GetSafely(i, j).HasTile)
                            {
                                for (int m = 0; m < 5; m++)
                                {
                                    WorldGen.PlaceTile(i - m, j, Tile, true, true);
                                    WorldGen.PlaceTile(i - m, j + 1, Tile, true, true);
                                }
                                bool placed = false;
                                for (int m = 0; m < 5; m++)
                                {
                                    if (!placed)
                                    { 
                                        t.PlaceTile(i + m, j + 2, (ushort)ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>(), true, false, 4, false);
                                        placed = Main.tile[i + m, j + 2].type == ModContent.TileType<ArchaeaMod.Tiles.m_chandelier>();
                                    }
                                    if (m == 2)
                                    {
                                        if (Main.rand.NextBool(8))
                                        {
                                            WorldGen.PlaceTile(i - m, j - 1, ModContent.TileType<ArchaeaMod.Tiles.m_chair>(), true, true);
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
                        if ((GetSafely(i - 1, j).HasTile || GetSafely(i + 1, j).HasTile) && GetSafely(i, j).WallType == Wall)
                        {
                            WorldGen.PlaceTile(i, j, TileID.Chain);
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
                    if (GetSafely(i + 1, j).WallType == Wall && GetSafely(i, j).TileType == Tile && GetSafely(i, j).HasTile && !GetSafely(i, j - 1).HasTile && !GetSafely(i + 1, j).HasTile)
                    {
                        flag = true;
                    }
                    if (flag)
                    {
                        for (int m = 0; m < 10; m++)
                        {
                            WorldGen.PlaceTile(i + m, j, TileID.Platforms, true, false); 
                            if (GetSafely(i + m + 1, j).HasTile)
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
