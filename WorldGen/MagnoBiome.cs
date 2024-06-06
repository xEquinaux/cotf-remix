using cotf;
using cotf.World;
using cotf.Base;
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace ArchaeaMod.Biome
{
    public class MagnoBiome
    {
        public static void Generate()
        {
            //int worldLeft = Main.rand.Next(Main.maxTilesX / 4, (int)(Main.maxTilesX * 0.75f));
            //int worldTop = 400;
            //int width = 600;
            //int height = 800;
            //List<Vector2> miner = new List<Vector2>();
            //List<Vector2> cavern = new List<Vector2>();
            //Vector2 start = new Vector2(worldLeft, worldTop + height * 0.75f);
            //float radius = 20f;
            //float tunnel = 10f;
            //float move = radius * 0.30f;
            //int ticks = 0, ticks2 = 0;
            //int max = 20, max2 = 25;
            //bool flag = false, flag2 = false;
            //bool level = true;
            //bool ash = false;
            int width = 800;
            int height = 450;
            int randLeft = 400;
            int randTop = 600;
            Rectangle bound = new Rectangle(randLeft, randTop, width, height);
            int left = bound.X;
            int top = bound.Y;
            int worldLeft = left;
            int worldTop = top;
            List<Vector2> miner = new List<Vector2>();
            List<Vector2> cavern = new List<Vector2>();
            Vector2 start = new Vector2(left, top + height * 0.75f);
            float radius = 20f;
            float tunnel = 10f;
            float move = radius * 0.30f;
            int ticks = 0, ticks2 = 0;
            int max = 20, max2 = 25;
            bool flag = false, flag2 = false;
            bool level = true;
            bool ash = false;
            while (start.X < width / 2)
            {
                if (Main.rand.NextDouble() >= 0.95f)
                {
                    radius = 40f;
                    cavern.Add(start);
                }
                else
                {
                    radius = 20f;
                }
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);                   //TODO
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0; //ArchaeaWorld.magnoStone;
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                //Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = ArchaeaWorld.Ash;
                            }
                        }
                    }
                }
                if (!flag && Main.rand.NextDouble() >= 0.99f)
                {
                    flag = true;
                }
                if (!level && Main.rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && Main.rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    start.Y += flag ? -1 * move : move;
                }
                miner.Add(start);
                start.X += move;
            }
            start = new Vector2(worldLeft, worldTop + height / 4);
            while (start.X < width / 2)
            {
                if (Main.rand.NextDouble() >= 0.95f)
                {
                    radius = 40f;
                    cavern.Add(start);
                }
                else
                {
                    radius = 20f;
                }
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);                  //TODOs
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;//ArchaeaWorld.magnoStone;
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                //Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = ArchaeaWorld.Ash;
                            }
                        }
                    }
                }
                if (!flag && Main.rand.NextDouble() >= 0.99f)
                {
                    flag = true;
                }
                if (!level && Main.rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && Main.rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    start.Y += flag ? -1 * move : move;
                }
                miner.Add(start);
                start.X += move;
            }
            flag = false;
            ticks = 0;
            bool back = true, forward = false;
            Vector2 branch = start;
            branch.X /= 2f;
            branch.Y = worldTop + height - radius;
            while (branch.Y > radius * 2f)
            {
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n >= tunnel)
                        {
                            float cos = branch.X + n * (float)Math.Cos(r);
                            float sin = branch.Y + n * (float)Math.Sin(r);                 //TODO
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;//ArchaeaWorld.magnoStone;
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                //Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = ArchaeaWorld.Ash;
                            }
                        }
                    }
                }
                if (!flag && Main.rand.NextDouble() >= 0.995f)
                {
                    flag = true;
                }
                if (!level && Main.rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && Main.rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    branch.X += flag ? -1 * move : move;
                }
                miner.Add(branch);
                branch.Y -= move;
            }
            branch.X *= 2f;
            branch.Y = worldTop + height - radius;
            while (branch.Y > radius * 2f)
            {
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n >= tunnel)
                        {
                            float cos = branch.X + n * (float)Math.Cos(r);
                            float sin = branch.Y + n * (float)Math.Sin(r);                //TODO
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;//ArchaeaWorld.magnoStone;
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                //Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = ArchaeaWorld.Ash;
                            }
                        }
                    }
                }
                if (!flag && Main.rand.NextDouble() >= 0.995f)
                {
                    flag = true;
                }
                if (!level && Main.rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && Main.rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    branch.X += flag ? -1 * move : move;
                }
                miner.Add(branch);
                branch.Y -= move;
            }
            while (true)
            {
                if (!flag && Main.rand.NextDouble() >= 0.50f)
                {
                    flag = true;
                    flag2 = true;
                }
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);                 //TODO
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;//ArchaeaWorld.magnoStone;
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                //Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = ArchaeaWorld.Ash;
                            }
                        }
                    }
                }
                if (ticks++ < max)
                {
                    if (start.Y >= height - radius)
                    {
                        start.Y -= move;
                        start.X -= move;
                    }
                    else if (start.Y <= 0f + radius)
                    {
                        start.Y += move;
                        start.X -= move;
                    }
                    else
                    {
                        if (flag2)
                        {
                            if (back)
                            {
                                start.Y -= move;
                                start.X -= move;
                            }
                            else if (!forward)
                            {
                                start.X -= move;
                            }
                            else
                            {
                                start.X += move;
                            }
                        }
                        else
                        {
                            if (back)
                            {
                                start.Y += move;
                                start.X -= move;
                            }
                            else if (!forward)
                            {
                                start.X -= move;
                            }
                            else
                            {
                                start.X += move;
                            }
                        }
                    }
                }
                else
                {
                    if (forward)
                    {
                        break;
                    }
                    if (!back)
                    {
                        forward = true;
                        max = 50;
                    }
                    back = false;
                    ticks = 0;
                }
                miner.Add(start);
            }
            max = 20;
            ticks = 0;
            while (start.X < width - radius)
            {
                if (Main.rand.NextDouble() >= 0.95f)
                {
                    radius = 40f;
                    cavern.Add(start);
                }
                else
                {
                    radius = 20f;
                }
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n >= tunnel)
                        {
                            float cos = start.X + n * (float)Math.Cos(r);
                            float sin = start.Y + n * (float)Math.Sin(r);                 //TODO
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;//ArchaeaWorld.magnoStone;
                            if (ash && flag && r <= Math.PI && r >= 0f)
                            {
                                //Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = ArchaeaWorld.Ash;
                            }
                        }
                    }
                }
                if (!flag && Main.rand.NextDouble() >= 0.995f)
                {
                    flag = true;
                }
                if (!level && Main.rand.NextDouble() >= 0.90f)
                {
                    level = true;
                }
                if (!ash && Main.rand.NextDouble() >= 0.95f)
                {
                    ash = true;
                }
                if (flag)
                {
                    if (ticks++ > max)
                    {
                        flag = false;
                        ticks = 0;
                    }
                }
                if (level)
                {
                    if (ticks++ > max)
                    {
                        level = false;
                        ticks = 0;
                    }
                }
                if (ash)
                {
                    if (ticks2++ > max2)
                    {
                        ash = false;
                        ticks2 = 0;
                    }
                }
                if (!level && start.Y < height - radius && start.Y > 0f + radius)
                {
                    start.Y += flag ? -1 * move : move;
                }
                miner.Add(start);
                start.X += move;
            }
            radius = 20f;
            foreach (var loc in miner)
            {
                tunnel = (float)Main.rand.Next(8, 15);
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n < tunnel)
                        {
                            float cos = loc.X + n * (float)Math.Cos(r);
                            float sin = loc.Y + n * (float)Math.Sin(r);
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;
                            cotf.World.Tile tile = Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)];
                            tile.active(false);
                        }
                    }
                }
            }
            radius = 40f;
            foreach (var loc in cavern)
            {
                tunnel = (float)Main.rand.Next(20, 35);
                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
                {
                    for (float n = 0; n < radius; n += Draw.radians(radius) * 5f)
                    {
                        if (n < tunnel)
                        {
                            float cos = loc.X + n * (float)Math.Cos(r);
                            float sin = loc.Y + n * (float)Math.Sin(r);
                            Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)].type = 0;
                            cotf.World.Tile tile = Main.tile[Math.Max((int)cos, 1), Math.Max((int)sin, 1)];
                            tile.active(false); 
                        }
                    }
                }
            }
        }
    }
}