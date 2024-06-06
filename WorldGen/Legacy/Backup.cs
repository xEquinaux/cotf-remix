//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Terraria;
//using Terraria.GameContent.Generation;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria.ModLoader.IO;
//using Terraria.WorldBuilding;

//using ArchaeaMod.GenLegacy;
//using ArchaeaMod.Items;
//using ArchaeaMod.Items.Alternate;
//using ArchaeaMod.Merged;
//using ArchaeaMod.Merged.Items;
//using ArchaeaMod.Merged.Tiles;
//using ArchaeaMod.Merged.Walls;
//using Terraria.Utilities;

//namespace ArchaeaMod.Gen
//{
//    public class MagnoV2
//    {
//        public static UnifiedRandom rand;
//        public static MagnoV2 Instance;
//        public MagnoV2(int originX, int originY)
//        {
//            Instance = this;
//            rand = Main.rand;
//            this.originX = originX;
//            this.originY = originY;
//            Init();
//        }

//        public static int rightWorld
//        {
//            get { return ScreenWidth; }
//        }
//        public static int bottomWorld
//        {
//            get { return ScreenHeight; }
//        }
//        public static int maxTilesX
//        {
//            get { return ScreenWidth; }
//        }
//        public static int maxTilesY
//        {
//            get { return ScreenHeight; }
//        }
//        public static int ScreenWidth;
//        public static int ScreenHeight;
//        private float ScreenX, ScreenY;
//        public int originX, originY;

//        protected virtual void Init()
//        {
//            ScreenWidth = 800;
//            ScreenHeight = 450;
//        }
//        public void tGenerate(GenerationProgress progress)
//        {
//            int width = ScreenWidth;
//            int height = ScreenHeight;
//            int randLeft = originX;
//            int randTop = originY;
//            Rectangle bound = new Rectangle(randLeft, randTop, width, height);
//            int left = bound.X;
//            int top = bound.Y;
//            List<Vector2> miner = new List<Vector2>();
//            List<Vector2> cavern = new List<Vector2>();
//            Vector2 start = new Vector2(left, top/* + height * 0.75f*/);
//            float radius = 8f; //20f
//            float radiusReset = 8f;
//            float tunnel = 10f;
//            float move = radius * 0.30f;
//            int ticks = 0, ticks2 = 0;
//            int max = 20, max2 = 25;
//            bool flag = false, flag2 = false;
//            bool level = true;
//            bool ash = false;
//            while (start.X < left + width / 2)
//            {
//                if (Main.rand.NextDouble() >= 0.95f)
//                {
//                    radius = 20f; //40f
//                    cavern.Add(start);
//                }
//                else
//                {
//                    radius = radiusReset;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.99f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= 0.95f)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    start.Y += flag ? -1 * move : move;
//                }
//                miner.Add(start);
//                start.X += move;
//            }
//            progress.Value = 0.2f;
//            start = new Vector2(left, top + height / 4);
//            while (start.X < left + width / 2)
//            {
//                if (Main.rand.NextDouble() >= 0.95f)
//                {
//                    radius = 20f; //40f
//                    cavern.Add(start);
//                }
//                else
//                {
//                    radius = radiusReset;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.99f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= 0.95f)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    start.Y += flag ? -1 * move : move;
//                }
//                miner.Add(start);
//                start.X += move;
//            }
//            progress.Value = 0.4f;
//            flag = false;
//            ticks = 0;
//            bool back = true, forward = false;
//            Vector2 branch = start;
//            branch.X = left + bound.Width / 4f;
//            branch.Y = top + radius;
//            while (branch.Y < height - radius * 2f)
//            {
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = branch.X + n * (float)Math.Cos(r);
//                            float sin = branch.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.995f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= 0.95f)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    branch.X += flag ? -1 * move : move;
//                }
//                miner.Add(branch);
//                branch.Y += move;
//            }
//            progress.Value = 0.6f;
//            branch.X = left + bound.Width * 0.75f;
//            branch.Y = top + radius;
//            while (branch.Y < height - radius * 2f)
//            {
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = branch.X + n * (float)Math.Cos(r);
//                            float sin = branch.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.995f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= 0.95f)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    branch.X += flag ? -1 * move : move;
//                }
//                miner.Add(branch);
//                branch.Y += move;
//            }
//            progress.Value = 0.8f;
//            while (true)
//            {
//                if (!flag && Main.rand.NextDouble() >= 0.50f)
//                {
//                    flag = true;
//                    flag2 = true;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (ticks++ < max)
//                {
//                    if (start.Y >= height - radius)
//                    {
//                        start.Y -= move;
//                        start.X -= move;
//                    }
//                    else if (start.Y <= 0f + radius)
//                    {
//                        start.Y += move;
//                        start.X -= move;
//                    }
//                    else
//                    {
//                        if (flag2)
//                        {
//                            if (back)
//                            {
//                                start.Y -= move;
//                                start.X -= move;
//                            }
//                            else if (!forward)
//                            {
//                                start.X -= move;
//                            }
//                            else
//                            {
//                                start.X += move;
//                            }
//                        }
//                        else
//                        {
//                            if (back)
//                            {
//                                start.Y += move;
//                                start.X -= move;
//                            }
//                            else if (!forward)
//                            {
//                                start.X -= move;
//                            }
//                            else
//                            {
//                                start.X += move;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    if (forward)
//                    {
//                        break;
//                    }
//                    if (!back)
//                    {
//                        forward = true;
//                        max = 50;
//                    }
//                    back = false;
//                    ticks = 0;
//                }
//                miner.Add(start);
//            }
//            max = 20;
//            ticks = 0;
//            while (start.X < left + width - radius)
//            {
//                if (Main.rand.NextDouble() >= 0.95f)
//                {
//                    radius = 20f; //40f
//                    cavern.Add(start);
//                }
//                else
//                {
//                    radius = radiusReset;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos, (int)sin].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.995f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= 0.95f)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    start.Y += flag ? -1 * move : move;
//                }
//                miner.Add(start);
//                start.X += move;
//            }
//            progress.Value = 0.9f;
//            radius = radiusReset;
//            foreach (var loc in miner)
//            {
//                tunnel = (float)Main.rand.Next(8, 15);
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n < tunnel)
//                        {
//                            float cos = loc.X + n * (float)Math.Cos(r);
//                            float sin = loc.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].active(false);
//                                if (rand.NextBool()) Main.tile[(int)cos, (int)sin].wall = ArchaeaWorld.magnoCaveWall;
//                            }
//                        }
//                    }
//                }
//            }
//            radius = 20f; //40f
//            progress.Value = 0.95f;
//            foreach (var loc in cavern)
//            {
//                tunnel = (float)Main.rand.Next(20, 35);
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n < tunnel)
//                        {
//                            float cos = loc.X + n * (float)Math.Cos(r);
//                            float sin = loc.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos, (int)sin].active(false);
//                                if (rand.NextBool()) Main.tile[(int)cos, (int)sin].wall = ArchaeaWorld.magnoCaveWall;
//                            }
//                        }
//                    }
//                }
//            }
//            progress.Value = 1f;
//        }
//    }
//}

//VERSION latest backup

//namespace ArchaeaMod.Gen
//{
//    public class MagnoV2
//    {
//        public static UnifiedRandom rand;
//        public static MagnoV2 Instance;
//        public MagnoV2(int originX, int originY)
//        {
//            Instance = this;
//            rand = Main.rand;
//            this.originX = originX;
//            this.originY = originY;
//            Init();
//        }

//        public static int rightWorld
//        {
//            get { return ScreenWidth; }
//        }
//        public static int bottomWorld
//        {
//            get { return ScreenHeight; }
//        }
//        public static int maxTilesX
//        {
//            get { return ScreenWidth; }
//        }
//        public static int maxTilesY
//        {
//            get { return ScreenHeight; }
//        }
//        public static int ScreenWidth;
//        public static int ScreenHeight;
//        private float ScreenX, ScreenY;
//        public int originX, originY;

//        protected virtual void Init()
//        {
//            ScreenWidth = 800;
//            ScreenHeight = 450;
//        }
//        public void tGenerate(GenerationProgress progress)
//        {
//            int width = ScreenWidth;
//            int height = ScreenHeight;
//            int randLeft = 0;
//            int randTop = 0;
//            Rectangle bound = new Rectangle(randLeft, randTop, width, height);
//            int left = bound.X;
//            int top = bound.Y;
//            List<Vector2> miner = new List<Vector2>();
//            List<Vector2> cavern = new List<Vector2>();
//            Vector2 start = new Vector2(left, top + height * 0.75f);
//            float radius = 20f;
//            float tunnel = 10f;
//            float move = radius * 0.30f;
//            int ticks = 0, ticks2 = 0;
//            int max = 20, max2 = 25;
//            bool flag = false, flag2 = false;
//            bool level = true;
//            bool ash = false;
//            float ashChance = 0.50f;
//            float radiusLimit = 1f;
//            int radiusDownsizer = 0;
//            while (start.X < left + width / 2)
//            {
//                if (Main.rand.NextDouble() >= 0.95f)
//                {
//                    radius = 40f - radiusDownsizer / radiusLimit;
//                    cavern.Add(start);
//                }
//                else
//                {
//                    radius = 20f - radiusDownsizer / radiusLimit;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.99f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= ashChance)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    start.Y += flag ? -1 * move : move;
//                }
//                miner.Add(start);
//                start.X += move;
//            }
//            progress.Value = 0.2f;
//            start = new Vector2(left, top + height / 4);
//            while (start.X < left + width / 2)
//            {
//                if (Main.rand.NextDouble() >= 0.95f)
//                {
//                    radius = 40f - radiusDownsizer / radiusLimit;
//                    cavern.Add(start);
//                }
//                else
//                {
//                    radius = 20f - radiusDownsizer / radiusLimit;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.99f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= ashChance)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    start.Y += flag ? -1 * move : move;
//                }
//                miner.Add(start);
//                start.X += move;
//            }
//            progress.Value = 0.4f;
//            flag = false;
//            ticks = 0;
//            bool back = true, forward = false;
//            Vector2 branch = start;
//            branch.X = left + bound.Width / 4f;
//            branch.Y = top + radius;
//            while (branch.Y < height - radius * 2f)
//            {
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = branch.X + n * (float)Math.Cos(r);
//                            float sin = branch.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.995f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= ashChance)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    branch.X += flag ? -1 * move : move;
//                }
//                miner.Add(branch);
//                branch.Y += move;
//            }
//            progress.Value = 0.6f;
//            branch.X = left + bound.Width * 0.75f;
//            branch.Y = top + radius;
//            while (branch.Y < height - radius * 2f)
//            {
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = branch.X + n * (float)Math.Cos(r);
//                            float sin = branch.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.995f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= ashChance)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    branch.X += flag ? -1 * move : move;
//                }
//                miner.Add(branch);
//                branch.Y += move;
//            }
//            progress.Value = 0.8f;
//            while (true)
//            {
//                if (!flag && Main.rand.NextDouble() >= 0.50f)
//                {
//                    flag = true;
//                    flag2 = true;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (ticks++ < max)
//                {
//                    if (start.Y >= height - radius)
//                    {
//                        start.Y -= move;
//                        start.X -= move;
//                    }
//                    else if (start.Y <= 0f + radius)
//                    {
//                        start.Y += move;
//                        start.X -= move;
//                    }
//                    else
//                    {
//                        if (flag2)
//                        {
//                            if (back)
//                            {
//                                start.Y -= move;
//                                start.X -= move;
//                            }
//                            else if (!forward)
//                            {
//                                start.X -= move;
//                            }
//                            else
//                            {
//                                start.X += move;
//                            }
//                        }
//                        else
//                        {
//                            if (back)
//                            {
//                                start.Y += move;
//                                start.X -= move;
//                            }
//                            else if (!forward)
//                            {
//                                start.X -= move;
//                            }
//                            else
//                            {
//                                start.X += move;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    if (forward)
//                    {
//                        break;
//                    }
//                    if (!back)
//                    {
//                        forward = true;
//                        max = 50;
//                    }
//                    back = false;
//                    ticks = 0;
//                }
//                miner.Add(start);
//            }
//            max = 20;
//            ticks = 0;
//            while (start.X < left + width - radius)
//            {
//                if (Main.rand.NextDouble() >= 0.95f)
//                {
//                    radius = 40f - radiusDownsizer / radiusLimit;
//                    cavern.Add(start);
//                }
//                else
//                {
//                    radius = 20f - radiusDownsizer / radiusLimit;
//                }
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n >= tunnel)
//                        {
//                            float cos = start.X + n * (float)Math.Cos(r);
//                            float sin = start.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.magnoStone;
//                                if (ash && flag && r <= Math.PI && r >= 0f)
//                                {
//                                    Main.tile[(int)cos + originX, (int)sin + originY].type = ArchaeaWorld.Ash;
//                                }
//                            }
//                        }
//                    }
//                }
//                if (!flag && Main.rand.NextDouble() >= 0.995f)
//                {
//                    flag = true;
//                }
//                if (!level && Main.rand.NextDouble() >= 0.90f)
//                {
//                    level = true;
//                }
//                if (!ash && Main.rand.NextDouble() >= ashChance)
//                {
//                    ash = true;
//                }
//                if (flag)
//                {
//                    if (ticks++ > max)
//                    {
//                        flag = false;
//                        ticks = 0;
//                    }
//                }
//                if (level)
//                {
//                    if (ticks++ > max)
//                    {
//                        level = false;
//                        ticks = 0;
//                    }
//                }
//                if (ash)
//                {
//                    if (ticks2++ > max2)
//                    {
//                        ash = false;
//                        ticks2 = 0;
//                    }
//                }
//                if (!level && start.Y < height - radius && start.Y > 0f + radius)
//                {
//                    start.Y += flag ? -1 * move : move;
//                }
//                miner.Add(start);
//                start.X += move;
//            }
//            progress.Value = 0.9f;
//            radius = 20f - radiusDownsizer / radiusLimit;
//            foreach (var loc in miner)
//            {
//                tunnel = (float)Main.rand.Next(8, 15);
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n < tunnel)
//                        {
//                            float cos = loc.X + n * (float)Math.Cos(r);
//                            float sin = loc.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].active(false);
//                                if (rand.NextFloat() < 0.001f) Main.tile[(int)cos + originX, (int)sin + originY].wall = ArchaeaWorld.magnoCaveWall;
//                            }
//                        }
//                    }
//                }
//            }
//            radius = 40 - radiusDownsizer / radiusLimit;
//            progress.Value = 0.95f;
//            foreach (var loc in cavern)
//            {
//                tunnel = (float)Main.rand.Next(20, 35);
//                for (float r = 0; r < Math.PI * 2f; r += Draw.radian)
//                {
//                    for (float n = 0; n < radius; n += Draw.radians(radius))
//                    {
//                        if (n < tunnel)
//                        {
//                            float cos = loc.X + n * (float)Math.Cos(r);
//                            float sin = loc.Y + n * (float)Math.Sin(r);
//                            if (cos > 0 && cos < Main.maxTilesX && sin > 0 && sin < Main.maxTilesY)
//                            {
//                                Main.tile[(int)cos + originX, (int)sin + originY].active(false);
//                                if (rand.NextFloat() < 0.001f) Main.tile[(int)cos + originX, (int)sin + originY].wall = ArchaeaWorld.magnoCaveWall;
//                            }
//                        }
//                    }
//                }
//            }
//            progress.Value = 1f;
//        }
//    }
//}
