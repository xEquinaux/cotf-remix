using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cotf.Base;
using cotf.Collections;
using cotf.ID;
using cotf.World;
using cotf.World.Traps;
using Microsoft.Xna.Framework;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    public class Worldgen 
    {
        public static Worldgen Instance;
        private const byte 
            maxTorches = 8,
            maxItems = 12;
        int[,] room0 = new int[,]
        {
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 0, 1, 0, 1, 0, 0 },
            { 0, 1, 1, 0, 1, 1, 0 },
            { 0, 1, 0, 0, 0, 1, 0 },
            { 1, 1, 0, 0, 0, 1, 1 },
            { 1, 0, 0, 0, 0, 0, 1 },
            { 1, 0, 0, 0, 0, 0, 1 },
            { 1, 1, 1, 1, 1, 1, 1 }
        };
        
        public Worldgen()
        {
            Instance = this;
        }

        public static void UpdateLampMaps()
        {
            foreach (Lamp lamp in Main.lamp)
            {
                if (lamp == null || !lamp.active) continue;
                int radius = (int)lamp.range / 2;
                for (int i = (int)lamp.Center.X - radius; i <= lamp.Center.X + radius; i += Tile.Size)
                {
                    for (int j = (int)lamp.Center.Y - radius; j <= lamp.Center.Y + radius; j += Tile.Size)
                    {
                        int x = i / Tile.Size;
                        int y = j / Tile.Size;
                        Lightmap.GetSafely(x, y).LampEffect(lamp);
                    }
                }
            }
        }
        
        public Lightmap[,] InitLightmap(int width, int height)
        {
            var map = new Lightmap[width / Lightmap.Size.Width, height / Lightmap.Size.Height];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = new Lightmap(i, j);
                }
            }
            return map;
        }
        public Lighting[,] Fog(int width, int height)
        {
            width /= Lighting.Size;
            height /= Lighting.Size;
            var fog = new Lighting[width, height];
            for (int i = 0; i < fog.GetLength(0); i++)
            {
                for (int j = 0; j < fog.GetLength(1); j++)
                {
                    fog[i, j] = new Lighting(i * Lighting.Size, j * Lighting.Size);
                }
            }
            return fog;
        }
        public static void GenerateFloor(int size = 50, int width = 3000, int height = 3000, int maxNodes = 7, float range = 300f, float nodeDistance = 800f)
        {
            Main.WorldWidth = width;
            Main.WorldHeight = height;

            Main.tile = Worldgen.Instance.CastleGen(size, width, height, maxNodes, range, nodeDistance);
            
            if (Main.FloorNumber > 0)
            {
                
            }
            //if (Main.rand.NextBool()) Main.tile = Worldgen.Instance.CastleGen(size, width, height, maxNodes, range, nodeDistance);
            //else                         Main.tile = Worldgen.Instance.DungeonGen(size, width, height, maxNodes, range);
        }
        public Tile[,] CastleGen(int size, int width, int height, int maxNodes = 5, float range = 300f, float nodeDistance = 800f)
        {
            //  Constructing values
            width += width % size;
            height += height % size;
            var brush = new Tile[width / size, height / size];
            Main.background = new Background[width / size, height / size];

            //  Filling entire space with background
            for (int j = 0; j < height; j += size)
            {
                for (int i = 0; i < width; i += size)
                {
                    if (i / size < Main.background.GetLength(0) && j / size < Main.background.GetLength(1))
                    {
                        Main.background[i / size, j / size] = new Background(i / size, j / size, size);
                    }
                }
            }

            Vector2[] nodes = new Vector2[maxNodes];
            int numNodes = 0;

            //  Filling entire space with brushes
            for (int j = 1; j < height; j += size)
            {
                for (int i = 1; i < width; i += size)
                {
                    if (i / size < brush.GetLength(0) && j / size < brush.GetLength(1))
                    { 
                        brush[i / size, j / size] = new Tile(i, j);
                    }
                }
            }

            //  Generating vector nodes
            int randX = 0,
                randY = 0;
            while (numNodes < maxNodes)
            {
                foreach (Vector2 node in nodes)
                {
                    do
                    {
                        randX = Main.rand.Next(size, width - size);
                        randY = Main.rand.Next(size, height - size);
                        nodes[numNodes] = new Vector2(randX, randY);
                    } while (nodes.All(t => Main.Distance(t, nodes[numNodes]) < nodeDistance));
                    numNodes++;
                }
            }

            //  Carve out rooms
            int W = 0, H = 0;
            const int maxSize = 7;
            int index = 0;
            foreach (Vector2 node in nodes)
            {
                Room room = new Room((short)Main.rand.Next(5, 7));
                int rand = Main.rand.Next(2);
                switch (rand)
                {
                    case 0:
                        W = Main.rand.Next(4, maxSize) * size;
                        H = Main.rand.Next(4, maxSize) * size;
                        //  Room construction
                        room.region = new short[W / size, H / size];
                        room.bounds = new Rectangle((int)node.X - W / 2, (int)node.Y - H / 2, W, H);
                        for (int i = (int)node.X - W / 2; i < node.X + W / 2; i++)
                        {
                            for (int j = (int)node.Y - H / 2; j < node.Y + H / 2; j++)
                            {
                                if (i > 0 && j > 0 && i < width && j < height)
                                {
                                    if (i / size < brush.GetLength(0) && j / size < brush.GetLength(1))
                                    { 
                                        Tile tile;
                                        (tile = brush[i / size, j / size]).active(false);
                                        //room.region[i / size, j / size] = tile;
                                    }
                                }
                            }
                        }
                        Main.room.Add(index++, room);
                        break;
                    case 1:
                        //  Room construction
                        room.region = new short[room0.GetLength(0), room0.GetLength(1)];
                        room.bounds = new Rectangle((int)node.X, (int)node.Y, room0.GetLength(0) * size, room0.GetLength(1) * size);
                        for (int i = 0; i < room0.GetLength(0); i++)
                        {
                            for (int j = 0; j < room0.GetLength(1); j++)
                            {
                                W = i * size + (int)node.X;
                                H = j * size + (int)node.Y;
                                W += W % size;
                                H += H % size;
                                if (W > 0 && H > 0 && W < width && H < height)
                                {
                                    if (W / size < brush.GetLength(0) && H / size < brush.GetLength(1))
                                    { 
                                        Tile tile;
                                        (tile = brush[W / size, H / size]).active(false);
                                        //room.region[W / size, H / size] = tile;
                                    }
                                }
                            }
                        }
                        room.InitRoom();
                        Main.room.Add(index++, room);
                        break;
                    default:
                        break;
                }
            }

            //  Generating hallways
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
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X < end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }

                //  Reversed pass
                start = nodes[k];
                end = nodes[k - 1];

                if (start.X < end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y < end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (Y++ <= (start.Y + end.Y) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X < end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                else if (start.X > end.X && start.Y > end.Y)
                {
                    X = (int)start.X + (int)start.X % size;
                    Y = (int)start.Y + (int)start.Y % size;

                    while (X++ <= (start.X + end.X) / 2 + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (Y++ <= end.Y + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                    while (X++ <= end.X + size)
                    {
                        brush[Math.Min(X / size, width / size - 1), Math.Min(Y / size, height / size - 1)].active(false);
                    }
                }
                #endregion

                //  Placing doors
                bool flag = false;
                bool randFlagX = Main.rand.NextBool();
                bool randFlagY = Main.rand.NextBool();
                for (int i = randFlagX ? -20 : 20; i < (randFlagX ? 20 : -20); i += randFlagX ? 1 : -1)
                {
                    for (int j = randFlagY ? -20 : 20; j < (randFlagY ? 20 : -20); j += randFlagY ? 1 : -1)
                    {
                        X = (int)(start.X + (int)start.X % size + i * size) / size;
                        Y = (int)(start.Y + (int)start.Y % size + j * size) / size;
                        //  Bottom
                        if (!Tile.GetSafely(X, Y, width, height, brush).Active && !Tile.GetSafely(X + 1, Y - 1, width, height, brush).Active && !Tile.GetSafely(X - 1, Y - 1, width, height, brush).Active && Tile.GetSafely(X - 1, Y, width, height, brush).Active && Tile.GetSafely(X + 1, Y, width, height, brush).Active && !Tile.GetSafely(X, Y + 1, width, height, brush).Active && !Tile.GetSafely(X, Y - 1, width, height, brush).Active)
                        {
                            Door.CreateDoor(false, Tile.GetSafely(X, Y, width, height, brush), Direction.Bottom);
                            flag = true;
                            break;
                        }
                        //  Top
                        if (!Tile.GetSafely(X, Y, width, height, brush).Active && !Tile.GetSafely(X + 1, Y + 1, width, height, brush).Active && !Tile.GetSafely(X - 1, Y + 1, width, height, brush).Active && Tile.GetSafely(X - 1, Y, width, height, brush).Active && !Tile.GetSafely(X + 1, Y, width, height, brush).Active && !Tile.GetSafely(X, Y + 1, width, height, brush).Active && !Tile.GetSafely(X, Y - 1, width, height, brush).Active)
                        {
                            Door.CreateDoor(false, Tile.GetSafely(X, Y, width, height, brush), Direction.Top);
                            flag = true;
                            break;
                        }
                        //  Right 
                        if (!Tile.GetSafely(X, Y, width, height, brush).Active && !Tile.GetSafely(X + 1, Y + 1, width, height, brush).Active && !Tile.GetSafely(X + 1, Y - 1, width, height, brush).Active && Tile.GetSafely(X, Y + 1, width, height, brush).Active && Tile.GetSafely(X, Y - 1, width, height, brush).Active && !Tile.GetSafely(X - 1, Y, width, height, brush).Active && !Tile.GetSafely(X + 1, Y, width, height, brush).Active)
                        {
                            Door.CreateDoor(false, Tile.GetSafely(X, Y, width, height, brush), Direction.Right);
                            flag = true;
                            break;
                        }
                        //  Left
                        if (!Tile.GetSafely(X, Y, width, height, brush).Active && !Tile.GetSafely(X - 1, Y + 1, width, height, brush).Active && !Tile.GetSafely(X - 1, Y - 1, width, height, brush).Active && Tile.GetSafely(X, Y + 1, width, height, brush).Active && Tile.GetSafely(X, Y - 1, width, height, brush).Active && !Tile.GetSafely(X - 1, Y, width, height, brush).Active && !Tile.GetSafely(X + 1, Y, width, height, brush).Active)
                        {
                            Door.CreateDoor(false, Tile.GetSafely(X, Y, width, height, brush), Direction.Left);
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        break;
                }
            }

            //  Map boundaries
            int m = 0, n = 0;
            while (true)
            {
                for (int i = 0; i < width; i += size)
                {
                    if (i / size < brush.GetLength(0) && m / size < brush.GetLength(1))
                    {    
                        brush[i / size, m / size] = new Tile(i, m);
                        brush[i / size, m / size].width += 1;
                        brush[i / size, m / size].height += 1; 
                    }
                }
                if (m < height - size)
                {
                    m = height - size;
                    continue;
                }
                break;
            }
            while (true)
            {
                for (int j = 0; j < height; j += size)
                {
                    if (n / size < brush.GetLength(0) && j / size < brush.GetLength(1))
                        brush[n / size, j / size] = new Tile(n, j);
                }
                if (n < width - size)
                {
                    n = width - size;
                    continue;
                }
                break;
            }

            #region TODO placing world objects
            int num = 0;
            int numTraps = 0;
            int numDown = 0, numUp = 0;
            int numChests = 0;
            int numItems = 0;
            int numNPCs = 0;
            int numTorches = 0;
            const float mult = 1.5f;
            //SquareBrush.InitializeArray(brush.Length);
            while (numTorches < maxTorches || numItems < maxItems)
            {
                foreach (var b in brush)
                {
                    //  Adding background objects
                    if (!b.Active) new Background(b.X / size, b.Y / size, size);

                    //  Adding tile objects
                    for (int k = 0; k < nodes.Length; k++)
                    {
                        if (!b.Active && Main.Distance(nodes[k], b.Center) < range * mult)
                        {
                            Vector2 randv2 = Vector2.Zero;
                            do
                            {
                                randX = Main.rand.Next(size, width - size);
                                randY = Main.rand.Next(size, height - size);
                                randv2 = new Vector2(randX, randY);
                            } while (brush[randX / size, randY / size].Active);
                            int rand = Main.rand.Next(8);
                            randv2.X -= randv2.X % size;
                            randv2.Y -= randv2.Y % size;
                            switch (rand)
                            {
                                case TileID.Empty:
                                    break;
                                case TileID.Item:
                                    if (numItems++ < 12)
                                    { 
                                        Item.NewItem(randv2.X, randv2.Y, 32, 32, (short)(Main.rand.Next(9) + 1));
                                    }
                                    break;
                                case TileID.Torch:
                                    //  Unoptimized: causes large slowdown
                                    if (numTorches++ < maxTorches)
                                    {
                                        int offsetX = Main.rand.Next(Tile.Size);
                                        int offsetY = Main.rand.Next(Tile.Size);
                                        index = Lamp.NewLamp(new Vector2(randv2.X + offsetX, randv2.Y + offsetY), 200f, Lamp.RandomLight(), b, true);
                                        Main.lamp[index].active = true;
                                        Main.lamp[index].owner = 255;
                                        //b.lamp = Main.lamp[index];
                                    }
                                    break;
                                case TileID.Monster:
                                    if (numNPCs++ < 12)
                                        Npc.NewNPC((int)randv2.X + randv2.X % Tile.Size + Tile.Size / 10, (int)randv2.Y + randv2.Y % Tile.Size + Tile.Size / 10, NpcType.Kobold);
                                    break;
                                case TileID.Trap:
                                    if (numTraps++ < 10)
                                        Trap.NewTrap((int)randv2.X, (int)randv2.Y, size, size, (short)(Main.rand.Next(TrapID.Sets.Total - 1) + 1));
                                    break;
                                case TileID.Chest:
                                    if (numChests++ < 3)
                                        Stash.NewStash((int)(randv2.X + randv2.X % Tile.Size + Tile.Size / 10), (int)(randv2.Y + randv2.Y % Tile.Size + Tile.Size / 10), 0, Item.FillStash((int)randv2.X, (int)randv2.Y, Main.rand.Next(3, 12)));
                                    break;
                                case TileID.StairsDown:
                                    break;
                                case TileID.StairsUp:
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            #endregion
            //  Pre lighting random rooms
            for (int k = 0; k < Main.room.Count; k++)
            {
                Room room = Main.room[k];
                if (Main.rand.NextFloat() > 0.33f)
                    continue;
                for (int i = room.bounds.Left; i < room.bounds.Right + Tile.Size; i += Tile.Size)
                {
                    for (int j = room.bounds.Top; j < room.bounds.Bottom + Tile.Size; j += Tile.Size)
                    {
                        Background bg = Background.GetSafely(i / Tile.Size, j / Tile.Size);
                        if (bg == null || !bg.active)
                            continue;
                        bg.lit = true;
                    }
                }
            }
            while (numDown < 1)
            {
                Vector2 randv2 = Vector2.Zero;
                do
                {
                    randX = Main.rand.Next(size, width - size);
                    randY = Main.rand.Next(size, height - size);
                    randv2 = new Vector2(randX, randY);
                } while (brush[randX / size, randY / size].Active);
                randv2.X -= randv2.X % size;
                randv2.Y -= randv2.Y % size;
                if (!PlaceDownStairs(ref numDown, randv2, range))
                    continue;
            }
            while (numUp < 1)
            {
                Vector2 randv2 = Vector2.Zero;
                do
                {
                    randX = Main.rand.Next(size, width - size);
                    randY = Main.rand.Next(size, height - size);
                    randv2 = new Vector2(randX, randY);
                } while (brush[randX / size, randY / size].Active);
                randv2.X -= randv2.X % size;
                randv2.Y -= randv2.Y % size;
                if (!PlaceUpStairs(ref numUp, randv2, range))
                    continue;
            }
            return brush;
        }
        private bool PlaceDownStairs(ref int numDown, Vector2 randv2, float range)
        {
            if (numDown < 1)
            { 
                //  Place down stairs
                Vector2 vector2 = randv2;
                var up = Main.staircase.Where(t => t != null && t.direction == StaircaseDirection.LeadingUp);
                if (up.Count() > 0)
                {
                    var stair = up.First();
                    if (Helper.Distance(stair.Center, vector2) > range)
                    {
                        Staircase.NewStaircase((int)vector2.X, (int)vector2.Y, stair, StaircaseDirection.LeadingDown);
                        numDown++;
                        return true;
                    }
                }
                else
                {
                    Staircase.NewStaircase((int)vector2.X, (int)vector2.Y, StaircaseDirection.LeadingDown);
                    numDown++;
                    return true;
                }
            }
            return false;
        }
        private bool PlaceUpStairs(ref int numUp, Vector2 randv2, float range)
        {
            if (numUp < 1)
            {
                //  Place up stairs
                Vector2 vector2 = randv2;
                var down = Main.staircase.Where(t => t != null && t.direction == StaircaseDirection.LeadingDown);
                if (down.Count() > 0)
                {
                    var stair = down.First();
                    if (Helper.Distance(stair.Center, vector2) > range)
                    {
                        Staircase.NewStaircase((int)vector2.X, (int)vector2.Y, stair, StaircaseDirection.LeadingUp);
                        numUp++;
                        return true;
                    }
                }
                else
                {
                    Staircase.NewStaircase((int)vector2.X, (int)vector2.Y, StaircaseDirection.LeadingUp);
                    numUp++;
                    return true;
                }
            }
            return false;
        }
        public Tile[,] DungeonGen(int size, int width, int height, int maxNodes = 4, float range = 300f)
        {
            //  Constructing values
            width += width % size;
            height += height % size;
            var brush = new Tile[width / size, height / size];
            Vector2[] nodes = new Vector2[maxNodes];
            int randX = 0, randY = 0;
            int numNodes = 0;

            //  Filling entire space with brushes
            for (int j = 0; j < height; j += size)
            {
                for (int i = 0; i < width; i += size)
                {
                    brush[i / size, j / size] = new Tile(i, j);
                }
            }

            //  Generating vector nodes
            while (numNodes < maxNodes)
            {
                foreach (Vector2 node in nodes)
                {
                    randX = Main.rand.Next(size, width - size);
                    randY = Main.rand.Next(size, height - size);
                    nodes[numNodes] = new Vector2(randX, randY);
                    numNodes++;
                }
            }

            //  Making rooms from node vectors                   
            foreach (var b in brush)
            {
                foreach (Vector2 node in nodes)
                {
                    if (Main.Distance(node, b.Center) < range)
                    {
                        b.active(false);
                    }
                }
            }

            //  Generating hallways
            for (int i = 1; i < nodes.Length; i++)
            {
                Vector2 start = nodes[i - 1];
                Vector2 end = nodes[i];
                while (Main.Distance(start, end) > size / 2)
                {
                    var line = Helper.AngleToSpeed(Helper.AngleTo(start, end), size / 3);
                    start.X += line.X;
                    start.Y += line.Y;
                    foreach (var b in brush)
                    {
                        if (Main.Distance(start, b.Center) < size * 1.34f)
                        {
                            b.active(false);
                        }
                    }
                }
            }
            //  Hallway generation reversal
            for (int i = nodes.Length - 1; i > 0; i--)
            {
                Vector2 start = nodes[i];
                Vector2 end = nodes[i - 1];
                while (Main.Distance(start, end) > size / 2)
                {
                    var line = Helper.AngleToSpeed(Helper.AngleTo(start, end), size / 3);
                    start.X += line.X;
                    start.Y += line.Y;
                    foreach (var b in brush)
                    {
                        if (Main.Distance(start, b.Center) < size * 1.34f)
                        {
                            b.active(false);
                        }
                    }
                }
            }

            //  Map boundaries
            int m = 0, n = 0;
            while (true)
            {
                for (int i = 0; i < width; i += size)
                {
                    brush[i / size, m / size] = new Tile(i, m);
                }
                if (m < height - size)
                {
                    m = height - size;
                    continue;
                }
                break;
            }
            while (true)
            {
                for (int j = 0; j < height; j += size)
                {
                    brush[n / size, j / size] = new Tile(n, j);
                }
                if (n < width - size)
                {
                    n = width - size;
                    continue;
                }
                break;
            }

            #region TODO add world objects
            //int num = 0;
            //int numDown = 0, numUp = 0;
            //int numTraps = 0;
            //int numItems = 0;
            //int numNpcs = 0;
            //int numTorches = 0;
            ////SquareBrush.InitializeArray(brush.Length);
            //while (numDown == 0 || numUp == 0)
            //{
            //    foreach (var b in brush)
            //    {
            //        if (!b.Active) Background.NewGround((int)b.Center.X - size / 2, (int)b.Center.Y - size / 2, size, size, 0, 128f);
            //        //  Adding tile objects
            //        Vector2 randv2 = Vector2.Zero;
            //        do
            //        {
            //            randX = Main.rand.Next(size, width - size);
            //            randY = Main.rand.Next(size, height - size);
            //            randv2 = new Vector2(randX, randY);
            //        } while (brush[randX / size, randY / size].Active);
            //        for (int k = 0; k < nodes.Length; k++)
            //        {
            //            if (!b.Active && b.Distance(nodes[k], b.Center) < range)
            //            {
            //                int rand = Main.rand.Next(13);
            //                switch (rand)
            //                {
            //                    case TileID.Empty:
            //                        break;
            //                    case TileID.Item:
            //                        if (numItems++ < 10)
            //                        {
            //                            Item.NewItem((int)randv2.X, (int)randv2.Y, 24, 24, Item.Owner_World, Main.rand.Next(11));
            //                        }
            //                        break;
            //                    case TileID.Torch:
            //                        //  Unoptimized: causes large slowdown
            //                        if (numTorches++ < maxTorches)
            //                            Light.NewTorch(randv2, Main.rand.Next(80, 150), 16, 0);
            //                        break;
            //                    case TileID.Monster:
            //                        if (numNpcs++ < 12)
            //                            NPC.NewNPC((int)randv2.X, (int)randv2.Y, NPCID.Kobold, Color.White);
            //                        break;
            //                    case TileID.Trap:
            //                        if (numTraps++ < 10)
            //                            Trap.NewTrap((int)randv2.X, (int)randv2.Y, size, TrapID.Acid);
            //                        break;
            //                    case TileID.StairsDown:
            //                        if (numDown < 1)
            //                        {
            //                            //  Place down stairs
            //                            Vector2 vector2 = randv2;
            //                            var up = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingUp);
            //                            if (up.Count() > 0)
            //                            {
            //                                var stair = up.First();
            //                                if (Distance(stair.Center, vector2) > range)
            //                                {
            //                                    Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingDown);
            //                                    numDown++;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingDown);
            //                                numDown++;
            //                            }
            //                        }
            //                        break;
            //                    case TileID.StairsUp:
            //                        if (numUp < 1)
            //                        {
            //                            //  Place up stairs
            //                            Vector2 vector2 = randv2;
            //                            var down = Main.stair.Where(t => t != null && t.transition == Staircase.Transition.GoingDown);
            //                            if (down.Count() > 0)
            //                            {
            //                                var stair = down.First();
            //                                if (Distance(stair.Center, vector2) > range)
            //                                {
            //                                    Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingUp);
            //                                    numUp++;
            //                                }
            //                            }
            //                            else
            //                            {
            //                                Staircase.NewStairs((int)vector2.X, (int)vector2.Y, size, 0, Staircase.Transition.GoingUp);
            //                                numUp++;
            //                            }
            //                        }
            //                        break;
            //                    default:
            //                        break;
            //                }
            //            }
            //        }
            //        //  Inserting into main brush array
            //        for (int i = num; i < Main.square.Count; i++)
            //        {
            //            if (Main.square[i] == null)
            //            {
            //                num = i;
            //                Main.square[i] = b;
            //                Main.square[i].active(b.Active);
            //                break;
            //            }
            //        }
            //    }
            //}
            #endregion
            return brush;
        }
    }
    public sealed class TileID
    {
        public const byte
            Empty = 0,
            StairsUp = 1,
            StairsDown = 2,
            Item = 3,
            Monster = 4,
            Torch = 5,
            Trap = 6,
            Chest = 7;
    }
    public sealed class SceneryType
    {
        public const byte
            Empty = 0,
            Ash = 1,
            Ore = 2,
            Stone = 3,
            Plant = 4,
            Furniture = 5,
            Pillar = 6,
            Torch = 7,
            CampFire = 8,
            Dirt = 9,
            Tile = 10,
            Wood = 11,
            Web = 12,
            Trap = 13;
    }
}

namespace cotf.Persistence
{
}