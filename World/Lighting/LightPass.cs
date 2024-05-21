using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Base;
using cotf.World;
using System.Threading;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf
{
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
            return;
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
    public class Surface
    {
        public Color value;
        public Bitmap bitmap;
        public Vector2 topLeft;
        public int 
            width, 
            height;
        public float range;
        public const int Range = 50;
        public Surface()
        {
        }
        public Surface(Bitmap bitmap, Vector2 topLeft, int width, int height)
        {
            this.bitmap = bitmap;
            this.topLeft = topLeft;
            this.width = width;
            this.height = height;
        }
        public static Surface[] GetSurface(Rectangle hitbox, Background[,] bg)
        {
            List<Surface> list = new List<Surface>();
            for (int i = 0; i < bg.GetLength(0); i++)
            {
                for (int j = 0; j < bg.GetLength(1); j++)
                {
                    if (bg[i, j] == null || !bg[i, j].active)
                        continue;
                    if (bg[i, j].hitbox.IntersectsWith(new Rectangle(hitbox.X - Range, hitbox.Y - Range, Range * 2, Range * 2)))
                    {
                        list.Add(new Surface() 
                        { 
                            width = bg[i, j].width,
                            height = bg[i, j].height,
                            bitmap = bg[i, j].preTexture,
                            topLeft = bg[i, j].position,
                            range = Range
                        });
                    }
                }
            }
            if (list.Count > 0)
                return list.ToArray();
            else return new Surface[] { };
        }
    }
}
