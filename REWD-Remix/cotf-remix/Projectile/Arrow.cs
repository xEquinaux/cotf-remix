using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Assets;
using cotf.Base;
using cotf.World;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf
{
    public class Proj_Arrow : Projectile
    {
        public override Rectangle hitbox => new Rectangle((int)position.X, (int)position.Y, width, height);
        public override void SetDefaults()
        {
            damage = 10;
            knockBack = 2f;
            friendly = true;
            width = 24;
            height = 8;
            timeLeft = 600;
            defaultColor = Color.Brown;
            base.SetDefaults();
        }
        protected override void Init()
        {
        }
        public override void AI()
        {
            position += velocity;
            if (timeLeft-- < 0)
                Dispose();
            base.AI();
        }
       public override bool HitPlayer(Player player)
        {
            if (player.ProjHit(this))
            {
                player.Hurt(damage, knockBack, AngleTo(player.Center));
                Dispose();
                return true;
            }
            return false;
        }
        public override void Collide()
        {
            if (Tile.GetSafely((int)position.X / Tile.Size, (int)position.Y / Tile.Size).Active)
            {
                color = Color.Transparent;
                velocity = Vector2.Zero;
                Dispose();
            }
        }
        public override void Draw(Graphics graphics)
        {
            Drawing.DrawRotate(texture, hitbox, new Rectangle(0, 0, hitbox.Width, hitbox.Height), angle, new PointF(width / 2, height / 2), color, Color.Black, RotateType.MatrixTransform, graphics);
        }
    }
}
