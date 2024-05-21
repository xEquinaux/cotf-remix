using System;
using System.Collections.Generic;
using System.Drawing;
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

namespace cotf
{
    public class Lighting
    {
        public static Lighting Instance;
        private bool visible = true;
        public bool active;
        private Color color = Color.Black;
        public Vector2 position => new Vector2(X, Y);
        private Rectangle hitbox => new Rectangle(X, Y, Size, Size);
        private int ticks = Max;
        private const int Max = 10;
        public int X, Y;
        private int Width, Height;
        public const int Range = 150;
        public const int Size = 30;
        public float alpha = 1f, oldAlpha = 1f;
        Lamp localLamp;
        float distance;
        int tileSize;
        public bool onScreen;     
        public bool lit = false;

        private static Image fow => Main.fow;
        public Vector2 Center => new Vector2(position.X + Size / 2, position.Y + Size / 2);
        public Lighting()
        {
            Instance = this;
        }
        public Lighting(bool active)
        {
            this.active = active;
        }
        public Lighting(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Init();
        }
        public void Init(int Width = 3000, int Height = 3000, int tileSize = Size)
        {
            this.Width = Width;
            this.Height = Height;
            this.tileSize = tileSize;
        }

        public bool Update()
        {
            return onScreen = 
                position.X >= Main.myPlayer.position.X - Main.ScreenWidth / 2 &&
                position.X <= Main.myPlayer.position.X + Main.ScreenWidth / 2 &&
                position.Y >= Main.myPlayer.position.Y - Main.ScreenHeight / 2 &&
                position.Y <= Main.myPlayer.position.Y + Main.ScreenHeight / 2;
        }
        public void Draw(Graphics graphics)
        {
            if (onScreen)
            {
                //if (lamp == default)
                //    graphics.FillRectangle(Brushes.Black, new Rectangle((int)position.X - Main.myPlayer.width / 4, (int)position.Y - Main.myPlayer.height / 4, Size, Size));
                //else 
                if (alpha >= 0.5f && alpha != 1f)
                {
                    graphics.FillRectangle(Convert(Opacity(alpha)), hitbox);
                    graphics.DrawImage(fow, new Rectangle((int)position.X - Size, (int)position.Y - Size, Size * 3, Size * 3));
                }                                 
                else if (alpha == 1f)
                    graphics.FillRectangle(Brushes.Black, hitbox);
                alpha = 1f;
            }
        }
        private Color Opacity(float value)
        {  
            byte min = (byte)(value * 255);
            return Color.FromArgb(min, 0, 0, 0);
        }
        private SolidBrush Convert(Color color)
        {
            return new SolidBrush(color);
        }
    }
    public class LitEffect
    {
        public int i, j;
        public bool active;
        public float alpha;
        public static LitEffect[,] Create(int width, int height, int size)
        {
            var e = new LitEffect[width / size, height / size];
            for (int i = 0; i < e.GetLength(0); i++)
            {
                for (int j = 0; j < e.GetLength(1); j++)
                {
                    e[i, j] = new LitEffect();
                    e[i, j].i = i;
                    e[i, j].j = j;
                }
            }
            return e;
        }
    }
}
