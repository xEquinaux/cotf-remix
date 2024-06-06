using cotf;
using cotf.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ArchaeaMod.Unused
{
    public class MagnoGen
    {
        public int bottomLimit = 250;
        public int sideLimit = 100;
        public int ceiling
        {
            get { return 0; }//(int)GenVars.worldSurfaceLow / 16; }
        }
        public int floor
        {
            get { return 300; }//GenVars.tBottom - bottomLimit; }
        }
        public Vector2[] DigStarts(int total)
        {
            List<Vector2> centers = new List<Vector2>();
            int randX = Main.rand.Next(sideLimit, /*GenVars.tRight - */sideLimit);
            int randY = Main.rand.Next(ceiling, floor);
            for (int i = 0; i < total; i++)
                centers.Add(new Vector2(randX, randY));
            return centers.ToArray();
        }
        public Vector2[] Path()
        {
            List<Vector2> list = new List<Vector2>();
            
            return list.ToArray();
        }
    }
}

namespace ArchaeaMod.GenLegacy
{
    public class SkyDen
    {
        public static bool active = true;
        public bool complete;
        public static bool miniBiome;
        private int width
        {
            get { return (int)4800 / 16 / 3; }
        }
        private int leftBounds
        {
            get { return Main.rand.Next(width, width * 2); }
        }
        private int upperBounds
        {
            get { return 1600 / 3 + 50; }
        }
        public int maxY
        {
            get { return (int)1600 / 16 - 200; }
        }
        private int centerX
        {
            get { return leftBounds + width / 2; }
        }
        private int centerY
        {
            get { return (int)(1600 / 16 / 1.5f); }
        }
        public int lookFurther;
        public static int whoAmI = 0;
        public static readonly int max = 80;
        private readonly int border = 3;
        private readonly int cave = 1;
        private int x
        {
            get { return (int)center.X; }
        }
        private int y
        {
            get { return (int)center.Y; }
        }
        public int points;
        private int cycle;
        private int id;
        private int X = 0;//Main.maxTilesX;
        private int Y = 0;//Main.maxTilesY;
        private int Width;
        private int Height;
        private int iterate;
        private float range;
        public Vector2 center;
        public static Vector2 origin;
        private Vector2 position;
        public static Microsoft.Xna.Framework.Rectangle[] bounds;
        public static Dictionary<Vector2, int> plots = new Dictionary<Vector2, int>();
        public static SkyDen[] mDen;
        private SkyDen den;
        public SkyDen(Vector2 position, float range)
        {
            this.position = position;
            origin = position;
            center = position;
            this.range = range;
        }
        public void Start(SkyDen den, bool miniBiome = true, int iterate = 8)
        {
            active = true;
            this.iterate = iterate;
            mDen = new SkyDen[max];
            bounds = new Rectangle[max / iterate];
            for (int i = 0; i < bounds.Length; i++)
                bounds[i] = Rectangle.Empty;
            mDen[whoAmI] = den;
            mDen[whoAmI].id = whoAmI;
            mDen[whoAmI].center = center;
            this.den = mDen[whoAmI];
        }
        public void Update()
        {
            if (den == mDen[0])
            {
                while (whoAmI < max - 1)
                {
                    foreach (SkyDen s in mDen.Where(t => t != null && Vector2.Distance(position - t.center, Vector2.Zero) > range))
                        MoveBackInRange(s);
                    SkyDen m = mDen[whoAmI];
                    if (m == null)
                    {
                        whoAmI++;
                        continue;
                    }
                    m.CheckComplete(2);
                    while (!m.StandardMove()) ;
                    m.lookFurther = 0;
                    m.points = 0;
                    if (m.center.Y > m.maxY)
                    {
                        int block = -200;
                        bool bRand = Main.rand.Next(2) == 0;
                        m.center += new Vector2(bRand ? block / 4 : block * -1 / 4, block);
                    }
                }
            }
            den.GetBounds();
            den.FinalDig();
            den.Terminate();
        }
        public void MoveBackInRange(SkyDen den)
        {
            Action method = delegate ()
            {
                Vector2 newPosition = Vector2.Zero;
                do
                {
                    newPosition = new Vector2(Main.rand.NextFloat(position.X - range / 2, position.X + range / 2), Main.rand.NextFloat(position.Y - range / 2, position.Y + range / 2));
                } while ((position - newPosition).Length() > range);
                den.center = newPosition;
            };
        }
        public bool StandardMove()
        {
            int size = Main.rand.Next(1, 4);
            int rand = Main.rand.Next(1, 5);
            if (Main.rand.Next(1, 4) == 1 && Main.tile[x + 1 + lookFurther, y].Active)
            {
                center.X += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (Main.rand.Next(1, 4) == 1 && Main.tile[x - 1 - lookFurther, y].Active)
            {
                center.X -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (Main.rand.Next(1, 4) == 1 && Main.tile[x, y + 1 + lookFurther].Active && center.Y < maxY)
            {
                center.Y += 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (Main.rand.Next(1, 4) == 1 && Main.tile[x, y - 1 - lookFurther].Active && center.Y > upperBounds)
            {
                center.Y -= 1f;
                lookFurther = 0;
                points++;
                DigPlot(size);
            }
            if (!Main.tile[x + 1 + lookFurther, y].Active &&
                !Main.tile[x - 1 - lookFurther, y].Active &&
                !Main.tile[x, y + 1 + lookFurther].Active &&
                !Main.tile[x, y - 1 - lookFurther].Active)
                lookFurther++;
            if (!plots.ContainsKey(center))
                plots.Add(center, size);
            if (points > 10)
                return true;
            return false;
        }
        public void VerticalMove()
        {
            Vector2 old = center;
            while (center == old)
            {
                int rand = Main.rand.Next(1, 5);
                int x = (int)center.X;
                int y = (int)center.Y;
                switch (rand)
                {
                    case 1:
                        do
                        {
                            center.X += 1f;
                            lookFurther++;
                        } while (!Main.tile[x + 1 + lookFurther, y].Active
                                     //TODO Main.rightWorld
                                && x < 4800 / 16);
                        break;
                    case 2:
                        do
                        {
                            center.X -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x - 1 - lookFurther, y].Active
                                && x > 50);
                        break;
                    case 3:
                        do
                        {
                            center.Y += 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y + 1 + lookFurther].Active
                                     //TODO Main.bottomWorld
                                && y < 1600 / 16);
                        break;
                    case 4:
                        do
                        {
                            center.Y -= 1f;
                            lookFurther++;
                        } while (!Main.tile[x, y - 1 - lookFurther].Active
                                && y > maxY);
                        break;
                    default:
                        break;
                }
                if (lookFurther % 2 == 0)
                    PlaceWater(center);
                lookFurther = 0;
            }
        }
        public bool AverageMove()
        {
            Vector2 old = center;
            if (Main.rand.Next(1, 4) == 1) center.X += 1f;
            if (Main.rand.Next(1, 4) == 1) center.X -= 1f;
            if (Main.rand.Next(1, 4) == 1) center.Y += 1f;
            if (Main.rand.Next(1, 4) == 1) center.Y -= 1f;
            return center != old;
        }
        public SkyDen GenerateNewMiner()
        {
            if (this == mDen[0])
            {
                whoAmI++;
                if (whoAmI == max)
                    FinalDig();
                if (whoAmI < max)
                {
                    mDen[whoAmI] = new SkyDen(position, range);
                    mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                }
                else
                    Terminate();
            }
            return mDen[Math.Min(whoAmI, max - 1)];
        }
        public Vector2 NewPosition(Vector2 previous)
        {
            return new Vector2(previous.X, origin.Y);
        }
        public static bool Inbounds(int x, int y)
        {            //TODO                     //TODO
            return x < 4800 - 50 && x > 50 && y < 1600 - 200 && y > 50;
        }
        public void DigPlot(int size)
        {
            for (int i = (int)center.X - size; i < (int)center.X + size; i++)
                for (int j = (int)center.Y - size; j < (int)center.Y + size; j++)
                {
                    if (Inbounds(i, j))
                    {
                        if (Main.rand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type = TileID.PearlstoneBrick;
                        cotf.World.Tile tile = Main.tile[i, j];
                        tile.active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, false, true);
                    }
                }
        }
        public void FinalDig()
        {
            var v2 = plots.Keys.ToArray();
            var s = plots.Values.ToArray();
            for (int k = 1; k < v2.Length; k++)
            {
                int x = (int)v2[k].X;
                int y = (int)v2[k].Y;
                for (int i = x - s[k] * border; i < x + s[k] * border; i++)
                {
                    for (int j = y - s[k] * border; j < y + s[k] * border; j++)
                    {
                        Main.tile[i, j].type = TileID.PearlstoneBrick;
                        cotf.World.Tile tile = Main.tile[i, j];
                        tile.active(true);
                        //  WorldGen.PlaceTile(i, j, TileID.PearlstoneBrick, true, true);
                        //  WorldGen.KillWall(i, j);
                    }
                }
            }
            for (int l = 1; l < v2.Length; l++)
            {
                int x = (int)v2[l].X;
                int y = (int)v2[l].Y;
                for (int i = (int)x - s[l]; i < (int)x + s[l]; i++)
                    for (int j = (int)y - s[l]; j < (int)y + s[l]; j++)
                    {
                        if (Main.rand.Next(60) == 0)
                            PlaceWater(new Vector2(i, j));
                        Main.tile[i, j].type = 0;
                        cotf.World.Tile tile = Main.tile[i, j];
                        tile.active(false);
                        //  WorldGen.KillTile(i, j, false, false, true);
                    }
            }
        }
        public void PlaceWater(Vector2 position)
        {
            int x = (int)position.X;
            int y = (int)position.Y;
            if (Inbounds(x, y))
            { 
                //WorldGen.PlaceLiquid(x, y, (byte)LiquidID.Water, 60);
            }
        }
        public void CheckComplete(int divisor = 2)
        {
            cycle++;
            if (cycle == max / divisor)
            {
                whoAmI++;
                if (whoAmI < mDen.Length)
                {
                    mDen[whoAmI] = new SkyDen(position, range);
                    mDen[whoAmI].id = whoAmI;
                    if (miniBiome)
                    {
                        if (whoAmI % iterate == 0)
                            mDen[whoAmI].center = NewPosition(mDen[whoAmI - 1].center);
                        else mDen[whoAmI].center = mDen[whoAmI - 1].center;
                    }
                    else mDen[whoAmI].center = mDen[whoAmI - 1].center;
                }
            }
        }
        public void GetBounds()
        {
            int count = 0;
            foreach (SkyDen m in mDen)
                if (m != null && m.center != Vector2.Zero)
                {
                    if (m.center.X < X)
                        X = (int)m.center.X;
                    if (m.center.Y < Y)
                        Y = (int)m.center.Y;
                    if (m.center.X - X > Width)
                        Width = (int)m.center.X - X;
                    if (m.center.Y - Y > Height)
                        Height = (int)m.center.Y - Y;
                    count++;
                    if (miniBiome)
                    {
                        if (count % iterate == 0)
                        {
                            bounds[count] = (new Rectangle(X, Y, Width, Height));
                            X = 0;
                            Y = 0;
                            Width = 0;
                            Height = 0;
                        }
                    }
                    else bounds[0] = new Rectangle(X, Y, Width, Height);
                }
        }
        public void Terminate()
        {
            Unload();
            active = false;
            for (int i = 0; i < mDen.Length; i++)
                mDen[i] = null;
        }
        protected void Unload()
        {
            origin = Vector2.Zero;
            plots.Clear();
        }
    }
    public class SkyHall
    {
        private int bottom = 500;
        private int top = 100;
        public int width = 500;
        public int start;
        public float progress;
        public int[][,] rooms = new int[20][,];
        private Vector2[] centers = new Vector2[20];
        //private Mod mod = ModLoader.GetMod("ArchaeaMod");
        public void SkyFortGen(ushort tileType, ushort wallType)
        {
            int roomX = Main.rand.Next(300);
            int roomY = 0;
            float maxTiles = Main.rand.Next(200, (int)(/*Main.maxTilesX*/4800 / 1.5));
            start = (int)maxTiles;//(int)(maxTiles / Main.rand.NextFloat(1.5f, 4f));
            for (int i = 0; i < rooms.Length; i++)
            {
                progress = i;

                int randX = Main.rand.Next(20, 30);
                int randY = Main.rand.Next(12, 24);
                rooms[i] = new int[randX, randY];
                rooms[i][randX / 2, randY / 2] = 3;

                roomX += Main.rand.Next(30, 50);
                roomY = Main.rand.Next(-20, 60);

                for (int n = 0; n < rooms[i].GetLength(1); n++)
                {
                    for (int m = 0; m < rooms[i].GetLength(0); m++)
                    {
                        if (n == 0 || n == rooms[i].GetLength(1) - 1)
                            rooms[i][m, n] = 1;
                        if (m == 0 || m == rooms[i].GetLength(0) - 1)
                            rooms[i][m, n] = 1;

                        int x = roomX + start - width;
                        int y = roomY + top;
                        cotf.World.Tile tile = Main.tile[x + m, y + n];
                        centers[i] = new Vector2(x + randX / 2, y + randY / 2);
                        switch (rooms[i][m, n])
                        {
                            case 0:
                                //tile.WallType = wallType;
                                tile.type = 0;
                                tile.active(false);
                                break;
                            case 1:
                                if (Main.rand.Next(2) == 1)
                                    tile.type = (short)tileType;
                                else
                                    tile.type = TileID.RainCloud;
                                tile.active(true);
                                break;
                            case 2:
                                tile.type = TileID.Bubble;
                                tile.active(true);
                                break;
                            case 3:
                                Vector2 center = new Vector2(centers[i].X, centers[i].Y + randY / 2);
                                float radius = randX / 2;
                                float variance = Main.rand.NextFloat(1f, 2f);
                                for (float k = MathHelper.ToRadians(0f); k < MathHelper.ToRadians(180f); k += 0.017f)
                                {
                                    float cos = (float)(radius * Math.Cos(k));
                                    float sin = (float)((radius / variance) * Math.Sin(k));
                                    Vector2 point = center + new Vector2(cos, sin);
                                    for (float l = 0; l < 1f; l += 0.025f)
                                    {
                                        Vector2 placement = Vector2.Lerp(center, point, l);
                                        cotf.World.Tile cloud = Main.tile[(int)Math.Max(placement.X, 0), (int)Math.Max(placement.Y, 0)];
                                        if (Main.rand.Next(2) == 0)
                                            cloud.type = TileID.Cloud;
                                        else
                                            cloud.type = TileID.RainCloud;
                                        cloud.active(true);
                                    }
                                }
                                break;
                        }
                        int previous = i - 1;
                        if (previous >= 0)
                        {
                            Vector2 placement = default(Vector2);
                            for (float k = 0; k < 1f; k += 0.01f)
                            {
                                placement = Vector2.Lerp(centers[previous], centers[i], k);
                                for (int l = -5; l <= 5; l++)
                                {
                                    for (int q = -5; q <= 5; q++)
                                    {
                                        int placeX = (int)Math.Max(placement.X + l, 0);
                                        int placeY = (int)Math.Max(placement.Y + q, 0);
                                        cotf.World.Tile hall = Main.tile[placeX, placeY];
                                        //if (hall.WallType == 0)
                                        //{
                                            hall.type = (short)tileType;
                                            hall.active(true);
                                        //}
                                    }
                                }
                            }
                            for (float l = 0; l < 1f; l += 0.01f)
                            {
                                placement = Vector2.Lerp(centers[previous], centers[i], l);
                                for (int p = -3; p < 3; p++)
                                {
                                    for (int q = -3; q < 3; q++)
                                    {
                                        int wallX = (int)Math.Max(placement.X + p, 0);
                                        int wallY = (int)Math.Max(placement.Y + q, 0);
                                        cotf.World.Tile wall = Main.tile[wallX, wallY];
                                        //wall.WallType = wallType;
                                        wall.type = 0;
                                        wall.active(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
