using cotf.Base;
using cotf.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cotf.IO
{
    internal class Container
    {
        public bool active;
        public float range;
        public float distance;
        public Bitmap texture;
        public Color[] color;
    }
    //internal class Worker
    //{
    //    public Bitmap[,] array;
        
    //    public static Bitmap IngestLightPass(Container o)
    //    {
    //        for (int n = 0; n < o.color.Length; n++)
    //        {
    //            if (o == null || !o.active)
    //                continue;
    //            Color c = o.color[n];

    //            List<Tile> brush = LightPass.NearbyTile(lamp);
    //            if (Helper.Distance(array[i, j].Center, lamp.Center) <= lamp.range)
    //            {
    //                array[i, j].preTexture = Drawing.Lightpass0(brush, array[i, j].preTexture, array[i, j].position, lamp, lamp.range);
    //            }
    //        }
    //        return array;
    //    }
    //}
    public sealed class LightPass
    {
        public static List<Tile> NearbyTile(Lamp lamp)
        {
            List<Tile> brush = new List<Tile>();
            for (int i = 0; i < Main.tile.GetLength(0); i++)
            {
                for (int j = 0; j < Main.tile.GetLength(1); j++)
                {
                    if (Main.tile[i, j] != null && Main.tile[i, j].Active && Main.tile[i, j].solid)
                    {
                        if (Helper.Distance(Main.tile[i, j].Center, lamp.position) < lamp.range)
                        {
                            brush.Add(Main.tile[i, j]);
                        }
                    }
                }
            }
            return brush;
        }
        public static List<Background> NearbyFloor(Lamp lamp)
        {
            List<Background> brush = new List<Background>();
            for (int i = 0; i < Main.background.GetLength(0); i++)
            {
                for (int j = 0; j < Main.background.GetLength(1); j++)
                {
                    if (Main.background[i, j] != null && Main.background[i, j].active)
                    {
                        if (Helper.Distance(Main.background[i, j].Center, lamp.position) < lamp.range)
                        {
                            brush.Add(Main.background[i, j]);
                        }
                    }
                }
            }
            return brush;
        }
        public static void PreProcessing()
        {
            //  DEBUG: comment out for lighting
            for (int n = 0; n < Main.lamp.Length; n++)
            {
                Lamp lamp = Main.lamp[n];
                if (lamp == null || !lamp.active || lamp.owner != 255 || lamp.parent == null)
                    continue;

                List<Tile> brush = NearbyTile(lamp);

                for (int i = 0; i < Main.background.GetLength(0); i++)
                {
                    for (int j = 0; j < Main.background.GetLength(1); j++)
                    {
                        if (Main.background[i, j] == null || !Main.background[i, j].active)
                            continue;
                        if (Helper.Distance(Main.background[i, j].Center, lamp.Center) <= lamp.range)
                        {
                            Main.background[i, j].preTexture = Drawing.Lightpass0(brush, Main.background[i, j].preTexture, Main.background[i, j].position, lamp, lamp.range);
                        }
                    }
                    //if (i % Main.background.GetLength(0) / 5 == 0)
                    //    GC.Collect();
                }
                /*
                for (int i = 0; i < Main.tile.GetLength(0); i++)
                {
                    for (int j = 0; j < Main.tile.GetLength(1); j++)
                    {
                        if (!Main.tile[i, j].Active)
                            continue;
                        Main.tile[i, j].preTexture = Drawing.Lightpass0(brush, Main.tile[i, j].preTexture, Main.tile[i, j].position, lamp, lamp.range / 2);
                    }
                    if (i % Main.tile.GetLength(0) / 5 == 0)
                        GC.Collect();
                }                       */
            }
            return;
            #region for lighting objects based on average texture pixel value
            for (int k = 0; k < Main.lightmap.GetLength(0); k++)
            {
                for (int l = 0; l < Main.lightmap.GetLength(1); l++)
                {
                    Background bgr = Main.background[Math.Max(k / 2 - 1, 0), Math.Max(l / 2 - 1, 0)];
                    if (bgr != null && bgr.active)
                    {
                        Main.lightmap[k, l] = new Lightmap(k, l);
                        Main.lightmap[k, l].active = true;
                        Main.lightmap[k, l].color = Drawing.LightAverage(bgr.preTexture);
                        Main.lightmap[k, l].parent = bgr;
                    }
                }
            }
            #endregion
            #region one light bounce onto the tile objects
            return;
            Main.tile[0, 0].active(false);
            Background bg = new Background(0, 0, 50);
            bg.color = Color.DarkBlue;
            bg.defaultColor = bg.color;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (!Main.tile[i, j].Active)
                        continue;
                    //WaitCallback callback = delegate(object sender)
                    //{ 
                    Tile.GetSafely(i, j).preTexture = Drawing.Lightpass1((i + j) / (float)Main.tile.LongLength, new Surface(Tile.GetSafely(i, j).preTexture, Tile.GetSafely(i, j).position, Tile.GetSafely(i, j).width, Tile.GetSafely(i, j).height), Surface.GetSurface(Tile.GetSafely(i, j).hitbox, Main.background), Tile.GetSafely(i, j));
                    //};
                    //ThreadPool.QueueUserWorkItem(callback);
                }
                if (i % Main.tile.GetLength(0) / 5 == 0)
                    GC.Collect();
            }
            #endregion
        }
    }
}
