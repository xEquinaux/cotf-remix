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
using cotf.Collections;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using Microsoft.Xna.Framework;
using Color = System.Drawing.Color;
using Rectangle = System.Drawing.Rectangle;

namespace cotf.World
{
    public class WorldObject : Entity, IDisposable
    {
        public bool physics;
        public new int X => (int)position.X;
        public new int Y => (int)position.Y;
        private void Init()
        {
            defaultColor = Color.Black;
            texture = preTexture = (Bitmap)Main.texture90;
        }
        public override void Update()
        {
            if (!active)
                return;
            position += velocity;
            collide = false;
            colUp = false;
            colRight = false;
            colDown = false;
            colLeft = false;
        }
        public void Draw(Graphics graphics)
        {
            if (!active)
                return;
            Drawing.TextureLighting(preTexture, hitbox, this, graphics);
        }
        public bool PhysicsMove(Player player)
        {
            List<CollisionType> list = new List<CollisionType>();
            foreach (Npc n in Main.npc)
            {
                if (n != null && n.active && n.InProximity(this, 150f))
                {
                    if (Collision(n, 2))
                    { 
                        if (velocity == Vector2.Zero)
                            continue;
                        n.velocity = Main.myPlayer.velocity;
                        foreach (Npc o in n.collideObj)
                            o.velocity = n.velocity;
                    }
                    foreach (var c in EntityHelper.Collision(this, n))
                        list.Add(c);
                    if (list.Contains(CollisionType.Unbuffered))
                        velocity = n.velocity;
                }
            }
            foreach (Tile t in Main.tile)
            {
                if (t != null && t.Active && t.InProximity(this, 150f))
                {
                    foreach (var c in EntityHelper.Collision(this, t))
                        list.Add(c);
                }
            }
            if (Collision(player) && player.KeyDown(Keys.E) && player.IsControlMove())
            { 
                velocity = Main.myPlayer.velocity;
                if (velocity.Y < 0f && list.Contains(CollisionType.Top) && player.GetControlType() == ControlType.MoveUp || velocity.X > 0f && list.Contains(CollisionType.Right) && player.GetControlType() == ControlType.MoveRight || velocity.Y > 0f && list.Contains(CollisionType.Bottom) && player.GetControlType() == ControlType.MoveDown || velocity.X < 0f && list.Contains(CollisionType.Left) && player.GetControlType() == ControlType.MoveLeft)
                {
                    velocity = Vector2.Zero;
                }
            }
            else velocity = Vector2.Zero;
            return velocity != Vector2.Zero;
        }
        public static int NewObject(float x, float y, int width, int height, bool physics = false)
        {
            int num = Main.wldObject.Length - 1;
            for (int i = 0; i < Main.wldObject.Length; i++)
            {
                if (Main.wldObject[i] == null || !Main.wldObject[i].active)
                {
                    num = i;
                    break;
                }
            }
            Main.wldObject[num] = new WorldObject();
            Main.wldObject[num].active = true;
            Main.wldObject[num].whoAmI = num;
            Main.wldObject[num].position = new Vector2(x, y);
            Main.wldObject[num].width = width;
            Main.wldObject[num].height = height;
            Main.wldObject[num].physics = physics;
            Main.wldObject[num].Init();
            return num;
        }
        public new bool Collision(Entity e, int buffer = 2)
        {
            if (!active)
                return false;

            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X, (int)e.position.Y, e.width, e.height)))
            { 
                e.collide = true;
                collide = true;
            }
            //  Directions
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X, (int)e.position.Y - buffer, e.width, 2)))
                e.colUp = true;
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X, (int)e.position.Y + e.height + buffer, e.width, 2)))
                e.colDown = true;
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X + e.width + buffer, (int)e.position.Y, 2, e.height)))
                e.colRight = true;
            if (hitbox.IntersectsWith(new Rectangle((int)e.position.X - buffer, (int)e.position.Y, 2, e.height)))
                e.colLeft = true;

            return e.colUp|| e.colDown || e.colRight || e.colLeft;
        }
        public override void Dispose()
        {
            if (Main.wldObject[whoAmI] != null)
            {
                Main.wldObject[whoAmI].active = false;
                Main.wldObject[whoAmI].physics = false;
                Main.wldObject[whoAmI] = null;
            }
        }
    }
}
