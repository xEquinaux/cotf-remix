using cotf.Base;
using SharpDX.Direct2D1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace cotf
{
    public class Wurm_Head : Npc
    {
        public const byte
            Normal = 0;
        public Wurm_Body[] body;
        public Wurm_Tail tail;
        const int maxIFrames = 15;
        private int length;
        public float rotation;
        bool init = false;
        int hitStun = 0;
        public override void SetDefaults()
        {
            name = "Lizard";
            width = 40;
            height = 40;
            hostile = true;
            speed = 8f;
            lifeMax = 60;
            life = lifeMax;
            damage = 4;
            knockBack = 2f;
            defaultColor = Color.White;
        }
        public bool IsHit()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj == null || !proj.active) continue;
                if (!proj.hostile) continue;
                if (proj.hitbox.IntersectsWith(hitbox) && iFrames <= 0)
                {
                    switch (Main.myPlayer.inventory[EquipType.MainHand].type)
                    {
                        case (int)ItemType.Broadsword:
                            hitStun = 30;  //45
                            break;
                        case (int)ItemType.Spear:
                            hitStun = 60;  //80
                            break;
                    }
                    life -= proj.damage;
                    iFrames = maxIFrames;
                    return true;
                }
            }
            return false;
        }
        public static Wurm_Head NewWurm(int x, int y, int length, int type)
        {
            var wurm = new Wurm_Head();
            //  Body
            wurm.body = new Wurm_Body[length];
            for (int i = 0; i < length; i++)
            {
                wurm.body[i] = new Wurm_Body(wurm);
                wurm.body[i].position = new Vector2(x, y);
                wurm.body[i].width = 32;
                wurm.body[i].height = 32;
                wurm.body[i].type = type;
                wurm.body[i].active = true;
            }
            wurm.length = length;

            //  Tail
            wurm.tail = new Wurm_Tail(wurm);
            wurm.tail.type = type;
            wurm.tail.width = 32;
            wurm.tail.height = 32;
            wurm.tail.position = new Vector2(x, y);
            wurm.tail.active = true;

            //  Head
            wurm.type = type;
            wurm.width = 32;
            wurm.height = 32;
            wurm.position = new Vector2(x, y);
            wurm.active = true;
            for (int i = 0; i < Main.wurm.Length; i++)
            {
                int num = 100;
                if (Main.wurm[i] == null || !Main.wurm[i].active)
                {
                    Main.wurm[i] = wurm;
                    Main.wurm[i].head = wurm;
                    break;
                }
                if (i == num)
                {
                    Main.wurm[num] = wurm;
                    break;
                }
            }
            return wurm;
        }
        private void Initialize()
        {
            lifeMax = 100;
            life = 100;
            damage = 10;
            defense = 3;
            iFrames = maxIFrames;
            //traits = new Traits()
            //{
            //    bookSmarts = 0.01f,
            //    streetSmarts = 0.1f,
            //    courage = 0.25f,
            //    wellBeing = 0.02f
            //};
        }
        public override void Dispose()
        {
            active = false;
            foreach (var b in body)
                b.active = false;
            tail.active = false;
        }
        public override void AI()
        {
            if (!active) return;

            if (!init)
            {
                Initialize();
                init = true;
            }

            if (hitStun-- > 0)
                return;
            
            rotation = Helper.AngleTo(Center, Main.myPlayer.Center);

            float range = 24f;
            float maxSpeed = 2f;
            float distance;

            if (IsHit() && life <= 0)
            {
                Dispose();
            }

            if (Main.myPlayer.IsMoving())
            {
                if (head == null)
                    return;
                if (iFrames > 0)
                    iFrames--;

                for (int i = 1; i < length; i++)
                {
                    var previous = body[i - 1];
                    if ((distance = previous.Distance(previous.Center, body[i].Center)) > range)
                    {
                        previous.velocity = Helper.AngleToSpeed(Helper.AngleTo(previous.Center, body[i].Center), Math.Min(distance, maxSpeed));
                    }
                    else previous.velocity = Vector2.Zero;
                    previous.position += previous.velocity;
                }
                int neck = length - 1;
                if ((distance = body[neck].Distance(head.Center, body[neck].Center)) > range)
                {
                    body[neck].velocity = Helper.AngleToSpeed(Helper.AngleTo(body[neck].Center, head.Center), Math.Min(distance, maxSpeed));
                }
                else body[neck].velocity = Vector2.Zero;
                body[neck].position += body[neck].velocity;

                if ((distance = tail.Distance(tail.Center, body[0].Center)) > range)
                {
                    tail.velocity = Helper.AngleToSpeed(Helper.AngleTo(tail.Center, body[0].Center), Math.Min(distance, maxSpeed));
                }
                else tail.velocity = Vector2.Zero;
                tail.position += tail.velocity;

                if ((distance = Main.myPlayer.Distance(head.Center, Main.myPlayer.Center)) > range)
                {
                    head.velocity = Helper.AngleToSpeed(Helper.AngleTo(head.Center, Main.myPlayer.Center), Math.Min(distance, maxSpeed));
                }
                else head.velocity = Vector2.Zero;
                head.position += head.velocity;
            }
        }
        public override void Draw(Graphics g)
        {
            if (!active || hidden) return;

            //  Head
            Drawing.DrawRotate(Main.wurmTex[0], hitbox, rotation, new PointF(width / 2, height / 2), Color.White, RotateType.GraphicsTransform, g);
            //  Tail + 1
            Drawing.DrawRotate(Main.wurmTex[1], body[0].hitbox, new Rectangle(0, 0, width, height), Helper.AngleTo(body[0].Center, body[1].Center), new PointF(width / 2, height / 2), Color.White, Color.White, RotateType.GraphicsTransform, g);
            //  Body
            for (int i = 1; i < length; i++)
            {
                var previous = body[i - 1];
                var segment = body[i]; 
                Drawing.DrawRotate(Main.wurmTex[1], segment.hitbox, Helper.AngleTo(previous.Center, segment.Center), new PointF(width / 2, height / 2), Color.White, RotateType.GraphicsTransform, g);
            }
            //  Tail
            Drawing.DrawRotate(Main.wurmTex[2], tail.hitbox, Helper.AngleTo(tail.Center, body[0].Center), new PointF(width / 2, height / 2), Color.White, RotateType.GraphicsTransform, g);
        }

        private Color IFrames(Color color)
        {
            return iFrames % 2 == 0 && iFrames > 0 ? Color.Red : color;
        }
    }

    public class Wurm_Body : Entity
    {
        public Wurm_Body(Wurm_Head head)
        {
            this.head = head;
        }
    }
    public class Wurm_Tail: Entity
    {
        public Wurm_Tail(Wurm_Head head)
        {
            this.head = head;
        }
    }
}