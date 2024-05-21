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
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    public class Scenery : Entity
    {
        bool onScreen = false;
        public bool solid = false;
        public Scenery()
        {
        }
        private void Init()
        {
            name = "Scenery";
            TextureName = "temp";
            texture = preTexture = Assets.Asset<Bitmap>.Request(TextureName);
            foreach (Tile t in Main.tile)
            {
                if (t.hitbox.IntersectsWith(hitbox))
                { 
                    this.Dispose();
                    return;
                }
            }
            switch (type)
            {
                case SceneryType.Empty:
                    defaultColor = Color.White;
                    goto default;
                case SceneryType.Pillar:
                    defaultColor = Color.LightSteelBlue;
                    goto default;
                case SceneryType.Wood:
                    defaultColor = Color.BurlyWood;
                    goto default;
                case SceneryType.Web:
                    defaultColor = Color.GhostWhite;
                    goto default;
                case SceneryType.Dirt:
                    defaultColor = Color.Brown;
                    goto default;
                default:
                    break;
            }
        }
        private bool PreUpdate()
        {
            return onScreen =
                Center.X >= Main.myPlayer.position.X - Main.ScreenWidth / 2 &&
                Center.X <= Main.myPlayer.position.X + Main.ScreenWidth / 2 &&
                Center.Y >= Main.myPlayer.position.Y - Main.ScreenHeight / 2 &&
                Center.Y <= Main.myPlayer.position.Y + Main.ScreenHeight / 2;
        }
        public override void Update()
        {
            if (!active || !PreUpdate())
                return;
            if (!discovered)
                discovered = Discovered(Main.myPlayer);
        }
        public void Draw(Graphics graphics)
        {
            if (active && onScreen && discovered)
            {
                if (preTexture == null)
                    return;
                Drawing.LightmapHandling(preTexture, this, 1.2f, graphics);
            }
        }
        public static int NewScenery(int x, int y, int width, int height, short type)
        {
            int num = Main.scenery.Length - 1;
            
            //  Checking tile collision and out of map region
            Vector2 position = new Vector2(x + (x % (Tile.Size / 2)), y + (y % (Tile.Size / 2)));
            //Tile tile = Tile.GetSafely(position.X + width / 2, position.Y + height / 2);
            //if (tile.Active && tile.hitbox.IntersectsWith(new Rectangle((int)position.X, (int)position.Y, width, height)))
            //    return num;
            //if (position.X + Tile.Size / 2 > Main.WorldWidth || position.Y + Tile.Size / 2 > Main.WorldHeight)
            //    return num;
            
            for (int i = 0; i < Main.scenery.Length; i++)
            {
                if (Main.scenery[i] == null || !Main.scenery[i].active)
                {
                    num = i;
                    break;
                }
            }
            Main.scenery[num] = new Scenery();
            switch (type)
            { 
                case SceneryType.Empty:
                case SceneryType.Pillar:
                    Main.scenery[num].solid = true;
                    break;
                case SceneryType.Wood:
                case SceneryType.Web:
                case SceneryType.Dirt:
                default:
                    break;
            }
            Main.scenery[num].active = true;
            Main.scenery[num].owner = 255;
            Main.scenery[num].whoAmI = num;
            Main.scenery[num].position = position;
            Main.scenery[num].width = width;
            Main.scenery[num].height = height;
            Main.scenery[num].type = type;
            Main.scenery[num].Init();
            return num;
        }
        public static short[,] Decorate(int width, int height, short type)
        {
            bool hasTraps = Main.rand.NextBool(); 
            short[,] brush = new short[width, height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (hasTraps && Main.rand.NextBool(8))
                    {
                        brush[i, j] = SceneryType.Trap;
                    }
                    switch (type)
                    {
                        case RoomType.Pillars:
                            if ((i == width / 4 || i == (int)(width / 1.5)) && (j == height / 4 || j == (int)(height / 1.5)))
                                brush[i, j] = SceneryType.Pillar;
                            else brush[i, j] = SceneryType.Empty;
                            break;
                        case RoomType.Webbed:
                            if ((i == 0 || i == width - 1) && (j == 0 || j == height - 1))
                            {
                                if (Main.rand.NextBool())
                                {
                                    brush[i, j] = SceneryType.Web;
                                }
                                else brush[i, j] = SceneryType.Dirt;
                            }
                            else
                            {
                                if (Main.rand.NextBool())
                                    break;
                                if (Main.rand.NextFloat() < 0.20f)
                                    brush[i, j] = SceneryType.Web;
                                else
                                {
                                    if (Main.rand.NextFloat() < 0.80f)
                                        brush[i, j] = SceneryType.Dirt;
                                    else brush[i, j] = SceneryType.Wood;
                                }
                            }
                            break;
                        case RoomType.Heated:
                            break;
                        default:
                            break;
                    }
                }
            }
            return brush;
        }
        public override string ToString()
        {
            return $"Name:{name}, X:{hitbox.X}, Y:{hitbox.Y}, Width:{hitbox.Width}, Height:{hitbox.Height}, Type:{type}";
        }
        public override void Dispose()
        {
            if (Main.scenery[whoAmI] != null)
            {
                Main.scenery[whoAmI].active = false;
                if (Main.scenery[whoAmI].lamp != null)
                { 
                    Main.scenery[whoAmI].lamp.active = false;
                    Main.scenery[whoAmI].lamp = null;
                }
                Main.scenery[whoAmI] = null;
            }
        }
    }
}
