using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Assets;
using cotf.Base;
using cotf.ID;
using cotf.World;
using cotf.World.Traps;
using cotf.Collections;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace cotf.Base
{
    public class CombatText : IDisposable
    {
        private CombatText(Vector2 position, int amount, Entity parent)
        {
            this.amount = amount;
            this.parent = parent;
            this.flipped = Main.rand.NextBool();
            this.position = position;
            this.ticks = 0;
            if (!flipped)
                 angle =  90f;
            else angle = -90f;
        }
        public bool active = true;
        private string Text => amount.ToString();
        private int amount;
        private int ticks;
        private float angle;
        private bool flipped;
        private Entity parent;
        private Vector2 position;
        private Color color()
        {
            if (amount < 0)
                return Color.Green;
            if (amount > 0)
                return Color.Red;
            return Color.Transparent;
        }
        public static IList<CombatText> text = new List<CombatText>();
        public static CombatText NewText(int amount, Entity parent)
        {
            CombatText text = new CombatText(parent.position, amount, parent);
            CombatText.text.Add(text);
            return text;
        }
        public void Update()
        {
            int offX = parent.width / 3;
            if (!active) return;
            if (!flipped) angle += Helper.Radian;
            else          angle -= Helper.Radian;
            double s = parent.hitbox.Left + offX + 5f * Math.Sin(angle);
            double c = parent.hitbox.Top + 30f * Math.Cos(angle); 
            position = new Vector2((float)s, (float)c);
            if (ticks++ > 90)
            {
                Dispose();
            }
        }
        public void Draw(Graphics graphics)
        {
            if (!active) return;
            graphics.DrawString(Text, Main.DefaultFont, new SolidBrush(color()), new PointF(position.X + Main.ScreenX, position.Y + Main.ScreenY));
        }

        public static void Dispose(CombatText value)
        {
            value.active = false;
            CombatText.text.Remove(value);
            value = default;
        }

        public void Dispose()
        {
            active = false;
            text.Remove(this);
        }

        public override string ToString()
        {
            return $"amount:{amount}, parent:{parent.Name}";
        }
    }
}
